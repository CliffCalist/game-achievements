namespace WhiteArrow.GameAchievements
{
    public class SimpleAchievementGroup : AchievementGroup<AchievementGroupConfig>
    {
        public SimpleAchievementGroup(AchievementGroupConfig config, IAchievementFactory achievementFactory)
            : base(config, achievementFactory)
        { }



        protected override void InitCore()
        {
            foreach (var achievementConfig in _config.Achievements)
            {
                var achievement = _achievementFactory.Create(achievementConfig);
                _achievements.Add(achievement);
            }
        }
    }
}