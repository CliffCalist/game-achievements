using System;
using System.Collections.Generic;

namespace WhiteArrow.GameAchievements
{
    public class AchievementsService : IDisposable
    {
        private readonly IAchievementFactory _factory;
        private readonly Dictionary<string, Achievement> _achievementsById = new();

        private readonly HashSet<IAchievementHandler> _handlers = new();


        public ICollection<Achievement> ActiveAchievements => _achievementsById.Values;



        public AchievementsService(IAchievementFactory factory)
        {
            if (UnityCheck.IsDestroyed(factory))
                throw new ArgumentNullException(nameof(factory));

            _factory = factory;
        }


        public void AddHandler(IAchievementHandler handler)
        {
            if (UnityCheck.IsDestroyed(handler))
                throw new ArgumentNullException(nameof(handler));

            _handlers.Add(handler);

            foreach (var achievement in _achievementsById.Values)
                TryBindAchievementToHandler(achievement, handler);
        }

        public void AddManyHandlers(IEnumerable<IAchievementHandler> handlers)
        {
            foreach (var handler in handlers)
                AddHandler(handler);
        }



        public void AddAchievementByConfig(AchievementConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var achievement = _factory.Create(config);
            _achievementsById[config.Id] = achievement;

            foreach (var handler in _handlers)
                TryBindAchievementToHandler(achievement, handler);
        }

        public void AddManyAchievementsByConfig(IEnumerable<AchievementConfig> configs)
        {
            foreach (var config in configs)
                AddAchievementByConfig(config);
        }



        public bool TryGetAchievement(string id, out Achievement achievement)
        {
            return _achievementsById.TryGetValue(id, out achievement);
        }

        public Achievement GetAchievement(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            if (_achievementsById.TryGetValue(id, out var achievement))
                return achievement;

            throw new KeyNotFoundException($"Achievement with ID '{id}' was not found.");
        }



        private void TryBindAchievementToHandler(Achievement achievement, IAchievementHandler handler)
        {
            if (handler.TargetConfigType == achievement.Config.GetType())
                handler.AddAchievement(achievement);
        }



        public void Dispose()
        {
            foreach (var handler in _handlers)
                handler.RemoveAllAchievements();
        }
    }
}