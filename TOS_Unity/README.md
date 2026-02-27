# 神魔：極限演武 (Tower of Saviors: Tactical Overdrive)

A 3D tactical RPG reimagination of Tower of Saviors, built with **Unity 6 (6000.3.10f1)** + URP.

## Tech Stack
- **Engine**: Unity 6 LTS with Universal Render Pipeline (URP 17)
- **Rendering**: 2.5D Cel-Shading (Shader Graph + HLSL)
- **Platforms**: PC, Android, iOS

## Project Structure

```
Assets/
├── Scripts/
│   ├── Core/           # Enums, constants, GameManager
│   ├── OrbBoard/       # 5×6 orb puzzle system
│   ├── Combat/         # Battle engine, turns, damage
│   ├── Characters/     # Character & skill ScriptableObjects
│   ├── Environment/    # Level configs, terrain, mechanics
│   ├── UI/             # Rubik's cube menu, HUD, dialogs
│   ├── Narrative/      # Dialogue, quests, cutscenes
│   └── Infrastructure/ # Input, save system, backend
├── Art/
│   ├── Shaders/        # Cel-shading, outline, trail shaders
│   ├── Materials/      # Material instances
│   ├── Models/         # 3D models (Characters, Environments, Orbs)
│   ├── VFX/            # Particles, trails
│   └── Animations/     # Character & UI animations
├── Audio/              # BGM, SFX, Voice
├── Prefabs/            # Ready-to-use game objects
├── Scenes/             # Unity scenes
└── Data/               # ScriptableObject instances
```

## Assembly Definitions
Modular compilation via 8 assemblies: `TOS.Core`, `TOS.OrbBoard`, `TOS.Combat`, `TOS.Characters`, `TOS.Environment`, `TOS.UI`, `TOS.Narrative`, `TOS.Infrastructure`.

## GDD Reference
See `/GDD/` folder for the complete 5-document Game Design Document.
