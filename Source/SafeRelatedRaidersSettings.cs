using UnityEngine;
using Verse;

namespace SafeRelatedRaiders
{
    public class SafeRelatedRaidersSettings : ModSettings
    {
        public bool enableMod = true;
        public bool showNotifications = true;
        public bool includeSpouses = true;
        public bool includeLovers = true;
        public bool includeParents = true;
        public bool includeChildren = true;
        public bool includeSiblings = true;
        public bool includeBondedAnimals = true;
        public bool includeFianceRelations = true;
        public bool includeExSpouses = false; // By default, don't include ex-relationships
        public bool includeExLovers = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref enableMod, "enableMod", true);
            Scribe_Values.Look(ref showNotifications, "showNotifications", true);
            Scribe_Values.Look(ref includeSpouses, "includeSpouses", true);
            Scribe_Values.Look(ref includeLovers, "includeLovers", true);
            Scribe_Values.Look(ref includeParents, "includeParents", true);
            Scribe_Values.Look(ref includeChildren, "includeChildren", true);
            Scribe_Values.Look(ref includeSiblings, "includeSiblings", true);
            Scribe_Values.Look(ref includeBondedAnimals, "includeBondedAnimals", true);
            Scribe_Values.Look(ref includeFianceRelations, "includeFianceRelations", true);
            Scribe_Values.Look(ref includeExSpouses, "includeExSpouses", false);
            Scribe_Values.Look(ref includeExLovers, "includeExLovers", false);
            base.ExposeData();
        }

        public void DoWindowContents(Rect inRect)
        {
            var listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            
            listingStandard.CheckboxLabeled("Enable Safe Related Raiders", ref enableMod, 
                "Enable or disable the entire mod functionality");
            
            listingStandard.Gap();
            
            if (enableMod)
            {
                listingStandard.CheckboxLabeled("Show notifications when a related raider is saved", ref showNotifications,
                    "Display a message when a raider is saved from death due to relationships");
                
                listingStandard.Gap();
                
                listingStandard.Label("Relationship types to protect:");
                
                listingStandard.CheckboxLabeled("Spouses", ref includeSpouses);
                listingStandard.CheckboxLabeled("Lovers", ref includeLovers);
                listingStandard.CheckboxLabeled("Fiances", ref includeFianceRelations);
                listingStandard.CheckboxLabeled("Parents", ref includeParents);
                listingStandard.CheckboxLabeled("Children", ref includeChildren);
                listingStandard.CheckboxLabeled("Siblings", ref includeSiblings);
                listingStandard.CheckboxLabeled("Bonded animals", ref includeBondedAnimals);
                
                listingStandard.Gap();
                listingStandard.Label("Ex-relationships (usually not recommended):");
                listingStandard.CheckboxLabeled("Ex-spouses", ref includeExSpouses);
                listingStandard.CheckboxLabeled("Ex-lovers", ref includeExLovers);
            }
            
            listingStandard.End();
        }
    }
}
