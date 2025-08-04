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

        protected readonly List<Achievement> _achievements = new();



        public TConfig Config => _config;
        protected bool _isInited { get; private set; }
        public IReadOnlyList<Achievement> Achievements => _achievements;



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

            ClearAllAchievements();

            var achievementSnapshots = snapshot.Achievements;
            foreach (var achievementSnapshot in achievementSnapshots)
            {
                if (UnityCheck.IsDestroyed(achievementSnapshot))
                    throw new ArgumentNullException(nameof(achievementSnapshot));

                var achievementConfig = _config.Achievements.FirstOrDefault(c => c.Id == achievementSnapshot.Id);
                if (achievementConfig == null)
                {
                    Debug.LogWarning($"Detected unknown achievement in snapshot. ID: [{achievementSnapshot.Id}]");
                    continue;
                }

                var achievement = _achievementFactory.Create(achievementConfig);
                achievement.RestoreState(achievementSnapshot);
                _achievements.Add(achievement);
            }
        }



        public virtual IAchievementGroupSnapshot CaptureStateTo(IAchievementGroupSnapshot snapshot)
        {
            if (UnityCheck.IsDestroyed(snapshot))
                throw new ArgumentNullException(nameof(snapshot));

            snapshot.Id = _config.Id;

            foreach (var achievement in _achievements)
            {
                var achievementSnapshot = snapshot.CreateAchievement();
                achievement.CaptureStateTo(achievementSnapshot);
                snapshot.AddAchievement(achievementSnapshot);
            }

            return snapshot;
        }



        public void Init()
        {
            if (_isInited)
                throw new InvalidOperationException("Achievement group is already inited");

            InitCore();
            _isInited = true;
        }

        protected virtual void InitCore() { }



        protected void ClearAllAchievements()
        {
            _achievements.Clear();
        }
    }
}