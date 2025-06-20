using UnityEngine;

namespace WhiteArrow.GameAchievements
{
    public abstract class AchievementConfig : ScriptableObject
    {
        [Header("Meta")]
        [SerializeField] private string _id;
        [SerializeField] private string _name;
        [SerializeField] private string _description;

        [Header("Base")]
        [SerializeField, Min(1)] private int _targetProgressPoints;
        [SerializeField] private RewardConfig _reward;



        public string Id => _id;
        public string Name => _name;
        public string Description => _description;

        public int TargetProgress => _targetProgressPoints;
        public RewardConfig Reward => _reward;
    }
}