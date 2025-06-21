using System;

namespace WhiteArrow.GameAchievements
{
    public interface IAchievementGroup : IDisposable
    {
        void RestoreState(IAchievementGroupSnapshot snapshot);
        IAchievementGroupSnapshot CaptureStateTo(IAchievementGroupSnapshot snapshot);

        void Init();
    }
}