using System;
using System.Collections.Generic;

namespace MelonLoader.MelonStartScreen.UI.Objects
{
    internal abstract class UI_Object : IDisposable
    {
        internal static List<UI_Object> AllElements = new List<UI_Object>();
        private bool disposedValue;

        internal virtual void Dispose() { }
        internal abstract void Render();

        protected virtual void Dispose(bool managed)
        {
            if (disposedValue)
                return;
            if (managed)
                Dispose();
            disposedValue = true;
        }

        void IDisposable.Dispose()
        {
            Dispose(managed: true);
            GC.SuppressFinalize(this);
        }

        internal static void DisposeOfAll()
        {
            if (AllElements.Count <= 0)
                return;
            foreach (UI_Object obj in AllElements)
                obj.Dispose();
            AllElements.Clear();
        }
    }
}
