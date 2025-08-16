# Game Achievements

Game Achievements is a flexible and extensible achievement system for Unity. It separates core logic, configuration, and runtime handlers, allowing for clean and modular achievement implementation in any game.

## Features

### General
- All components — achievements, rewards, and groups — are fully composable and work independently
- All configurations (for achievements, rewards, and groups) are defined via ScriptableObjects
- Full snapshot system to save and restore runtime state, including individual achievements and groups

### Achievements
- Handlers are MonoBehaviours, allowing both inspector-based setup and full dependency injection
- One handler can manage multiple achievements of the same type
- No manual mapping: handlers and rewards are resolved automatically based on config type

### Groups
- Achievement groups support custom logic and grouping behavior
- Groups can be static or time-based with automatic refresh
- Timed groups can optionally preserve completed (but unclaimed) achievements during refresh

# Installing

To install via UPM, use "Install package from git URL" and add the following:

```
1. https://github.com/CliffCalist/Unity-Tools.git
2. https://github.com/CliffCalist/game-achievements.git
```

# Quick Start

## Step 1 — Create a reward dispenser

Each reward consists of two parts:

- `RewardConfig`: a ScriptableObject that holds configuration data
- `IRewardDispencer`: interface that defines how the reward is actually granted

The base `RewardConfig` already includes `Amount`. Is a numeric value representing how much reward to give.
These field don’t need to be duplicated in your subclass — just inherit and add what’s specific to your reward type.

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
    public Type TargetConfigType {get; } = typeof(CurrencyRewardConfig);

    public void DispenseReward(RewardConfig rewardConfig)
    {
        if (rewardConfig is not CurrencyRewardConfig config)
            throw new InvalidCastException($"Expected {nameof(CurrencyRewardConfig)}, got {rewardConfig.GetType().Name}");

        PlayerWallet.Add(config.CurrencyId, config.Amount);
    }
}
```

## Step 2 — Create an achievement

Each achievement is defined by two parts:

- `AchievementConfig`: a ScriptableObject that holds metadata and conditions
- `AchievementHandler`: a component that listens for game events and forwards progress to the correct achievements

The base `AchievementConfig` includes:

- `Id` — identification
- `TargetProgress` — required value for completion
- `Reward` — the reward to be granted

To define your own achievement, subclass `AchievementConfig` with additional parameters:

```csharp
[CreateAssetMenu(menuName = "Achievements/Configs/Kill Enemies")]
public class KillEnemiesAchievementConfig : AchievementConfig
{
    [SerializeField] private string _enemyType;
    public string EnemyType => _enemyType;
}
```

Then, create a handler that tracks related gameplay events and reports progress. Handlers inherit from `AchievementHandler`, which provides:

- A list of tracked achievements
- Validation logic
- Utility methods for applying progress

Example handler:

```csharp
public class KillEnemiesAchievementHandler : AchievementHandler
{
    public void OnEnemyKilled(string killedType)
    {
        foreach (var achievement in _achievements)
        {
            if (achievement is KillEnemiesAchievement kill && kill.EnemyType == killedType)
                kill.AddProgress(1);
        }
    }
}
```

The framework does not require handlers to be initialized in any specific way. However, if a handler needs to subscribe to events, retrieve dependencies, or perform other setup logic, the developer is responsible for implementing that according to their project requirements.

## Step 3 — Configure achievement groups

The framework provides built-in support for static and time-based achievement groups.

- **Simple Group**: Displays all assigned achievements.
- **Timed Group**: Periodically refreshes a limited set of visible achievements.

Both group types require only a ScriptableObject config to set up.

Example: Create a timed group config via the Unity editor menu:

```
Assets → Create → Achievements → Simple/Timed Group
```

All group configs inherit from the base `AchievementGroupConfig`, which includes:

- `Id`: a unique identifier for the group
- `Achievements`: a list of achievement configs included in the group

The default `AchievementGroupConfig` adds no extra logic — it simply groups achievements, making it suitable for representing categories like "Daily", "Exploration", or "Challenges".

The `TimedAchievementGroupConfig` provides additional behavior for time-based groups:

- `_activeCount`: maximum number of active achievements selected from the group
- `_refreshTimeRate`: how often to refresh the active achievements (in seconds)
- `_saveUnreceivedAchievements`: if true, completed but unclaimed achievements are preserved during refresh

This makes it easy to configure rotating daily, weekly, or seasonal achievement groups.

Custom groups can be implemented by inheriting from `AchievementGroup<TConfig>` and creating your own config type.

## Step 4 — Create and initialize the AchievementsService

```csharp
public class MyAchievementSystem : MonoBehaviour
{
    [SerializeField] private List<AchievementHandler> _handlers;

