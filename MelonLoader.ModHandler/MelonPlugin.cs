using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MelonLoader
{
    public abstract class MelonPlugin : MelonBase
    {
        public virtual void OnPreInitialization() { }
    }
}
