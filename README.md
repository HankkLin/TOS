# 《神魔：極限演武》 (Tower of Saviors: Extreme Martial Arts)

Welcome to the **《神魔：極限演武》** project repository! This is a conceptual and foundational Unity project for a non-profit fan-made game that reimagines the classic *Tower of Saviors (神魔之塔)* experience. By blending the iconic runestone match-3 puzzle mechanics with dynamic 3D combat, deep narrative world-building, and complex environmental level designs, this project aims to create a highly ambitious "Action-Puzzle RPG" experience.

## 📖 Project Overview

This repository houses both the comprehensive **Game Design Documents (GDD)** and the core **Unity 6 (LTS)** scaffolding for the game. 

### Key Features (Planned & Designed)
*   **Dynamic 3D Match-3 Combat**: A revolutionary combat system that evolves traditional 2D runestone turning into a 3D tactical experience with a 5-second turn limit and dynamic character skills.
*   **Rich Narrative Worldbuilding**: An expansive multi-ending storyline involving timeline branches, character variants (e.g., historical divergences, era evolutions), and deep philosophical themes surrounding "possibilities and choices."
*   **Complex Level Environments**: 3D level designs integrating weather systems, gravity manipulation, elemental terrains (Volcano, Water, Shadow, Chaos), and lethal topological hazards.
*   **Modern Technical Architecture**: Developed on Unity 6 LTS leveraging the Universal Render Pipeline (URP) to achieve high-quality Cel-shaded aesthetics while balancing mobile performance limits.

## 📂 Repository Structure

The project is divided into two primary directories:

*   **[`GDD/`](./GDD/)**: Contains the comprehensive Game Design Documents outlining the vision, mechanics, and design philosophies.
    *   `01_combat_system.md`: Core combat loop and skill systems.
    *   `02_narrative_worldbuilding.md`: Scripts, lore, world rules, and character variant concepts.
    *   `03_level_environment.md`: 3D environments, gravity systems, and level hazards.
    *   `04_technical_architecture.md`: Engineering choices, URP configurations, and timeline planning.
    *   `05_critique_final_summary.md`: A strict "Devil's Advocate" QA analysis evaluating all systems for UX flow, resource constraints, and potential mechanical contradictions.
*   **[`TOS_Unity/`](./TOS_Unity/)**: The foundational Unity project directory. 
    *   Pre-configured with essential Unity packages (URP, UI Toolkit, Input System, TextMeshPro).
    *   Includes assembly definitions and foundational scripts (Enums, Constants).
    *   A clean and organized `Assets` structure intended for modular and scalable development.

## 🛠️ Development Setup & Technologies

### Prerequisites
*   **Game Engine**: Unity `6000.x.x` (LTS equivalent)
*   **IDE**: Visual Studio 2022 or JetBrains Rider
*   **Version Control**: Git

### Technical Stack
*   **Engine**: Unity 6
*   **Graphics Pipeline**: Universal Render Pipeline (URP) tailored for a Cel-shaded/anime aesthetic.
*   **Scripting Language**: C#

### Getting Started
1. Clone the repository to your local machine:
   ```bash
   git clone <repository-url>
   ```
2. Open Unity Hub.
3. Click on **Add** project and select the `TOS_Unity` folder located within this repository.
4. Allow Unity to resolve packages and build the initial Library (this might take a few minutes on the first launch).
5. Ensure you are using the correct target platform (Android/iOS/PC) as specified in the technical architecture document.

## 🤝 Contributing

This is a non-profit, educational/fan-project endeavor. If you are reviewing the documentation, please refer to the `05_critique_final_summary.md` for our identified design challenges and proposed solutions before proposing new mechanics.

---
> **Disclaimer**: This is a non-profit fan project. All original character concepts and IP elements that parallel *Tower of Saviors (神魔之塔)* belong to Madhead. This project is not affiliated with, endorsed by, or sponsored by Madhead.
