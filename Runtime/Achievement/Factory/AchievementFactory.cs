using System;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.GameAchievements
{
    public class AchievementFactory : MonoBehaviour, IAchievementFactory
    {
        [SerializeField] private InterfacesList<IAchievementHandler> _handlers;
        [SerializeField] private InterfacesList<IRewardDispencer> _rewardDispensers;



        public Achievement Create(AchievementConfig config)
        {
            var handler = _handlers.First(h => h.TargetConfigType == config.GetType());
            if (handler == null)
                throw new InvalidOperationException($"There is no handler for achievement type {config.GetType().Name}");

            var rewardDispencer = _rewardDispensers.First(d => d.TargetConfigType == config.Reward.GetType());
            if (rewardDispencer == null)
                throw new InvalidOperationException($"There is no reward dispenser for reward type {config.Reward.GetType().Name}");

            return new(config, handler, rewardDispencer);
        }
    }
}