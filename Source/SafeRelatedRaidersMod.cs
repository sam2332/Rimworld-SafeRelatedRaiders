using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SafeRelatedRaiders
{
    public class SafeRelatedRaidersMod : Mod
    {
        public static SafeRelatedRaidersSettings Settings;

        public SafeRelatedRaidersMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<SafeRelatedRaidersSettings>();
            
            // Apply Harmony patches
            var harmony = new Harmony("SafeRelatedRaiders.ProgrammerLily.com");
            harmony.PatchAll();
            
            Log.Message("[Safe Related Raiders] Mod loaded successfully!");
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Safe Related Raiders";
        }
    }
}
