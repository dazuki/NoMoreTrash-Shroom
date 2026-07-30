using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MelonLoader;

namespace NoMoreTrash
{
    public class ConfigData
    {
        public static MelonPreferences_Category ClearTrash;
        public static MelonPreferences_Category UnknownItems;
        public static MelonPreferences_Entry<bool> Soilbag;
        public static MelonPreferences_Entry<bool> Soilbag2;
        public static MelonPreferences_Entry<bool> Seedvial;
        public static MelonPreferences_Entry<bool> Cuke;
        public static MelonPreferences_Entry<bool> Pgr;
        public static MelonPreferences_Entry<bool> Speedgrow;
        public static MelonPreferences_Entry<bool> Fertilizer;
        public static MelonPreferences_Entry<bool> Plantscrap;
        public static MelonPreferences_Entry<bool> Trashbag;
        public static MelonPreferences_Entry<bool> Soilbag3;
        public static MelonPreferences_Entry<bool> Cigarette;
        public static MelonPreferences_Entry<bool> Usedcigarette;
        public static MelonPreferences_Entry<bool> Cigarettebox;
        public static MelonPreferences_Entry<bool> Coffeecup;
        public static MelonPreferences_Entry<bool> Crushedcuke;
        public static MelonPreferences_Entry<bool> Glassbottle;
        public static MelonPreferences_Entry<bool> Litter1;
        public static MelonPreferences_Entry<bool> Waterbottle;
        public static MelonPreferences_Entry<bool> Energydrink;
        public static MelonPreferences_Entry<bool> Flumedicine;
        public static MelonPreferences_Entry<bool> Gasoline;
        public static MelonPreferences_Entry<bool> Mouthwash;
        public static MelonPreferences_Entry<bool> Motoroil;
        public static MelonPreferences_Entry<bool> Iodine;
        public static MelonPreferences_Entry<bool> Bong;
        public static MelonPreferences_Entry<bool> Syringe;
        public static MelonPreferences_Entry<bool> Pipe;
        public static MelonPreferences_Entry<bool> Chemicaljug;
        public static MelonPreferences_Entry<bool> M1911mag;
        public static MelonPreferences_Entry<bool> Revolvercylinder;
        public static MelonPreferences_Entry<bool> Acid;
        public static MelonPreferences_Entry<bool> Addy;
        public static MelonPreferences_Entry<bool> Phosphorus;
        public static MelonPreferences_Entry<bool> Substratebag;

        public Dictionary<string, bool> TrashItems;

        private readonly MelonLogger.Instance _logger;

        public ConfigData(MelonLogger.Instance logger)
        {
            _logger = logger;
            UnknownItems = MelonPreferences.CreateCategory(
                "NoMoreTrash-Shroom_Unknown",
                "Modded/Unknown Trash"
            );
            ClearTrash = MelonPreferences.CreateCategory(
                "NoMoreTrash-Shroom_Vanilla",
                "Vanilla Trash"
            );

            // Initialize entries
            Trashbag = CreateVanillaEntry("trashbag", "Trash Bag");
            Soilbag = CreateVanillaEntry("soilbag", "Soil");
            Soilbag2 = CreateVanillaEntry("soilbag2", "Long-Life Soil");
            Soilbag3 = CreateVanillaEntry("soilbag3", "Extra Long-Life Soil");
            Seedvial = CreateVanillaEntry("seedvial", "Seed Vials");
            Speedgrow = CreateVanillaEntry("speedgrow", "Speed Grow");
            Fertilizer = CreateVanillaEntry("fertilizer", "Fertilizer");
            Pgr = CreateVanillaEntry("pgr", "PGR");
            Cuke = CreateVanillaEntry("cuke", "Cuke");
            Gasoline = CreateVanillaEntry("gasoline", "Gasoline");
            Mouthwash = CreateVanillaEntry("mouthwash", "Mouth Wash");
            Motoroil = CreateVanillaEntry("motoroil", "Motor Oil");
            Iodine = CreateVanillaEntry("iodine", "Iodine");
            Energydrink = CreateVanillaEntry("energydrink", "Energy Drink");
            Flumedicine = CreateVanillaEntry("flumedicine", "Flu Medicine");
            Plantscrap = CreateVanillaEntry("plantscrap", "Plant Scrap");
            Cigarette = CreateVanillaEntry("cigarette", "Cigarette");
            Usedcigarette = CreateVanillaEntry("usedcigarette", "Used Cigarette");
            Cigarettebox = CreateVanillaEntry("cigarettebox", "Cigarette Pack");
            Coffeecup = CreateVanillaEntry("coffeecup", "Coffe Cup");
            Crushedcuke = CreateVanillaEntry("crushedcuke", "Crushed Cuke");
            Glassbottle = CreateVanillaEntry("glassbottle", "Glass Bottle");
            Litter1 = CreateVanillaEntry("litter1", "Litter");
            Waterbottle = CreateVanillaEntry("waterbottle", "Water Bottle");
            Bong = CreateVanillaEntry("bong", "Bong");
            Syringe = CreateVanillaEntry("syringe", "Syringe");
            Pipe = CreateVanillaEntry("pipe", "Pipe");
            Chemicaljug = CreateVanillaEntry("chemicaljug", "Chemical Jug");
            M1911mag = CreateVanillaEntry("m1911mag", "M1911 Magazine");
            Revolvercylinder = CreateVanillaEntry("revolvercylinder", "Revolver Cylinder");
            Acid = CreateVanillaEntry("acid", "Acid");
            Addy = CreateVanillaEntry("addy", "Addy");
            Phosphorus = CreateVanillaEntry("phosphorus", "Phosphorus");
            Substratebag = CreateVanillaEntry("substratebag", "Mushroom Substrate");

            LoadUnknownItemsFromCfg();
            MelonPreferences.Save();

            Reload();
            SubscribeToChanges();
        }

