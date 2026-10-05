#include <stdio.h>
#include <stdlib.h>

int main(void)
{
    const char *keys[] = { "DYLD_INSERT_LIBRARIES", "ML_REGRESSION_UNRELATED",
        "ML_REGRESSION_OVERRIDE", "ML_REGRESSION_INJECTED", "DOTNET_BUNDLE_EXTRACT_BASE_DIR" };
    for (size_t i = 0; i < sizeof(keys) / sizeof(keys[0]); ++i)
    {
        const char *value = getenv(keys[i]);
        printf("REPORT:%s=%s\n", keys[i], value == NULL ? "<absent>" : value);
    }
    return 0;
}
