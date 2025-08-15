using UnityEngine;

namespace WhiteArrow.GameAchievements
{
    public abstract class AchievementRewardConfig : ScriptableObject
    {
        [SerializeField, Min(0)] private int _amount = 1;



        public int Amount => _amount;
    }
}