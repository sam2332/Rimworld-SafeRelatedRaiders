using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SafeRelatedRaiders
{
    /// <summary>
    /// Harmony patch to intercept pawn death and prevent it for related raiders
    /// </summary>
    [HarmonyPatch(typeof(Pawn), "Kill")]
    public static class Pawn_Kill_Patch
    {
        /// <summary>
        /// Prefix patch that runs before the Kill method
        /// Returns false to prevent the original method from running if we save the pawn
        /// </summary>
        public static bool Prefix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit = null)
        {
            // Only proceed if mod is enabled
            if (!SafeRelatedRaidersMod.Settings.enableMod)
                return true;

            // Skip if pawn is already dead or null
            if (__instance?.Dead == true || __instance?.Map == null)
                return true;

            // Only apply to hostile pawns (raiders)
            if (!IsHostileRaider(__instance))
                return true;

            // Check if this pawn has qualifying relationships with colonists
            if (RelationshipUtility.HasQualifyingRelationshipWithColonists(__instance, __instance.Map, out Pawn relatedColonist))
            {
                // Save the pawn by downing them instead of killing
                SavePawnFromDeath(__instance, relatedColonist, dinfo);
                return false; // Prevent the original Kill method from running
            }

            return true; // Allow normal death to proceed
        }

        /// <summary>
        /// Checks if a pawn is a hostile raider that should be subject to this mod's effects
        /// </summary>
        private static bool IsHostileRaider(Pawn pawn)
        {
            // Must be hostile to player
            if (!pawn.HostileTo(Faction.OfPlayer))
                return false;

            // Must not be player faction
            if (pawn.Faction?.IsPlayer == true)
                return false;

            // Must not be a wild animal (unless specifically a bonded animal that turned hostile)
            if (pawn.AnimalOrWildMan() && pawn.Faction == null)
                return false;

            return true;
        }

        /// <summary>
        /// Saves a pawn from death by downing them and applying emergency healing
        /// </summary>
        private static void SavePawnFromDeath(Pawn pawn, Pawn relatedColonist, DamageInfo? dinfo)
        {
            // Force the pawn to be downed instead of killed
            pawn.health.forceDowned = true;
            
            // Heal critical injuries to prevent immediate re-death
            HealCriticalInjuries(pawn);

            // Show notification if enabled
            if (SafeRelatedRaidersMod.Settings.showNotifications && relatedColonist != null)
            {
                ShowSavedNotification(pawn, relatedColonist);
            }

            Log.Message($"[Safe Related Raiders] Saved {pawn.NameShortColored} from death due to relationship with {relatedColonist?.NameShortColored}");
        }

        /// <summary>
        /// Heals critical injuries that would cause immediate death
        /// </summary>
        private static void HealCriticalInjuries(Pawn pawn)
        {
            // Heal brain injuries that would cause death
            var brain = pawn.health.hediffSet.GetBrain();
            if (brain != null)
            {
                var brainInjuries = pawn.health.hediffSet.hediffs
                    .OfType<Hediff_Injury>()
                    .Where(injury => injury.Part == brain && injury.Severity > brain.def.GetMaxHealth(pawn) * 0.8f)
                    .ToList();

                foreach (var injury in brainInjuries)
                {
                    injury.Severity = brain.def.GetMaxHealth(pawn) * 0.7f; // Heal to 70% of max health
                }
            }

            // Heal other critical body parts if needed
            var torso = pawn.health.hediffSet.GetNotMissingParts().FirstOrDefault(part => part.def == BodyPartDefOf.Torso);
            if (torso != null)
            {
                var torsoInjuries = pawn.health.hediffSet.hediffs
                    .OfType<Hediff_Injury>()
                    .Where(injury => injury.Part == torso && injury.Severity > torso.def.GetMaxHealth(pawn) * 0.8f)
                    .ToList();

                foreach (var injury in torsoInjuries)
                {
                    injury.Severity = torso.def.GetMaxHealth(pawn) * 0.7f;
                }
            }
        }

        /// <summary>
        /// Shows a notification that a related raider was saved
        /// </summary>
        private static void ShowSavedNotification(Pawn savedPawn, Pawn relatedColonist)
        {
            var relationDef = RelationshipUtility.GetMostImportantQualifyingRelation(savedPawn, relatedColonist);
            string relationshipName = relationDef?.GetGenderSpecificLabel(savedPawn) ?? "related";

            string message = $"{savedPawn.NameShortColored}, {relatedColonist.NameShortColored}'s {relationshipName}, was spared from death and downed instead.";

            Find.LetterStack.ReceiveLetter(
                "Related Raider Spared",
                message,
                LetterDefOf.NeutralEvent,
                savedPawn);
        }
    }
}
