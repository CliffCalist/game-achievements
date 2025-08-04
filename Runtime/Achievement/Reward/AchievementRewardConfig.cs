using UnityEngine;

namespace WhiteArrow.GameAchievements
{
    public abstract class AchievementRewardConfig : ScriptableObject
    {
        [Header("Optional")]
        [SerializeField] private Sprite _icon;

        [Header("Required")]
        [SerializeField, Min(0)] private int _amount = 1;



        public Sprite Icon => _icon;

        public int Amount => _amount;
    }
}