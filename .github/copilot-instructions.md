# GitHub Copilot Instructions for RimWorld Modding Project

## Mod Overview and Purpose
This mod introduces a sophisticated Field of View (FoW) system to RimWorld. By integrating visibility mechanics, surveillance systems, and dynamic line-of-sight calculations, it aims to enhance the strategic depth of gameplay. Players will experience a realistic approach to vision where only observed areas remain visible, creating new challenges and opportunities for tactical planning.

## Key Features and Systems
- **Field of View Mechanics:** Implements a line-of-sight system that allows only certain areas of the map to be visible based on character and object positioning.
- **Surveillance Systems:** Introduces buildable surveillance cameras and consoles that can be powered to extend visibility in specific areas.
- **Dynamic Vision Calculations:** Utilizes C# to dynamically update and manage vision states based on game events and actions.
- **Environmental Influence:** Leverages environmental statistics and terrain types to influence sight ranges and visibility extents.
- **Harmony Patching:** Utilizes Harmony for runtime modification of existing game methods to integrate new functionalities.

## Coding Patterns and Conventions
- **Static Classes:** Utilized for utility functions and global game helpers, aiding in centralized management of specific functionalities (e.g., `BeautyUtility`, `HaulAIUtility`).
- **ThingComps:** Leveraged for component-based modularity, allowing seamless addition of behavior to in-game entities (`CompAffectVision`, `CompHiddenable`).
- **Namespace Organization:** Maintained throughout to logically separate utilities, components, and Harmony patches, enhancing code readability and structure.
- **Method Visibility:** Public methods for external/inter-class usage, private methods to encapsulate internal logic.

## XML Integration
XML integration is used to define game data such as defs for items and buildings. Although specifics aren't highlighted here, ensure your XML files align with RimWorld's schema, enabling easy integration with defined classes in C#. Use XML for components like `CompProperties_AffectVision` and `CompProperties_ProvideVision`.

## Harmony Patching
- **Usage of Harmony:** Modify existing game behavior at runtime to inject new functionality without altering original source code.
- **Targeted Patches:** Implement targeted method patches, such as `Patch_RegisterSustainer` and `Patch_PlayOneShot`, to overwrite or extend base game functionality.
- **Patch Safety:** Ensure patches are efficient and safe to prevent conflicts, using prefixes and postfixes judiciously.

## Suggestions for Copilot
When utilizing Copilot for this RimWorld mod project, consider the following:
- **Static Utility Methods:** Focus on generating utility functions for calculating vision or processing environmental variables.
- **Edge Cases Handling:** Encourage Copilot to suggest proper null-checks and conditional logic, particularly for dynamic vision calculations.
- **Modular Component Design:** Utilize Copilot to draft components that can be easily adapted or extended, particularly for `ThingComp` based classes.
- **Harmony Patch Suggestions:** While crafting Harmony patches, guide Copilot to generate stub methods with pre/postfix patterns to ensure non-intrusive modifications.
- **XML Definition Guidance:** Ensure Copilot leverages proper XML schema for defining mods' data, maintaining seamless integration with C# codebases.

By adhering to these guidelines, Copilot can assist in streamlining development and maintaining quality within your RimWorld modding projects.
