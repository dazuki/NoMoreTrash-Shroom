using UnityEngine;
using HarmonyLib;
using MelonLoader;
using NoMoreTrash;

using System;
using System.Reflection;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

#if Mono
using ScheduleOne.Trash;
#elif IL2CPP
using Il2CppScheduleOne.Trash;
#endif

[assembly: MelonInfo(typeof(NoMoreTrashMod), "NoMoreTrash-Shroom", "1.0.2b-Shroom", "Voidane (Shroom Update by DazUki)")]
[assembly: MelonGame("TVGS", "Schedule I")]
#if !Mono
[assembly: MelonOptionalDependencies("ModManager&PhoneApp")]
#endif

namespace NoMoreTrash
{
    public class NoMoreTrashMod : MelonMod
    {
        public static ConfigData configData;

        private const string versionCurrent = "1.0.2b-Shroom";
        private const string versionMostUpToDateURL = "https://raw.githubusercontent.com/dazuki/NoMoreTrash-Shroom/refs/heads/main/NoMoreTrash/Version.txt";
        private const string gitURL = "https://github.com/dazuki/NoMoreTrash-Shroom/releases";

        private static readonly HttpClient client = new HttpClient();

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg($"===========================================");
            MelonLogger.Msg($"Initializing, Original mod created by Voidane.");
            MelonLogger.Msg($"This is a fixed version made by DazUki to work with Shroom update.");
            MelonLogger.Msg($"NoMoreTrash Original: github.com/Voidane/NoMoreTrash");
            MelonLogger.Msg($"Discord: discord.gg/XB7ruKtJje");

            configData = new ConfigData();
            InitializeModManager();
            HarmonyPatches();
            _ = CheckForUpdates(); // Fire and forget safely
        }

        public override void OnDeinitializeMelon()
        {
            DeinitializeModManager();
        }

        private async Task CheckForUpdates()
        {
            try
            {
                // Set a timeout to avoid hanging indefinitely
                client.Timeout = TimeSpan.FromSeconds(10);
                string content = await client.GetStringAsync(versionMostUpToDateURL);
                string versionUpdate = content.Trim();

                if (versionCurrent != versionUpdate)
                {
                    MelonLogger.Msg($"New Update for NoMoreTrash-Shroom! {gitURL}, Current: {versionCurrent}, Update: {versionUpdate}");
                }
            }
            catch (Exception e)
            {
                MelonLogger.Msg($"Could not fetch most up to date version: {e.Message}");
            }

            MelonLogger.Msg($"Has been initialized...");
            MelonLogger.Msg($"===========================================");
        }

        private void HarmonyPatches()
        {
            HarmonyLib.Harmony patcher = new HarmonyLib.Harmony("com.voidane.nomoretrash");

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
            if (__instance == null || __instance.transform.parent == null) return;

            if (__instance.transform.parent.gameObject.name.Contains("_Temp"))
            {
                if (!configData.TrashItems.TryGetValue(__instance.ID, out bool value))
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
                ModManagerPhoneApp.ModSettingsEvents.OnPhonePreferencesSaved += HandleSettingsUpdate;
                ModManagerPhoneApp.ModSettingsEvents.OnMenuPreferencesSaved += HandleSettingsUpdate;
                MelonLogger.Msg("Successfully subscribed to Mod Manager events.");
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
            configData.Reload();
        }

        private void DeinitializeModManager()
        {
            if (!_modManagerFound) return;

            try
            {
                ModManagerPhoneApp.ModSettingsEvents.OnPhonePreferencesSaved -= HandleSettingsUpdate;
                ModManagerPhoneApp.ModSettingsEvents.OnMenuPreferencesSaved -= HandleSettingsUpdate;
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