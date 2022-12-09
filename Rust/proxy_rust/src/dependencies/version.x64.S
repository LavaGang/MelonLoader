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
    jmp qword ptr [rip + OriginalFuncs_version + 0 * 8]
GetFileVersionInfoByHandle:
    jmp qword ptr [rip + OriginalFuncs_version + 1 * 8]
GetFileVersionInfoExA:
    jmp qword ptr [rip + OriginalFuncs_version + 2 * 8]
GetFileVersionInfoExW:
    jmp qword ptr [rip + OriginalFuncs_version + 3 * 8]
GetFileVersionInfoSizeA:
    jmp qword ptr [rip + OriginalFuncs_version + 4 * 8]
GetFileVersionInfoSizeExA:
    jmp qword ptr [rip + OriginalFuncs_version + 5 * 8]
GetFileVersionInfoSizeExW:
    jmp qword ptr [rip + OriginalFuncs_version + 6 * 8]
GetFileVersionInfoSizeW:
    jmp qword ptr [rip + OriginalFuncs_version + 7 * 8]
GetFileVersionInfoW:
    jmp qword ptr [rip + OriginalFuncs_version + 8 * 8]
VerFindFileA:
    jmp qword ptr [rip + OriginalFuncs_version + 9 * 8]
VerFindFileW:
    jmp qword ptr [rip + OriginalFuncs_version + 10 * 8]
VerInstallFileA:
    jmp qword ptr [rip + OriginalFuncs_version + 11 * 8]
VerInstallFileW:
    jmp qword ptr [rip + OriginalFuncs_version + 12 * 8]
VerLanguageNameA:
    jmp qword ptr [rip + OriginalFuncs_version + 13 * 8]
VerLanguageNameW:
    jmp qword ptr [rip + OriginalFuncs_version + 14 * 8]
VerQueryValueA:
    jmp qword ptr [rip + OriginalFuncs_version + 15 * 8]
VerQueryValueW:
    jmp qword ptr [rip + OriginalFuncs_version + 16 * 8]