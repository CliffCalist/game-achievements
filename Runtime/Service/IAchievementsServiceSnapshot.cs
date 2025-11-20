using System.Collections.Generic;

namespace WhiteArrow.GameAchievements
{
    public interface IAchievementsServiceSnapshot
    {
        IEnumerable<IAchievementSnapshot> NonGroupedAchievements { get; }
        IEnumerable<IAchievementGroupSnapshot> Groups { get; }
        IEnumerable<ITimedAchievementGroupSnapshot> TimedGroups { get; }



        void AddNonGroupedAchievement(IAchievementSnapshot snapshot);
        void AddGroup(IAchievementGroupSnapshot snapshot);
        void AddTimedGroup(IAchievementGroupSnapshot snapshot);



        IAchievementSnapshot CreateAchievement();
        IAchievementGroupSnapshot CreateGroup();
        ITimedAchievementGroupSnapshot CreateTimedGroup();
    }
}