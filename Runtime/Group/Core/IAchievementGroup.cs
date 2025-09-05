using System;
using System.Collections.Generic;

namespace WhiteArrow.GameAchievements
{
    public interface IAchievementGroup
    {
        AchievementGroupConfig Config { get; }
        IReadOnlyCollection<Achievement> Achievements { get; }
        bool IsAllAchievementsCompleted { get; }



        event Action<AchievementGroupUpdate> ActiveAchievementsChanged;
        event Action AllAchievementsCompleted;


        void RestoreState(IAchievementGroupSnapshot snapshot);
        IAchievementGroupSnapshot CaptureStateTo(IAchievementGroupSnapshot snapshot);

        void Init();
    }
}