namespace WhiteArrow.GameAchievements
{
    public interface ITimedAchievementGroupSnapshot : IAchievementGroupSnapshot
    {
        public string LastRefreshDate { get; set; }
    }
}