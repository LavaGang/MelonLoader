# macOS x64 native dependencies

## libplthook.a

Built from [johanntan/plthook](https://github.com/johanntan/plthook) revision
`5970d2642afb6346cdb957186fcace36f3b1c540` (universal Mach-O chained-fixup correction).
The original `kubo/plthook` repository is archived and cannot accept PRs.
The fix is reviewable in [johanntan/plthook#1](https://github.com/johanntan/plthook/pull/1).
The archive contains only the rebuilt `plthook_osx.o`; updating an existing
archive with a differently named object can leave the old implementation linked.

From a checkout of that exact revision, with Xcode command-line tools:

```sh
xcrun clang -arch x86_64 -mmacosx-version-min=15.0 -O2 -fPIC -c plthook_osx.c -o plthook_osx.o
xcrun ar rcs libplthook-rebuilt.a plthook_osx.o
```

Copy the newly created `libplthook-rebuilt.a` over this directory's `libplthook.a`.
Use a fresh output archive when reproducing the build. The macOS 15.0 deployment
target matches the previously bundled object. Built locally with Apple clang
21.0.0; SDK and compiler versions can affect the resulting archive bytes.

plthook is distributed under the BSD 2-Clause license:

```text
Copyright 2014-2024 Kubo Takehiro <kubo@jiubao.org>

 Redistribution and use in source and binary forms, with or without modification, are
 permitted provided that the following conditions are met:

    1. Redistributions of source code must retain the above copyright notice, this list of
       conditions and the following disclaimer.

    2. Redistributions in binary form must reproduce the above copyright notice, this list
       of conditions and the following disclaimer in the documentation and/or other materials
       provided with the distribution.

 THIS SOFTWARE IS PROVIDED BY THE AUTHORS ''AS IS'' AND ANY EXPRESS OR IMPLIED
 WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
 FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> OR
 CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
```
