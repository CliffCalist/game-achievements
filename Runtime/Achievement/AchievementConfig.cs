using UnityEngine;

namespace WhiteArrow.GameAchievements
{
    public abstract class AchievementConfig : ScriptableObject
    {
        [Header("Meta")]
        [SerializeField] private string _id;
        [SerializeField, Min(1)] private int _targetProgressPoints;
        [SerializeField] private AchievementRewardConfig _reward;



        public string Id => _id;
        public int TargetProgress => _targetProgressPoints;
        public AchievementRewardConfig Reward => _reward;
    }
}