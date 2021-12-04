using System;

namespace MelonLoader
{
	[Serializable]
	public class LemonTuple<T1>
		: object
	{ public T1 Item1; }

	[Serializable]
	public class LemonTuple<T1, T2>
		: LemonTuple<T1>
	{ public T2 Item2; }

	[Serializable]
	public class LemonTuple<T1, T2, T3>
		: LemonTuple<T1, T2>
	{ public T3 Item3; }

	[Serializable]
	public class LemonTuple<T1, T2, T3, T4>
		: LemonTuple<T1, T2, T3>
	{ public T4 Item4; }

	[Serializable]
	public class LemonTuple<T1, T2, T3, T4, T5>
		: LemonTuple<T1, T2, T3, T4>
	{ public T5 Item5; }

	[Serializable]
	public class LemonTuple<T1, T2, T3, T4, T5, T6>
		: LemonTuple<T1, T2, T3, T4, T5>
	{ public T6 Item6; }

	[Serializable]
	public class LemonTuple<T1, T2, T3, T4, T5, T6, T7>
		: LemonTuple<T1, T2, T3, T4, T5, T6>
	{ public T7 Item7; }

	[Serializable]
	public class LemonTuple<T1, T2, T3, T4, T5, T6, T7, T8>
		: LemonTuple<T1, T2, T3, T4, T5, T6, T7>
	{ public T8 Item8; }
}
