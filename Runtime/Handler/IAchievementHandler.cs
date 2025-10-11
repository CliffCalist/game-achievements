using System;

namespace WhiteArrow.GameAchievements
{
    public interface IAchievementHandler
    {
        Type TargetConfigType { get; }


        void Init();
        bool HasAchievement(Achievement achievement);
        void AddAchievement(Achievement achievement);
        void RemoveAchievement(Achievement achievement);
        void RemoveAllAchievements();
    }
}