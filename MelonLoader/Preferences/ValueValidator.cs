using System;

namespace MelonLoader.Preferences
{
    public abstract class ValueValidator
    {
        public abstract bool IsValid(object value);
        public abstract object EnsureValid(object value);
    }

    public interface IValueRange
    {
        object MinValue { get; }
        object MaxValue { get; }
    }

    public class ValueRange<T> : ValueValidator, IValueRange where T : IComparable
    {
        public T MinValue { get; }
        public T MaxValue { get; }

        public ValueRange(T minValue, T maxValue)
        {
            if (minValue.CompareTo(maxValue) >= 0)
                throw new ArgumentException($"Min value ({minValue}) must be less than max value ({maxValue})!");

            MinValue = minValue;
            MaxValue = maxValue;
        }

        public override bool IsValid(object value)
            => MaxValue.CompareTo(value) >= 0 && MinValue.CompareTo(value) <= 0;

        public override object EnsureValid(object value)
        {
            if (MaxValue.CompareTo(value) < 0)
                return MaxValue;
            if (MinValue.CompareTo(value) > 0)
                return MinValue;
            return value;
        }

        object IValueRange.MinValue => MinValue;
        object IValueRange.MaxValue => MaxValue;
    }
}
