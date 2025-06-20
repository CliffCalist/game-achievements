using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.GameAchievements
{
    public class AchievementFactory : MonoBehaviour, IAchievementFactory
    {
        [SerializeField] private RewardDispencerFactory _rewardDispencerFactory;
        [SerializeField] private List<AchievementFactoryByConfig> _factories;


        public Achievement Create(AchievementConfig config)
        {
            var factory = _factories.First(q => q.CanCreateBy(config));
            return factory.Create(config, _rewardDispencerFactory);
        }
    }
}