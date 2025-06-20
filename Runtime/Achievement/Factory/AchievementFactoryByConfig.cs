using System;
using UnityEngine;

namespace WhiteArrow.GameAchievements
{
    public abstract class AchievementFactoryByConfig : MonoBehaviour
    {
        public abstract bool CanCreateBy(AchievementConfig config);



        public Achievement Create(AchievementConfig config, RewardDispencerFactory rewardDispencerFactory)
        {
            if (config is null)
                throw new ArgumentNullException(nameof(config));

            if (rewardDispencerFactory is null)
                throw new ArgumentNullException(nameof(rewardDispencerFactory));

            var rewardDispencer = rewardDispencerFactory.CreateDispencer(config.Reward);
            return Create(config, rewardDispencer);
        }

        protected abstract Achievement Create(AchievementConfig config, IRewardDispencer rewardDispencer);
    }
}