    [SerializeField] private List<IAchievementRewardDispencer> _rewardDispencers;
    [SerializeField] private List<AchievementConfig> _achievementConfigs;

    [SerializeField] private AchievementGroupConfig _simpleGroupConfig;
    [SerializeField] private TimedAchievementGroupConfig _timedGroupConfig;

    private AchievementsService _service;

    private void Start()
    {
        // Create achievement factory and service, then register handlers and achievements
        var factory = new AchievementFactory(_rewardDispencers);
        _service = new AchievementsService(factory);
        _service.AddManyHandlers(_handlers);
        _service.AddManyAchievementsByConfig(_achievementConfigs);

        // Create and add groups (simple and time-based)
        // If the factory instance is not accessible, you can use service.AchievementFactory
        var simpleGroup = new SimpleAchievementGroup(_simpleGroupConfig, factory);
        _service.AddGroup(simpleGroup);

        // The myTickableRegistrar enables ticking for time-based groups — see advanced section for more
        var timedGroup = new TimedAchievementGroup(_timedGroupConfig, factory, new MyTickableRegistrar());
        _service.AddGroup(timedGroup);

        // Initialize the service — this also initializes all groups (starts timers, etc.)
        _service.Init();
    }

    private void OnDestroy()
    {
        _service.Dispose();
    }
}
```


# Advanced Topics

## Saving and Restoring State

The framework interacts with achievement and group state through interfaces. This allows you to implement your own snapshot classes tailored to your save system.

Both `Achievement` and `AchievementGroup` support saving and restoring via:

```csharp
var snapshot = new MySnapshotImplementation();
target.CaptureStateTo(snapshot); // Save current state into snapshot

// ... Later:
target.RestoreState(snapshot);  // Restore state from snapshot
```

### Achievement Snapshot

Implement `IAchievementSnapshot` to represent the state of a single achievement:

```csharp
[Serializable]
public class MyAchievementSnapshot : IAchievementSnapshot
{
    [SerializeField] private string _id;
    [SerializeField] private int _progress;
    [SerializeField] private bool _isRewardDispenced;



    public string Id
    {
        get => _id;
        set => _id = value;
    }

    public int Progress
    {
        get => _progress;
        set => _progress = value;
    }

    public bool IsRewardDispensed
    {
        get => _isRewardDispenced;
        set => _isRewardDispenced = value;
    }
}
```

### Group Snapshot

Implement `IAchievementGroupSnapshot` to represent a group of achievements:

```csharp
[Serializable]
public class MyAchievementGroupSnapshot : IAchievementGroupSnapshot
{
    [SerializeField] private string _id;
    [SerializeField] private List<MyAchievementSnapshot> _achievements = new();



    public string Id
    {
        get => _id;
        set => _id = value;
    }
    
    public IEnumerable<IAchievementSnapshot> Achievements => _achievements.Cast<IAchievementSnapshot>();



    public IAchievementSnapshot CreateAchievement()
    {
        return new MyAchievementSnapshot();
    }

    public void AddAchievement(IAchievementSnapshot snapshot)
    {
        _achievements.Add(snapshot);
    }
}
```

These snapshot classes are fully serializable and can be saved using your preferred system (e.g. JSON, binary, PlayerPrefs, cloud, etc.).

### Achievements Service Snapshot

The `AchievementsService` also supports saving and restoring its full internal state, including both standalone achievements and registered groups.

Keep in mind the following important constraint:

> Before calling `RestoreState()`, you must ensure that all achievement configs and groups have already been added to the service. If a snapshot refers to an unknown achievement or group ID, it will be ignored and a warning will be logged to the console.

To implement a snapshot class for the service, implement the `IAchievementsServiceSnapshot` interface:

```csharp
[Serializable]
public class MyAchievementsServiceSnapshot : IAchievementsServiceSnapshot
{
    [SerializeField] private List<MyAchievementSnapshot> _achievements = new();
    [SerializeField] private List<MyAchievementGroupSnapshot> _groups = new();



