using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.GameAchievements
{
    public class AchievementGroup<TConfig> : IAchievementGroup
        where TConfig : AchievementGroupConfig
    {
        protected readonly TConfig _config;

        protected readonly IAchievementFactory _achievementFactory;
        private readonly Dictionary<string, Achievement> _achievements = new();



        public bool IsInited { get; protected set; }

        AchievementGroupConfig IAchievementGroup.Config => _config;
        public TConfig Config => _config;

        public IReadOnlyCollection<Achievement> Achievements => _achievements.Values;
        public bool IsAllAchievementsCompleted => _achievements.Values.All(a => a.IsCompleted);
        public bool IsAllRewardsDispenced => _achievements.Values.All(a => a.IsRewardDispensed);



        public event Action<AchievementGroupUpdate> ActiveAchievementsChanged;
        public event Action AllAchievementsCompleted;
        public event Action AllRewardsDispenced;



        protected AchievementGroup(TConfig config, IAchievementFactory achievementFactory)
        {
            if (achievementFactory == null)
                throw new ArgumentNullException(nameof(achievementFactory));

            _config = config ?? throw new ArgumentNullException(nameof(config));
            _achievementFactory = achievementFactory;
        }



        public virtual void RestoreState(IAchievementGroupSnapshot snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            if (snapshot.Id != _config.Id)
                throw new ArgumentException($"Snapshot ID '{snapshot.Id}' does not match group ID '{_config.Id}'");

            var achievementSnapshots = snapshot.Achievements;
            foreach (var achievementSnapshot in achievementSnapshots)
            {
                if (achievementSnapshot == null)
                    throw new ArgumentNullException(nameof(achievementSnapshot));

                if (TryGetAchievementById(achievementSnapshot.Id, out var achievement))
                    achievement.RestoreState(achievementSnapshot);
                else
                {
                    var achievementConfig = GetAchievementConfigById(achievementSnapshot.Id);
                    if (achievementConfig == null)
                    {
                        Debug.LogWarning($"Detected unknown achievement in snapshot. ID: [{achievementSnapshot.Id}]");
                        continue;
                    }

                    achievement = _achievementFactory.Create(achievementConfig);
                    achievement.RestoreState(achievementSnapshot);
                    achievement.Completed += OnAchievementCompleted;
                    achievement.RewardDispensed += OnRewardDispenced;
                    _achievements.Add(achievement.Config.Id, achievement);
                }
            }

            if (IsAllAchievementsCompleted)
                AllAchievementsCompleted?.Invoke();

            if (IsAllRewardsDispenced)
                AllRewardsDispenced?.Invoke();
        }

        public virtual IAchievementGroupSnapshot CaptureStateTo(IAchievementGroupSnapshot snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            snapshot.Id = _config.Id;

            foreach (var achievement in _achievements.Values)
            {
                var achievementSnapshot = snapshot.CreateAchievement();
                achievement.CaptureStateTo(achievementSnapshot);
                snapshot.AddAchievement(achievementSnapshot);
            }

            return snapshot;
        }



        public virtual void Init()
        {
            if (IsInited)
                throw new InvalidOperationException("Group is already inited.");

            foreach (var achievementConfig in _config.AllAchievements)
            {
                if (!TryGetAchievementById(achievementConfig.Id, out var achievement))
                {
                    achievement = _achievementFactory.Create(achievementConfig);
                    achievement.Completed += OnAchievementCompleted;
                    achievement.RewardDispensed += OnRewardDispenced;
                    _achievements.Add(achievement.Config.Id, achievement);
                }
            }

            IsInited = true;

            if (IsAllAchievementsCompleted)
                AllAchievementsCompleted?.Invoke();

            if (IsAllRewardsDispenced)
                AllRewardsDispenced?.Invoke();
        }



        public Achievement GetAchievementById(string id)
        {
            return _achievements[id];
        }

        public bool TryGetAchievementById(string id, out Achievement achievement)
        {
            return _achievements.TryGetValue(id, out achievement);
        }



        public AchievementConfig GetAchievementConfigById(string id)
        {
            return _config.AllAchievements.FirstOrDefault(c => c.Id == id);
        }

        public TAchievementConfig GetAchievementConfigById<TAchievementConfig>(string id)
            where TAchievementConfig : AchievementConfig
        {
            var config = GetAchievementConfigById(id);

            if (config is not TAchievementConfig typedConfig)
                throw new KeyNotFoundException($"Achievement config with ID '{id}' was not found.");
            else return typedConfig;
        }




        protected void ApplyActiveAchievementsChange(AchievementGroupUpdate update)
        {
            if (update.Removed != null)
            {
                foreach (var removed in update.Removed)
                {
                    if (_achievements.ContainsKey(removed.Config.Id))
                    {
                        removed.Completed -= OnAchievementCompleted;
                        removed.RewardDispensed -= OnRewardDispenced;
                        _achievements.Remove(removed.Config.Id);
                    }
                    else Debug.LogWarning($"Cannot remove {nameof(Achievement)} with ID '{removed.Config.Id}' because it was not found in the group.");
                }
            }

            if (update.Added != null)
            {
                foreach (var added in update.Added)
                {
                    if (!_achievements.ContainsKey(added.Config.Id))
                    {
                        added.Completed += OnAchievementCompleted;
                        added.RewardDispensed += OnRewardDispenced;
                        _achievements.Add(added.Config.Id, added);
                    }
                    else Debug.LogWarning($"Cannot add {nameof(Achievement)} with ID '{added.Config.Id}' because it already exists in the group.");
                }
            }

            ActiveAchievementsChanged?.Invoke(update);

            if (IsAllAchievementsCompleted)
                AllAchievementsCompleted?.Invoke();
        }

        private void OnAchievementCompleted()
        {
            if (IsAllAchievementsCompleted)
                AllAchievementsCompleted?.Invoke();
        }

        private void OnRewardDispenced()
        {
            if (IsAllRewardsDispenced)
                AllRewardsDispenced?.Invoke();
        }
    }
}