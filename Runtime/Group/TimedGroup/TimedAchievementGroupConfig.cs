using System.Collections.Generic;
using UnityEngine;

namespace WhiteArrow.GameAchievements
{
    [CreateAssetMenu(fileName = "TimedAchievementGroupConfig", menuName = "White Arrow/Achievements/Timed Group")]
    public class TimedAchievementGroupConfig : AchievementGroupConfig
    {
        [SerializeField] private List<AchievementConfig> _achievements;
        [SerializeField, Min(1)] private int _activeCount = 1;
        [SerializeField, Min(1)] private int _refreshTimeRate = 1;
        [SerializeField] private bool _saveUnreceivedAchievements = true;



        public int ActiveCount => _activeCount;
        public int RefreshTimeRate => _refreshTimeRate;
        public bool SaveUnreceivedAchievements => _saveUnreceivedAchievements;

        public override IReadOnlyList<AchievementConfig> AllAchievements => _achievements;
    }
}