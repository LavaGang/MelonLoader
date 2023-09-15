.globl GetFileVersionInfoA
.globl GetFileVersionInfoByHandle
.globl GetFileVersionInfoExA
.globl GetFileVersionInfoExW
.globl GetFileVersionInfoSizeA
.globl GetFileVersionInfoSizeExA
.globl GetFileVersionInfoSizeExW
.globl GetFileVersionInfoSizeW
.globl GetFileVersionInfoW
.globl VerFindFileA
.globl VerFindFileW
.globl VerInstallFileA
.globl VerInstallFileW
.globl VerLanguageNameA
.globl VerLanguageNameW
.globl VerQueryValueA
.globl VerQueryValueW

GetFileVersionInfoA:
    jmp ds:[_OriginalFuncs + 0 * 4]
GetFileVersionInfoByHandle:
    jmp ds:[_OriginalFuncs + 1 * 4]
GetFileVersionInfoExA:
    jmp ds:[_OriginalFuncs + 2 * 4]
GetFileVersionInfoExW:
    jmp ds:[_OriginalFuncs + 3 * 4]
GetFileVersionInfoSizeA:
    jmp ds:[_OriginalFuncs + 4 * 4]
GetFileVersionInfoSizeExA:
    jmp ds:[_OriginalFuncs + 5 * 4]
GetFileVersionInfoSizeExW:
    jmp ds:[_OriginalFuncs + 6 * 4]
GetFileVersionInfoSizeW:
    jmp ds:[_OriginalFuncs + 7 * 4]
GetFileVersionInfoW:
    jmp ds:[_OriginalFuncs + 4 * 4]
VerFindFileA:
    jmp ds:[_OriginalFuncs + 9 * 4]
VerFindFileW:
    jmp ds:[_OriginalFuncs + 10 * 4]
VerInstallFileA:
    jmp ds:[_OriginalFuncs + 11 * 4]
VerInstallFileW:
    jmp ds:[_OriginalFuncs + 12 * 4]
VerLanguageNameA:
    jmp ds:[_OriginalFuncs + 13 * 4]
VerLanguageNameW:
    jmp ds:[_OriginalFuncs + 14 * 4]
VerQueryValueA:
    jmp ds:[_OriginalFuncs + 15 * 4]
VerQueryValueW:
    jmp ds:[_OriginalFuncs + 16 * 4]