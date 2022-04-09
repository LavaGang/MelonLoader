using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(Mono.Cecil.Rocks.DocCommentId))]
[assembly: TypeForwardedTo(typeof(Mono.Cecil.Rocks.IILVisitor))]
[assembly: TypeForwardedTo(typeof(Mono.Cecil.Rocks.ILParser))]
[assembly: TypeForwardedTo(typeof(Mono.Cecil.Rocks.MethodBodyRocks))]
[assembly: TypeForwardedTo(typeof(Mono.Cecil.Rocks.MethodDefinitionRocks))]
[assembly: TypeForwardedTo(typeof(Mono.Cecil.Rocks.ModuleDefinitionRocks))]
[assembly: TypeForwardedTo(typeof(Mono.Cecil.Rocks.ParameterReferenceRocks))]
[assembly: TypeForwardedTo(typeof(Mono.Cecil.Rocks.TypeDefinitionRocks))]
[assembly: TypeForwardedTo(typeof(Mono.Cecil.Rocks.TypeReferenceRocks))]

#if !NET6_0
[assembly: TypeForwardedTo(typeof(Mono.Cecil.Rocks.SecurityDeclarationRocks))]
#endif
