using System.Text;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Semver;
#pragma warning restore IDE0130 // Namespace does not match folder structure

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
        => n < 10 ? 1 : n < 100 ? 2 : n < 1_000 ? 3 : n < 10_000 ? 4 : n < 100_000 ? 5 : n < 1_000_000 ? 6 : n < 10_000_000 ? 7 : n < 100_000_000 ? 8 : n < 1_000_000_000 ? 9 : 10;
}
