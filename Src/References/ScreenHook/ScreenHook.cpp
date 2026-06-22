// ScreenHook.cpp - 32-bit DLL for intercepting Red Spider screen capture
// Only Win32 (x86) target - redcomm.dll is a 32-bit DLL

#include "ScreenHook.h"
#include "MinHook.h"
#include "resource.h"

#include <objidl.h>
#include <gdiplus.h>
#include <string.h>

#pragma comment(lib, "gdiplus.lib")

using namespace Gdiplus;

// ============================================================================
// Forward Declarations
// ============================================================================
static void LoadFakeImage();
static BOOL HookRedSpiderInternal();
static void UnhookRedSpiderInternal();
BOOL WINAPI HookedGetCursorInfo(PCURSORINFO pci);
BOOL WINAPI HookedDrawIconEx(HDC hDC, int xLeft, int yTop, HICON hIcon, int cxWidth, int cyWidth, UINT istepIfAniCur, HBRUSH hbrFlickerFreeDraw, UINT diFlags);
BOOL WINAPI HookedStretchBlt(HDC hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, HDC hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, DWORD dwRop);

// ============================================================================
// Global Variables
// ============================================================================
static BOOL g_hookActive = FALSE;
static HMODULE g_hModule = NULL;
static BOOL g_hookEnabled = FALSE;  // default disabled, user enables via SetHookEnabled
static ULONG_PTR g_gdiplusToken = 0;
static HANDLE g_configThread = NULL;
static BOOL g_running = TRUE;
static BOOL g_redSpiderHooked = FALSE;

// Cached fake image HBITMAP - avoid creating/destroying every frame
static HBITMAP g_cachedBmp = NULL;
static int g_cachedW = 0;
static int g_cachedH = 0;

// Original GDI function pointers
typedef BOOL (WINAPI *BitBltPtr)(HDC, int, int, int, int, HDC, int, int, DWORD);
typedef int  (WINAPI *GetDIBitsPtr)(HDC, HBITMAP, UINT, UINT, LPVOID, LPBITMAPINFO, UINT);
typedef BOOL (WINAPI *GetCursorInfoPtr)(PCURSORINFO);
typedef BOOL (WINAPI *DrawIconExPtr)(HDC, int, int, HICON, int, int, UINT, HBRUSH, UINT);
typedef BOOL (WINAPI *StretchBltPtr)(HDC, int, int, int, int, HDC, int, int, int, int, DWORD);
typedef FARPROC (WINAPI *GetProcAddressPtr)(HMODULE, LPCSTR);

static BitBltPtr g_origBitBlt = NULL;
static GetDIBitsPtr g_origGetDIBits = NULL;
static GetProcAddressPtr g_origGetProcAddress = NULL;
static GetCursorInfoPtr g_origGetCursorInfo = NULL;
static DrawIconExPtr g_origDrawIconEx = NULL;
static StretchBltPtr g_origStretchBlt = NULL;

// Red Spider internal function types
typedef int (__thiscall* FnBitBlt_NotHooked_redGdi)(void* thisPtr, int w, int h);
typedef int (__thiscall* FnGetChangedRect_NotHooked_DIB)(void* thisPtr, void* rects, int flags);

static FnBitBlt_NotHooked_redGdi g_origBitBlt_NotHooked = NULL;
static FnGetChangedRect_NotHooked_DIB g_origGetChangedRect_DIB = NULL;

// Saved original bytes for JMP patch restore (6-byte push+ret)
static BYTE g_savedBytes_BitBlt[6] = {0};
static BYTE g_savedBytes_GetChanged[6] = {0};
static BYTE* g_patchedAddr_BitBlt = NULL;
static BYTE* g_patchedAddr_GetChanged = NULL;
static BOOL g_jmpPatched_BitBlt = FALSE;
static BOOL g_jmpPatched_GetChanged = FALSE;

// ============================================================================
// Load ScreenGuard.png from embedded resource
// ============================================================================
static Bitmap* g_fakeBitmap = NULL;

