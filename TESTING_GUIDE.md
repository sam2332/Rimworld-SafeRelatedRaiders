# Safe Related Raiders - Testing Guide

## Manual Testing Scenarios

### Setup Required
1. Start a new colony or load existing save
2. Use dev mode to create raiders with relationships
3. Enable mod in mod settings

### Test Case 1: Basic Spouse Relationship
**Setup:**
1. Create a colonist named "Alice"
2. Use dev mode to spawn a raider
3. Use "Set relation" dev tool to make the raider Alice's spouse
4. Initiate combat between raider and colony

**Expected Result:**
- Raider should be downed instead of killed when taking fatal damage
- Notification should appear: "[Raider name], Alice's spouse, was spared from death and downed instead."
- Raider should have healed critical injuries

### Test Case 2: Parent-Child Relationship
**Setup:**
1. Create a colonist 
2. Spawn a raider and set as parent/child relationship
3. Initiate combat

**Expected Result:**
- Same behavior as spouse test
- Notification should show correct relationship type

### Test Case 3: Settings Verification
**Setup:**
1. Go to mod settings and disable spouse relationships
2. Repeat Test Case 1

**Expected Result:**
- Raider should die normally (not be saved)
- No notification should appear

### Test Case 4: Multiple Relationships
**Setup:**
1. Create multiple colonists with different relationships to same raider
2. Test prioritization of relationship importance

**Expected Result:**
- Most important relationship should be displayed in notification
- Only one notification per saved raider

### Test Case 5: Notification Toggle
**Setup:**
1. Disable notifications in settings
2. Repeat Test Case 1

**Expected Result:**
- Raider should still be saved from death
- No notification should appear

### Test Case 6: Non-Hostile Pawns
**Setup:**
1. Test with friendly faction member
2. Test with neutral wildlife
3. Test with player faction colonist

**Expected Result:**
- Mod should not interfere with non-hostile pawns
- Normal death mechanics should apply

### Test Case 7: Already Downed Raiders
**Setup:**
1. Down a related raider (don't kill)
2. Attempt to execute or inflict fatal damage while downed

**Expected Result:**
- Behavior depends on how RimWorld handles damage to downed pawns
- Should not create infinite save loops

## Debug Mode Testing

### Commands to Use
- `Kill` - Force kill pawn to test death prevention
- `Damage` - Apply specific damage amounts
- `Set relation` - Create relationships between pawns
- `Spawn pawn` - Create test raiders
- `Faction relation` - Set hostile/friendly status

### Dev Mode Verification
1. Open dev tools
2. Check logs for mod messages
3. Verify relationship detection is working
4. Test edge cases (mechanoids, animals, etc.)

## Performance Testing

### Large Raids
1. Spawn large raid (20+ pawns)
2. Have multiple colonists with relationships
3. Monitor performance during combat

**Expected Result:**
- No significant performance impact
- All related raiders should be properly identified and saved

### Memory Usage
1. Monitor memory usage before/after enabling mod
2. Test with extended play sessions

**Expected Result:**
- Minimal memory footprint
- No memory leaks

## Compatibility Testing

### Other Mods to Test With
- Death Rattle
- Harvest Everything
- Combat Extended
- Hospitality
- Psychology

**Process:**
1. Enable Safe Related Raiders with other mod
2. Run basic test scenarios
3. Check for conflicts or unexpected behavior

## Reporting Issues

### Information to Collect
- RimWorld version
- Mod version
- Other active mods
- Save file details
- Steps to reproduce
- Expected vs actual behavior
- Log files (if errors occur)

### Common Issues to Watch For
- Raiders not being saved when they should be
- Non-raiders being affected by mod
- Performance issues during large raids
- Settings not saving properly
- Notification spam or missing notifications
- Compatibility issues with other mods

## Success Criteria

✅ **Core Functionality:**
- Related raiders are saved from death
- Only hostile raiders are affected
- Notifications work correctly

✅ **Settings System:**
- All relationship types can be toggled
- Settings persist between game sessions
- Master toggle works

✅ **Performance:**
- No noticeable impact on game performance
- Works with large raids

✅ **Compatibility:**
- No conflicts with major mods
- Follows RimWorld modding best practices

✅ **Edge Cases:**
- Handles null/invalid data gracefully
- Doesn't interfere with non-combat deaths
- Prevents infinite save loops
