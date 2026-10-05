"""Compile the production native copy function with the standalone ARM64 fixture."""
from pathlib import Path
import subprocess
import sys
import tempfile

repository = Path(sys.argv[1] if len(sys.argv) > 1 else Path(__file__).parents[2])
source = repository / "MelonLoader.Bootstrap/OSXEntry/osxentry.cpp"
text = source.read_text()
helper_start = text.index("#if defined(__APPLE__) && defined(__arm64__)\nstatic pthread_mutex_t")
helper_end = text.index("#endif", helper_start) + len("#endif")
function_start = text.index("int MLMacOSJitCopy(void* dst, const void* src, size_t len)\n{")
function_end = text.index("// The game's engine", function_start)
headers = "\n".join(line for line in text.splitlines() if line.startswith("#include"))

with tempfile.TemporaryDirectory(prefix="melonloader-copy-test-") as folder:
    root = Path(folder)
    (root / "copy.cpp").write_text(
        headers + "\n" + text[helper_start:helper_end] + "\nextern \"C\" " +
        text[function_start:function_end]
    )
    executable = root / "test"
    subprocess.run([
        "xcrun", "clang++", "-std=c++11", "-arch", "arm64", "-O2", "-pthread",
        str(root / "copy.cpp"), str(Path(__file__).with_name("jit-copy-test.cpp")),
        "-o", str(executable)
    ], check=True)
    subprocess.run([str(executable)], check=True)
