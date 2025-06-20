using System.Collections.Generic;
using UnityEngine;

namespace WhiteArrow.GameAchievements
{
    [CreateAssetMenu(fileName = "SimpleAchievementGroup", menuName = "White Arrow/Achievements/Simple Group")]
    public class AchievementGroupConfig : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private List<AchievementConfig> _achievements;



        public string Id => _id;
        public IReadOnlyList<AchievementConfig> Achievements => _achievements;
    }
}