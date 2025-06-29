using System;
using System.Collections.Generic;
using UnityEngine;

namespace WhiteArrow.GameAchievements
{
    public abstract class AchievementHandler : MonoBehaviour, IAchievementHandler
    {
        protected readonly List<Achievement> _achievements = new();



        public abstract bool CanHandle(Achievement achievement);

        public bool Contains(Achievement achievement)
        {
            return _achievements.Contains(achievement);
        }

        public void AddAchievement(Achievement achievement)
        {
            if (!CanHandle(achievement))
                throw new ArgumentException($"Cannot add {achievement.GetType().Name} to {GetType().Name}");

            if (Contains(achievement))
                throw new ArgumentException($"{achievement.GetType().Name} is already added to {GetType().Name}");

            _achievements.Add(achievement);
        }

        public void RemoveAchievement(Achievement achievement)
        {
            _achievements.Remove(achievement);
        }



        public void ForceAddProgressPointsToAll(int value = 1)
        {
            foreach (var achievement in _achievements)
            {
                if (!achievement.IsCompleted)
                    achievement.AddProgress(value);
            }
        }



        public virtual void Init() { }
    }
}