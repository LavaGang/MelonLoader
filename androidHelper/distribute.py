import helper.get_tools
import os
import helper.common
import json
import glob
import shutil

save_map = helper.get_tools.default_save_map + [
    (os.path.join(helper.common.Settings.bin_path, os.path.basename(helper.common.Settings.apksigner_path())),
     helper.common.Settings.apksigner_path(), "ApkSignerPath"),
    (os.path.join(helper.common.Settings.bin_path, os.path.basename(helper.common.Settings.keytool_path())),
     helper.common.Settings.keytool_path(), "KeytoolPath"),
]

output_dir = os.path.join(helper.common.Settings.file_path, "dist")
glob_pattern = [
    "distribute.py"
]


def main():
    # get all tools
    m_save_map = []
    for el in save_map:
        m_save_map.append((os.path.join(output_dir, os.path.relpath(el[0]).lstrip(helper.common.Settings.base_dir)), el[1]))

    helper.get_tools.get_tools(m_save_map)

    save_config_name = os.path.join(
        output_dir,
        os.path.realpath(os.path.dirname(helper.common.Settings.config_path)).lstrip(helper.common.Settings.base_dir),
        os.path.basename(helper.common.Settings.config_path)
    )

    # update json
    with open(helper.common.Settings.config_path, "r") as f:
        config = json.load(f)

    for el in save_map:
        if len(el) < 3:
            continue

        save, _, update = el
        if update not in config:
            continue

        config[update] = os.path.relpath(os.path.realpath(save))

    with open(save_config_name, "w") as f:
        json.dump(config, f, indent=4)

    # copy python files
    found_files = glob.glob("**/[!build]*.py") + glob.glob("*.py") + glob.glob("variants/release/**/*.py")
    for file in found_files:
        if not os.path.isdir(os.path.dirname(os.path.join(output_dir, file))):
            os.makedirs(os.path.dirname(os.path.join(output_dir, file)))
        shutil.copyfile(os.path.join(helper.common.Settings.base_dir, file), os.path.join(output_dir, file))

    for pattern in glob_pattern:
        rem = glob.glob(pattern)
        for file in rem:
            os.remove(os.path.join(output_dir, file))

    # fix variants
    with open(os.path.join(output_dir, "variants", "__init__.py"), "r") as f:
        buffer = f.read()

    buffer = buffer.replace(" dev ", " release ")

    with open(os.path.join(output_dir, "variants", "__init__.py"), "w") as f:
        f.write(buffer)


if __name__ == '__main__':
    main()
