using System;

namespace MelonLoader.TinyJSON
{
	[Flags]
    [Obsolete("Please use Newtonsoft.Json or System.Text.Json instead. This will be removed in a future version.", true)]
    public enum EncodeOptions
	{
		None = 0,
		PrettyPrint = 1,
		NoTypeHints = 2,
		IncludePublicProperties = 4,
		EnforceHierarchyOrder = 8,

		[Obsolete( "Use EncodeOptions.EnforceHierarchyOrder instead." )]
		EnforceHeirarchyOrder = EnforceHierarchyOrder
	}
}