static void LoadFakeImage()
{
    if (g_fakeBitmap) { delete g_fakeBitmap; g_fakeBitmap = NULL; }

    HRSRC hResource = FindResource(g_hModule, MAKEINTRESOURCE(ScreenGuard), L"PNG");
    if (!hResource) { return; }

    HGLOBAL hData = LoadResource(g_hModule, hResource);
    if (!hData) { return; }

    DWORD size = SizeofResource(g_hModule, hResource);
    LPVOID pData = LockResource(hData);
    if (!pData || size == 0) { return; }

    HGLOBAL hGlobal = GlobalAlloc(GMEM_MOVEABLE, size);
    if (!hGlobal) return;

    LPVOID pGlobal = GlobalLock(hGlobal);
    if (!pGlobal) { GlobalFree(hGlobal); return; }

    memcpy(pGlobal, pData, size);
    GlobalUnlock(hGlobal);

    IStream* pStream = NULL;
    if (CreateStreamOnHGlobal(hGlobal, FALSE, &pStream) == S_OK)
    {
        g_fakeBitmap = new Bitmap(pStream);
        if (g_fakeBitmap && g_fakeBitmap->GetLastStatus() != Ok)
        {
            delete g_fakeBitmap; g_fakeBitmap = NULL;
        }
        else
        {
        }
        pStream->Release();
        GlobalFree(hGlobal);
    }
    else
    {
        GlobalFree(hGlobal);
    }

    // Invalidate cached HBITMAP
    if (g_cachedBmp) { DeleteObject(g_cachedBmp); g_cachedBmp = NULL; }
    g_cachedW = 0; g_cachedH = 0;
}

// Get cached HBITMAP of fake image at specified size
static HBITMAP GetCachedFakeHBITMAP(HDC hdcRef, int w, int h)
{
    if (g_cachedBmp && g_cachedW == w && g_cachedH == h)
        return g_cachedBmp;

    // Delete old cache
    if (g_cachedBmp) { DeleteObject(g_cachedBmp); g_cachedBmp = NULL; }

    if (!g_fakeBitmap) LoadFakeImage();
    if (!g_fakeBitmap) return NULL;

    HDC memDC = CreateCompatibleDC(hdcRef);
    g_cachedBmp = CreateCompatibleBitmap(hdcRef, w, h);
    HBITMAP oldBmp = (HBITMAP)SelectObject(memDC, g_cachedBmp);

    Graphics g(memDC);
    g.SetInterpolationMode(InterpolationModeHighQualityBilinear);
    g.DrawImage(g_fakeBitmap, 0, 0, w, h);

    SelectObject(memDC, oldBmp);
    DeleteDC(memDC);

    g_cachedW = w;
    g_cachedH = h;
    return g_cachedBmp;
}

// Draw fake image onto a DC, scaled to w x h (uses cached HBITMAP)
static void DrawFakeImage(HDC hdcDest, int x, int y, int w, int h)
{
    HBITMAP hBmp = GetCachedFakeHBITMAP(hdcDest, w, h);
    if (!hBmp) return;

    HDC memDC = CreateCompatibleDC(hdcDest);
    HBITMAP oldBmp = (HBITMAP)SelectObject(memDC, hBmp);

    BitBlt(hdcDest, x, y, w, h, memDC, 0, 0, SRCCOPY);

    SelectObject(memDC, oldBmp);
    DeleteDC(memDC);
}

// ============================================================================
// GDI API Hooks
// ============================================================================

BOOL WINAPI HookedBitBlt(HDC hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, HDC hdcSrc, int nXSrc, int nYSrc, DWORD dwRop)
{
    if (!g_hookEnabled)
        return g_origBitBlt(hdcDest, nXDest, nYDest, nWidth, nHeight, hdcSrc, nXSrc, nYSrc, dwRop);

    DrawFakeImage(hdcDest, nXDest, nYDest, nWidth, nHeight);
    return TRUE;
}

