using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.GameAchievements
{
    public abstract class AchievementGroup<TConfig>
        where TConfig : AchievementGroupConfig
    {
        protected readonly TConfig _config;

        protected readonly IAchievementFactory _achievementFactory;
        protected readonly Dictionary<string, Achievement> _achievements = new();



        public bool IsInited { get; protected set; }
        public TConfig Config => _config;
        public IReadOnlyCollection<Achievement> Achievements => _achievements.Values;



        protected AchievementGroup(TConfig config, IAchievementFactory achievementFactory)
        {
            if (UnityCheck.IsDestroyed(achievementFactory))
                throw new ArgumentNullException(nameof(achievementFactory));

            _config = config ?? throw new ArgumentNullException(nameof(config));
            _achievementFactory = achievementFactory;
        }



        public virtual void RestoreState(IAchievementGroupSnapshot snapshot)
        {
            if (UnityCheck.IsDestroyed(snapshot))
                throw new ArgumentNullException(nameof(snapshot));

            if (snapshot.Id != _config.Id)
                throw new ArgumentException($"Snapshot ID '{snapshot.Id}' does not match group ID '{_config.Id}'");

            var achievementSnapshots = snapshot.Achievements;
            foreach (var achievementSnapshot in achievementSnapshots)
            {
                if (UnityCheck.IsDestroyed(achievementSnapshot))
                    throw new ArgumentNullException(nameof(achievementSnapshot));

                var achievement = GetAchievementById(achievementSnapshot.Id);

                if (achievement == null)
                {
                    var achievementConfig = GetAchievementConfigById(achievementSnapshot.Id);
                    if (achievementConfig == null)
                    {
                        Debug.LogWarning($"Detected unknown achievement in snapshot. ID: [{achievementSnapshot.Id}]");
                        continue;
                    }

                    achievement = _achievementFactory.Create(achievementConfig);
                    achievement.RestoreState(achievementSnapshot);
                    _achievements.Add(achievement.Config.Id, achievement);
                }
                else achievement.RestoreState(achievementSnapshot);
            }
        }

        public virtual IAchievementGroupSnapshot CaptureStateTo(IAchievementGroupSnapshot snapshot)
        {
            if (UnityCheck.IsDestroyed(snapshot))
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

            foreach (var achievementConfig in _config.Achievements)
            {
                var achievement = GetAchievementById(achievementConfig.Id);
                if (achievement == null)
                {
                    achievement = _achievementFactory.Create(achievementConfig);
                    _achievements.Add(achievement.Config.Id, achievement);
                }
            }

            IsInited = true;
        }



        public Achievement GetAchievementById(string id)
        {
            return _achievements[id];
        }



        public AchievementConfig GetAchievementConfigById(string id)
        {
            return _config.Achievements.FirstOrDefault(c => c.Id == id);
        }

        public TAchievementConfig GetAchievementConfigById<TAchievementConfig>(string id)
            where TAchievementConfig : AchievementConfig
        {
            var config = GetAchievementConfigById(id);

            if (config is not TAchievementConfig typedConfig)
                throw new KeyNotFoundException($"Achievement config with ID '{id}' was not found.");
            else return typedConfig;
        }



        protected void ClearAllAchievements()
        {
            _achievements.Clear();
        }
    }
}