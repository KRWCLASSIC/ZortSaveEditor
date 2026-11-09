#!/usr/bin/env python3
"""
ZortSaveEditor (zse) - Unified Setup & Build CLI
Commands:
    python zse.py setup  - Download Qt6 runtime dependencies into .deps/qt6
    python zse.py build  - Build C-ABI Qt bridge and publish self-contained .NET 8 binary
"""

import os
import sys
import shutil
import argparse
import subprocess
from pathlib import Path

# Enable ANSI escape sequences on Windows console
if sys.platform == "win32":
    os.system("")

ROOT_DIR = Path(__file__).resolve().parent
DEPS_DIR = ROOT_DIR / ".deps"
QT_DIR = DEPS_DIR / "qt6"
BUILD_DIR = ROOT_DIR / ".build"
PUBLISH_DIR = ROOT_DIR / "publish"

QT_VERSION = "6.8.0"
QT_ARCH = "win64_msvc2022_64"


class Color:
    RESET = "\033[0m"
    BOLD = "\033[1m"
    CYAN = "\033[96m"
    GREEN = "\033[92m"
    YELLOW = "\033[93m"
    RED = "\033[91m"
    DIM = "\033[2m"


def print_banner(text: str):
    line = "=" * 57
    print(f"{Color.CYAN}{line}{Color.RESET}")
    print(f"{Color.CYAN}{text.center(57)}{Color.RESET}")
    print(f"{Color.CYAN}{line}{Color.RESET}")


def print_step(text: str):
    print(f"{Color.GREEN}[*] {text}{Color.RESET}")


def print_success(text: str):
    print(f"{Color.GREEN}[+] {text}{Color.RESET}")


def print_warn(text: str):
    print(f"{Color.YELLOW}[!] {text}{Color.RESET}")


def print_error(text: str):
    print(f"{Color.RED}[!] {text}{Color.RESET}")


def ensure_aqtinstall():
    print_step("Checking for aqtinstall...")
    try:
        import aqt  # noqa: F401
        print_success("aqtinstall is available.")
    except ImportError:
        print_warn("aqtinstall not found. Installing via pip...")
        subprocess.run([sys.executable, "-m", "pip", "install", "aqtinstall"], check=True)
        print_success("aqtinstall successfully installed.")


def fetch_qt6() -> Path:
    target_qt_base = QT_DIR / QT_VERSION / "msvc2022_64"
    if (target_qt_base / "bin" / "Qt6Core.dll").exists():
        print_success(f"Qt {QT_VERSION} already present in {target_qt_base}")
        return target_qt_base

    print_step(f"Fetching minimal Qt {QT_VERSION} ({QT_ARCH}) to {QT_DIR}...")
    DEPS_DIR.mkdir(parents=True, exist_ok=True)

    cmd = [
        sys.executable,
        "-m",
        "aqt",
        "install-qt",
        "windows",
        "desktop",
        QT_VERSION,
        QT_ARCH,
        "--outputdir",
        str(QT_DIR)
    ]
    print(f"{Color.DIM}[*] Running: {' '.join(cmd)}{Color.RESET}")
    try:
        subprocess.run(cmd, check=True)
    finally:
        log_file = ROOT_DIR / "aqtinstall.log"
        if log_file.exists():
            try:
                log_file.unlink()
            except Exception:
                pass

    if not target_qt_base.exists():
        candidates = list(QT_DIR.glob(f"**/{QT_VERSION}/msvc2022_64"))
        if candidates:
            return candidates[0]
        raise RuntimeError(f"Could not locate installed Qt directory inside {QT_DIR}")

    return target_qt_base


def stage_runtime_libs(qt_base: Path, dest_dir: Path):
    dest_dir.mkdir(parents=True, exist_ok=True)
    platforms_dir = dest_dir / "platforms"
    platforms_dir.mkdir(parents=True, exist_ok=True)

    bin_dir = qt_base / "bin"
    plugins_platforms = qt_base / "plugins" / "platforms"

    required_dlls = [
        "Qt6Core.dll",
        "Qt6Gui.dll",
        "Qt6Widgets.dll",
    ]

    for dll_name in required_dlls:
        src = bin_dir / dll_name
        dst = dest_dir / dll_name
        if src.exists():
            shutil.copy2(src, dst)
        else:
            print_warn(f"Source {src} not found!")

    qwindows = plugins_platforms / "qwindows.dll"
    if qwindows.exists():
        shutil.copy2(qwindows, platforms_dir / "qwindows.dll")
    else:
        print_warn(f"Platform plugin {qwindows} not found!")

    print_success(f"Runtime libraries staged directly in {dest_dir}")