int WINAPI HookedGetDIBits(HDC hdc, HBITMAP hbmp, UINT uStartScan, UINT cScanLines, LPVOID lpvBits, LPBITMAPINFO lpbmi, UINT uUsage)
{
    if (!g_hookEnabled)
        return g_origGetDIBits(hdc, hbmp, uStartScan, cScanLines, lpvBits, lpbmi, uUsage);

    if (!g_fakeBitmap) LoadFakeImage();

    if (g_fakeBitmap && lpvBits && lpbmi)
    {
        int width = (int)g_fakeBitmap->GetWidth();
        int height = (int)g_fakeBitmap->GetHeight();

        BitmapData bmpData;
        Rect rect(0, 0, width, height);
        if (g_fakeBitmap->LockBits(&rect, ImageLockModeRead, PixelFormat32bppARGB, &bmpData) == Ok)
        {
            lpbmi->bmiHeader.biWidth = width;
            lpbmi->bmiHeader.biHeight = height;
            lpbmi->bmiHeader.biBitCount = 24;
            lpbmi->bmiHeader.biCompression = BI_RGB;

            int dstStride = (width * 3 + 3) & ~3;
            size_t offset = (size_t)(uStartScan * dstStride);
            size_t totalSize = (size_t)(dstStride * height);

            if (offset < totalSize)
            {
                size_t copyLength = (size_t)(cScanLines * dstStride);
                copyLength = (copyLength < (totalSize - offset)) ? copyLength : (totalSize - offset);

                BYTE* src = (BYTE*)bmpData.Scan0;
                for (UINT row = 0; row < cScanLines && (uStartScan + row) < (UINT)height; row++)
                {
                    int srcY = (int)(uStartScan + row);
                    BYTE* srcRow = src + srcY * bmpData.Stride;
                    BYTE* dstRow = (BYTE*)lpvBits + row * dstStride;
                    for (int cx = 0; cx < width; cx++)
                    {
                        dstRow[cx * 3 + 0] = srcRow[cx * 4 + 0];
                        dstRow[cx * 3 + 1] = srcRow[cx * 4 + 1];
                        dstRow[cx * 3 + 2] = srcRow[cx * 4 + 2];
                    }
                }
                g_fakeBitmap->UnlockBits(&bmpData);
                return (int)cScanLines;
            }
            g_fakeBitmap->UnlockBits(&bmpData);
        }
    }

    return g_origGetDIBits(hdc, hbmp, uStartScan, cScanLines, lpvBits, lpbmi, uUsage);
}

// ============================================================================
// Red Spider Internal Function Hooks (32-bit __thiscall)
// ============================================================================

static int __fastcall HookedBitBlt_NotHooked_redGdi(void* thisPtr, int /*dummyEdx*/, int width, int height)
{
    // Always call original first to keep Red Spider's state machine happy
    int result = 1;
    if (g_origBitBlt_NotHooked)
        result = g_origBitBlt_NotHooked(thisPtr, width, height);

    if (!g_hookEnabled)
        return result;

    // Overwrite destination DC with fake image
    HDC hdcDest = *(HDC*)((BYTE*)thisPtr + 0xac);
    if (hdcDest)
        DrawFakeImage(hdcDest, 0, 0, width, height);

    return result;
}

static int __fastcall HookedGetChangedRect_NotHooked_DIB(void* thisPtr, int /*dummyEdx*/, void* rects, int flags)
{
    if (g_origGetChangedRect_DIB)
        return g_origGetChangedRect_DIB(thisPtr, rects, flags);
    return 0;
}

// ============================================================================
// Hook GetProcAddress for dynamic API interception
// ============================================================================
FARPROC WINAPI HookedGetProcAddress(HMODULE hModule, LPCSTR lpProcName)
{
    if (g_hookEnabled && lpProcName != NULL)
    {
        if (strcmp(lpProcName, "BitBlt") == 0) return (FARPROC)&HookedBitBlt;
        if (strcmp(lpProcName, "GetDIBits") == 0) return (FARPROC)&HookedGetDIBits;
        if (strcmp(lpProcName, "GetCursorInfo") == 0) return (FARPROC)&HookedGetCursorInfo;
        if (strcmp(lpProcName, "DrawIconEx") == 0) return (FARPROC)&HookedDrawIconEx;
        if (strcmp(lpProcName, "StretchBlt") == 0) return (FARPROC)&HookedStretchBlt;
    }
    return g_origGetProcAddress(hModule, lpProcName);
}

// ============================================================================
// Cursor Hiding Hooks
// ============================================================================

BOOL WINAPI HookedGetCursorInfo(PCURSORINFO pci)
{
    BOOL result = g_origGetCursorInfo(pci);
    if (result && g_hookEnabled && pci)
    {
        pci->flags = 0;
        pci->hCursor = NULL;
    }
    return result;
}

BOOL WINAPI HookedDrawIconEx(HDC hDC, int xLeft, int yTop, HICON hIcon, int cxWidth, int cyWidth, UINT istepIfAniCur, HBRUSH hbrFlickerFreeDraw, UINT diFlags)
{
    if (g_hookEnabled) return TRUE;
    return g_origDrawIconEx(hDC, xLeft, yTop, hIcon, cxWidth, cyWidth, istepIfAniCur, hbrFlickerFreeDraw, diFlags);
}

// ============================================================================
// StretchBlt Hook
// ============================================================================

