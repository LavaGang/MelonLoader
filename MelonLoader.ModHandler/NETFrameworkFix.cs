namespace System.Reflection
{
    public static class NETFrameworkFix
    {
        public static bool Equals(this Assembly left, Assembly right)
        {
            if ((Object)left == (Object)right)
                return true;
            return left.Equals(right);
        }

        public static bool Equals(this MethodBase left, MethodBase right)
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

        public static bool Equals(this MethodInfo left, MethodInfo right)
        {
            if ((Object)left == (Object)right)
                return true;
            return left.Equals(right);
        }

        public static bool Equals(this Type left, Type right) => ((Object)left == (Object)right);
    }
}
