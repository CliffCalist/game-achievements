using UnityEngine;

namespace WhiteArrow.GameAchievements
{
    public abstract class AchievementFactoryByConfig : MonoBehaviour
    {
        public abstract bool CanCreateBy(AchievementConfig config);

        public abstract Achievement Create(AchievementConfig config, IRewardDispencer rewardDispencer);
    }
}