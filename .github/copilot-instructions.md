# .github/copilot-instructions.md

## Mod Overview and Purpose

### Mod Name: (NWN) Real Fog of War (Continued)

This mod is a continuation of the original mod by Luca De Petrillo. It adds a realistic fog of war system to RimWorld, requiring players to explore the map to reveal it. The mod enhances gameplay by introducing vision mechanics for both players and AI, creating a more immersive and strategic experience.

## Key Features and Systems

- **Fog of War:** The map starts unrevealed. Exploration is required to reveal it.
- **Field of View Mechanics:** 
  - Implemented for humans, animals, and mechanoids.
  - Influenced by sight attributes, darkness, and weather.
  - Bionic eyes negate darkness penalties.
- **Combat and Vision Interaction:**
  - Ranged attacks are limited to revealed areas, except for weapons like mortars.
  - Field of View increases when standing and attacking.
- **Technology and Surveillance:**
  - Surveillance cameras and watchtowers extend the Field of View.
  - Automatic turrets have an extended view range at lower difficulties.
- **Adaptive Vision:** 
  - Customizable vision ranges with adjustable settings.
  - Sound and body size impact vision for blind people and animals, respectively.
- **Additional Features:**
  - Night vision integration.
  - Factional sharing of vision data.
  - Toggle for prisoner vision and raid letter suppression.

## Coding Patterns and Conventions

- **Static Classes and Methods:** Use static classes for utility purposes, ensuring easy access and singleton-like behavior where appropriate (e.g., `BeautyUtility`, `Designation`).
- **Thing and Component System:** Make use of the ThingComp and ThingSubComp classes to model components affecting vision and behavior.
- **Consistent Naming:** Follow C# naming conventions for class, method, and variable names (PascalCase for public members, camelCase for private members).

## XML Integration

- **Mod Configuration:** XML configuration for customizable settings, such as vision range adjustments, should be maintained in the mod's XML files. Ensure configurations are user-friendly and well-documented for easy tweaking.

## Harmony Patching

- **Harmony Usage:** Utilize Harmony for patching game methods to integrate fog of war features seamlessly:
  - **Audio and Visual Effects:** Patch methods to register and unregister sound, affecting auditory fog of war.
  - **AI and Player Interactions:** Extend AI behavior to consider fog of war when targeting or interacting with the environment.
  - **UI Elements:** Ensure UI elements account for and display fog of war status appropriately, but take care not to introduce performance issues.

## Suggestions for Copilot

- **Method Autocompletion:** Suggest method names and signatures based on existing patterns in the codebase for consistent implementation.
- **Error Handling Tips:** Provide suggestions for handling exceptions or potential edge cases, especially in methods dealing with game logic.
- **Performance Optimization:** Advise code patterns that minimize lag, especially in the visibility update logic affecting large numbers of pawns.
- **XML Generation:** Assist in generating well-structured XML configuration elements related to mod settings and default values.
- **Harmony Patch Templates:** Offer templates for common Harmony patch patterns, reducing manual setup time and ensuring best practices are followed.

This instruction file is designed to guide developers working on the `(NWN) Real Fog of War (Continued)` mod in maintaining standardized and efficient codebase updates, leveraging both existing conventions and intelligent tools like GitHub Copilot.
