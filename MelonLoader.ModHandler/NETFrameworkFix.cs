using System;
using System.Reflection;

namespace MelonLoader
{
    public class NETFrameworkFix
    {
        public static Boolean Assembly_op_Equality(Assembly left, Assembly right)
        {
            if ((Object)left == (Object)right)
                return true;
            return left.Equals(right);
        }
        public static Boolean Assembly_op_Inequality(Assembly left, Assembly right) => !Assembly_op_Equality(left, right);

        public static Boolean MethodBase_op_Equality(MethodBase left, MethodBase right)
        {
            if ((Object)left == (Object)right)
                return true;
            return left.Equals(right);
        }
        public static Boolean MethodBase_op_Inequality(MethodBase left, MethodBase right) =>  !MethodBase_op_Equality(left, right);

        public static Boolean MethodInfo_op_Equality(MethodInfo left, MethodInfo right)
        {
            if ((Object)left == (Object)right)
                return true;
            return left.Equals(right);
        }
        public static Boolean MethodInfo_op_Inequality(MethodInfo left, MethodInfo right) => !MethodBase_op_Equality(left, right);

        public static Boolean Type_op_Equality(Type left, Type right)
        {
            if ((Object)left == (Object)right)
                return true;
            return left.Equals(right);
        }
        public static Boolean Type_op_Inequality(Type left, Type right) => !Type_op_Equality(left, right);
    }
}
