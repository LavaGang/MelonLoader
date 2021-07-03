#pragma once

#ifdef _WIN64
#include "detours.x64.h"
#else
#include "detours.x86.h"
#endif