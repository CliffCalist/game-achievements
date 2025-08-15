namespace WhiteArrow.GameAchievements
{
    public interface ITickableAchievementGroupRegistrar
    {
        void Register(ITickableAchievementGroup tickable);
        void Unregister(ITickableAchievementGroup tickable);
    }
}