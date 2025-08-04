using System;
using System.Collections.Generic;

namespace WhiteArrow.GameAchievements
{
    public class AchievementFactory : IAchievementFactory
    {
        private readonly List<IAchievementHandler> _handlers;
        private readonly List<IAchievementRewardDispencer> _rewardDispensers;



        public AchievementFactory(
            List<IAchievementHandler> handlers,
            List<IAchievementRewardDispencer> rewardDispensers)
        {
            _handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
            _rewardDispensers = rewardDispensers ?? throw new ArgumentNullException(nameof(rewardDispensers));
        }



        public Achievement Create(AchievementConfig config)
        {
            var handler = _handlers.Find(h => h.TargetConfigType == config.GetType());
            if (UnityCheck.IsDestroyed(handler))
                throw new InvalidOperationException($"There is no handler for achievement type {config.GetType().Name}");

            var rewardDispencer = _rewardDispensers.Find(d => d.TargetConfigType == config.Reward.GetType());
            if (UnityCheck.IsDestroyed(handler))
                throw new InvalidOperationException($"There is no reward dispenser for reward type {config.Reward.GetType().Name}");

            return new(config, handler, rewardDispencer);
        }
    }
}