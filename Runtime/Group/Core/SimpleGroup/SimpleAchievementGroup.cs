namespace WhiteArrow.GameAchievements
{
    public class SimpleAchievementGroup : AchievementGroup<SimpleAchievementGroupConfig>
    {
        public SimpleAchievementGroup(SimpleAchievementGroupConfig config, IAchievementFactory achievementFactory)
            : base(config, achievementFactory)
        { }
    }
}