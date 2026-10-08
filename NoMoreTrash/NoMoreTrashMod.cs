using System.Reflection;
using HarmonyLib;
using MelonLoader;
using NoMoreTrash;
using UnityEngine;
#if Mono
using ScheduleOne.DevUtilities;
using ScheduleOne.Persistence;
using ScheduleOne.Trash;
#elif IL2CPP
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Persistence;
using Il2CppScheduleOne.Trash;
#endif

[assembly: MelonInfo(
    typeof(NoMoreTrashMod),
    "NoMoreTrash-Shroom",
    "1.0.8",
    "Voidane (Fork by dazuki)"
)]
[assembly: MelonGame("TVGS", "Schedule I")]
[assembly: AssemblyMetadata("NexusModID", "1444")]

namespace NoMoreTrash;

public class NoMoreTrashMod : MelonMod
{
    internal static ConfigData Config { get; private set; }

    public override void OnInitializeMelon()
    {
        LoggerInstance.Msg("NoMoreTrash: https://github.com/Voidane/NoMoreTrash");
        LoggerInstance.Msg("Shroom Fork: https://github.com/dazuki/NoMoreTrash-Shroom");
        LoggerInstance.Msg("Voidane Discord: https://discord.gg/XB7ruKtJje");

        Config = new ConfigData(LoggerInstance);

        LoggerInstance.Msg("NoMoreTrash-Shroom has been initialized...");
    }
}

// Postfix: DestroyTrash needs the GUID that Initialize registers.
[HarmonyPatch(typeof(TrashItem), nameof(TrashItem.Initialize))]
internal static class TrashItemInitializePatch
{
    private static void Postfix(TrashItem __instance)
    {
        Transform parent = __instance.transform.parent;
        if (parent == null || !parent.name.Contains("_Temp"))
            return;

        ConfigData config = NoMoreTrashMod.Config;
        string id = __instance.ID;
        string keepReason =
            !config.IsEnabled(id) ? "disabled in config"
            : config.IsLoadOnly(id) && !IsLoadingSaveData() ? "load-only, not loading"
            : null;

        if (keepReason != null)
        {
            config.Log($"spawn: keeping '{id}' ({keepReason})");
            return;
        }

        config.Log($"spawn: destroying '{id}'");
        __instance.DestroyTrash();
    }

    // Not IsLoading: that starts before the spawner's first balls.
    private static bool IsLoadingSaveData()
    {
        return Singleton<LoadManager>.InstanceExists
            && Singleton<LoadManager>.Instance.LoadStatus == LoadManager.ELoadStatus.LoadingData;
    }
}

// String target: Start is protected on Mono.
[HarmonyPatch(typeof(TrashManager), "Start")]
internal static class TrashManagerStartPatch
{
    [HarmonyPriority(Priority.Last)]
    private static void Postfix(TrashManager __instance)
    {
        if (__instance.TrashPrefabs == null)
            return;

        string[] ids = __instance
            .TrashPrefabs.Where(t => t != null)
            .Select(t => t.ID)
            .Where(id => !string.IsNullOrEmpty(id))
            .ToArray();

        NoMoreTrashMod.Config.ScanTrashPrefabs(ids);
    }
}
