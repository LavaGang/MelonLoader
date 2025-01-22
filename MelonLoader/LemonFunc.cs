namespace MelonLoader
{
    public delegate RT LemonFunc<RT>();
    public delegate RT LemonFunc<in T1, RT>(T1 arg1);
    public delegate RT LemonFunc<in T1, in T2, RT>(T1 arg1, T2 arg2);
    public delegate RT LemonFunc<in T1, in T2, in T3, RT>(T1 arg1, T2 arg2, T3 arg3);
    public delegate RT LemonFunc<in T1, in T2, in T3, in T4, RT>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate RT LemonFunc<in T1, in T2, in T3, in T4, in T5, RT>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    public delegate RT LemonFunc<in T1, in T2, in T3, in T4, in T5, in T6, RT>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    public delegate RT LemonFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, RT>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    public delegate RT LemonFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, RT>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
}