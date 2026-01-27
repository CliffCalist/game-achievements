using UnityEngine;

namespace WhiteArrow.GameAchievements
{
    public abstract class AchievementRewardConfig : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField, Min(0)] private int _amount = 1;



        public string ID => _id;
        public int Amount => _amount;
    }
}