using System;

namespace WhiteArrow.GameAchievements
{
    public interface IRewardDispencer
    {
        Type TargetConfigType { get; }


        void DispenseReward(RewardConfig rewardConfig);
    }
}