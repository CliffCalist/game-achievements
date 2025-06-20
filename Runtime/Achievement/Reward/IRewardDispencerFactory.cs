namespace WhiteArrow.GameAchievements
{
    public interface IRewardDispencerFactory
    {
        IRewardDispencer CreateDispencer(RewardConfig config);
    }
}