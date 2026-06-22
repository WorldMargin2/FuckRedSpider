/*
 *  MinHook - Minimal API Hooking Library
 *  Simplified implementation for ScreenHookDll
 */

#include "MinHook.h"
#include <tlhelp32.h>

// ============================================================================
// Internal Structures
// ============================================================================

#pragma pack(push, 1)

typedef struct _JMP_REL
{
    UINT8  opcode;
    UINT32 operand;
} JMP_REL, *PJMP_REL;

#ifdef _M_X64
typedef struct _JMP_ABS
{
    UINT8  opcode;
    UINT8  modrm;
    UINT32 operand;
    UINT64 address;
} JMP_ABS, *PJMP_ABS;
#endif

#pragma pack(pop)

typedef struct _HOOK_ENTRY
{
    LPVOID pTarget;
    LPVOID pDetour;
    LPVOID pTrampoline;
    BOOL   isEnabled;
    UINT8  backup[32];
    UINT   backupSize;
} HOOK_ENTRY, *PHOOK_ENTRY;

// ============================================================================
// Global Variables
// ============================================================================

static PHOOK_ENTRY g_hooks = NULL;
static UINT        g_hookCount = 0;
static UINT        g_hookCapacity = 0;
static BOOL        g_isInitialized = FALSE;
static HANDLE      g_heap = NULL;

// ============================================================================
// Memory Functions
// ============================================================================

