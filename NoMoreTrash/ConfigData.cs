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
            clearTrash = MelonPreferences.CreateCategory("NoMoreTrash-Shroom", "No More Trash Settings");

            // Initialize entries
            soilbag = clearTrash.CreateEntry<bool>("soilbag", true);
            soilbag2 = clearTrash.CreateEntry<bool>("soilbag2", true);
            seedvial = clearTrash.CreateEntry<bool>("seedvial", true);
            cuke = clearTrash.CreateEntry<bool>("cuke", true);
            pgr = clearTrash.CreateEntry<bool>("pgr", true);
            speedgrow = clearTrash.CreateEntry<bool>("speedgrow", true);
            fertilizer = clearTrash.CreateEntry<bool>("fertilizer", true);
            plantscrap = clearTrash.CreateEntry<bool>("plantscrap", true);
            trashbag = clearTrash.CreateEntry<bool>("trashbag", true);
            soilbag3 = clearTrash.CreateEntry<bool>("soilbag3", true);
            cigarette = clearTrash.CreateEntry<bool>("cigarette", true);
            usedcigarette = clearTrash.CreateEntry<bool>("usedcigarette", true);
            cigarettebox = clearTrash.CreateEntry<bool>("cigarettebox", true);
            coffeecup = clearTrash.CreateEntry<bool>("coffeecup", true);
            crushedcuke = clearTrash.CreateEntry<bool>("crushedcuke", true);
            glassbottle = clearTrash.CreateEntry<bool>("glassbottle", true);
            litter1 = clearTrash.CreateEntry<bool>("litter1", true);
            waterbottle = clearTrash.CreateEntry<bool>("waterbottle", true);
            energydrink = clearTrash.CreateEntry<bool>("energydrink", true);
            flumedicine = clearTrash.CreateEntry<bool>("flumedicine", true);
            gasoline = clearTrash.CreateEntry<bool>("gasoline", true);
            mouthwash = clearTrash.CreateEntry<bool>("mouthwash", true);
            motoroil = clearTrash.CreateEntry<bool>("motoroil", true);
            iodine = clearTrash.CreateEntry<bool>("iodine", true);
            bong = clearTrash.CreateEntry<bool>("bong", true);
            syringe = clearTrash.CreateEntry<bool>("syringe", true);
            pipe = clearTrash.CreateEntry<bool>("pipe", true);
            chemicaljug = clearTrash.CreateEntry<bool>("chemicaljug", true);
            m1911mag = clearTrash.CreateEntry<bool>("m1911mag", true);
            revolvercylinder = clearTrash.CreateEntry<bool>("revolvercylinder", true);
            acid = clearTrash.CreateEntry<bool>("acid", true);
            addy = clearTrash.CreateEntry<bool>("addy", true);
            phosphorus = clearTrash.CreateEntry<bool>("phosphorus", true);
            substratebag = clearTrash.CreateEntry<bool>("substratebag", true);

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
