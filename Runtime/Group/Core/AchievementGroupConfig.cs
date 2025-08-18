using System.Collections.Generic;
using UnityEngine;

namespace WhiteArrow.GameAchievements
{
    public abstract class AchievementGroupConfig : ScriptableObject
    {
        [SerializeField] private string _id;



        public string Id => _id;
        public abstract IReadOnlyList<AchievementConfig> AllAchievements { get; }
    }
}