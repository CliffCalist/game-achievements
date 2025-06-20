namespace WhiteArrow.GameAchievements
{
    public interface IAchievementHandler
    {
        void AddAchievement(Achievement achievement);
        void RemoveAchievement(Achievement achievement);
    }
}