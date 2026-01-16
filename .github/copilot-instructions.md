# NoMoreTrash-Shroom Development Guide

## Project Overview

MelonLoader mod for Schedule I game that automatically destroys trash items on spawn. This is a temporary fork maintaining Shroom update compatibility until the official version by Voidane is updated.

**Key Architecture:**

- Harmony patches target `TrashItem.Start()` method to intercept trash spawning
- Dual runtime support: Mono (netstandard2.1) and IL2CPP (net6.0) with conditional compilation
- MelonPreferences-based configuration system with reflection-driven reload capability
- Optional ModManager integration for dynamic settings (IL2CPP only)

## Build System

### Initial Setup

1. Copy `Local.props.example` to `Local.props` and configure game paths:
   - `MonoGamePath`: Full path to Mono game installation
   - `IL2CPPGamePath`: Full path to IL2CPP game installation
2. Build automatically copies DLL to appropriate `Mods` directory (if paths exist)
3. **Without valid paths**: Build will fail with CS0246 errors (expected - game references unavailable)

**Critical:** `Local.props` contains user-specific paths and is in `.gitignore`. Always use `Local.props.example` as template.

**Reference paths for this project owner** (hidden in .gitignore):

- `MonoGamePath`: `/home/dazuki/Games/Schedule I Mono`
- `IL2CPPGamePath`: `/home/dazuki/Games/Schedule I`

### Build Configurations

- Solution defines 4 configurations: `Mono|AnyCPU`, `Mono|x64`, `IL2CPP|AnyCPU`, `IL2CPP|x64`
- Target framework determines output: `netstandard2.1` → Mono, `net6.0` → IL2CPP
- Output names: `NoMoreTrash.Mono.dll` and `NoMoreTrash.Il2cpp.dll`

**Build command:**

```bash
# Build both runtimes
dotnet build NoMoreTrash.sln

# Build specific configuration
dotnet build -c Mono
dotnet build -c IL2CPP
```

## Code Patterns

### Conditional Compilation

Use `#if Mono / #elif IL2CPP` for runtime-specific code:

```csharp
#if Mono
using ScheduleOne.Trash;
#elif IL2CPP
using Il2CppScheduleOne.Trash;
#endif
```

IL2CPP assemblies are prefixed with `Il2Cpp` (e.g., `Il2CppFishNet.Runtime`, `Il2CppNewtonsoft.Json`).

### Assembly Attributes

