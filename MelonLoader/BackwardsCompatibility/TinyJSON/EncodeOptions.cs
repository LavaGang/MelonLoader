using System;

namespace MelonLoader.TinyJSON;

[Obsolete("Please use Newtonsoft.Json or System.Text.Json instead. This will be removed in a future version.", true)]
[Flags]
public enum EncodeOptions
{
    None = 0,
    PrettyPrint = 1,
    NoTypeHints = 2,
    IncludePublicProperties = 4,
    EnforceHierarchyOrder = 8,

    [Obsolete("Use EncodeOptions.EnforceHierarchyOrder instead. This will be removed in a future version.", true)]
    EnforceHeirarchyOrder = EnforceHierarchyOrder
}
