using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using MelonLoader;

namespace NoMoreTrash
{
    public class ConfigData
    {
        public static MelonPreferences_Category ClearTrash;
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

        // Trash Limit changer

        public Dictionary<string, bool> TrashItems;

        public ConfigData()
        {
            ClearTrash = MelonPreferences.CreateCategory("NoMoreTrash-Shroom", "Clear Trash");

            // Initialize entries
            Trashbag = ClearTrash.CreateEntry("trashbag", true, "Trash Bag");
            Soilbag = ClearTrash.CreateEntry("soilbag", true, "Soil");
            Soilbag2 = ClearTrash.CreateEntry("soilbag2", true, "Long-Life Soil");
            Soilbag3 = ClearTrash.CreateEntry("soilbag3", true, "Extra Long-Life Soil");
            Seedvial = ClearTrash.CreateEntry("seedvial", true, "Seed Vials");
            Speedgrow = ClearTrash.CreateEntry("speedgrow", true, "Speed Grow");
            Fertilizer = ClearTrash.CreateEntry("fertilizer", true, "Fertilizer");
            Pgr = ClearTrash.CreateEntry("pgr", true, "PGR");
            Cuke = ClearTrash.CreateEntry("cuke", true, "Cuke");
            Gasoline = ClearTrash.CreateEntry("gasoline", true, "Gasoline");
            Mouthwash = ClearTrash.CreateEntry("mouthwash", true, "Mouth Wash");
            Motoroil = ClearTrash.CreateEntry("motoroil", true, "Motor Oil");
            Iodine = ClearTrash.CreateEntry("iodine", true, "Iodine");
            Energydrink = ClearTrash.CreateEntry("energydrink", true, "Energy Drink");
            Flumedicine = ClearTrash.CreateEntry("flumedicine", true, "Flu Medicine");
            Plantscrap = ClearTrash.CreateEntry("plantscrap", true, "Plant Scrap");
            Cigarette = ClearTrash.CreateEntry("cigarette", true, "Cigarette");
            Usedcigarette = ClearTrash.CreateEntry("usedcigarette", true, "Used Cigarette");
            Cigarettebox = ClearTrash.CreateEntry("cigarettebox", true, "Cigarette Pack");
            Coffeecup = ClearTrash.CreateEntry("coffeecup", true, "Coffe Cup");
            Crushedcuke = ClearTrash.CreateEntry("crushedcuke", true, "Crushed Cuke");
            Glassbottle = ClearTrash.CreateEntry("glassbottle", true, "Glass Bottle");
            Litter1 = ClearTrash.CreateEntry("litter1", true, "Litter");
            Waterbottle = ClearTrash.CreateEntry("waterbottle", true, "Water Bottle");
            Bong = ClearTrash.CreateEntry("bong", true, "Bong");
            Syringe = ClearTrash.CreateEntry("syringe", true, "Syringe");
            Pipe = ClearTrash.CreateEntry("pipe", true, "Pipe");
            Chemicaljug = ClearTrash.CreateEntry("chemicaljug", true, "Chemical Jug");
            M1911mag = ClearTrash.CreateEntry("m1911mag", true, "M1911 Magazine");
            Revolvercylinder = ClearTrash.CreateEntry("revolvercylinder", true, "Revolver Cylinder");
            Acid = ClearTrash.CreateEntry("acid", true, "Acid");
            Addy = ClearTrash.CreateEntry("addy", true, "Addy");
            Phosphorus = ClearTrash.CreateEntry("phosphorus", true, "Phosphorus");
            Substratebag = ClearTrash.CreateEntry("substratebag", true, "Mushroom Substrate");

            ClearTrash.SetFilePath("UserData/NoTrashMod.cfg");
            ClearTrash.SaveToFile();

            Reload();
        }

        public void Reload()
        {
            TrashItems = [];

            // Use reflection to get all static MelonPreferences_Entry<bool> fields
            var fields = typeof(ConfigData)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(MelonPreferences_Entry<bool>));

            foreach (var field in fields)
            {
                var entry = (MelonPreferences_Entry<bool>)field.GetValue(null);
                if (entry != null)
                {
                    // Use the entry's Identifier (key) as the dictionary key
                    TrashItems[entry.Identifier] = entry.Value;
                }
            }
        }
    }
}