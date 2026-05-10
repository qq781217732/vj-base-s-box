# VJBASE-S&BOX

VJ Base NPC AI framework ported to s&box C# — community-driven, open for contribution.

Built on [DrVrej's VJ Base](https://steamcommunity.com/sharedfiles/filedetails/?id=131759821) (GMod Lua NPC AI framework), fully ported to s&box C# with zombie survival / TTT gameplay on top. Active development.

> **Note**: Weapon model and zombie animation sources are not included in this repository.

---

## Status

VJ Base translation is ~**98%** complete. Core AI loop (sensing, scheduling, movement, combat, sound, animation) is fully functional.

| Subsystem | Status |
|-----------|--------|
| Schedule (32 methods) | ✅ |
| AA movement / Sound / Relationships | ✅ |
| HumanNPC (18 methods + SelectSchedule) | ✅ |
| DamageInfo + immunity chain / Entity flags / Allies | ✅ |
| Weapon system + Animation system (Route A, ~1800 lines) | ✅ |
| Edge systems (Follow / Fire / Eating / Bullseye) | ⬜ ~19 SKIP |

---

## Structure

```
├── Code/
│   ├── VJBase/                     — VJ Base C# port (NPC AI framework)
│   │   ├── Core/                   — BaseNPC, Schedule, Animation, Sound, etc.
│   │   ├── Engine/                 — AISenses perception layer
│   │   ├── Schedule/               — Schedule / task data structures
│   │   └── Bases/                  — CreatureNPC / HumanNPC / TankNPC
│   ├── Zombies/                    — Zombie NPC types
│   ├── Gamemodes/                  — TTT / Zombie Horde modes
│   ├── Weapons/                    — Custom weapons
│   ├── Player/                     — Player systems
│   ├── AI/                         — AI director / encounter system
│   ├── swb_base/ / swb_player/     — SWB weapon base
│   └── ui/                         — UI components
├── Assets/                         — Game assets (models, materials, sounds, etc.)
├── docs/                           — Translation guides, progress, API mapping
└── tools/                          — Utility scripts
```

---

## Contributing

Issues and Pull Requests welcome.

- Translation guide: [docs/translation-guide.md](docs/translation-guide.md)
- Remaining tasks: [docs/phase3-progress.md](docs/phase3-progress.md)
- Animation system: [docs/animation-system-analysis.md](docs/animation-system-analysis.md)

### Commit convention

```
type(scope): short description
```

`type`: `translate` / `fill` / `fix` / `cleanup` / `field` / `docs`

### Development

- [s&box](https://sbox.game) client
- Open `testzombie.sbproj` in the s&box IDE

---

## License

Source code is for educational reference. VJ Base copyright belongs to [DrVrej](https://steamcommunity.com/sharedfiles/filedetails/?id=131759821).
