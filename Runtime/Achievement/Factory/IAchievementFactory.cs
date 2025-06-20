namespace WhiteArrow.GameAchievements
{
    public interface IAchievementFactory
    {
        Achievement Create(AchievementConfig config);
    }
}