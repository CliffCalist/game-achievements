using UnityEngine;

namespace WhiteArrow.GameAchievements
{
    [CreateAssetMenu(fileName = "TimedAchievementGroupConfig", menuName = "White Arrow/Achievements/Timed Achievement Group")]
    public class TimedAchievementGroupConfig : AchievementGroupConfig
    {
        [SerializeField, Min(1)] private int _activeCount = 1;
        [SerializeField, Min(1)] private int _refreshTimeRate = 1;
        [SerializeField] private bool _saveUnreceivedAchievements = true;



        public int ActiveCount => _activeCount;
        public int RefreshTimeRate => _refreshTimeRate;
        public bool SaveUnreceivedAchievements => _saveUnreceivedAchievements;
    }
}