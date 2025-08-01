using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace SafeRelatedRaiders
{
    public static class RelationshipUtility
    {
        /// <summary>
        /// Checks if a pawn has any qualifying relationships with colonists on the map
        /// </summary>
        public static bool HasQualifyingRelationshipWithColonists(Pawn pawn, Map map, out Pawn relatedColonist)
        {
            relatedColonist = null;
            
            if (!SafeRelatedRaidersMod.Settings.enableMod)
                return false;
                
            if (pawn?.relations == null || map == null)
                return false;

            // Get all player colonists (living, non-hostile, player faction members)
            var colonists = map.mapPawns.FreeColonists.Where(colonist => 
                !colonist.Dead && 
                !colonist.Downed && 
                colonist.Faction?.IsPlayer == true &&
                colonist != pawn).ToList();

            foreach (var colonist in colonists)
            {
                if (HasQualifyingRelationship(pawn, colonist))
                {
                    relatedColonist = colonist;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if two pawns have a qualifying relationship based on mod settings
        /// </summary>
        public static bool HasQualifyingRelationship(Pawn pawn1, Pawn pawn2)
        {
            if (pawn1?.relations == null || pawn2?.relations == null)
                return false;

            var settings = SafeRelatedRaidersMod.Settings;
            
            // Check direct relationships
            foreach (var relation in pawn1.relations.DirectRelations)
            {
                if (relation.otherPawn != pawn2)
                    continue;

                var def = relation.def;

                // Check each relationship type based on settings
                if (settings.includeSpouses && def == PawnRelationDefOf.Spouse)
                    return true;
                    
                if (settings.includeLovers && def == PawnRelationDefOf.Lover)
                    return true;
                    
                if (settings.includeFianceRelations && def == PawnRelationDefOf.Fiance)
                    return true;
                    
                if (settings.includeParents && (def == PawnRelationDefOf.Parent))
                    return true;
                    
                if (settings.includeChildren && (def == PawnRelationDefOf.Child))
                    return true;
                    
                if (settings.includeSiblings && (def == PawnRelationDefOf.Sibling))
                    return true;
                    
                if (settings.includeBondedAnimals && def == PawnRelationDefOf.Bond)
                    return true;
                    
                if (settings.includeExSpouses && def == PawnRelationDefOf.ExSpouse)
                    return true;
                    
                if (settings.includeExLovers && def == PawnRelationDefOf.ExLover)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the most important qualifying relationship between two pawns for display
        /// </summary>
        public static PawnRelationDef GetMostImportantQualifyingRelation(Pawn pawn1, Pawn pawn2)
        {
            if (pawn1?.relations == null || pawn2?.relations == null)
                return null;

            var settings = SafeRelatedRaidersMod.Settings;
            PawnRelationDef mostImportant = null;
            float highestImportance = 0f;

            foreach (var relation in pawn1.relations.DirectRelations)
            {
                if (relation.otherPawn != pawn2)
                    continue;

                var def = relation.def;
                bool qualifies = false;

                // Check if this relation type is enabled and qualifies
                if (settings.includeSpouses && def == PawnRelationDefOf.Spouse) qualifies = true;
                else if (settings.includeLovers && def == PawnRelationDefOf.Lover) qualifies = true;
                else if (settings.includeFianceRelations && def == PawnRelationDefOf.Fiance) qualifies = true;
                else if (settings.includeParents && def == PawnRelationDefOf.Parent) qualifies = true;
                else if (settings.includeChildren && def == PawnRelationDefOf.Child) qualifies = true;
                else if (settings.includeSiblings && def == PawnRelationDefOf.Sibling) qualifies = true;
                else if (settings.includeBondedAnimals && def == PawnRelationDefOf.Bond) qualifies = true;
                else if (settings.includeExSpouses && def == PawnRelationDefOf.ExSpouse) qualifies = true;
                else if (settings.includeExLovers && def == PawnRelationDefOf.ExLover) qualifies = true;

                if (qualifies && def.importance > highestImportance)
                {
                    mostImportant = def;
                    highestImportance = def.importance;
                }
            }

            return mostImportant;
        }
    }
}
