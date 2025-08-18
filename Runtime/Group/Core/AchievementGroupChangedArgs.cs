using System.Collections.Generic;

namespace WhiteArrow.GameAchievements
{
    public readonly struct AchievementGroupChangedArgs
    {
        public IEnumerable<Achievement> Previous { get; }
        public IEnumerable<Achievement> Current { get; }



        public AchievementGroupChangedArgs(IEnumerable<Achievement> previous, IEnumerable<Achievement> current)
        {
            Previous = previous;
            Current = current;
        }
    }
}