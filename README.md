# Game Achievements

**Game Achievements** is a flexible and extensible achievement system for Unity. It provides a clean separation between logic, configuration, and runtime handlers, making it easy to implement various achievement types in any project.

## 🔧 Features

- Separation of achievement logic, config, and event handling
- Support for different achievement types and custom reward logic
- Built on `ScriptableObject`-based configs for easy authoring
- Achievement groups (e.g. permanent, daily, timed)
- Refreshable timed groups with optional persistence of completed achievements
- Snapshot system (`CaptureState` / `RestoreState`) for saving and loading progress
- Handlers can be `MonoBehaviour`s for scene-based interaction
- Plug-and-play architecture with interfaces and factories

## 🧱 Architecture Overview

- `Achievement`: core logic of a single achievement
- `AchievementConfig`: serialized configuration with target conditions
- `AchievementHandler`: event handler that triggers progress
- `RewardDispenser`: strategy object for handling rewards
- `AchievementGroup`: manages collections of achievements (with optional refresh rules)

## 🚀 Getting Started

1. Create a custom `AchievementConfig` with target progress and reward type.
2. Implement or reuse an `AchievementHandler` to provide progress triggers.
3. Use the provided factory system to create `Achievement` instances.
4. Optionally group achievements into a `TimedAchievementGroup` or similar.
5. Use `CaptureState()` and `RestoreState()` to manage persistence.

## 📦 Status

✅ Production-ready  
🧪 Tested and modular  
🔌 Easy to integrate with existing systems

---

> Found a bug or want to contribute? Open an issue or submit a pull request!