static LPVOID AllocateBuffer(LPVOID pOrigin, SIZE_T size)
{
    SYSTEM_INFO si;
    GetSystemInfo(&si);
    
    UINT_PTR minAddr = (UINT_PTR)pOrigin - 0x80000000;
    UINT_PTR maxAddr = (UINT_PTR)pOrigin + 0x80000000;
    
    minAddr = (minAddr > (UINT_PTR)si.lpMinimumApplicationAddress) ? minAddr : (UINT_PTR)si.lpMinimumApplicationAddress;
    maxAddr = (maxAddr < (UINT_PTR)si.lpMaximumApplicationAddress) ? maxAddr : (UINT_PTR)si.lpMaximumApplicationAddress;
    
    UINT_PTR addr = (UINT_PTR)pOrigin;
    
    for (int i = 0; i < 256; i++)
    {
        MEMORY_BASIC_INFORMATION mbi;
        if (VirtualQuery((LPVOID)addr, &mbi, sizeof(mbi)) == 0)
            break;
        
        if (mbi.State == MEM_FREE)
        {
            LPVOID pAlloc = VirtualAlloc((LPVOID)addr, size, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
            if (pAlloc)
                return pAlloc;
        }
        
        addr = (UINT_PTR)mbi.BaseAddress + mbi.RegionSize;
        if (addr > maxAddr)
            break;
    }
    
    return VirtualAlloc(NULL, size, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
}

static VOID FreeBuffer(LPVOID pBuffer)
{
    if (pBuffer)
        VirtualFree(pBuffer, 0, MEM_RELEASE);
}

// ============================================================================
// Hook Management
// ============================================================================

static PHOOK_ENTRY FindHook(LPVOID pTarget)
{
    for (UINT i = 0; i < g_hookCount; i++)
    {
        if (g_hooks[i].pTarget == pTarget)
            return &g_hooks[i];
    }
    return NULL;
}

static PHOOK_ENTRY AddHook(LPVOID pTarget, LPVOID pDetour, LPVOID pTrampoline)
{
    if (g_hookCount >= g_hookCapacity)
    {
        UINT newCapacity = (g_hookCapacity == 0) ? 32 : g_hookCapacity * 2;
        PHOOK_ENTRY newHooks = (PHOOK_ENTRY)HeapReAlloc(g_heap, 0, g_hooks, newCapacity * sizeof(HOOK_ENTRY));
        if (!newHooks)
            return NULL;
        
        g_hooks = newHooks;
        g_hookCapacity = newCapacity;
    }
    
    PHOOK_ENTRY pHook = &g_hooks[g_hookCount++];
    pHook->pTarget = pTarget;
    pHook->pDetour = pDetour;
    pHook->pTrampoline = pTrampoline;
    pHook->isEnabled = FALSE;
    
    return pHook;
}

// ============================================================================
// Instruction Analysis
// ============================================================================

static UINT GetInstructionSize(LPVOID pCode)
{
    UINT8* p = (UINT8*)pCode;
    
    switch (p[0])
    {
    case 0xE9: return 5;
    case 0xE8: return 5;
    case 0xEB: return 2;
    case 0xC3: return 1;
    case 0xC2: return 3;
    case 0x90: return 1;
    case 0xCC: return 1;
    case 0x50: case 0x51: case 0x52: case 0x53: case 0x54: case 0x55: case 0x56: case 0x57: return 1;
    case 0x58: case 0x59: case 0x5A: case 0x5B: case 0x5C: case 0x5D: case 0x5E: case 0x5F: return 1;
    case 0x68: return 5;
    case 0x6A: return 2;
    case 0x48:
        switch (p[1])
        {
        case 0x89: return 3;
        case 0x8B: return 3;
        case 0xB8: case 0xB9: case 0xBA: case 0xBB: case 0xBC: case 0xBD: case 0xBE: case 0xBF: return 10;
        case 0x83: return 4;
        case 0x81: return 7;
        }
        return 2;
    case 0x89: return 2;
    case 0x8B: return 2;
    case 0xB8: case 0xB9: case 0xBA: case 0xBB: case 0xBC: case 0xBD: case 0xBE: case 0xBF: return 5;
    case 0x83: return 3;
    case 0x81: return 6;
    case 0x33: return 2;
    case 0x85: return 2;
    case 0xA1: return 5;
    case 0xA3: return 5;
    case 0xFF:
        if (p[1] == 0x25 || p[1] == 0x15) return 6;
        return 2;
    }
    
    return 1;
}

// ============================================================================
// Trampoline Creation
// ============================================================================

static LPVOID CreateTrampoline(LPVOID pTarget, LPVOID pDetour)
{
    UINT copySize = 0;
    UINT8* pTargetBytes = (UINT8*)pTarget;
    
    while (copySize < sizeof(JMP_REL))
    {
        UINT instSize = GetInstructionSize(pTargetBytes + copySize);
        copySize += instSize;
        if (copySize > 32)
            return NULL;
    }
    
    SIZE_T trampolineSize = copySize + sizeof(JMP_REL) + 32;
    LPVOID pTrampoline = AllocateBuffer(pTarget, trampolineSize);
    if (!pTrampoline)
        return NULL;
    
    memcpy(pTrampoline, pTarget, copySize);
    
#ifdef _M_X64
    PJMP_ABS pJump = (PJMP_ABS)((UINT8*)pTrampoline + copySize);
    pJump->opcode = 0xFF;
    pJump->modrm = 0x25;
    pJump->operand = 0;
    pJump->address = (UINT64)pTarget + copySize;
#else
    PJMP_REL pJump = (PJMP_REL)((UINT8*)pTrampoline + copySize);
    pJump->opcode = 0xE9;
    pJump->operand = (UINT32)((UINT_PTR)pTarget + copySize - (UINT_PTR)pJump - sizeof(JMP_REL));
#endif
    
    return pTrampoline;
}

// ============================================================================
// Hook Installation
// ============================================================================

static BOOL WriteJump(LPVOID pFrom, LPVOID pTo, UINT size)
{
    DWORD oldProtect;
    if (!VirtualProtect(pFrom, size, PAGE_EXECUTE_READWRITE, &oldProtect))
        return FALSE;
    
#ifdef _M_X64
    PJMP_ABS pJump = (PJMP_ABS)pFrom;
    pJump->opcode = 0xFF;
    pJump->modrm = 0x25;
    pJump->operand = 0;
    pJump->address = (UINT64)pTo;
#else
    PJMP_REL pJump = (PJMP_REL)pFrom;
    pJump->opcode = 0xE9;
    pJump->operand = (UINT32)((UINT_PTR)pTo - (UINT_PTR)pFrom - sizeof(JMP_REL));
#endif
    
    DWORD dummy;
    VirtualProtect(pFrom, size, oldProtect, &dummy);
    FlushInstructionCache(GetCurrentProcess(), pFrom, size);
    
    return TRUE;
}

static BOOL EnableHookEntry(PHOOK_ENTRY pHook)
{
    if (pHook->isEnabled)
        return TRUE;
    
    UINT jumpSize = sizeof(JMP_REL);
#ifdef _M_X64
    jumpSize = sizeof(JMP_ABS);
#endif
    
    memcpy(pHook->backup, pHook->pTarget, jumpSize);
    pHook->backupSize = jumpSize;
    
    if (!WriteJump(pHook->pTarget, pHook->pDetour, jumpSize))
        return FALSE;
    
    pHook->isEnabled = TRUE;
    return TRUE;
}

static BOOL DisableHookEntry(PHOOK_ENTRY pHook)
{
    if (!pHook->isEnabled)
        return TRUE;
    
    DWORD oldProtect;
    if (!VirtualProtect(pHook->pTarget, pHook->backupSize, PAGE_EXECUTE_READWRITE, &oldProtect))
        return FALSE;
    
    memcpy(pHook->pTarget, pHook->backup, pHook->backupSize);
    
    DWORD dummy;
    VirtualProtect(pHook->pTarget, pHook->backupSize, oldProtect, &dummy);
    FlushInstructionCache(GetCurrentProcess(), pHook->pTarget, pHook->backupSize);
    
    pHook->isEnabled = FALSE;
    return TRUE;
}

// ============================================================================
// Public API
// ============================================================================

MH_STATUS WINAPI MH_Initialize(VOID)
{
    if (g_isInitialized)
        return MH_ERROR_ALREADY_INITIALIZED;
    
    g_heap = HeapCreate(0, 0, 0);
    if (!g_heap)
        return MH_ERROR_MEMORY_ALLOC;
    
    g_hooks = NULL;
    g_hookCount = 0;
    g_hookCapacity = 0;
    g_isInitialized = TRUE;
    
    return MH_OK;
}

MH_STATUS WINAPI MH_Uninitialize(VOID)
{
    if (!g_isInitialized)
        return MH_ERROR_NOT_INITIALIZED;
    
    for (UINT i = 0; i < g_hookCount; i++)
    {
        DisableHookEntry(&g_hooks[i]);
        FreeBuffer(g_hooks[i].pTrampoline);
    }
    
    if (g_hooks)
        HeapFree(g_heap, 0, g_hooks);
    
    HeapDestroy(g_heap);
    
    g_hooks = NULL;
    g_hookCount = 0;
    g_hookCapacity = 0;
    g_heap = NULL;
    g_isInitialized = FALSE;
    
    return MH_OK;
}

MH_STATUS WINAPI MH_CreateHook(LPVOID pTarget, LPVOID pDetour, LPVOID *ppOriginal)
{
    if (!g_isInitialized)
        return MH_ERROR_NOT_INITIALIZED;
    
    if (FindHook(pTarget))
        return MH_ERROR_ALREADY_CREATED;
    
    LPVOID pTrampoline = CreateTrampoline(pTarget, pDetour);
    if (!pTrampoline)
        return MH_ERROR_MEMORY_ALLOC;
    
    PHOOK_ENTRY pHook = AddHook(pTarget, pDetour, pTrampoline);
    if (!pHook)
    {
        FreeBuffer(pTrampoline);
        return MH_ERROR_MEMORY_ALLOC;
    }
    
    if (ppOriginal)
        *ppOriginal = pTrampoline;
    
    return MH_OK;
}

MH_STATUS WINAPI MH_CreateHookApi(LPCWSTR pszModule, LPCSTR pszProcName, LPVOID pDetour, LPVOID *ppOriginal)
{
    HMODULE hModule = GetModuleHandleW(pszModule);
    if (!hModule)
        return MH_ERROR_MODULE_NOT_FOUND;
    
    LPVOID pTarget = GetProcAddress(hModule, pszProcName);
    if (!pTarget)
        return MH_ERROR_FUNCTION_NOT_FOUND;
    
    return MH_CreateHook(pTarget, pDetour, ppOriginal);
}

MH_STATUS WINAPI MH_RemoveHook(LPVOID pTarget)
{
    if (!g_isInitialized)
        return MH_ERROR_NOT_INITIALIZED;
    
    if (pTarget == MH_ALL_HOOKS)
    {
        for (UINT i = 0; i < g_hookCount; i++)
        {
            DisableHookEntry(&g_hooks[i]);
            FreeBuffer(g_hooks[i].pTrampoline);
        }
        g_hookCount = 0;
        return MH_OK;
    }
    
    PHOOK_ENTRY pHook = FindHook(pTarget);
    if (!pHook)
        return MH_ERROR_NOT_CREATED;
    
    DisableHookEntry(pHook);
    FreeBuffer(pHook->pTrampoline);
    
    UINT index = (UINT)(pHook - g_hooks);
    for (UINT i = index; i < g_hookCount - 1; i++)
        g_hooks[i] = g_hooks[i + 1];
    g_hookCount--;
    
    return MH_OK;
}

MH_STATUS WINAPI MH_EnableHook(LPVOID pTarget)
{
    if (!g_isInitialized)
        return MH_ERROR_NOT_INITIALIZED;
    
    if (pTarget == MH_ALL_HOOKS)
    {
        for (UINT i = 0; i < g_hookCount; i++)
        {
            if (!EnableHookEntry(&g_hooks[i]))
                return MH_ERROR_MEMORY_PROTECT;
        }
        return MH_OK;
    }
    
    PHOOK_ENTRY pHook = FindHook(pTarget);
    if (!pHook)
        return MH_ERROR_NOT_CREATED;
    
    if (!EnableHookEntry(pHook))
        return MH_ERROR_MEMORY_PROTECT;
    
    return MH_OK;
}

MH_STATUS WINAPI MH_DisableHook(LPVOID pTarget)
{
    if (!g_isInitialized)
        return MH_ERROR_NOT_INITIALIZED;
    
    if (pTarget == MH_ALL_HOOKS)
    {
        for (UINT i = 0; i < g_hookCount; i++)
        {
            if (!DisableHookEntry(&g_hooks[i]))
                return MH_ERROR_MEMORY_PROTECT;
        }
        return MH_OK;
    }
    
    PHOOK_ENTRY pHook = FindHook(pTarget);
    if (!pHook)
        return MH_ERROR_NOT_CREATED;
    
    if (!DisableHookEntry(pHook))
        return MH_ERROR_MEMORY_PROTECT;
    
    return MH_OK;
}