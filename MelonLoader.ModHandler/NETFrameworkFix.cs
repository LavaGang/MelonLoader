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
            MethodInfo methodInfo = left as MethodInfo;
            MethodInfo methodInfo1 = methodInfo;
            if (methodInfo != null)
            {
                MethodInfo methodInfo2 = right as MethodInfo;
                MethodInfo methodInfo3 = methodInfo2;
                if (methodInfo2 != null)
                    return methodInfo1 == methodInfo3;
            }
            ConstructorInfo constructorInfo = left as ConstructorInfo;
            ConstructorInfo constructorInfo1 = constructorInfo;
            if (constructorInfo != null)
            {
                ConstructorInfo constructorInfo2 = right as ConstructorInfo;
                ConstructorInfo constructorInfo3 = constructorInfo2;
                if (constructorInfo2 != null)
                    return constructorInfo1 == constructorInfo3;
            }
            return false;
        }
        public static Boolean MethodBase_op_Inequality(MethodBase left, MethodBase right) =>  !MethodBase_op_Equality(left, right);

        public static Boolean MethodInfo_op_Equality(MethodInfo left, MethodInfo right)
        {
            if ((Object)left == (Object)right)
                return true;
            return left.Equals(right);
        }
        public static Boolean MethodInfo_op_Inequality(MethodInfo left, MethodInfo right) => !MethodBase_op_Equality(left, right);

        public static Boolean Type_op_Equality(Type left, Type right) => ((Object)left == (Object)right);
        public static Boolean Type_op_Inequality(Type left, Type right) => !Type_op_Equality(left, right);
    }
}
