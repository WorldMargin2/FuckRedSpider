#pragma once

#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif
#ifndef NOMINMAX
#define NOMINMAX
#endif
#include <windows.h>
#include <string>

// Export functions
extern "C" {
    __declspec(dllexport) BOOL InitializeHook();
    __declspec(dllexport) void SetImagePath(const wchar_t* path);
    __declspec(dllexport) void SetHookEnabled(void* pEnabled);
    __declspec(dllexport) void UninitializeHook();
    __declspec(dllexport) BOOL IsHookActive();
}