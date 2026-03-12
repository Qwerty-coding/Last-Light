# Last Light

> *"Something woke you up. Something worse is waiting outside."*

**Last Light** is an immersive first-person survival horror game built in Unity. Navigate a dangerous, atmospheric world — scavenge resources, fight the infected, and uncover the source of the outbreak before it's too late.

---

## 🛠️ Tech Stack

| | |
|---|---|
| **Engine** | Unity 2022.3 LTS |
| **Language** | C# |
| **Version Control** | Git / GitHub |
| **Platform** | PC (Windows / Linux) |

---

## 🕹️ Controls

| Category | Key / Input | Action |
|---|---|---|
| **Movement** | W A S D | Walk / Run |
| | Left Shift | Sprint |
| | Spacebar | Jump |
| | Mouse | Look around |
| **Combat** | Left Click | Attack (Shoot / Swing Axe) |
| | R | Reload Gun |
| | 1 | Equip Gun |
| | 2 | Equip Axe |
| **Interaction** | E | Interact (pickup, doors, NPC) |
| **System** | ESC | Pause Game |

---

## 📋 Gameplay Loop

### Act 1 — Escape

1. **Wake Up** — Cinematic camera rise from the floor. First objective: *exit the house from the main gate.*
2. **Find the Axe** — Located in the outside storeroom. Your only tool and your first weapon.
3. **Gather Wood** — Chop 10 trees in the forest. Each drops 1 log. Watch your back.
4. **Trade with the Survivor** — Give 10 logs to the NPC in exchange for a Gun. It auto-equips.
5. **Clear the Horde** — Kill 10 zombies. The 10th drops a Key.

### Act 2 — Ascend

6. **Reach the Fire Tower** — Use the Key to unlock the tower door.
7. **Enter the Portal** — At the top of the tower, a portal waits. Step through.

### Act 3 — Confrontation

8. **Fight Subject Alpha** — The boss arena. Fog lifts. Spawning stops. Something massive is here.
9. **Victory** — Defeat Subject Alpha. The screen fades. It's over.

---

##  Features

- 🪓 **Dual weapon system** — Axe (melee) and Gun (ranged), switchable mid-combat
- 🧟 **Zombie AI** — Patrol, chase, and attack states with dynamic spawning
- 👹 **Boss fight** — Subject Alpha: 500 HP, AOE attacks, enrage phase at 50% health
- 🌲 **Resource gathering** — Chop trees, manage inventory, trade to progress
- 💀 **Horror atmosphere** — Jump scares, lightning, camera shake, blood overlay
- 🎯 **Objective tracking system** — HUD tracker with world-space markers
- 🔊 **Full audio system** — BGM, SFX, and persistent volume settings across scenes

---

##  Getting Started

### Play (PC)
1. Download the latest release from the [Releases](../../releases) page
2. Extract the zip
3. Run `LastLight.exe` (Windows) or `chmod +x LastLight.x86_64 && ./LastLight.x86_64` (Linux)
4. Keep the `_Data` folder in the same directory as the executable

### Build from Source
1. Clone the repo
   ```bash
   git clone https://github.com/yourusername/last-light.git
   ```
2. Open in **Unity 2022.3 LTS** or newer
3. Open `Assets/Scenes/MainMenu.unity`
4. Hit Play, or go to **File → Build Settings** to build

> ⚠️ Requires **TextMeshPro** and **NavMesh Components** — both available via Unity Package Manager

---

## 📁 Project Structure

```
Assets/
├── SCRIPTS/
│   ├── Core Systems/       # Game manager, objectives, inventory, victory
│   ├── Player/             # Movement, camera, health, interaction
│   ├── Weapons/            # Gun, axe, bullets, weapon switching
│   ├── Enemies/            # Zombie AI, boss, spawner
│   ├── NPCs & Trading/     # NPC shop, markers
│   ├── Items & Interaction/# Pickups, trees, doors
│   ├── UI & Visual/        # HUD, damage effects, inventory UI
│   ├── Audio/              # Sound manager, BGM/SFX volume system
│   ├── Scene Management/   # Async loading, teleporters, triggers
│   └── Special Effects/    # Intro sequence, jump scares, weather
├── Animations/
├── Scenes/
│   ├── MainMenu.unity
│   └── SampleScene.unity   # Full game — house, forest, tower, boss arena
└── ...
```

---

##  Scene Structure

| Scene | Description |
|---|---|
| `MainMenu` | Main menu with BGM and volume settings |
| `SampleScene` | Entire game world — house, forest, Fire Tower, and boss arena |

---

*Built with Unity 2022.3 LTS — Last Light &copy; 2026*
