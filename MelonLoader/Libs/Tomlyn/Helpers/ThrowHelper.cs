// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license. 
// See license.txt file in the project root for full license information.
using System;

namespace MelonLoader.Tomlyn.Helpers
{
    internal static class ThrowHelper
    {
        public static ArgumentOutOfRangeException GetIndexNegativeArgumentOutOfRangeException() =>
            new ArgumentOutOfRangeException("index", "Index must be positive");

        public static ArgumentOutOfRangeException GetIndexArgumentOutOfRangeException(int maxValue) =>
            new ArgumentOutOfRangeException("index", $"Index must be less than {maxValue}");

        public static InvalidOperationException GetExpectingNoParentException() =>
            new InvalidOperationException("The node is already attached to another parent");
    }
}
