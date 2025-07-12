# Game Achievements

Game Achievements is a flexible and extensible achievement system for Unity. It separates core logic, configuration, and runtime handlers, allowing for clean and modular achievement implementation in any game.

# Features

- Achievements and rewards are independent and fully composable
- Achievements and rewards are configured via ScriptableObjects
- Handlers are MonoBehaviours, allowing scene context and inspector-based setup
- One handler can manage multiple achievements of the same type
- Achievement groups support custom logic and grouping behavior
- Groups can be static or time-based with automatic refresh
- Timed groups can optionally preserve completed (but unclaimed) achievements during refresh
- Snapshot system for saving and restoring both achievements and groups
- No manual mapping: handlers and rewards are resolved automatically based on config type

# Installing

Add the package to Unity via UPM:
https://github.com/white-arrow-studio/game-achievements.git

# Usage: Basic Setup

## Architectural note

This system is designed around flexible abstraction to support arbitrary types of achievements and rewards. As a result:

- You’ll often receive objects like configs or logic in their **base types**
- It’s your responsibility to cast them to the expected type and throw a clear error if the cast fails
- This keeps the system extensible without requiring reflection or rigid bindings

The framework guarantees that, under correct configuration, you'll receive the expected type — even if it arrives in base form. The cast check is just a safeguard in case of internal mapping bugs. If you ever encounter such a case, let us know — we'll fix it. But under normal use, these casts will always succeed.

We intentionally avoided generics in most places, as they introduce tight coupling and excess boilerplate. If we find a clean way to integrate generics without sacrificing flexibility — we’ll adopt it.

Here’s the recommended pattern:

```csharp
public class CurrencyRewardDispencer : IRewardDispencer
{
    public void DispenseReward(RewardConfig rewardConfig)
    {
        if (rewardConfig is not CurrencyRewardConfig config)
            throw new InvalidCastException($"Expected {nameof(CurrencyRewardConfig)}, got {rewardConfig.GetType().Name}");

        PlayerWallet.Add(config.CurrencyId, config.Amount);
    }
}
```

All object mapping (from config → logic → handler → reward) is handled via **factories**, which will be shown in a later step.

## Step 1 — Create an achievement config and handler

Each achievement is defined by two components:

- **`AchievementConfig`**: a ScriptableObject that stores metadata and parameters
- **`AchievementHandler`**: a MonoBehaviour that tracks progress for a specific type of achievement

The base `AchievementConfig` already includes:

- `Id`, `Name`, `Description` — for UI and identification
- `TargetProgress` — how much progress is required to complete the achievement
- `RewardConfig` — reference to a reward to be granted upon completion

To define your own achievement type, simply inherit from the base config:

```csharp
[CreateAssetMenu(menuName = "Achievements/Kill Enemies")]
public class KillEnemiesAchievementConfig : AchievementConfig
{
    public string EnemyType;
}
```

Next, implement a handler for this type of achievement. All handlers must inherit from the base `AchievementHandler`, which already provides:

- A list of registered achievements
- Validation when adding/removing achievements
- A utility method to add progress to all non-completed achievements

Here’s a minimal handler implementation:

```csharp
public class KillEnemiesAchievementHandler : AchievementHandler
{
    public override bool CanHandle(Achievement achievement)
    {
        return achievement is KillEnemiesAchievement;
    }

    public void OnEnemyKilled(string type)
    {
        foreach (var achievement in _achievements)
        {
            if (achievement is KillEnemiesAchievement kill && kill.MatchesType(type))
                kill.AddProgress(1);
        }
    }
}
```

This approach keeps your logic modular and allows multiple achievement types to coexist in the same scene, each with their own handler.

## Step 2 — Create a reward dispenser

Each reward consists of two parts:

- `RewardConfig`: a ScriptableObject that holds configuration data
- `IRewardDispencer`: interface that defines how the reward is actually granted

The base `RewardConfig` already includes:

- `Amount`: numeric value representing how much reward to give
- `Icon`: optional sprite to show in UI

These fields don’t need to be duplicated in your subclass — just inherit and add what’s specific to your reward type.

For example, a reward that grants a currency might look like this:

```csharp
[CreateAssetMenu(menuName = "Achievements/Rewards/Currency")]
public class CurrencyRewardConfig : RewardConfig
{
    [SerializeField] private string _currencyId;
    public string CurrencyId => _currencyId;
}
```

