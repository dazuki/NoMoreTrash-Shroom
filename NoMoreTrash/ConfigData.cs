using MelonLoader;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace NoMoreTrash
{
    public class ConfigData
    {
        public static MelonPreferences_Category clearTrash;
        public static MelonPreferences_Entry<bool> soilbag;
        public static MelonPreferences_Entry<bool> soilbag2;
        public static MelonPreferences_Entry<bool> seedvial;
        public static MelonPreferences_Entry<bool> cuke;
        public static MelonPreferences_Entry<bool> pgr;
        public static MelonPreferences_Entry<bool> speedgrow;
        public static MelonPreferences_Entry<bool> fertilizer;
        public static MelonPreferences_Entry<bool> plantscrap;
        public static MelonPreferences_Entry<bool> trashbag;
        public static MelonPreferences_Entry<bool> soilbag3;
        public static MelonPreferences_Entry<bool> cigarette;
        public static MelonPreferences_Entry<bool> usedcigarette;
        public static MelonPreferences_Entry<bool> cigarettebox;
        public static MelonPreferences_Entry<bool> coffeecup;
        public static MelonPreferences_Entry<bool> crushedcuke;
        public static MelonPreferences_Entry<bool> glassbottle;
        public static MelonPreferences_Entry<bool> litter1;
        public static MelonPreferences_Entry<bool> waterbottle;
        public static MelonPreferences_Entry<bool> energydrink;
        public static MelonPreferences_Entry<bool> flumedicine;
        public static MelonPreferences_Entry<bool> gasoline;
        public static MelonPreferences_Entry<bool> mouthwash;
        public static MelonPreferences_Entry<bool> motoroil;
        public static MelonPreferences_Entry<bool> iodine;
        public static MelonPreferences_Entry<bool> bong;
        public static MelonPreferences_Entry<bool> syringe;
        public static MelonPreferences_Entry<bool> pipe;
        public static MelonPreferences_Entry<bool> chemicaljug;
        public static MelonPreferences_Entry<bool> m1911mag;
        public static MelonPreferences_Entry<bool> revolvercylinder;
        public static MelonPreferences_Entry<bool> acid;
        public static MelonPreferences_Entry<bool> addy;
        public static MelonPreferences_Entry<bool> phosphorus;
        public static MelonPreferences_Entry<bool> substratebag;

        // Trash Limit changer

        public Dictionary<string, bool> TrashItems;

        public ConfigData()
        {
            clearTrash = MelonPreferences.CreateCategory("NoMoreTrash-Shroom", "Clear Trash");

            // Initialize entries
            trashbag = clearTrash.CreateEntry<bool>("trashbag", true, "Trash Bag");
            soilbag = clearTrash.CreateEntry<bool>("soilbag", true, "Soil");
            soilbag2 = clearTrash.CreateEntry<bool>("soilbag2", true, "Long-Life Soil");
            soilbag3 = clearTrash.CreateEntry<bool>("soilbag3", true, "Extra Long-Life Soil");
            seedvial = clearTrash.CreateEntry<bool>("seedvial", true, "Seed Vials");
            speedgrow = clearTrash.CreateEntry<bool>("speedgrow", true, "Speed Grow");
            fertilizer = clearTrash.CreateEntry<bool>("fertilizer", true, "Fertilizer");
            pgr = clearTrash.CreateEntry<bool>("pgr", true, "PGR");
            cuke = clearTrash.CreateEntry<bool>("cuke", true, "Cuke");
            gasoline = clearTrash.CreateEntry<bool>("gasoline", true, "Gasoline");
            mouthwash = clearTrash.CreateEntry<bool>("mouthwash", true, "Mouth Wash");
            motoroil = clearTrash.CreateEntry<bool>("motoroil", true, "Motor Oil");
            iodine = clearTrash.CreateEntry<bool>("iodine", true, "Iodine");
            energydrink = clearTrash.CreateEntry<bool>("energydrink", true, "Energy Drink");
            flumedicine = clearTrash.CreateEntry<bool>("flumedicine", true, "Flu Medicine");
            plantscrap = clearTrash.CreateEntry<bool>("plantscrap", true, "Plant Scrap");
            cigarette = clearTrash.CreateEntry<bool>("cigarette", true, "Cigarette");
            usedcigarette = clearTrash.CreateEntry<bool>("usedcigarette", true, "Used Cigarette");
            cigarettebox = clearTrash.CreateEntry<bool>("cigarettebox", true, "Cigarette Pack");
            coffeecup = clearTrash.CreateEntry<bool>("coffeecup", true, "Coffe Cup");
            crushedcuke = clearTrash.CreateEntry<bool>("crushedcuke", true, "Crushed Cuke");
            glassbottle = clearTrash.CreateEntry<bool>("glassbottle", true, "Glass Bottle");
            litter1 = clearTrash.CreateEntry<bool>("litter1", true, "Litter");
            waterbottle = clearTrash.CreateEntry<bool>("waterbottle", true, "Water Bottle");
            bong = clearTrash.CreateEntry<bool>("bong", true, "Bong");
            syringe = clearTrash.CreateEntry<bool>("syringe", true, "Syringe");
            pipe = clearTrash.CreateEntry<bool>("pipe", true, "Pipe");
            chemicaljug = clearTrash.CreateEntry<bool>("chemicaljug", true, "Chemical Jug");
            m1911mag = clearTrash.CreateEntry<bool>("m1911mag", true, "M1911 Magazine");
            revolvercylinder = clearTrash.CreateEntry<bool>("revolvercylinder", true, "Revolver Cylinder");
            acid = clearTrash.CreateEntry<bool>("acid", true, "Acid");
            addy = clearTrash.CreateEntry<bool>("addy", true, "Addy");
            phosphorus = clearTrash.CreateEntry<bool>("phosphorus", true, "Phosphorus");
            substratebag = clearTrash.CreateEntry<bool>("substratebag", true, "Mushroom Substrate");

            clearTrash.SetFilePath("UserData/NoTrashMod.cfg");
            clearTrash.SaveToFile();

            Reload();
        }

        public void Reload()
        {
            TrashItems = new Dictionary<string, bool>();

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
