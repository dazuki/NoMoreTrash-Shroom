using MelonLoader;
using MelonLoader.Utils;

namespace NoMoreTrash;

public class ConfigData
{
    private const string UnknownCategoryId = "NoMoreTrash-Shroom_Unknown";

    private static readonly TrashType[] VanillaTrash =
    [
        new("trashbag", "Trash Bag"),
        new("soilbag", "Soil"),
        new("soilbag2", "Long-Life Soil"),
        new("soilbag3", "Extra Long-Life Soil"),
        new("seedvial", "Seed Vials"),
        new("speedgrow", "Speed Grow"),
        new("fertilizer", "Fertilizer"),
        new("pgr", "PGR"),
        new("cuke", "Cuke"),
        new("gasoline", "Gasoline"),
        new("mouthwash", "Mouth Wash"),
        new("motoroil", "Motor Oil"),
        new("iodine", "Iodine"),
        new("energydrink", "Energy Drink"),
        new("flumedicine", "Flu Medicine"),
        new("plantscrap", "Plant Scrap"),
        new("cigarette", "Cigarette"),
        new("usedcigarette", "Used Cigarette"),
        new("cigarettebox", "Cigarette Pack"),
        new("coffeecup", "Coffe Cup"),
        new("crushedcuke", "Crushed Cuke"),
        new("glassbottle", "Glass Bottle"),
        new("litter1", "Litter"),
        new("waterbottle", "Water Bottle"),
        new("bong", "Bong"),
        new("syringe", "Syringe"),
        new("pipe", "Pipe"),
        new("chemicaljug", "Chemical Jug"),
        new("m1911mag", "M1911 Magazine"),
        new("revolvercylinder", "Revolver Cylinder"),
        new("acid", "Acid"),
        new("addy", "Addy"),
        new("phosphorus", "Phosphorus"),
        new("substratebag", "Mushroom Substrate"),
        // Its spawner respawns it on destroy, so only clear it while a save loads.
        new("tabletennisball", "Beer Pong Ball", loadOnly: true),
    ];

    private readonly MelonLogger.Instance _logger;
    private readonly MelonPreferences_Entry<bool> _debugLogging;
    private readonly MelonPreferences_Category _unknownCategory;
    private readonly Dictionary<string, MelonPreferences_Entry<bool>> _entries = [];
    private readonly HashSet<string> _loadOnlyIds = [];

    public ConfigData(MelonLogger.Instance logger)
    {
        _logger = logger;

        // Ids are the cfg format, do not rename.
        MelonPreferences_Category general = MelonPreferences.CreateCategory(
            "NoMoreTrash-Shroom_General",
            "General"
        );
        _unknownCategory = MelonPreferences.CreateCategory(
            UnknownCategoryId,
            "Modded/Unknown Trash"
        );
        MelonPreferences_Category vanillaCategory = MelonPreferences.CreateCategory(
            "NoMoreTrash-Shroom_Vanilla",
            "Vanilla Trash"
        );

        _debugLogging = general.CreateEntry(
            "DebugLogging",
            false,
            "Debug Logging",
            "Log extra info for troubleshooting."
        );

        foreach (TrashType trash in VanillaTrash)
        {
            AddEntry(vanillaCategory, trash.Id, trash.Name, true, trash.LoadOnly);
        }

        LoadUnknownItemsFromCfg();
        MelonPreferences.Save();
    }

    // Unknown ids get registered as a new entry (default off).
    public bool IsEnabled(string id)
    {
        if (_entries.TryGetValue(id, out MelonPreferences_Entry<bool> entry))
            return entry.Value;

        AddUnknownItem(id);
        return false;
    }

    public bool IsLoadOnly(string id) => _loadOnlyIds.Contains(id);

    public void Log(string message)
    {
        if (_debugLogging.Value)
            _logger.Msg($"[Debug] {message}");
    }

    public void ScanTrashPrefabs(string[] ids)
    {
        Log($"prefab scan: TrashManager exposes {ids.Length} ids [{string.Join(", ", ids)}]");

        foreach (string id in ids)
        {
            AddUnknownItem(id);
        }
    }

    private void AddUnknownItem(string id)
    {
        if (_entries.ContainsKey(id))
            return;

        AddEntry(_unknownCategory, id, id, false);
        MelonPreferences.Save();
        _logger.Warning(
            $"Auto-detected unknown trash item '{id}' - added to config (default: off). Enable it in your mod manager."
        );
    }

    private void AddEntry(
        MelonPreferences_Category category,
        string id,
        string displayName,
        bool defaultValue,
        bool loadOnly = false
    )
    {
        string note = loadOnly ? " - Only cleared while a save loads, never during play." : "";
        MelonPreferences_Entry<bool> entry = category.CreateEntry(
            id,
            defaultValue,
            displayName,
            $"ID: {id}{note}"
        );
        entry.OnEntryValueChanged.Subscribe(
            (oldValue, newValue) => _logger.Msg($"{displayName} changed: {oldValue} -> {newValue}")
        );
        _entries[id] = entry;
        if (loadOnly)
            _loadOnlyIds.Add(id);
    }

    // MelonPreferences can't list keys that have no entry yet, so read the cfg directly.
    private void LoadUnknownItemsFromCfg()
    {
        string cfgPath = Path.Combine(MelonEnvironment.UserDataDirectory, "MelonPreferences.cfg");
        if (!File.Exists(cfgPath))
            return;

        bool inSection = false;
        foreach (string line in File.ReadLines(cfgPath))
        {
            string trimmed = line.Trim();
            if (trimmed.StartsWith("["))
            {
                inSection = trimmed == $"[{UnknownCategoryId}]";
                continue;
            }

            int eq = trimmed.IndexOf('=');
            if (!inSection || trimmed.StartsWith("#") || eq <= 0)
                continue;

            string id = trimmed[..eq].Trim();

            // Promoted to vanilla since it was auto-detected.
            if (_entries.ContainsKey(id))
            {
                _unknownCategory.DeleteEntry(id);
                _logger.Msg($"Migrated '{id}' from Modded/Unknown to Vanilla Trash.");
                continue;
            }

            AddEntry(_unknownCategory, id, id, false);
        }
    }

    // Not a record: netstandard2.1 lacks IsExternalInit.
    private sealed class TrashType(string id, string name, bool loadOnly = false)
    {
        public string Id { get; } = id;
        public string Name { get; } = name;
        public bool LoadOnly { get; } = loadOnly;
    }
}
