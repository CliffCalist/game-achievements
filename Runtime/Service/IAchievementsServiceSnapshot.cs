using System.Collections.Generic;

namespace WhiteArrow.GameAchievements
{
    public interface IAchievementsServiceSnapshot
    {
        IEnumerable<IAchievementSnapshot> NonGroupedAchievements { get; }
        IEnumerable<IAchievementGroupSnapshot> Groups { get; }



        void AddNonGroupedAchievement(IAchievementSnapshot snapshot);
        void AddGroup(IAchievementGroupSnapshot snapshot);



        IAchievementSnapshot CreateAchievement();
        IAchievementGroupSnapshot CreateGroup();
        ITimedAchievementGroupSnapshot CreateTimedGroup();
    }
}