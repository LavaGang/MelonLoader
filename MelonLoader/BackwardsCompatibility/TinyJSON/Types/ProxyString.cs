using System;

namespace MelonLoader.TinyJSON
{
    [Obsolete("Please use Newtonsoft.Json or System.Text.Json instead. This will be removed in a future version.", true)]
    public sealed class ProxyString : Variant
	{
		readonly string value;


		public ProxyString( string value )
		{
			this.value = value;
		}


		public override string ToString( IFormatProvider provider )
		{
			return value;
		}
	}
}