Then implement the actual reward logic by creating a class that implements `IRewardDispencer`. The config arrives as the base `RewardConfig`, so cast it to your type:

```csharp
public class CurrencyRewardDispencer : IRewardDispencer
{
    public void DispenseReward(RewardConfig rewardConfig)
    {
        if (rewardConfig is not CurrencyRewardConfig config)
            throw new InvalidCastException($"Expected {nameof(CurrencyRewardConfig)}, got {rewardConfig.GetType().Name}");

        PlayerWallet.Add(config.CurrencyId, config.Amount);
    }
}
```

## Step 3 — Set up factories for achievements and rewards

Factories are `MonoBehaviour` components placed in the scene. This provides flexibility and access to scene context if needed (e.g., serialized references or injected services). Their purpose is to instantiate achievements and reward dispensers based on config types, keeping creation logic modular and decoupled.

There are two primary typed factory bases:

- `AchievementFactoryByConfig` — creates an `Achievement` from an `AchievementConfig`
- `RewardDispencerFactoryByConfig` — creates an `IRewardDispencer` from a `RewardConfig`

The central factories (`AchievementFactory`, `RewardDispencerFactory`) automatically route creation requests to the matching factory via `CanCreateBy`.

**Example of a reward dispenser factory:**

```csharp
public class CurrencyRewardDispenserFactory : RewardDispencerFactoryByConfig
{
    public override bool CanCreateBy(RewardConfig config) => config is CurrencyRewardConfig;

    public override IRewardDispencer Create(RewardConfig config)
    {
        if (config is not CurrencyRewardConfig currencyConfig)
            throw new InvalidCastException();

        return new CurrencyRewardDispencer();
    }
}
```

**Example of an achievement factory:**

```csharp
public class KillEnemiesAchievementFactory : AchievementFactoryByConfig
{
    [SerializeField] private KillEnemiesHandler _handler;

    public override bool CanCreateBy(AchievementConfig config) => config is KillEnemiesAchievementConfig;

    protected override Achievement Create(AchievementConfig config, IRewardDispencer rewardDispencer)
    {
        if (config is not KillEnemiesAchievementConfig killConfig)
            throw new InvalidCastException();

        return new Achievement(killConfig, _handler, rewardDispencer);
    }
}
```

Each sub-factory handles only the types it supports. The central factory delegates creation based on `CanCreateBy`, allowing you to add new achievement and reward types by simply creating new config + factory pairs — no need to modify any existing code.

## Step 4 — Scene setup and initialization

Once your configs, handlers, and factories are created, it's time to wire everything together in the scene.

### 1. Set up the reward dispenser factory

1. Create a `GameObject` named `AchievementRewardDispencerFactory` and attach the `RewardDispencerFactory` component.
2. Create separate child GameObjects under it — one for each specific reward dispenser factory (e.g., `CurrencyRewardFactory`, `ItemRewardFactory`, etc.).
3. Assign all these sub-factories to the `Factories` list in the parent `RewardDispencerFactory` via the Inspector.

### 2. Set up the achievement factory

1. Create a similar structure for achievement factories.
2. The root factory should be of type `AchievementFactory`, with:
   - The `_factories` list populated with your concrete `AchievementFactoryByConfig` instances
   - The `_rewardDispencerFactory` field referencing the `RewardDispencerFactory` object created earlier

This allows the achievement factory to create achievements and automatically inject the correct reward dispenser based on the configuration.

### 3. Set up achievement handlers

Handlers are also `MonoBehaviour` components and should be added to appropriate GameObjects in the scene.

- You can organize them under a single root object if desired.
- There is no central handler registry required.
- Each handler implements a virtual `Init()` method, which should be manually called at the appropriate point in your game's lifecycle.
- This gives you full control over when handlers are initialized, which is helpful if they depend on other systems or data.

### 4. Initialize everything in code

You can create a `MonoBehaviour` like `AchievementStorage` to wire things up.

Example:

```csharp
public class AchievementStorage : MonoBehaviour
{
    [SerializeField] private AchievementFactory _achievementFactory;
    [SerializeField] private List<AchievementConfig> _configs;
    [SerializeField] private List<AchievementHandler> _handlers;

    private List<Achievement> _achievements = new();

    public void Init()
    {
        foreach (var config in _configs)
        {
            var achievement = _achievementFactory.Create(config);
            _achievements.Add(achievement);
        }

        foreach (var achievement in _achievements)
        {
            foreach (var handler in _handlers)
            {
                if (handler.CanHandle(achievement))
                    handler.AddAchievement(achievement);
            }
        }

        foreach (var handler in _handlers)
        {
            handler.Init();
        }
    }
}
```

