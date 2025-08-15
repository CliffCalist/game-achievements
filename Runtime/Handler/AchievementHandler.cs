using System;
using System.Collections.Generic;
using UnityEngine;

namespace WhiteArrow.GameAchievements
{
    public abstract class AchievementHandler : MonoBehaviour, IAchievementHandler
    {
        protected readonly HashSet<Achievement> _achievements = new();



        public abstract Type TargetConfigType { get; }



        public bool HasAchievement(Achievement achievement)
        {
            return _achievements.Contains(achievement);
        }

        public void AddAchievement(Achievement achievement)
        {
            if (HasAchievement(achievement))
                throw new ArgumentException($"{achievement.GetType().Name} is already added to {GetType().Name}");

            _achievements.Add(achievement);
        }

        public void RemoveAchievement(Achievement achievement)
        {
            _achievements.Remove(achievement);
        }

        public void RemoveAllAchievements()
        {
            _achievements.Clear();
        }



        public void AddProgressPointsToAll(int value)
        {
            foreach (var achievement in _achievements)
            {
                if (!achievement.IsCompleted)
                    achievement.AddProgress(value);
            }
        }

        public void SetProgressPointsToAll(int value)
        {
            foreach (var achievement in _achievements)
            {
                achievement.SetProgress(value);
            }
        }
    }
}