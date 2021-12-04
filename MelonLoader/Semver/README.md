[![Build Status](https://ci.appveyor.com/api/projects/status/kek3h7gflo3qqidb/branch/master?svg=true)](https://ci.appveyor.com/project/maxhauser/semver/branch/master)
[![NuGet](https://img.shields.io/nuget/v/semver.svg)](https://www.nuget.org/packages/semver/)

A Semantic Version Library for .Net
===================================

This library implements the `SemVersion` class, which
complies with v2.0.0 of the spec from <http://semver.org.>

## Installation

With the NuGet console:

```powershell
Install-Package semver
```

## Parsing

```csharp
var version = SemVersion.Parse("1.1.0-rc.1+nightly.2345");
```

## Comparing

```csharp
if(version >= "1.0")
    Console.WriteLine("released version {0}!", version)
```