This pattern gives you full control over scene setup, allows dependency injection through the Inspector, and avoids tight coupling between systems.

# Achievement Groups

The framework includes a flexible grouping system for achievements. Groups allow you to:
- Organize related achievements (e.g., Daily, Weekly, Seasonal)
- Control when and how achievements become active
- Implement time-based or custom logic for refreshing achievements
- Persist and restore group state via snapshots

Each group consists of:
- A **config** (inherits from `AchievementGroupConfig`)
- A **runtime group** (inherits from `AchievementGroup<TConfig>`)

`AchievementGroupConfig` class provides the minimal data needed to define a group:
- `Id` — the unique group identifier
- `Achievements` — the list of `AchievementConfig` objects in the group

You only need to subclass it if your group requires additional parameters.

## Built-in Groups

| Group Type              | Description                                        | Config Type                   | Runtime Class              |
|-------------------------|----------------------------------------------------|--------------------------------|-----------------------------|
| Simple Group            | Shows all configured achievements, no logic       | `AchievementGroupConfig`       | `SimpleAchievementGroup`    |
| Timed Group             | Refreshes its active achievements over time       | `TimedAchievementGroupConfig`  | `TimedAchievementGroup`     |

### `TimedAchievementGroupConfig` includes:

- `ActiveCount` — number of visible achievements per refresh
- `RefreshTimeRate` — time between refreshes (in seconds)
- `SaveUnreceivedAchievements` — whether to preserve completed-but-unclaimed achievements

## Snapshots for Groups

All achievement groups support full state persistence via:

```csharp
IAchievementGroupSnapshot CaptureStateTo();
void RestoreState(IAchievementGroupSnapshot snapshot);
```

Snapshots store:
- The group's ID
- Achievement snapshots within the group
- Custom group-specific data (e.g., time of last refresh)

## Creating Custom Groups

You can implement your own achievement group logic by:

1. Creating a config class that inherits from `AchievementGroupConfig`
2. Creating a runtime group that inherits from `AchievementGroup<TConfig>`

This group only activates when the player's level is high enough.
```csharp
[CreateAssetMenu(menuName = "Achievements/Groups/Level Locked")]
public class LevelLockedGroupConfig : AchievementGroupConfig
{
    [SerializeField] private int _requiredPlayerLevel;
    public int RequiredLevel => _requiredPlayerLevel;
}
```

```csharp
public class LevelLockedAchievementGroup : AchievementGroup<LevelLockedGroupConfig>
{
    private bool _unlocked;

    protected override void InitCore()
    {
        _unlocked = Player.Level >= Config.RequiredLevel;

        if (_unlocked)
        {
            foreach (var config in Config.Achievements)
                _achievements.Add(_achievementFactory.Create(config));
        }
    }

    public override IAchievementGroupSnapshot CaptureStateTo(IAchievementGroupSnapshot snapshot)
    {
        if (snapshot is not LevelLockedGroupSnapshot typed)
            throw new InvalidCastException();

        typed.Id = Config.Id;
        typed.IsUnlocked = _unlocked;

        foreach (var achievement in _achievements)
        {
            var achievementSnapshot = typed.CreateAchievement();
            achievement.CaptureStateTo(achievementSnapshot);
            typed.AddAchievement(achievementSnapshot);
        }

        return typed;
    }

    public override void RestoreState(IAchievementGroupSnapshot snapshot)
    {
        if (snapshot is not LevelLockedGroupSnapshot typed)
            throw new InvalidCastException();

        _unlocked = typed.IsUnlocked;
        ClearAllAchievements();

        foreach (var achievementSnapshot in typed.Achievements)
        {
            var config = Config.Achievements.FirstOrDefault(c => c.Id == achievementSnapshot.Id);
            if (config == null)
                continue;

            var achievement = _achievementFactory.Create(config);
            achievement.RestoreState(achievementSnapshot);
            _achievements.Add(achievement);
        }
    }
}
```

This approach allows you to customize how and when achievements appear or update, without modifying core achievement logic.
