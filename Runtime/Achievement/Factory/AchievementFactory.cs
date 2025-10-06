using System;
using System.Collections.Generic;

namespace WhiteArrow.GameAchievements
{
    public class AchievementFactory : IAchievementFactory
    {
        private readonly List<IAchievementRewardDispencer> _rewardDispensers;



        public AchievementFactory(List<IAchievementRewardDispencer> rewardDispensers)
        {
            _rewardDispensers = rewardDispensers ?? throw new ArgumentNullException(nameof(rewardDispensers));
        }



        public Achievement Create(AchievementConfig config)
        {
            var rewardDispencer = _rewardDispensers.Find(d => d.TargetConfigType == config.Reward.GetType());
            if (rewardDispencer == null)
                throw new InvalidOperationException($"There is no reward dispenser for reward type {config.Reward.GetType().Name}");

            return new(config, rewardDispencer);
        }
    }
}