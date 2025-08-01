# Safe Related Raiders - Implementation Summary

## Overview
This mod prevents raiders who are blood or bonded relations of your colonists from dying outright during raids. Instead, if they would normally die, they are downed instead—allowing you the chance to capture, rescue, or recruit them.

## Core Files Created

### 1. SafeRelatedRaidersMod.cs
- Main mod entry point
- Handles Harmony patching on startup
- Provides settings UI integration
- Uses Harmony ID: "SafeRelatedRaiders.ProgrammerLily.com"

### 2. SafeRelatedRaidersSettings.cs
- Mod settings with configurable relationship types
- Includes toggles for: spouses, lovers, fiances, parents, children, siblings, bonded animals
- Optional: ex-relationships (disabled by default)
- Notification system toggle
- Master enable/disable switch

### 3. RelationshipUtility.cs
- Core relationship checking logic
- `HasQualifyingRelationshipWithColonists()` - Main check for raiders
- `HasQualifyingRelationship()` - Pawn-to-pawn relationship verification
- `GetMostImportantQualifyingRelation()` - For notification display
- Filters colonists to living, non-hostile, player faction members

### 4. Pawn_Kill_Patch.cs
- Harmony prefix patch on `Pawn.Kill()` method
- Intercepts death before it occurs
- Checks if dying pawn is hostile raider with qualifying relationships
- If relationship found, forces downed state and heals critical injuries
- Shows optional notification to player
- Returns false to prevent original Kill method execution

## Key Implementation Details

### Relationship Detection
- Uses `pawn.relations.DirectRelations` to check relationships
- Supports all major relationship types through `PawnRelationDefOf`
- Configurable through settings which relationships to honor

### Hostile Raider Detection
- Must be hostile to player faction (`pawn.HostileTo(Faction.OfPlayer)`)
- Must not be player faction member
- Excludes wild animals (unless bonded animals that turned hostile)

### Death Prevention Mechanism
- Sets `pawn.health.forceDowned = true`
- Heals critical brain injuries to 70% of max health
- Heals critical torso injuries to 70% of max health
- Prevents immediate re-death from existing injuries

### Notification System
- Optional letter notification when raider is saved
- Shows relationship type using `GetGenderSpecificLabel()`
- Uses `LetterDefOf.NeutralEvent` for non-urgent notification

## Compatibility Considerations

### Other Mods
- Harmony prefix patch allows other death-related mods to still function
- Only affects hostile raiders with relationships to colonists
- Doesn't interfere with player faction death mechanics
- Uses reflection-safe relationship checking

### Game Balance
- Only prevents death during combat, not from other causes
- Pawns are still downed and vulnerable
- Player must actively choose to rescue/recruit
- Configurable relationship types allow customization

## Configuration Files Updated

### About.xml
- Changed package ID to "SafeRelatedRaiders.ProgrammerLily.com"
- Updated name to "Safe Related Raiders"
- Added proper description

### Project.csproj
- Changed assembly name to "SafeRelatedRaiders"
- Maintains existing RimWorld and Harmony references

## Testing Scenarios
1. Raider with spouse/lover relationship attacks colony
2. Raider with child/parent relationship attacks colony
3. Bonded animal turned hostile attacks colony
4. Settings verification for each relationship type
5. Notification system functionality
6. Critical injury healing effectiveness

## Future Enhancements Possible
- Hediff to track "saved from death" status
- Cooldown system to prevent abuse
- More granular relationship importance thresholds
- Integration with other relationship mods
- Combat log integration

## Code Safety Features
- Null safety checks throughout
- Early returns for invalid states
- Harmony patch safety (prefix pattern)
- Settings validation
- Comprehensive logging for debugging

This implementation follows RimWorld modding best practices and provides a solid foundation for the Safe Related Raiders functionality while maintaining compatibility and configurability.
