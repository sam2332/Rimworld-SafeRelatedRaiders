# Safe Related Raiders - Development Scratch Pad

## Key RimWorld Code Discoveries

### Death Handling
- `Pawn.Kill()` method is the main entry point for pawn death
- Located in `Verse.Pawn` class
- Method signature: `public override void Kill(DamageInfo? dinfo, Hediff exactCulprit = null)`
- Process includes: DoKillSideEffects, PreDeathPawnModifications, DropBeforeDying, health.SetDead()

### Relationship System
- `Pawn_RelationsTracker` manages all relationships for a pawn
- `DirectRelations` property contains list of `DirectPawnRelation` objects
- Each relation has `.def` (PawnRelationDef) and `.otherPawn` (Pawn)
- Key relation defs: Spouse, Lover, Fiance, Parent, Child, Sibling, Bond, ExSpouse, ExLover

### Health System
- `Pawn_HealthTracker` manages pawn health
- `forceDowned` property can force a pawn to be downed instead of dead
- `CheckForStateChange()` method handles transitions between states
- Brain injuries are critical - use `hediffSet.GetBrain()` to find brain part

### Body Parts
- Use `BodyPartDefOf.Torso` not `BodyPartTagDefOf.Torso` (compilation error fixed)
- Brain access: `pawn.health.hediffSet.GetBrain()`
- Torso access: find by `part.def == BodyPartDefOf.Torso`
- Critical injury healing threshold: 80% max health, heal to 70%

### Faction and Hostility
- Check hostility: `pawn.HostileTo(Faction.OfPlayer)`
- Player faction: `Faction.OfPlayer`
- Map colonists: `map.mapPawns.FreeColonists`

## Current Issues Fixed (January 2025)

### Problem 1: Too Many Notifications
**Issue**: Mod was sending a letter for every raider saved, causing notification spam
**Solution**: Added notification throttling system:
- Track recently saved pawns in `HashSet<Pawn> recentlySavedPawns`
- Only send notification if pawn wasn't recently saved
- Periodic cleanup of tracking set every ~1 minute

### Problem 2: Raiders Not Staying Down  
**Issue**: Setting `forceDowned = true` wasn't sufficient - pawns would get back up
**Solution**: Implemented proper incapacitation like the game does:
- Use `HealthUtility.DamageLegsUntilIncapableOfMoving()` pattern
- Damage moving limb core parts with blunt damage (12-27% of max health)
- Check `WouldDieAfterAddingHediff()` before damaging to prevent accidental death
- Set `forceDowned = true` AND actually incapacitate through injury

### Key Code Changes
1. Added `using UnityEngine;` for `Mathf` access
2. Fixed syntax error (extra `{` brace)
3. Added `CleanupOldNotifications()` method
4. Replaced simple `forceDowned` with proper `DamageLegsToIncapacitate()` method
5. Added notification cooldown tracking with `recentlySavedPawns` HashSet
6. Improved notification system to prevent spam

### Implementation Notes
- Based downing logic on game's `HealthUtility.DamageLegsUntilIncapableOfMoving()`
- Uses moving limb core parts targeting with blunt damage to avoid bleeding
- Heals critical brain/torso injuries to prevent immediate re-death
- Calls `CheckForStateChange()` to ensure proper health state transitions
- Notification cooldown prevents spam while still informing player of saves

## Implementation Notes

### Harmony Patch Strategy
- Prefix patch on `Pawn.Kill()` returning false prevents original execution
- Must check if raider is hostile and has qualifying relationships
- Force downed state with `health.forceDowned = true`
- Heal critical injuries to prevent immediate re-death

### Safety Considerations
- Null checks for pawn, relations, map
- Early returns for invalid states
- Only apply to hostile raiders, not player faction
- Avoid infinite loops or mod conflicts

### Settings Integration
- Full configurability for relationship types
- Master enable/disable toggle
- Notification system optional
- Ex-relationships disabled by default (usually undesired)

## Testing Requirements
1. Create raider with spouse relationship to colonist
2. Verify death prevention during combat
3. Test notification system
4. Check settings UI functionality
5. Verify compatibility with other death-related mods

## Current Status
✅ Core implementation complete
✅ Harmony patches working
✅ Settings system implemented
✅ Builds without errors
⏳ Ready for in-game testing
