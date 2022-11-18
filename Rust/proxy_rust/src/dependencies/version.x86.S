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
    jmp ds:[_OriginalFuncs_version + 0 * 4]
GetFileVersionInfoByHandle:
    jmp ds:[_OriginalFuncs_version + 1 * 4]
GetFileVersionInfoExA:
    jmp ds:[_OriginalFuncs_version + 2 * 4]
GetFileVersionInfoExW:
    jmp ds:[_OriginalFuncs_version + 3 * 4]
GetFileVersionInfoSizeA:
    jmp ds:[_OriginalFuncs_version + 4 * 4]
GetFileVersionInfoSizeExA:
    jmp ds:[_OriginalFuncs_version + 5 * 4]
GetFileVersionInfoSizeExW:
    jmp ds:[_OriginalFuncs_version + 6 * 4]
GetFileVersionInfoSizeW:
    jmp ds:[_OriginalFuncs_version + 7 * 4]
GetFileVersionInfoW:
    jmp ds:[_OriginalFuncs_version + 4 * 4]
VerFindFileA:
    jmp ds:[_OriginalFuncs_version + 9 * 4]
VerFindFileW:
    jmp ds:[_OriginalFuncs_version + 10 * 4]
VerInstallFileA:
    jmp ds:[_OriginalFuncs_version + 11 * 4]
VerInstallFileW:
    jmp ds:[_OriginalFuncs_version + 12 * 4]
VerLanguageNameA:
    jmp ds:[_OriginalFuncs_version + 13 * 4]
VerLanguageNameW:
    jmp ds:[_OriginalFuncs_version + 14 * 4]
VerQueryValueA:
    jmp ds:[_OriginalFuncs_version + 15 * 4]
VerQueryValueW:
    jmp ds:[_OriginalFuncs_version + 16 * 4]