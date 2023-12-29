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
    jmp qword ptr [rip + OriginalFuncs + 0 * 8]
GetFileVersionInfoByHandle:
    jmp qword ptr [rip + OriginalFuncs + 1 * 8]
GetFileVersionInfoExA:
    jmp qword ptr [rip + OriginalFuncs + 2 * 8]
GetFileVersionInfoExW:
    jmp qword ptr [rip + OriginalFuncs + 3 * 8]
GetFileVersionInfoSizeA:
    jmp qword ptr [rip + OriginalFuncs + 4 * 8]
GetFileVersionInfoSizeExA:
    jmp qword ptr [rip + OriginalFuncs + 5 * 8]
GetFileVersionInfoSizeExW:
    jmp qword ptr [rip + OriginalFuncs + 6 * 8]
GetFileVersionInfoSizeW:
    jmp qword ptr [rip + OriginalFuncs + 7 * 8]
GetFileVersionInfoW:
    jmp qword ptr [rip + OriginalFuncs + 8 * 8]
VerFindFileA:
    jmp qword ptr [rip + OriginalFuncs + 9 * 8]
VerFindFileW:
    jmp qword ptr [rip + OriginalFuncs + 10 * 8]
VerInstallFileA:
    jmp qword ptr [rip + OriginalFuncs + 11 * 8]
VerInstallFileW:
    jmp qword ptr [rip + OriginalFuncs + 12 * 8]
VerLanguageNameA:
    jmp qword ptr [rip + OriginalFuncs + 13 * 8]
VerLanguageNameW:
    jmp qword ptr [rip + OriginalFuncs + 14 * 8]
VerQueryValueA:
    jmp qword ptr [rip + OriginalFuncs + 15 * 8]
VerQueryValueW:
    jmp qword ptr [rip + OriginalFuncs + 16 * 8]