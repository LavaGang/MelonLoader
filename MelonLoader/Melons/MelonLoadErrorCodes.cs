using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MelonLoader
{
    public enum MelonLoadErrorCodes
    {
        None,
        InvalidPath,
        WrongFileExtension,
        FailedToReadFile,
        FailedToLoadAssembly,
        AssemblyIsNull,
        ModNotSupported,
        InvalidMelonType,
        FailedToInitializeMelon,
    }
}
