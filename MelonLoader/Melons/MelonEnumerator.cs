using System.Collections;

namespace MelonLoader
{
    internal class MelonEnumerator<T> : IEnumerator where T : MelonBase
    {
        private T[] ObjectTable;
        private int CurrentIndex = 0;

        internal MelonEnumerator(T[] objecttable)
            => ObjectTable = objecttable;

        object IEnumerator.Current => Current;
        public T Current { get; private set; }

        public bool MoveNext()
        {
            if ((ObjectTable == null)
                || (ObjectTable.Length <= 0)
                || (CurrentIndex >= ObjectTable.Length))
                return false;

            Current = ObjectTable[CurrentIndex];
            CurrentIndex++;

            return true;
        }

        public void Reset()
        {
            CurrentIndex = 0;
            Current = null;
        }
    }
}
