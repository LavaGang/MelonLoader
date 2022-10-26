using System;

namespace MelonLoader
{
	[Serializable]
	public class LemonTuple<T1>
		: object
	{
		public T1 Item1;
		public LemonTuple() { }
		public LemonTuple(T1 item1) 
		{ Item1 = item1; }
	}

	[Serializable]
	public class LemonTuple<T1, T2>
		: LemonTuple<T1>
	{
		public T2 Item2;
		public LemonTuple() : base() { }
		public LemonTuple(T1 item1, T2 item2) 
			: base(item1) 
		{ Item2 = item2; }
	}

	[Serializable]
	public class LemonTuple<T1, T2, T3>
		: LemonTuple<T1, T2>
	{
		public T3 Item3;
		public LemonTuple() : base() { }
		public LemonTuple(T1 item1, T2 item2, T3 item3)
			: base(item1, item2)
		{ Item3 = item3; }
	}

	[Serializable]
	public class LemonTuple<T1, T2, T3, T4>
		: LemonTuple<T1, T2, T3>
	{
		public T4 Item4;
		public LemonTuple() : base() { }
		public LemonTuple(T1 item1, T2 item2, T3 item3, T4 item4)
			: base(item1, item2, item3)
		{ Item4 = item4; }
	}

	[Serializable]
	public class LemonTuple<T1, T2, T3, T4, T5>
		: LemonTuple<T1, T2, T3, T4>
	{
		public T5 Item5;
		public LemonTuple() : base() { }
		public LemonTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
			: base(item1, item2, item3, item4)
		{ Item5 = item5; }
	}

	[Serializable]
	public class LemonTuple<T1, T2, T3, T4, T5, T6>
		: LemonTuple<T1, T2, T3, T4, T5>
	{
		public T6 Item6;
		public LemonTuple() : base() { }
		public LemonTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
			: base(item1, item2, item3, item4, item5)
		{ Item6 = item6; }
	}

	[Serializable]
	public class LemonTuple<T1, T2, T3, T4, T5, T6, T7>
		: LemonTuple<T1, T2, T3, T4, T5, T6>
	{
		public T7 Item7;
		public LemonTuple() : base() { }
		public LemonTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
			: base(item1, item2, item3, item4, item5, item6)
		{ Item7 = item7; }
	}

	[Serializable]
	public class LemonTuple<T1, T2, T3, T4, T5, T6, T7, T8>
		: LemonTuple<T1, T2, T3, T4, T5, T6, T7>
	{
		public T8 Item8;
		public LemonTuple() : base() { }
		public LemonTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8)
			: base(item1, item2, item3, item4, item5, item6, item7)
		{ Item8 = item8; }
	}

	[Serializable]
	public class LemonTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9>
		: LemonTuple<T1, T2, T3, T4, T5, T6, T7, T8>
	{
		public T9 Item9;
		public LemonTuple() : base() { }
		public LemonTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9)
			: base(item1, item2, item3, item4, item5, item6, item7, item8)
		{ Item9 = item9; }
	}

	[Serializable]
	public class LemonTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
		: LemonTuple<T1, T2, T3, T4, T5, T6, T7, T8, T9>
	{
		public T10 Item10;
		public LemonTuple() : base() { }
		public LemonTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9, T10 item10)
			: base(item1, item2, item3, item4, item5, item6, item7, item8, item9)
		{ Item10 = item10; }
	}
}
