namespace WhiteArrow.GameAchievements
{
    public class SimpleAchievementGroup : AchievementGroup<AchievementGroupConfig>
    {
        public SimpleAchievementGroup(AchievementGroupConfig config, IAchievementFactory achievementFactory)
            : base(config, achievementFactory)
        { }
    }
}