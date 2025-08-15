using System;

namespace WhiteArrow.GameAchievements
{
    public interface ITickableAchievementGroup : IDisposable
    {
        void Tick(float deltaTime);
    }
}