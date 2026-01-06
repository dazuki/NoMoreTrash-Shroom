using UnityEngine;
using HarmonyLib;
using MelonLoader;
using NoMoreTrash;

using System;
using System.Reflection;
using System.Net.Http;
using System.Threading.Tasks;

#if Mono
using ScheduleOne.Trash;
#elif IL2CPP
using Il2CppScheduleOne.Trash;
#endif

[assembly: MelonInfo(typeof(NoMoreTrashMod), "NoMoreTrash-Shroom", "1.0.2a-Shroom", "Voidane")]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace NoMoreTrash
{
    public class NoMoreTrashMod : MelonMod
    {
        public static ConfigData configData;

        private const string versionCurrent = "1.0.2a-Shroom";
        private const string versionMostUpToDateURL = "https://raw.githubusercontent.com/Voidane/NoMoreTrash/refs/heads/Il2cpp/NoMoreTrash/Version.txt";
        private const string githubBranchURL = "https://github.com/Voidane/NoMoreTrash";
        private const string nexusURL = "https://www.nexusmods.com/schedule1/mods/221?tab=files";
        
        private static readonly HttpClient client = new HttpClient();

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg($"===========================================");
            MelonLogger.Msg($"Initializing, Created by Voidane.");
            MelonLogger.Msg($"Discord: discord.gg/XB7ruKtJje");

            configData = new ConfigData();
            HarmonyPatches();
            _ = CheckForUpdates(); // Fire and forget safely
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
                    MelonLogger.Msg($"New Update for no trash mod (IL2CPP)! {nexusURL}, Current: {versionCurrent}, Update: {versionUpdate}");
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
    }
}
