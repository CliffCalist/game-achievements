using System.Collections.Generic;

namespace WhiteArrow.GameAchievements
{
    public interface IAchievementGroupSnapshot
    {
        string Id { get; set; }
        List<IAchievementSnapshot> Achievements { get; set; }



        IAchievementSnapshot CreateAchievementInstance();
    }
}