using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MelonLoader;
using NoMoreTrash;
using UnityEngine;
#if Mono
using ScheduleOne.Trash;
#elif IL2CPP
using Il2CppScheduleOne.Trash;
#endif

[assembly: MelonInfo(
    typeof(NoMoreTrashMod),
    "NoMoreTrash-Shroom",
    "1.0.7",
    "Voidane (Temporary Fix by DazUki)"
)]
[assembly: MelonGame("TVGS", "Schedule I")]
[assembly: AssemblyMetadata("NexusModID", "1444")]

#if !Mono
[assembly: MelonOptionalDependencies("ModManager&PhoneApp")]

#endif

namespace NoMoreTrash
{
    public class NoMoreTrashMod : MelonMod
    {
        public static ConfigData ConfigData;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg($"NoMoreTrash (Original");
            MelonLogger.Msg($"- https://github.com/Voidane/NoMoreTrash");
            MelonLogger.Msg($"NoMoreTrash Fork (This Mod)");
            MelonLogger.Msg($"- https://github.com/dazuki/NoMoreTrash-Shroom");
            MelonLogger.Msg($"Voidane Discord");
            MelonLogger.Msg($"- https://discord.gg/XB7ruKtJje");

            ConfigData = new ConfigData(LoggerInstance);
            InitializeModManager();
            HarmonyPatches();

            MelonLogger.Msg($"NoMoreTrash-Shroom has been initialized...");
        }

        public override void OnDeinitializeMelon()
        {
            DeinitializeModManager();
        }

        public override void OnPreferencesSaved()
        {
            ConfigData?.Reload();
        }

        private void HarmonyPatches()
        {
            HarmonyLib.Harmony patcher = new("com.voidane.nomoretrash");

            MethodInfo original = AccessTools.Method(typeof(TrashItem), "Start");
            if (original == null)
            {
                MelonLogger.Error("Failed to find 'Start' method on TrashItem.");
                return;
            }

            patcher.Patch(
                original,
                null,
                new HarmonyLib.HarmonyMethod(
                    typeof(NoMoreTrashMod).GetMethod(
                        nameof(Patch_TrashItem_Start),
                        BindingFlags.Static | BindingFlags.NonPublic
                    )
                )
            );

            MethodInfo tmStart = AccessTools.Method(typeof(TrashManager), "Start");
            if (tmStart != null)
            {
                HarmonyLib.HarmonyMethod scanPostfix = new(
                    typeof(NoMoreTrashMod).GetMethod(
                        nameof(Patch_TrashManager_Start),
                        BindingFlags.Static | BindingFlags.NonPublic
                    )
                );
                scanPostfix.priority = HarmonyLib.Priority.Last;
                patcher.Patch(tmStart, null, scanPostfix);
            }
        }

        private static void Patch_TrashManager_Start(TrashManager __instance)
        {
            if (__instance?.TrashPrefabs == null)
            {
                ConfigData.Log("prefab scan: TrashManager.TrashPrefabs was null, skipped");
                return;
            }

            string[] ids = __instance
                .TrashPrefabs.Where(t => t != null)
                .Select(t => t.ID)
                .Where(id => !string.IsNullOrEmpty(id))
                .ToArray();

            ConfigData.ScanTrashPrefabs(ids);
        }

        private static void Patch_TrashItem_Start(TrashItem __instance)
        {
            if (__instance == null || __instance.transform.parent == null)
            {
                return;
            }

            if (__instance.transform.parent.gameObject.name.Contains("_Temp"))
            {
                if (!ConfigData.TrashItems.TryGetValue(__instance.ID, out bool value))
                {
                    ConfigData.AddUnknownItem(__instance.ID);
                    return;
                }

                if (value)
                {
                    ConfigData.Log($"spawn: destroying '{__instance.ID}'");
                    __instance.DestroyTrash();
                }
                else
                {
                    ConfigData.Log($"spawn: keeping '{__instance.ID}' (disabled in config)");
                }
            }
        }

        // --- Mod Manager Support ---
#if !Mono
        private bool _modManagerFound = false;

        private void InitializeModManager()
        {
            try
            {
                _modManagerFound = MelonBase.RegisteredMelons.Any(mod =>
                    mod?.Info?.Name == "Mod Manager & Phone App"
                );
                if (_modManagerFound)
                {
                    MelonLogger.Msg("Mod Manager detected. Enabling dynamic settings...");
                    SubscribeToModManagerEvents_Helper();
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Error checking for Mod Manager: {ex}");
                _modManagerFound = false;
            }
        }

        private void SubscribeToModManagerEvents_Helper()
        {
            try
            {
                // Use reflection to access ModManager APIs since it's an optional dependency
                var modManagerAssembly = AppDomain
                    .CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == "ModManager&PhoneApp");

                if (modManagerAssembly == null)
                {
                    MelonLogger.Warning("ModManager assembly not found.");
                    _modManagerFound = false;
                    return;
                }

                var eventsType = modManagerAssembly.GetType("ModManagerPhoneApp.ModSettingsEvents");
                if (eventsType == null)
                {
                    MelonLogger.Warning("ModSettingsEvents type not found in ModManager.");
                    _modManagerFound = false;
                    return;
                }

                var onPhoneSavedEvent = eventsType.GetEvent("OnPhonePreferencesSaved");
                var onMenuSavedEvent = eventsType.GetEvent("OnMenuPreferencesSaved");

                if (onPhoneSavedEvent != null && onMenuSavedEvent != null)
                {
                    var handlerDelegate = new Action(HandleSettingsUpdate);
                    onPhoneSavedEvent.AddEventHandler(null, handlerDelegate);
                    onMenuSavedEvent.AddEventHandler(null, handlerDelegate);
                    MelonLogger.Msg("Successfully subscribed to Mod Manager events.");
                }
                else
                {
                    MelonLogger.Warning("Could not find ModManager events.");
                    _modManagerFound = false;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Unexpected error during subscription: {ex}");
                _modManagerFound = false;
            }
        }

        private void HandleSettingsUpdate()
        {
            MelonLogger.Msg("Dynamic settings update triggered.");
            ConfigData.Reload();
        }

        private void DeinitializeModManager()
        {
            if (!_modManagerFound)
            {
                return;
            }

            try
            {
                // Use reflection to unsubscribe from ModManager events
                var modManagerAssembly = AppDomain
                    .CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == "ModManager&PhoneApp");

                if (modManagerAssembly != null)
                {
                    var eventsType = modManagerAssembly.GetType(
                        "ModManagerPhoneApp.ModSettingsEvents"
                    );
                    if (eventsType != null)
                    {
                        var onPhoneSavedEvent = eventsType.GetEvent("OnPhonePreferencesSaved");
                        var onMenuSavedEvent = eventsType.GetEvent("OnMenuPreferencesSaved");

                        if (onPhoneSavedEvent != null && onMenuSavedEvent != null)
                        {
                            var handlerDelegate = new Action(HandleSettingsUpdate);
                            onPhoneSavedEvent.RemoveEventHandler(null, handlerDelegate);
                            onMenuSavedEvent.RemoveEventHandler(null, handlerDelegate);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"Error during unsubscribe: {ex.Message}");
            }
        }
#else
        private void InitializeModManager() { }

        private void DeinitializeModManager() { }
#endif
    }
}
