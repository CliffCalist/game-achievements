using System;

namespace WhiteArrow.GameAchievements
{
    public interface IAchievementHandler
    {
        Type TargetConfigType { get; }



        bool HasAchievement(Achievement achievement);
        void AddAchievement(Achievement achievement);
        void RemoveAchievement(Achievement achievement);
        void RemoveAllAchievements();
    }
}