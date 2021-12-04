using System.Text;

namespace Semver
{
    internal static class IntExtensions
    {
        /// <summary>
        /// The number of digits in a non-negative number. Returns 1 for all
        /// negative numbers. That is ok because we are using it to calculate
        /// string length for a <see cref="StringBuilder"/> for numbers that
        /// aren't supposed to be negative, but when they are it is just a little
        /// slower.
        /// </summary>
        /// <remarks>
        /// This approach is based on https://stackoverflow.com/a/51099524/268898
        /// where the poster offers performance benchmarks showing this is the
        /// fastest way to get a number of digits.
        /// </remarks>
        public static int Digits(this int n)
        {
            if (n < 10) return 1;
            if (n < 100) return 2;
            if (n < 1_000) return 3;
            if (n < 10_000) return 4;
            if (n < 100_000) return 5;
            if (n < 1_000_000) return 6;
            if (n < 10_000_000) return 7;
            if (n < 100_000_000) return 8;
            if (n < 1_000_000_000) return 9;
            return 10;
        }
    }
}