BOOL WINAPI HookedStretchBlt(HDC hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, HDC hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, DWORD dwRop)
{
    if (!g_hookEnabled)
        return g_origStretchBlt(hdcDest, nXOriginDest, nYOriginDest, nWidthDest, nHeightDest, hdcSrc, nXOriginSrc, nYOriginSrc, nWidthSrc, nHeightSrc, dwRop);

    DrawFakeImage(hdcDest, nXOriginDest, nYOriginDest, nWidthDest, nHeightDest);
    return TRUE;
}

// ============================================================================
// Hook Red Spider internal functions by offset (with save/restore)
// ============================================================================
static BOOL HookRedSpiderInternal()
{
    HMODULE hRedComm = GetModuleHandleW(L"redcomm.dll");
    if (!hRedComm) return FALSE;

    BYTE* pBase = (BYTE*)hRedComm;
    int patched = 0;

    struct { DWORD offset; const char* name; void* hook; void** orig; BYTE* savedBytes; BYTE** pAddr; BOOL* isPatched; } funcs[] = {
        { 0x53c0, "BitBlt_NotHooked_redGdi", (void*)&HookedBitBlt_NotHooked_redGdi, (void**)&g_origBitBlt_NotHooked, g_savedBytes_BitBlt, &g_patchedAddr_BitBlt, &g_jmpPatched_BitBlt },
        { 0x5a10, "GetChangedRect_NotHooked_DIB", (void*)&HookedGetChangedRect_NotHooked_DIB, (void**)&g_origGetChangedRect_DIB, g_savedBytes_GetChanged, &g_patchedAddr_GetChanged, &g_jmpPatched_GetChanged },
    };

    for (int i = 0; i < 2; i++)
    {
        BYTE* pAddr = pBase + funcs[i].offset;

        MEMORY_BASIC_INFORMATION mbi;
        if (VirtualQuery(pAddr, &mbi, sizeof(mbi)) == 0 ||
            mbi.State != MEM_COMMIT ||
            !(mbi.Protect & (PAGE_EXECUTE | PAGE_EXECUTE_READ | PAGE_EXECUTE_READWRITE | PAGE_EXECUTE_WRITECOPY)))
        {
            continue;
        }

        MH_STATUS st = MH_CreateHook(pAddr, funcs[i].hook, funcs[i].orig);
        if (st == MH_OK)
        {
            st = MH_EnableHook(pAddr);
            if (st == MH_OK)
            {
                patched++;
            }
        }

        if (st != MH_OK)
        {
            // Fallback: 6-byte push+ret patch (68 xx xx xx xx C3) + trampoline
            DWORD oldProt;
            if (VirtualProtect(pAddr, 6, PAGE_EXECUTE_READWRITE, &oldProt))
            {
                memcpy(funcs[i].savedBytes, pAddr, 6);
                *funcs[i].pAddr = pAddr;
                *funcs[i].isPatched = TRUE;

                void* trampoline = VirtualAlloc(NULL, 11, MEM_COMMIT, PAGE_EXECUTE_READWRITE);
                if (trampoline)
                {
                    memcpy(trampoline, pAddr, 6);
                    BYTE* t = (BYTE*)trampoline;
                    DWORD jmpBack = (DWORD)(pAddr + 6) - (DWORD)(t + 6) - 5;
                    t[6] = 0xE9;
                    memcpy(&t[7], &jmpBack, 4);
                    *funcs[i].orig = trampoline;
                }

                pAddr[0] = 0x68;
                memcpy(&pAddr[1], &funcs[i].hook, 4);
                pAddr[5] = 0xC3;
                VirtualProtect(pAddr, 6, oldProt, &oldProt);
                patched++;
            }
        }
    }

    return patched > 0;
}

// Restore original bytes for JMP patches
static void UnhookRedSpiderInternal()
{
    struct { BYTE* savedBytes; BYTE** pAddr; BOOL* isPatched; const char* name; } patches[] = {
        { g_savedBytes_BitBlt, &g_patchedAddr_BitBlt, &g_jmpPatched_BitBlt, "BitBlt_NotHooked_redGdi" },
        { g_savedBytes_GetChanged, &g_patchedAddr_GetChanged, &g_jmpPatched_GetChanged, "GetChangedRect_NotHooked_DIB" },
    };

    for (int i = 0; i < 2; i++)
    {
        if (!*patches[i].isPatched || !*patches[i].pAddr) continue;

        BYTE* pAddr = *patches[i].pAddr;
        DWORD oldProt;
        if (VirtualProtect(pAddr, 6, PAGE_EXECUTE_READWRITE, &oldProt))
        {
            memcpy(pAddr, patches[i].savedBytes, 6);
            VirtualProtect(pAddr, 6, oldProt, &oldProt);
            *patches[i].isPatched = FALSE;
        }
    }

    // Also disable MinHook hooks
    MH_DisableHook(MH_ALL_HOOKS);
    g_redSpiderHooked = FALSE;
}

