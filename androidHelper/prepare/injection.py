import os
import helpers
import mmap

smali_directories = ["smali", "smali_assets", "smali_classes2"]

injection_target_path = os.path.join("com", "unity3d", "player", "UnityPlayer.smali")
tab_char = b"    "


def find_line_endings(s):
    line_endings = b"\n"

    s.seek(0)

    if s.find(b"\r\n") >= 0:
        line_endings = b"\r\n"

    return line_endings


def find_section(path, section_fn):
    if isinstance(section_fn, list):
        section_fns = section_fn
    else:
        section_fns = [section_fn]

    with open(path, 'rb', 0) as file:
        with mmap.mmap(file.fileno(), 0, access=mmap.ACCESS_READ) as s:
            for func in section_fns:
                s.seek(0)
                line_endings = find_line_endings(s)

                found_pos = s.find(func)
                if found_pos == -1:
                    continue
                s.seek(found_pos)

                local_pos = s.find(b".locals ")
                if found_pos == -1:
                    continue
                s.seek(local_pos)

                return s.find(line_endings) + len(line_endings)


    return -1


def get_injection_label(key, line_endings, end=False):
    if not end:
        return line_endings + tab_char + b"# INJECTION START - ID " + key + line_endings
    return tab_char + b"# INJECTION END - ID " + key + line_endings


def create_injection(key, code, line_endings):
    msg = b""

    msg += get_injection_label(key, line_endings)
    msg += tab_char + code + line_endings
    msg += get_injection_label(key, line_endings, end=True)

    return msg


def find_injection(path, key, line_endings):
    end_label = get_injection_label(key, line_endings, end=True)

    with open(path, "rb") as file:
        with mmap.mmap(file.fileno(), 0, access=mmap.ACCESS_READ) as s:
            injection_start = s.find(get_injection_label(key, line_endings))
            s.seek(0)
            injection_end = s.find(end_label)

    if injection_start == -1 and injection_end == -1:
        return None

    if injection_start == -1 or injection_end == -1:
        helpers.error("Injection in %s is missing a key")

    return injection_start, injection_end + len(end_label)


def cleanup_injection_s(contents, current_injection):
    start, end = current_injection

    return b"".join(
        [contents[:start],
         contents[-(len(contents)-end):]])


def write_injection(path, loc, key, code):
    with open(path, "rb") as file:
        with mmap.mmap(file.fileno(), 0, access=mmap.ACCESS_READ) as s:
            line_endings = find_line_endings(s)

        file.seek(0)

        contents = file.read()
        size = file.tell()

    current_injection = find_injection(path, key, line_endings)
    if current_injection is not None:
        print("injection found, cleaning up [%s]" % key.decode('utf-8'))
        contents = cleanup_injection_s(contents, current_injection)
        size = len(contents)

    contents = b"".join([contents[:loc], create_injection(key, code, line_endings), contents[-(size - loc):]])

    with open(path, "wb") as f:
        f.write(contents)


def install_injection(path):
    injection_target = None
    default_err = None
    for sm_dir in smali_directories:
        injection_target = os.path.join(path, sm_dir, injection_target_path)

        if not os.path.isfile(injection_target):
            if default_err is None:
                default_err = "Failed to find %s for injection." % injection_target
        else:
            default_err = None
            break

    if default_err is not None:
        helpers.error(default_err)

    if injection_target is None:
        helpers.error("Cannot find injection target")

    write_injection(
        injection_target,
        find_section(injection_target,
                     b".method static constructor <clinit>()V"),
        b"BOOTSTRAP",
        b"invoke-static {}, Lcom/melonloader/InjectionHelper;->Inject()V"
    )

    write_injection(
        injection_target,
        find_section(injection_target,
             [
                 b".method public constructor <init>(Landroid/content/Context;Lcom/unity3d/player/IUnityPlayerLifecycleEvents;)V",
                 b".method public constructor <init>(Landroid/content/Context;)V"
             ]
        ),
        b"CONTEXT LISTENER",
        b"invoke-static {p0}, Lcom/melonloader/ContextHelper;->DefineContext(Landroid/content/Context;)V"
    )

    return True