        // Description carries the item code so mod managers show which prefab an entry maps to.
        private static MelonPreferences_Entry<bool> CreateVanillaEntry(
            string id,
            string displayName
        )
        {
            return ClearTrash.CreateEntry(id, true, displayName, $"ID: {id}");
        }

        public void Reload()
        {
            TrashItems = [];

            var fields = typeof(ConfigData)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(MelonPreferences_Entry<bool>));

            foreach (var field in fields)
            {
                var entry = (MelonPreferences_Entry<bool>)field.GetValue(null);
                if (entry != null)
                {
                    TrashItems[entry.Identifier] = entry.Value;
                }
            }

            foreach (var entry in UnknownItems.Entries)
            {
                if (entry is MelonPreferences_Entry<bool> boolEntry)
                {
                    TrashItems[boolEntry.Identifier] = boolEntry.Value;
                }
            }
        }

        public void ScanTrashPrefabs(string[] ids)
        {
            foreach (string id in ids)
            {
                AddUnknownItem(id);
            }
        }

        public void AddUnknownItem(string id)
        {
            if (TrashItems.ContainsKey(id))
            {
                return;
            }

            MelonPreferences_Entry<bool> entry = CreateUnknownEntry(id);
            TrashItems[id] = entry.Value;
            MelonPreferences.Save();
            _logger.Warning(
                $"Auto-detected unknown trash item '{id}' - added to config (default: off). Enable it in your mod manager."
            );
        }

        private void LoadUnknownItemsFromCfg()
        {
            string cfgPath = Path.Combine(
                System.Environment.CurrentDirectory,
                "UserData",
                "MelonPreferences.cfg"
            );
            if (!File.Exists(cfgPath))
                return;

            bool inSection = false;
            foreach (string line in File.ReadLines(cfgPath))
            {
                string trimmed = line.Trim();
                if (trimmed.StartsWith("["))
                {
                    inSection = trimmed == "[NoMoreTrash-Shroom_Unknown]";
                    continue;
                }
                if (!inSection || string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                    continue;

                int eq = trimmed.IndexOf('=');
                if (eq <= 0)
                    continue;

                string id = trimmed.Substring(0, eq).Trim();
                if (string.IsNullOrEmpty(id))
                    continue;

                CreateUnknownEntry(id);
            }
        }

        private MelonPreferences_Entry<bool> CreateUnknownEntry(string id)
        {
            MelonPreferences_Entry<bool> entry = UnknownItems.CreateEntry(
                id,
                false,
                id,
                $"ID: {id}"
            );
            entry.OnEntryValueChanged.Subscribe(
                (oldValue, newValue) =>
                {
                    TrashItems[entry.Identifier] = newValue;
                    _logger.Msg($"{entry.DisplayName} changed: {oldValue} -> {newValue}");
                }
            );
            return entry;
        }

        private void SubscribeToChanges()
        {
            var fields = typeof(ConfigData)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(MelonPreferences_Entry<bool>));

            foreach (var field in fields)
            {
                var entry = (MelonPreferences_Entry<bool>)field.GetValue(null);
                if (entry != null)
                {
                    entry.OnEntryValueChanged.Subscribe(
                        (oldValue, newValue) =>
                        {
                            TrashItems[entry.Identifier] = newValue;
                            _logger.Msg($"{entry.DisplayName} changed: {oldValue} -> {newValue}");
                        }
                    );
                }
            }
        }
    }
}