// ============================================================================
// Config Monitor Thread
// ============================================================================
static DWORD WINAPI ConfigMonitorThread(LPVOID param)
{
    while (g_running)
    {
        if (!g_redSpiderHooked && g_hookEnabled)
        {
            if (HookRedSpiderInternal())
                g_redSpiderHooked = TRUE;
        }
        Sleep(2000);
    }
    return 0;
}

// ============================================================================
// DLL Entry Point
// ============================================================================
BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    {
        g_hModule = hModule;
        DisableThreadLibraryCalls(hModule);

        GdiplusStartupInput gsi;
        if (GdiplusStartup(&g_gdiplusToken, &gsi, NULL) != Ok)
            return FALSE;

        if (MH_Initialize() != MH_OK)
            return FALSE;

        LoadFakeImage();

        // Hook BitBlt
        MH_STATUS st = MH_CreateHookApi(L"gdi32.dll", "BitBlt", &HookedBitBlt, (LPVOID*)&g_origBitBlt);
        if (st != MH_OK)
        {
            FARPROC p = GetProcAddress(GetModuleHandleW(L"gdi32.dll"), "BitBlt");
            if (p) st = MH_CreateHook(p, &HookedBitBlt, (LPVOID*)&g_origBitBlt);
        }

        // Hook GetDIBits
        st = MH_CreateHookApi(L"gdi32.dll", "GetDIBits", &HookedGetDIBits, (LPVOID*)&g_origGetDIBits);
        if (st != MH_OK)
        {
            FARPROC p = GetProcAddress(GetModuleHandleW(L"gdi32.dll"), "GetDIBits");
            if (p) st = MH_CreateHook(p, &HookedGetDIBits, (LPVOID*)&g_origGetDIBits);
        }

        // Hook GetProcAddress
        st = MH_CreateHookApi(L"kernel32.dll", "GetProcAddress", &HookedGetProcAddress, (LPVOID*)&g_origGetProcAddress);

        // Hook GetCursorInfo
        st = MH_CreateHookApi(L"user32.dll", "GetCursorInfo", &HookedGetCursorInfo, (LPVOID*)&g_origGetCursorInfo);

        // Hook DrawIconEx
        st = MH_CreateHookApi(L"user32.dll", "DrawIconEx", &HookedDrawIconEx, (LPVOID*)&g_origDrawIconEx);

        // Hook StretchBlt
        st = MH_CreateHookApi(L"gdi32.dll", "StretchBlt", &HookedStretchBlt, (LPVOID*)&g_origStretchBlt);

        // Enable all hooks
        MH_EnableHook(MH_ALL_HOOKS);
        g_hookActive = TRUE;

        // Try hooking Red Spider immediately
        if (HookRedSpiderInternal())
            g_redSpiderHooked = TRUE;

        g_running = TRUE;
        g_configThread = CreateThread(NULL, 0, ConfigMonitorThread, NULL, 0, NULL);
        break;
    }

    case DLL_PROCESS_DETACH:
        g_running = FALSE;
        if (g_configThread) { WaitForSingleObject(g_configThread, 1000); CloseHandle(g_configThread); }
        UnhookRedSpiderInternal();
        if (g_cachedBmp) { DeleteObject(g_cachedBmp); g_cachedBmp = NULL; }
        if (g_fakeBitmap) { delete g_fakeBitmap; g_fakeBitmap = NULL; }
        GdiplusShutdown(g_gdiplusToken);
        MH_Uninitialize();
        break;
    }
    return TRUE;
}

// ============================================================================
// Exported Functions - called remotely by C# host
// ============================================================================

void SetHookEnabled(void* pEnabled)
{
    g_hookEnabled = pEnabled ? (*(BOOL*)pEnabled) : FALSE;
}

void UninitializeHook()
{
    if (g_hookActive)
    {
        UnhookRedSpiderInternal();
        MH_DisableHook(MH_ALL_HOOKS);
        g_hookActive = FALSE;
    }
}

BOOL IsHookActive() { return g_hookActive && g_hookEnabled; }
BOOL InitializeHook() { return TRUE; }
void SetImagePath(const wchar_t* path) { (void)path; }
