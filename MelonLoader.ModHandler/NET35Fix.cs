using System;
using System.Reflection;

namespace MelonLoader
{
    public class NET35Fix
    {
        public static Boolean Assembly_op_Equality(Assembly left, Assembly right)
        {
            if ((Object)left == (Object)right)
                return true;
            if (left == null ^ right == null)
                return false;
            return left.Equals(right);
        }

        public static Boolean Assembly_op_Inequality(Assembly left, Assembly right)
        {
            if ((Object)left == (Object)right)
                return false;
            if (left == null ^ right == null)
                return true;
            return !left.Equals(right);
        }
    }
}
