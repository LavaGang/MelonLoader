#include <stdlib.h>

// Harmless test dylib: proves dyld honors the test variable in the control build.
__attribute__((constructor)) static void mark_injection(void)
{
    setenv("ML_REGRESSION_INJECTED", "yes", 1);
}