def cmd_setup(args=None):
    print_banner("ZortSaveEditor (zse) - Setup Pipeline")
    ensure_aqtinstall()
    qt_base = fetch_qt6()
    stage_runtime_libs(qt_base, PUBLISH_DIR / "lib")
    print()
    print_success("Setup completed successfully!")
    print(f"{Color.GREEN}    Qt Base Location : {qt_base}{Color.RESET}")
    print(f"{Color.GREEN}    Staged In        : {PUBLISH_DIR / 'lib'}{Color.RESET}\n")


def cmd_build(args=None):
    print_banner("Building ZortSaveEditor (zse)")

    # 1. Ensure Qt6 dependencies exist in .deps/qt6
    local_qt = QT_DIR / QT_VERSION / "msvc2022_64"
    if not (local_qt / "bin" / "Qt6Core.dll").exists():
        print_warn("Qt6 dependency not found in .deps/qt6. Running setup first...")
        cmd_setup()
        if not (local_qt / "bin" / "Qt6Core.dll").exists():
            candidates = list(QT_DIR.glob(f"**/{QT_VERSION}/msvc2022_64"))
            if candidates:
                local_qt = candidates[0]
            else:
                sys.exit(f"{Color.RED}[!] Error: Qt6 installation not found.{Color.RESET}")

    # 2. Build C-ABI Qt6 Bridge (zse_bridge.dll) into .build
    print()
    print_step("Configuring and building zse_bridge.dll via CMake...")
    BUILD_DIR.mkdir(parents=True, exist_ok=True)

    cmake_config = [
        "cmake",
        "-B", str(BUILD_DIR),
        "-S", str(ROOT_DIR),
        "-DCMAKE_BUILD_TYPE=Release",
        f"-DCMAKE_PREFIX_PATH={local_qt}"
    ]
    subprocess.run(cmake_config, check=True)

    cmake_build = [
        "cmake",
        "--build", str(BUILD_DIR),
        "--config", "Release"
    ]
    subprocess.run(cmake_build, check=True)

    # Export compile_commands.json to workspace root for language servers (clangd)
    src_compile_cmds = BUILD_DIR / "compile_commands.json"
    if src_compile_cmds.exists():
        shutil.copy2(src_compile_cmds, ROOT_DIR / "compile_commands.json")
        print_success("Staged compile_commands.json to workspace root for language server.")

    # 3. Publish C# .NET 8 Self-Contained Executable
    print()
    print_step("Publishing C# .NET 8 self-contained executable...")
    csproj_path = ROOT_DIR / "ZortSaveEditor.csproj"
    dotnet_cmd = [
        "dotnet",
        "publish",
        str(csproj_path),
        "-c", "Release",
        "-r", "win-x64",
        "--self-contained", "true",
        "-o", str(PUBLISH_DIR)
    ]
    subprocess.run(dotnet_cmd, check=True)

    # 4. Stage runtime Qt DLLs directly into publish/lib/
    print()
    print_step("Staging Qt runtime libraries into publish/lib/...")
    publish_lib = PUBLISH_DIR / "lib"
    stage_runtime_libs(local_qt, publish_lib)

    # Write default settings.json if missing
    publish_settings = PUBLISH_DIR / "settings.json"
    if not publish_settings.exists():
        default_settings = (
            "{\n"
            '  "isDarkMode": true,\n'
            '  "lastSaveDirectory": "",\n'
            '  "createBackups": true\n'
            "}\n"
        )
        publish_settings.write_text(default_settings, encoding="utf-8")

    print()
    line = "=" * 57
    print(f"{Color.CYAN}{line}{Color.RESET}")
    print(f"{Color.GREEN}[+] Build complete!{Color.RESET}")
    print(f"{Color.GREEN}    Executable : {PUBLISH_DIR / 'zse.exe'}{Color.RESET}")
    print(f"{Color.GREEN}    Libraries  : {publish_lib}{Color.RESET}")
    print(f"{Color.GREEN}    Settings   : {publish_settings}{Color.RESET}")
    print(f"{Color.CYAN}{line}{Color.RESET}\n")


def main():
    parser = argparse.ArgumentParser(
        description="ZortSaveEditor (zse) build & setup automation CLI",
        formatter_class=argparse.RawDescriptionHelpFormatter
    )
    subparsers = parser.add_subparsers(dest="command", help="Available commands")

    sub_setup = subparsers.add_parser("setup", help="Download Qt6 dependencies into .deps/qt6")
    sub_setup.set_defaults(func=cmd_setup)

    sub_build = subparsers.add_parser("build", help="Build zse_bridge.dll and publish self-contained .NET 8 app")
    sub_build.set_defaults(func=cmd_build)

    if len(sys.argv) == 1:
        parser.print_help()
        sys.exit(0)

    args = parser.parse_args()
    if hasattr(args, "func"):
        args.func(args)
    else:
        parser.print_help()


if __name__ == "__main__":
    main()
