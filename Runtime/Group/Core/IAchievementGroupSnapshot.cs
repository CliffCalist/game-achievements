using System.Collections.Generic;

namespace WhiteArrow.GameAchievements
{
    public interface IAchievementGroupSnapshot
    {
        string Id { get; set; }
        IEnumerable<IAchievementSnapshot> Achievements { get; }



        IAchievementSnapshot CreateAchievement();
        void AddAchievement(IAchievementSnapshot snapshot);
    }
}