Located in [NoMoreTrashMod.cs](NoMoreTrash/NoMoreTrashMod.cs#L18-L22):

- `MelonInfo`: Update version in 3 places (assembly attribute, `versionCurrent`, `Version.txt`)
- `MelonGame`: Identifies target game ("TVGS", "Schedule I")
- `AssemblyMetadata("NexusModID")`: Links to Nexus Mods page
- `MelonOptionalDependencies`: ModManager support (IL2CPP only)

### Configuration System

[ConfigData.cs](NoMoreTrash/ConfigData.cs) uses reflection to dynamically load entries:

1. Static `MelonPreferences_Entry<bool>` fields define each trash item
2. `Reload()` reflects over fields to populate `TrashItems` dictionary
3. Configuration saved to `UserData/NoTrashMod.cfg`
4. Add new items by: creating static field → initializing in constructor → reflection handles rest

### Harmony Patching

[NoMoreTrashMod.cs](NoMoreTrash/NoMoreTrashMod.cs#L86-L100) postfix patches `TrashItem.Start()`:

- Only affects items with `_Temp` parent (dynamically spawned trash)
- Checks `configData.TrashItems[__instance.ID]` before destroying
- Logs errors for unknown IDs (helps identify new items post-updates)

### Async Version Check

Fire-and-forget async pattern in `CheckForUpdates()`:

- Uses `HttpClient` with 10-second timeout
- Fetches `Version.txt` from GitHub raw URL
- Never blocks initialization (async Task without await)

## ModManager Integration

IL2CPP builds optionally integrate with "Mod Manager & Phone App":

1. Check for mod existence via `MelonBase.RegisteredMelons`
2. Use reflection to access ModManager APIs (avoid direct references for optional dependencies)
3. Subscribe to events dynamically via `eventsType.GetEvent()` and `AddEventHandler()`
4. Call `configData.Reload()` on setting changes
5. Mono builds skip this (empty stubs)

**Critical:** ModManager integration uses reflection to avoid compile-time dependency on optional ModManager DLL. This allows the mod to compile and run whether ModManager is installed or not.

**Critical:** Mono excludes this entirely via `#if !Mono` blocks.

## Adding New Trash Items

When game updates introduce new trash:

1. Add static field in [ConfigData.cs](NoMoreTrash/ConfigData.cs): `public static MelonPreferences_Entry<bool> newitem;`
2. Initialize in constructor: `newitem = clearTrash.CreateEntry<bool>("newitem", true, "Display Name");`
3. Reflection automatically includes it in `TrashItems` dictionary
4. Test by observing logs for "could not find: {ID}" errors

### Discovering New Trash Item IDs

After game updates, identify new trash items:

1. **Launch game with mod enabled** and monitor MelonLoader console
2. **Play normally** to trigger trash spawning (wait, move around, perform activities)
3. **Check console for error messages**: `"could not find: {ID} in config"`
4. **Note the unknown ID** - this is the new trash item identifier
5. **Add to ConfigData.cs** following the pattern above
6. **Rebuild and test** to verify the new item is now handled

The Harmony patch intentionally logs unknown IDs without crashing, making discovery safe during gameplay.

## Testing Workflow

Test trash destruction behavior ingame:

1. **Build mod** for target runtime (Mono or IL2CPP)
2. **Launch game** - DLL auto-copies to Mods folder via PostBuild
3. **Monitor MelonLoader console** for initialization messages and errors
4. **Trigger trash spawning** by playing normally or waiting for environmental spawns
5. **Verify destruction** - trash should disappear immediately on spawn
6. **Check configuration** - Test toggling items in `UserData/NoTrashMod.cfg` and reloading
7. **ModManager test** (IL2CPP only) - Use phone app to change settings dynamically

## Common Issues

**Build fails with "missing Local.props":**

- Copy `Local.props.example` to `Local.props` and configure game paths
- File is gitignored to prevent committing user-specific paths

**Build fails with CS0246 errors (type or namespace not found):**

- Expected when game paths aren't configured in `Local.props`
- Solution: Set `MonoGamePath` and/or `IL2CPPGamePath` to valid game installations
- References are conditionally loaded only when paths exist and DLLs are present

**MSB3245 warnings about unresolved references:**

- Fixed in current version - references now use `Condition="Exists(...)"` attributes
- If seeing these, ensure you have the latest .csproj changes

**Wrong DLL copied to Mods folder:**

- Check target framework: `netstandard2.1` = Mono, `net6.0` = IL2CPP
- PostBuild event uses `$(TargetFramework)` condition to determine destination
- Skips copy if path doesn't exist (with informative message)

**Trash not being destroyed:**

- Check parent object name contains `_Temp` (only dynamic spawns are targeted)
- Verify item ID exists in `configData.TrashItems` (check logs)
- Confirm configuration value is `true` in `UserData/NoTrashMod.cfg`

## Version Management

Update version in 3 synchronized locations:

1. [NoMoreTrashMod.cs](NoMoreTrash/NoMoreTrashMod.cs#L18) `MelonInfo` assembly attribute
2. [NoMoreTrashMod.cs](NoMoreTrash/NoMoreTrashMod.cs#L28) `versionCurrent` constant
3. [Version.txt](NoMoreTrash/Version.txt) for update checker

GitHub raw URL must point to correct branch/repo in `versionMostUpToDateURL`.
