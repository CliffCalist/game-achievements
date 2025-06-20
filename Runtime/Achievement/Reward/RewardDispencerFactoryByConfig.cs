using UnityEngine;

namespace WhiteArrow.GameAchievements
{
    public abstract class RewardDispencerFactoryByConfig : MonoBehaviour
    {
        public abstract bool CanCreateBy(RewardConfig config);
        public abstract IRewardDispencer Create(RewardConfig config);
    }
}