namespace WhiteArrow.GameAchievements
{
    public interface IAchievementSnapshot
    {
        string Id { get; set; }
        int Progress { get; set; }
        bool IsRewardDispensed { get; set; }
    }
}