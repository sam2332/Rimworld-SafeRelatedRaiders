# Safe Related Raiders

A RimWorld mod that prevents raiders who are blood or bonded relations of your colonists from dying outright during raids. Instead, if they would normally die, they are downed instead—allowing you the chance to capture, rescue, or recruit them.

## Features

### Core Functionality
- **Relationship Protection**: Raiders with qualifying relationships to your colonists are saved from death
- **Configurable Relationships**: Choose which types of relationships to protect
- **Smart Detection**: Only affects hostile raiders, not your colonists or friendlies
- **Critical Injury Healing**: Automatically heals life-threatening injuries when saving a raider

### Supported Relationships
- **Spouses** - Married partners
- **Lovers** - Romantic partners
- **Fiances** - Engaged partners
- **Parents** - Biological parents
- **Children** - Biological children  
- **Siblings** - Brothers and sisters
- **Bonded Animals** - Animal companions with bonds
- **Ex-Relationships** - Former spouses/lovers (optional, disabled by default)

### Customization Options
- **Master Toggle**: Enable/disable entire mod functionality
- **Relationship Types**: Individual toggles for each relationship type
- **Notifications**: Optional letters when raiders are saved
- **Per-Save Settings**: All settings are saved with your game

## Installation

1. Download the mod from the Steam Workshop or GitHub
2. Enable in RimWorld's mod list
3. Configure settings in Options → Mod Settings → Safe Related Raiders

## How It Works

When a raider would normally die from injuries:

1. **Check Hostility**: Verify the pawn is actually a hostile raider
2. **Scan Relationships**: Look for qualifying relationships with living colonists
3. **Prevent Death**: Force the raider to be downed instead of killed
4. **Heal Critical Injuries**: Automatically reduce fatal brain/torso injuries
5. **Notify Player**: Show optional notification with relationship details

## Compatibility

### Compatible With
- Most health and combat mods (Death Rattle, Combat Extended, etc.)
- Relationship mods (Psychology, Hospitality, etc.)
- Colony management mods
- Storytelling mods

### Technical Details
- Uses Harmony prefix patch on `Pawn.Kill()` method
- Minimal performance impact
- Safe mod loading/unloading
- No save corruption risk

## Settings

Access mod settings through: **Options → Mod Settings → Safe Related Raiders**

### General Settings
- **Enable Safe Related Raiders**: Master toggle for all functionality
- **Show notifications**: Display letters when raiders are saved

### Relationship Types
Individual toggles for:
- Spouses, Lovers, Fiances
- Parents, Children, Siblings  
- Bonded Animals
- Ex-Spouses, Ex-Lovers (advanced)

## FAQ

**Q: Will this make raids too easy?**
A: No - saved raiders are still downed and vulnerable. You must actively choose to rescue them, and they may die from other causes.

**Q: Does this work with modded factions?**
A: Yes - any hostile pawn with qualifying relationships will be protected, regardless of faction.

**Q: What about mechanoids?**
A: Mechanoids cannot form most relationships, so they're unaffected. However, bonded mechanoid animals would be protected.

**Q: Can I disable this for specific relationships?**
A: Yes - each relationship type has its own toggle in the settings.

**Q: Will this conflict with other death-related mods?**
A: Unlikely - the mod uses safe Harmony patching and only affects the specific death scenario.

## Development

### Source Code
Available on Steam store at: https://steamcommunity.com/sharedfiles/filedetails/?id=3539795384

### Bug Reports
Please report issues on GitHub with:
- RimWorld version
- Mod version  
- Other active mods
- Steps to reproduce
- Log files (if applicable)

### Contributing
Pull requests welcome! Please follow RimWorld modding conventions.

## Changelog

### Version 1.0.0
- Initial release
- Core death prevention functionality
- Configurable relationship types
- Notification system
- Comprehensive settings UI

## Credits

- **Author**: ProgrammerLily
- **Framework**: Harmony (by pardeike)
- **Game**: RimWorld (by Ludeon Studios)

## License

This mod is provided as-is for the RimWorld community. Feel free to modify or learn from the source code.

---

*"Keep families together, even in the chaos of the Rim."*
