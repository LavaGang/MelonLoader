# Generator helper environment regression

On macOS with the .NET 9 SDK and Xcode command-line tools:

```sh
bash tests/ExecutablePackage/run.sh
```

Set `DOTNET=/absolute/path/to/dotnet` to select an installed SDK. The harness has
no NuGet package dependencies. It builds the actual production
`ExecutablePackage.cs` as linked source, stubbing only the unrelated package
base and logger. Its small native child reports five selected environment
variables; two locally compiled, harmless dylibs set a test marker when loaded.
All generated files live in a temporary directory removed on exit.

Each of the `OSX` and non-`OSX` builds runs five cases: null overrides, empty
overrides, supplied overrides including an injection list, absent inherited
injection, and an override introducing injection. Checks cover removal of the
entire injection list on macOS, unchanged inheritance/override behavior when
`OSX` is not defined, parent environment and caller dictionary preservation,
unrelated variables, the existing bundle extraction setting, and paths with
spaces. The control build confirms the test dylibs actually load, so automatic
dyld environment scrubbing cannot produce a false pass.

This executes the process-launch implementation, not a copied environment
filter. It does not compile the complete loader, exercise a game or Cpp2IL,
prove cross-architecture loader support, or run Windows/Linux binaries. The
non-`OSX` case is a compile-time behavior check on the same Mac.
