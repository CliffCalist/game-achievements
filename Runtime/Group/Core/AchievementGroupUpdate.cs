using System.Collections.Generic;

namespace WhiteArrow.GameAchievements
{
    public class AchievementGroupUpdate
    {
        public IEnumerable<Achievement> Removed { get; }
        public IEnumerable<Achievement> Added { get; }



        public AchievementGroupUpdate(IEnumerable<Achievement> removed, IEnumerable<Achievement> added)
        {
            Removed = removed;
            Added = added;
        }
    }
}