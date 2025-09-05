using System.Collections.Generic;

namespace WhiteArrow.GameAchievements
{
    public readonly struct AchievementGroupChangedArgs
    {
        public IEnumerable<Achievement> RemovedAchievements { get; }
        public IEnumerable<Achievement> AddedAchievement { get; }



        public AchievementGroupChangedArgs(IEnumerable<Achievement> removed, IEnumerable<Achievement> added)
        {
            RemovedAchievements = removed;
            AddedAchievement = added;
        }
    }
}