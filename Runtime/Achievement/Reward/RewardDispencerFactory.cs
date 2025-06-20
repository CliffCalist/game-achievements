using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.GameAchievements
{
    public class RewardDispencerFactory : MonoBehaviour, IRewardDispencerFactory
    {
        [SerializeField] private List<RewardDispencerFactoryByConfig> _factories;



        public IRewardDispencer CreateDispencer(RewardConfig config)
        {
            var factory = _factories.First(q => q.CanCreateBy(config));
            return factory.Create(config);
        }
    }
}