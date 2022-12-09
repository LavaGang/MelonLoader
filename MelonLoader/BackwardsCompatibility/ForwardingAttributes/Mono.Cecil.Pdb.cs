using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(Mono.Cecil.Pdb.NativePdbReader))]
[assembly: TypeForwardedTo(typeof(Mono.Cecil.Pdb.NativePdbWriter))]
[assembly: TypeForwardedTo(typeof(Mono.Cecil.Pdb.NativePdbReaderProvider))]
[assembly: TypeForwardedTo(typeof(Mono.Cecil.Pdb.PdbReaderProvider))]
[assembly: TypeForwardedTo(typeof(Mono.Cecil.Pdb.NativePdbWriterProvider))]
[assembly: TypeForwardedTo(typeof(Mono.Cecil.Pdb.PdbWriterProvider))]

#if !NET6_0
[assembly: TypeForwardedTo(typeof(Microsoft.Cci.ILocalScope))]
[assembly: TypeForwardedTo(typeof(Microsoft.Cci.INamespaceScope))]
[assembly: TypeForwardedTo(typeof(Microsoft.Cci.IUsedNamespace))]
[assembly: TypeForwardedTo(typeof(Microsoft.Cci.IName))]
#endif
