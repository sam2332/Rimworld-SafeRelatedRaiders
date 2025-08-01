using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;

namespace SafeRelatedRaiders
{
    /// <summary>
    /// Harmony patch to intercept pawn death and prevent it for related raiders
    /// </summary>
    [HarmonyPatch(typeof(Pawn), "Kill")]
    public static class Pawn_Kill_Patch
    {
        // Track recently saved pawns to prevent notification spam  
        private static readonly HashSet<Pawn> recentlySavedPawns = new HashSet<Pawn>();
        private static int lastCleanupTick = 0;
        private const int NotificationCooldownTicks = 60; // 1 second at 60 FPS
        private const int CleanupIntervalTicks = 2500; // ~1 minute cleanup interval

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

            // Clean up old entries periodically
            CleanupOldNotifications();

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
        /// Cleans up old notification tracking entries
        /// </summary>
        private static void CleanupOldNotifications()
        {
            int currentTick = Find.TickManager.TicksGame;
            
            // Only cleanup every so often to avoid performance issues
            if (currentTick - lastCleanupTick < CleanupIntervalTicks)
                return;
                
            lastCleanupTick = currentTick;
            
            // Remove pawns that are no longer valid or have been saved too long ago
            recentlySavedPawns.RemoveWhere(pawn => pawn == null || pawn.Destroyed || 
                                          (currentTick - (pawn.GetHashCode() % CleanupIntervalTicks)) > NotificationCooldownTicks);
        }

        /// <summary>
        /// Saves a pawn from death by properly downing them using game mechanics
        /// </summary>
        private static void SavePawnFromDeath(Pawn pawn, Pawn relatedColonist, DamageInfo? dinfo)
        {
            // Use the game's proper downing mechanism - force downed first
            pawn.health.forceDowned = true;
            
            // Damage the pawn's legs to incapacitate movement, like the game does
            DamageLegsToIncapacitate(pawn);
            
            // Heal any immediately lethal injuries
            HealLethalInjuries(pawn);
            
            // Force health state check to ensure downed state sticks
            pawn.health.CheckForStateChange(dinfo, null);

            // Show notification if enabled and not recently shown for this pawn
            if (SafeRelatedRaidersMod.Settings.showNotifications && 
                relatedColonist != null && 
                !recentlySavedPawns.Contains(pawn))
            {
                ShowSavedNotification(pawn, relatedColonist);
                recentlySavedPawns.Add(pawn);
            }

            Log.Message($"[Safe Related Raiders] Saved {pawn.NameShortColored} from death due to relationship with {relatedColonist?.NameShortColored}");
        }

        /// <summary>
        /// Damages legs to incapacitate movement, similar to HealthUtility.DamageLegsUntilIncapableOfMoving
        /// </summary>
        private static void DamageLegsToIncapacitate(Pawn pawn)
        {
            int attempts = 0;
            while (pawn.health.capacities.CapableOf(PawnCapacityDefOf.Moving) && attempts < 50)
            {
                attempts++;
                
                // Find moving limb core parts that can be damaged
                var movingLimbs = pawn.health.hediffSet.GetNotMissingParts()
                    .Where(x => x.def.tags.Contains(BodyPartTagDefOf.MovingLimbCore) && 
                               pawn.health.hediffSet.GetPartHealth(x) >= 2f)
                    .ToList();
                
                if (!movingLimbs.Any())
                    break;
                
                var targetLimb = movingLimbs.RandomElement();
                float maxHealth = targetLimb.def.GetMaxHealth(pawn);
                float partHealth = pawn.health.hediffSet.GetPartHealth(targetLimb);
                
                // Calculate damage amount (12-27% of max health)
                int minDamage = Mathf.Clamp(Mathf.RoundToInt(maxHealth * 0.12f), 1, (int)partHealth - 1);
                int maxDamage = Mathf.Clamp(Mathf.RoundToInt(maxHealth * 0.27f), 1, (int)partHealth - 1);
                int damage = Rand.RangeInclusive(minDamage, maxDamage);
                
                // Use blunt damage to avoid bleeding
                var damageInfo = new DamageInfo(DamageDefOf.Blunt, damage, 999f, -1f, null, targetLimb);
                damageInfo.SetAllowDamagePropagation(false);
                
                // Check if this would kill the pawn - if so, skip
                if (pawn.health.WouldDieAfterAddingHediff(HealthUtility.GetHediffDefFromDamage(DamageDefOf.Blunt, pawn, targetLimb), targetLimb, damage))
                    continue;
                
                pawn.TakeDamage(damageInfo);
            }
        }

        /// <summary>
        /// Heals immediately lethal injuries to prevent re-death
        /// </summary>
        private static void HealLethalInjuries(Pawn pawn)
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
