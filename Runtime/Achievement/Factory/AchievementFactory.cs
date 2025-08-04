using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.GameAchievements
{
    public class AchievementFactory : MonoBehaviour, IAchievementFactory
    {
        [SerializeField] private InterfacesList<IRewardDispencer> _rewardDispensers;
        [SerializeField] private List<AchievementFactoryByConfig> _factories;


        public Achievement Create(AchievementConfig config)
        {
            var factory = _factories.First(q => q.CanCreateBy(config));

            var rewardDispencer = _rewardDispensers.First(q => q.TargetConfigType == config.Reward.GetType());
            if (rewardDispencer == null)
                throw new InvalidOperationException($"There is no reward dispenser for reward type {config.Reward.GetType().Name}");

            return factory.Create(config, rewardDispencer);
        }
    }
}