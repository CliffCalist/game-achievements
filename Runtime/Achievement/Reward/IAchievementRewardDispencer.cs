using System;

namespace WhiteArrow.GameAchievements
{
    public interface IAchievementRewardDispencer
    {
        Type TargetConfigType { get; }


        void DispenseReward(AchievementRewardConfig rewardConfig);
    }
}