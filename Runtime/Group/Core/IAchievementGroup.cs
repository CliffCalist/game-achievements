using System;
using System.Collections.Generic;

namespace WhiteArrow.GameAchievements
{
    public interface IAchievementGroup : IDisposable
    {
        IReadOnlyList<Achievement> Achievements { get; }


        void RestoreState(IAchievementGroupSnapshot snapshot);
        IAchievementGroupSnapshot CaptureStateTo(IAchievementGroupSnapshot snapshot);

        void Init();
    }
}