    public IEnumerable<IAchievementSnapshot> Achievements => _achievements;
    public IEnumerable<IAchievementGroupSnapshot> Groups => _groups;



    public IAchievementSnapshot CreateAchievement()
    {
        return new MyAchievementSnapshot();
    }

    public IAchievementGroupSnapshot CreateGroup()
    {
        return new MyAchievementGroupSnapshot();
    }



    public void AddAchievement(IAchievementSnapshot snapshot)
    {
        _achievements.Add((MyAchievementSnapshot)snapshot);
    }

    public void AddGroup(IAchievementGroupSnapshot snapshot)
    {
        _groups.Add((MyAchievementGroupSnapshot)snapshot);
    }
}
```

This implementation is serializable and can be persisted using any storage system (e.g. JSON, binary files, PlayerPrefs, cloud saves, etc.).


## Custom Achievement Groups

To define a custom group, you can either reuse the default `AchievementGroupConfig` or subclass it to extend functionality.

If the default config suits your needs, use it directly. Otherwise, create your own config type:

```csharp
[CreateAssetMenu(menuName = "Achievements/Configs/My Custom Group")]
public class MyCustomAchievementGroupConfig : AchievementGroupConfig
{
    [SerializeField] private int _someExtraParameter;
    public int SomeExtraParameter => _someExtraParameter;
}
```

Next, create a custom achievement group by inheriting from `AchievementGroup<TConfig>`. You get access to the following fields and properties:

- `_config` — the group configuration
- `_achievementFactory` — the factory used to create achievements
- `_achievements` — the list of active achievements
- `IsInited` — whether the group has been initialized

You can override these virtual methods:

- `void RestoreState(IAchievementGroupSnapshot snapshot)`
- `IAchievementGroupSnapshot CaptureStateTo(IAchievementGroupSnapshot snapshot)`
- `void Init()`

Utility methods available:

- `Achievement GetAchievementById(string id)`
- `AchievementConfig GetAchievementConfigById(string id)`
- `TAchievementConfig GetAchievementConfigById<TAchievementConfig>(string id)`
- `void ClearAllAchievements()`

Example:

```csharp
public class MyCustomGroup : AchievementGroup<MyCustomAchievementGroupConfig>
{
    public MyCustomGroup(MyCustomAchievementGroupConfig config, IAchievementFactory factory) 
        : base(config, factory)
    {}

    public override void Init()
    {
        // Optional: run custom logic before or after default initialization
        Debug.Log("Custom Init");
        base.Init(); // Remove this line if you don't want the base behavior
    }

    public override void RestoreState(IAchievementGroupSnapshot snapshot)
    {
        base.RestoreState(snapshot);
        Debug.Log("State Restored");
    }

    public override IAchievementGroupSnapshot CaptureStateTo(IAchievementGroupSnapshot snapshot)
    {
        Debug.Log("State Captured");
        return base.CaptureStateTo(snapshot);
    }
}
```

**Note:** The base `Init()` implementation creates achievements based on the configs if they are not already in `_achievements`. If this behavior is not desired, you can skip calling `base.Init()` and implement your own setup.

## UI Integration

While it might be tempting to add UI-specific fields like icons, titles, or descriptions directly to the achievement and group configs, doing so would violate the Single Responsibility Principle and reduce flexibility.

Instead, we recommend using a dedicated UI config layer — for example, by integrating the [ViewConfigurations](https://github.com/CliffCalist/view-configurations) framework. It allows you to define separate ScriptableObject-based configurations specifically for the UI representation of your data.

This approach offers multiple benefits:

- Clear separation of concerns between gameplay logic and UI
- Easier reuse and testing of configs
- More scalable and flexible customization options (e.g., per-locale labels, conditional formatting, etc.)

Refer to the [ViewConfigurations](https://github.com/CliffCalist/view-configurations) README for more information on how to structure and link UI configs to achievements or groups.

# Roadmap

- [x] Centralized service for managing all achievement mechanics
- [ ] Have a feature request? Open an issue or discussion to suggest improvements or extensions.
