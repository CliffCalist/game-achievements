using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.GameAchievements
{
    public class AchievementsService : IDisposable
    {
        public readonly IAchievementFactory AchievementFactory;
        private ITickableAchievementGroupRegistrar _tickableRegistrar;

        private readonly HashSet<IAchievementHandler> _handlers = new();
        private readonly Dictionary<string, IAchievementGroup> _groupsById = new();
        private readonly Dictionary<string, Achievement> _achievementsById = new();



        public bool IsInited { get; private set; }

        public IReadOnlyCollection<IAchievementGroup> Groups => _groupsById.Values;
        public IReadOnlyCollection<Achievement> NonGroupedAchievements => _achievementsById.Values;
        public IEnumerable<Achievement> AllAchievements => _achievementsById.Values.Concat(_groupsById.Values.SelectMany(g => g.Achievements));



        public AchievementsService(IAchievementFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            AchievementFactory = factory;
        }



        public void RestoreState(IAchievementsServiceSnapshot snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            foreach (var achievementSnapshot in snapshot.NonGroupedAchievements)
            {
                if (_achievementsById.TryGetValue(achievementSnapshot.Id, out var achievement))
                    achievement.RestoreState(achievementSnapshot);
                else
                    Debug.LogWarning($"Cannot restore {nameof(Achievement)} with ID '{achievementSnapshot.Id}' because it was not found in the service.");
            }

            foreach (var groupSnapshot in snapshot.Groups)
            {
                if (_groupsById.TryGetValue(groupSnapshot.Id, out var group))
                    group.RestoreState(groupSnapshot);
                else
                    Debug.LogWarning($"Cannot restore {nameof(IAchievementGroup)} with ID '{groupSnapshot.Id}' because it was not found in the service.");
            }
        }

        public void CaptureStateTo(IAchievementsServiceSnapshot snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            foreach (var achievement in _achievementsById.Values)
            {
                var achievementSnapshot = snapshot.CreateAchievement();
                achievement.CaptureStateTo(achievementSnapshot);
                snapshot.AddNonGroupedAchievement(achievementSnapshot);
            }

            foreach (var group in _groupsById.Values)
            {
                var groupSnapshot = snapshot.CreateGroup();
                group.CaptureStateTo(groupSnapshot);
                snapshot.AddGroup(groupSnapshot);
            }
        }



        public void SetTickableRegistrar(ITickableAchievementGroupRegistrar registrar)
        {
            _tickableRegistrar = registrar ?? throw new ArgumentNullException(nameof(registrar));
        }

        private void ThrowIfTickableRegistrarNotSet()
        {
            if (_tickableRegistrar == null)
                throw new NullReferenceException($"{nameof(ITickableAchievementGroupRegistrar)} is not set.");
        }



        public void AddHandler(IAchievementHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _handlers.Add(handler);

            foreach (var achievement in _achievementsById.Values)
                TryAddAchievementToHandler(achievement, handler);
        }

        public void AddManyHandlers(IEnumerable<IAchievementHandler> handlers)
        {
            foreach (var handler in handlers)
                AddHandler(handler);
        }



        public void AddAchievementByConfig(AchievementConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var achievement = AchievementFactory.Create(config);
            _achievementsById[config.Id] = achievement;

            AddAchievementToAllHandlers(achievement);
        }

        public void AddManyAchievementsByConfig(IEnumerable<AchievementConfig> configs)
        {
            foreach (var config in configs)
                AddAchievementByConfig(config);
        }



        public bool TryGetAchievement(string id, out Achievement achievement)
        {
            return _achievementsById.TryGetValue(id, out achievement);
        }

        public Achievement GetAchievement(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            if (_achievementsById.TryGetValue(id, out var achievement))
                return achievement;

            throw new KeyNotFoundException($"Achievement with ID '{id}' was not found.");
        }



        public void AddGroup(IAchievementGroup group)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            if (group is ITickableAchievementGroup tickableGroup)
            {
                ThrowIfTickableRegistrarNotSet();
                _tickableRegistrar.Register(tickableGroup);
            }

            _groupsById[group.Config.Id] = group;

            if (IsInited)
            {
                group.Init();
                AddManyAchievementsToAllHandlers(group.Achievements);
                group.ActiveAchievementsChanged += OnGroupActiveAchievementsChanged;
            }
        }

        public void AddManyGroups(IEnumerable<IAchievementGroup> group)
        {
            if (group is null)
                throw new ArgumentNullException(nameof(group));

            foreach (var g in group)
                AddGroup(g);
        }

        public void RemoveGroup(string id)
        {
            var group = _groupsById[id];
            if (group == null)
            {
                Debug.LogWarning($"Cannot remove group with ID {id} because it was not found.");
                return;
            }

            group.ActiveAchievementsChanged -= OnGroupActiveAchievementsChanged;
            RemoveManyAchievementFromAllHandlers(group.Achievements);
            _groupsById.Remove(id);

            if (group is ITickableAchievementGroup tickableGroup)
            {
                ThrowIfTickableRegistrarNotSet();
                _tickableRegistrar.Unregister(tickableGroup);
            }

            if (group is IDisposable disposableGroup)
                disposableGroup.Dispose();
        }

        private void OnGroupActiveAchievementsChanged(AchievementGroupUpdate args)
        {
            if (!IsInited)
                return;

            if (args.Removed != null)
                RemoveManyAchievementFromAllHandlers(args.Removed);

            if (args.Added != null)
                AddManyAchievementsToAllHandlers(args.Added);
        }



        public bool TryGetGroup(string id, out IAchievementGroup group)
        {
            return _groupsById.TryGetValue(id, out group);
        }



        public void Init()
        {
            if (IsInited)
                throw new InvalidOperationException($"{nameof(AchievementsService)} is already inited.");

            IsInited = true;

            foreach (var group in _groupsById.Values)
            {
                group.Init();
                AddManyAchievementsToAllHandlers(group.Achievements);
                group.ActiveAchievementsChanged += OnGroupActiveAchievementsChanged;
            }

            foreach (var handler in _handlers)
                handler.Init();
        }



        private bool TryAddAchievementToHandler(Achievement achievement, IAchievementHandler handler)
        {
            if (!IsInited)
                return false;

            if (handler.TargetConfigType == achievement.Config.GetType())
            {
                handler.AddAchievement(achievement);
                return true;
            }
            else return false;
        }

        private void AddAchievementToAllHandlers(Achievement achievement)
        {
            foreach (var handler in _handlers)
                TryAddAchievementToHandler(achievement, handler);
        }

        private void AddManyAchievementsToAllHandlers(IEnumerable<Achievement> achievements)
        {
            foreach (var achievement in achievements)
                AddAchievementToAllHandlers(achievement);
        }


        private void RemoveManyAchievementFromAllHandlers(IEnumerable<Achievement> achievements)
        {
            foreach (var handler in _handlers)
            {
                foreach (var achievement in achievements)
                {
                    if (handler.HasAchievement(achievement))
                        handler.RemoveAchievement(achievement);
                }
            }
        }



        public void Dispose()
        {
            foreach (var handler in _handlers)
                handler.RemoveAllAchievements();

            foreach (var group in _groupsById.Values)
            {
                if (group is ITickableAchievementGroup tickableGroup)
                {
                    ThrowIfTickableRegistrarNotSet();
                    _tickableRegistrar.Unregister(tickableGroup);
                }

                if (group is IDisposable disposableGroup)
                    disposableGroup.Dispose();
            }
        }
    }
}