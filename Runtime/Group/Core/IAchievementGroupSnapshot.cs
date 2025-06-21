using System.Collections.Generic;

namespace WhiteArrow.GameAchievements
{
    public interface IAchievementGroupSnapshot
    {
        string Id { get; set; }
        IReadOnlyList<IAchievementSnapshot> Achievements { get; }



        IAchievementSnapshot CreateAchievement();
        void AddAchievement(IAchievementSnapshot snapshot);
    }
}