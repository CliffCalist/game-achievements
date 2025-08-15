using System.Collections.Generic;

namespace WhiteArrow.GameAchievements
{
    public interface IAchievementGroup
    {
        AchievementGroupConfig Config { get; }
        IReadOnlyCollection<Achievement> Achievements { get; }

        void Init();
    }
}