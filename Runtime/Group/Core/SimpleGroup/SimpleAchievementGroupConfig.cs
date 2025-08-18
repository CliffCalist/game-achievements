using System.Collections.Generic;
using UnityEngine;

namespace WhiteArrow.GameAchievements
{
    [CreateAssetMenu(fileName = "SimpleAchievementGroup", menuName = "White Arrow/Achievements/Simple Group")]
    public class SimpleAchievementGroupConfig : AchievementGroupConfig
    {
        [SerializeField] private List<AchievementConfig> _achievements;



        public override IReadOnlyList<AchievementConfig> AllAchievements => _achievements;
    }
}