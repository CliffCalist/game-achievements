using System.Collections.Generic;

namespace WhiteArrow.GameAchievements
{
    public interface IAchievementGroup
    {
        AchievementGroupConfig Config { get; }
        IReadOnlyCollection<Achievement> Achievements { get; }

        void RestoreState(IAchievementGroupSnapshot snapshot);
        IAchievementGroupSnapshot CaptureStateTo(IAchievementGroupSnapshot snapshot);

        void Init();
    }
}