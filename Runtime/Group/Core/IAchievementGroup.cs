using System;
using System.Collections.Generic;

namespace WhiteArrow.GameAchievements
{
    public interface IAchievementGroup
    {
        AchievementGroupConfig Config { get; }
        IReadOnlyCollection<Achievement> Achievements { get; }
        bool IsAllAchievementsCompleted { get; }
        bool IsAllRewardsDispenced { get; }



        event Action<AchievementGroupUpdate> ActiveAchievementsChanged;
        event Action AllAchievementsCompleted;
        event Action AllRewardsDispenced;


        void RestoreState(IAchievementGroupSnapshot snapshot);
        IAchievementGroupSnapshot CaptureStateTo(IAchievementGroupSnapshot snapshot);

        void Init();
    }
}