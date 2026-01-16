> [!IMPORTANT]
> This mod is a fork of the original [**No More Trash**](https://github.com/Voidane/NoMoreTrash) mod by Voidane for Schedule I, providing compatibility with the **Shroom update** until the official version is updated.

<div align="center">

# 🗑️ NoMoreTrash 🗑️

[![Discord](https://img.shields.io/badge/Discord-VOID_Community-7289DA?style=for-the-badge&logo=discord&logoColor=white)](https://discord.gg/XB7ruKtJje)
[![License](https://img.shields.io/badge/LICENSE-MIT-5466b8?style=for-the-badge)](https://opensource.org/licenses/MIT)
[![Downloads](https://img.shields.io/badge/DOWNLOADS-30,000+-00B81F?style=for-the-badge)](https://www.nexusmods.com/schedule1/mods/221)

[![Patreon](https://img.shields.io/badge/Patreon-Support_Me-FF424D?style=for-the-badge&logo=patreon&logoColor=white)](https://www.patreon.com/c/Voidane)

</div>

<div align="center">

### [⬇️ Download from Nexus Mods](https://www.nexusmods.com/schedule1/mods/221)

</div>

## 📋 Overview

**NoMoreTrash** automatically destroys all trash items in the game world as soon as they spawn, giving you a cleaner environment and eliminating the need for manual trash collection.

## ✨ Features

- **Automatic Trash Removal**: All trash items are destroyed immediately upon spawning
- **Zero Configuration**: Works right out of the box with no settings to adjust
- **Performance Friendly**: Reduces entity count and improves game performance

## 📥 Installation

1. Ensure you have MelonLoader installed
2. Download the NoMoreTrash.dll file
3. Place the DLL in your Schedule 1 `Mods` folder
4. Launch the game

## 🔄 Compatibility

- Works with Schedule 1 (current version)
- Compatible with most other mods

## 🛠️ Development Setup

To build this mod from source, you'll need to configure your local game paths:

1. **Copy the configuration template:**

   ```bash
   cp Local.props.example Local.props
   ```

2. **Edit `Local.props` and set your game installation paths:**
   - `MonoGamePath`: Path to your Mono version of Schedule I (if you have it)
   - `IL2CPPGamePath`: Path to your IL2CPP version of Schedule I

   **Example paths:**
   - Windows: `C:\Program Files (x86)\Steam\steamapps\common\Schedule I`
   - Linux: `/home/username/.local/share/Steam/steamapps/common/Schedule I`

3. **Build the project:**

   ```bash
   dotnet build NoMoreTrash.sln
   ```

   Or build for a specific runtime:

   ```bash
   dotnet build -c Mono      # For Mono runtime
   dotnet build -c IL2CPP    # For IL2CPP runtime
   ```

4. **The built DLL will automatically copy to your game's Mods folder** (if paths are configured correctly)

**Note:** If you don't have the game installed or haven't configured the paths, the build will fail with reference errors. This is expected - configure `Local.props` with valid game paths to build successfully.

## 🆘 Support

Having issues? Join our Discord community for support:

- **Discord**: [discord.gg/XB7ruKtJje](https://discord.gg/XB7ruKtJje)

## 👨‍💻 Credits

- Created by Voidane

## ⚖️ License and Usage

This mod is released as fair use. Other modders are welcome to:

- Study and learn from the code
- Incorporate portions of the code into their own projects
- Modify and redistribute the code

**Requirements:**

- Credit must be given to Voidane as the original creator
- Include a link to the original mod or Discord server when redistributing

---

<div align="center">
<i>Note: This mod permanently removes trash items from your game. If trash collection is required for any missions or achievements, this mod may interfere with your ability to complete them.</i>
</div>
