# Zort Save Editor `1.0.0`

![Static Badge](https://img.shields.io/badge/OS-Windows-blue)
![Static Badge](https://img.shields.io/badge/.NET-8.0-512BD4)
![Static Badge](https://img.shields.io/badge/C%2B%2B-17-00599C)
![Static Badge](https://img.shields.io/badge/UI-Qt6-41CD52)
![Static Badge](https://img.shields.io/badge/Game-Zort%20v0.4.9-orange)

An offline save game editor and configuration manager for **Zort v0.4.9**.

Built with C# .NET 8 using native Qt6 for a lightweight, self-contained desktop UI.

## Features

- **Seagate Save (`seagatesave.bin`)**:
  - Edit boat upgrades, repair status, fuel reserve, position coordinates, and supplies (planks, fuel cans, plushie dolls, food).
  - Complete or reset all 16 archipelago islands with one click.
  - Picked collectibles mask and story progression dialog flags.
- **Dynamic Multi-Player Co-op Inventory**:
  - Full support for arbitrary co-op profiles: add/remove player profiles and edit 64-bit Steam IDs.
  - Configure all 5 active loadout slots per player with automatic ammo/item data handling.
- **Game Settings & Finished Maps**:
  - Display settings, resolution, refresh rate / target FPS dropdown, volume sliders, and controls.
  - Finished map flags (`finished.json`) and encountered entities log (`entitylog.txt`).
- **Safety**:
  - Automatically creates timestamped `.bak` backups before modifying any save files on disk.

## Save Location

Zort saves its game data in `%LocalAppData%Low`:

```text
%USERPROFILE%\AppData\LocalLow\Londer Software\Zort\
```

The editor automatically detects this directory on launch. You can also manually browse or select any custom save folder via the toolbar.

## Building from Source

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/)
- Python (3.11=< recommended)
- CMake & Visual Studio 2022 C++ Build Tools

### Build Instructions

1. **Pull Dependencies**:
   ```cmd
   setup
   ```
   *(Pulls minimal Qt 6.8 runtime via `aqtinstall` into `.deps/`)*

2. **Build and Publish**:
   ```cmd
   build
   ```
   *(Compiles native Qt bridge with CMake and publishes self-contained executable to `publish/`)*

The ready-to-run release is placed in `publish\zse.exe`.
