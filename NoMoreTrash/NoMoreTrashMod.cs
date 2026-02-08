using UnityEngine;

using HarmonyLib;

using MelonLoader;

using NoMoreTrash;

using System;
using System.Reflection;
using System.Linq;

#if Mono
using ScheduleOne.Trash;
#elif IL2CPP
using Il2CppScheduleOne.Trash;
#endif

[assembly: MelonInfo(typeof(NoMoreTrashMod), "NoMoreTrash-Shroom", "1.0.6", "Voidane (Temporary Fix by DazUki)")]
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
            MelonLogger.Msg($"===========================================");
            MelonLogger.Msg($"Initializing, Original mod created by Voidane.");
            MelonLogger.Msg($"This is a temporary fixed version made by DazUki to work with Shroom update.");
            MelonLogger.Msg($"NoMoreTrash Original: github.com/Voidane/NoMoreTrash");
            MelonLogger.Msg($"Discord: discord.gg/XB7ruKtJje");

            ConfigData = new ConfigData(LoggerInstance);
            InitializeModManager();
            HarmonyPatches();

            MelonLogger.Msg($"NoMoreTrash has been initialized...");
            MelonLogger.Msg($"===========================================");
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

            patcher.Patch(original, null, new HarmonyLib.HarmonyMethod(
                typeof(NoMoreTrashMod).GetMethod(nameof(Patch_TrashItem_Start), BindingFlags.Static | BindingFlags.NonPublic)));
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
                    MelonLogger.Error($"could not find: {__instance.ID} in config");
                    MelonLogger.Error($"This might be a new or custom item that needs to added to the config!");
                    MelonLogger.Error($"Report here: discord.gg/XB7ruKtJje");
                    return; // Don't crash or proceed if not found
                }

                if (value)
                {
                    __instance.DestroyTrash();
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
                _modManagerFound = MelonBase.RegisteredMelons.Any(mod => mod?.Info?.Name == "Mod Manager & Phone App");
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
                var modManagerAssembly = AppDomain.CurrentDomain.GetAssemblies()
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
                var modManagerAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == "ModManager&PhoneApp");

                if (modManagerAssembly != null)
                {
                    var eventsType = modManagerAssembly.GetType("ModManagerPhoneApp.ModSettingsEvents");
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