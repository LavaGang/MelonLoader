using System;
using System.Collections;
using System.Collections.Generic;

namespace MelonLoader
{
	// Modified Version of System.ArraySegment from .NET Framework's mscorlib.dll
	[Serializable]
    public class LemonArraySegment<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
    {
        /// <summary>Gets the original array containing the range of elements that the array segment delimits.</summary>
        /// <returns>The original array that was passed to the constructor, and that contains the range delimited by the <see cref="T:MelonLoader.LemonArraySegment`1" />.</returns>
        public T[] Array { get; private set; }

        /// <summary>Gets the position of the first element in the range delimited by the array segment, relative to the start of the original array.</summary>
        /// <returns>The position of the first element in the range delimited by the <see cref="T:MelonLoader.LemonArraySegment`1" />, relative to the start of the original array.</returns>
        public int Offset { get; private set; }

        /// <summary>Gets the number of elements in the range delimited by the array segment.</summary>
        /// <returns>The number of elements in the range delimited by the <see cref="T:MelonLoader.LemonArraySegment`1" />.</returns>
        public int Count { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="T:MelonLoader.LemonArraySegment`1" /> structure that delimits all the elements in the specified array.</summary>
		/// <param name="array">The array to wrap.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
        public LemonArraySegment(T[] array)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            Array = array;
            Offset = 0;
            Count = array.Length;
        }

        /// <summary>Initializes a new instance of the <see cref="T:MelonLoader.LemonArraySegment`1" /> structure that delimits the specified range of the elements in the specified array.</summary>
		/// <param name="array">The array containing the range of elements to delimit.</param>
		/// <param name="offset">The zero-based index of the first element in the range.</param>
		/// <param name="count">The number of elements in the range.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> or <paramref name="count" /> is less than 0.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="offset" /> and <paramref name="count" /> do not specify a valid range in <paramref name="array" />.</exception>
        public LemonArraySegment(T[] array, int offset, int count)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", "Non-negative number required.");

            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "Non-negative number required.");

            if (array.Length - offset < count)
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

            Array = array;
            Offset = offset;
            Count = count;
        }

		/// <summary>Returns the hash code for the current instance.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			if (Array != null)
				return Array.GetHashCode() ^ Offset ^ Count;
			return 0;
		}

		/// <summary>Determines whether the specified object is equal to the current instance.</summary>
		/// <param name="obj">The object to be compared with the current instance.</param>
		/// <returns>
		///   <see langword="true" /> if the specified object is a <see cref="T:MelonLoader.LemonArraySegment`1" /> structure and is equal to the current instance; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
            => obj is LemonArraySegment<T> && Equals((LemonArraySegment<T>)obj);

        /// <summary>Determines whether the specified <see cref="T:MelonLoader.LemonArraySegment`1" /> structure is equal to the current instance.</summary>
        /// <param name="obj">The structure to compare with the current instance.</param>
        /// <returns>
        ///   <see langword="true" /> if the specified <see cref="T:MelonLoader.LemonArraySegment`1" /> structure is equal to the current instance; otherwise, <see langword="false" />.</returns>
        public bool Equals(LemonArraySegment<T> obj)
            => obj.Array == Array && obj.Offset == Offset && obj.Count == Count;

		/// <summary>Indicates whether two <see cref="T:MelonLoader.LemonArraySegment`1" /> structures are equal.</summary>
		/// <param name="a">The  structure on the left side of the equality operator.</param>
		/// <param name="b">The structure on the right side of the equality operator.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="a" /> is equal to <paramref name="b" />; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(LemonArraySegment<T> a, LemonArraySegment<T> b)
			=> a.Equals(b);

		/// <summary>Indicates whether two <see cref="T:MelonLoader.LemonArraySegment`1" /> structures are unequal.</summary>
		/// <param name="a">The structure on the left side of the inequality operator.</param>
		/// <param name="b">The structure on the right side of the inequality operator.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="a" /> is not equal to <paramref name="b" />; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(LemonArraySegment<T> a, LemonArraySegment<T> b)
			=> !(a == b);

		T IList<T>.this[int index]
		{
			get
			{
				if (Array == null)
					throw new InvalidOperationException("The underlying array is null.");
				if (index < 0 || index >= Count)
					throw new ArgumentOutOfRangeException("index");
				return Array[Offset + index];
			}
			set
			{
				if (Array == null)
					throw new InvalidOperationException("The underlying array is null.");
				if (index < 0 || index >= Count)
					throw new ArgumentOutOfRangeException("index");
				Array[Offset + index] = value;
			}
		}

		int IList<T>.IndexOf(T item)
		{
			if (Array == null)
				throw new InvalidOperationException("The underlying array is null.");
			int num = System.Array.IndexOf(Array, item, Offset, Count);
			if (num < 0)
				return -1;
			return num - Offset;
		}

		void IList<T>.Insert(int index, T item)
			=> throw new NotSupportedException();

		void IList<T>.RemoveAt(int index)
			=> throw new NotSupportedException();

		bool ICollection<T>.IsReadOnly { get => true; }

		void ICollection<T>.Add(T item)
			=> throw new NotSupportedException();

		void ICollection<T>.Clear()
			=> throw new NotSupportedException();

		bool ICollection<T>.Contains(T item)
		{
			if (Array == null)
				throw new InvalidOperationException("The underlying array is null.");
			return System.Array.IndexOf<T>(Array, item, Offset, Count) >= 0;
		}

		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			if (Array == null)
				throw new InvalidOperationException("The underlying array is null.");
			System.Array.Copy(Array, Offset, array, arrayIndex, Count);
		}

		bool ICollection<T>.Remove(T item)
			=> throw new NotSupportedException();

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			if (Array == null)
				throw new InvalidOperationException("The underlying array is null.");
			return new LemonArraySegmentEnumerator(this);
		}

		/// <summary>Returns an enumerator that iterates through an array segment.</summary>
		/// <returns>An enumerator that can be used to iterate through the array segment.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			if (Array == null)
				throw new InvalidOperationException("The underlying array is null.");
			return new LemonArraySegmentEnumerator(this);
		}

		[Serializable]
		private sealed class LemonArraySegmentEnumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			private T[] _array;
			private int _start;
			private int _end;
			private int _current;

			internal LemonArraySegmentEnumerator(LemonArraySegment<T> arraySegment)
			{
				_array = arraySegment.Array;
				_start = arraySegment.Offset;
				_end = _start + arraySegment.Count;
				_current = _start - 1;
			}

			public bool MoveNext()
			{
				if (_current < _end)
				{
					_current++;
					return _current < _end;
				}
				return false;
			}

			public T Current
			{
				get
				{
					if (_current < _start)
						throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
					if (_current >= _end)
						throw new InvalidOperationException("Enumeration already finished.");
					return _array[_current];
				}
			}

			object IEnumerator.Current { get => Current; }

			void IEnumerator.Reset()
				=> _current = _start - 1;

			public void Dispose() { }
		}
	}
}
