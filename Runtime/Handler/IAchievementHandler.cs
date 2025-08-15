using System;

namespace WhiteArrow.GameAchievements
{
    public interface IAchievementHandler
    {
        Type TargetConfigType { get; }



        void AddAchievement(Achievement achievement);
        void RemoveAllAchievements();
    }
}