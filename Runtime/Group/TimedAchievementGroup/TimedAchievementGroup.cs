using System;
using System.Linq;
using Random = UnityEngine.Random;

namespace WhiteArrow.GameAchievements
{
    public class TimedAchievementGroup : AchievementGroup<TimedAchievementGroupConfig>, ITickableAchievementGroup
    {
        private DateTime _lastRefreshTime = DateTime.UtcNow;



        public event Action Refreshed;



        public TimedAchievementGroup(TimedAchievementGroupConfig config, IAchievementFactory achievementFactory)
            : base(config, achievementFactory)
        { }



        public override void RestoreState(IAchievementGroupSnapshot snapshot)
        {
            base.RestoreState(snapshot);

            if (snapshot is not ITimedAchievementGroupSnapshot timedSnapshot)
                throw new ArgumentException($"Invalid snapshot type: {snapshot.GetType().Name}. Expected: {nameof(ITimedAchievementGroupSnapshot)}");

            _lastRefreshTime = DateTime.Parse(
                timedSnapshot.LastRefreshDate,
                null,
                System.Globalization.DateTimeStyles.RoundtripKind
            );
        }

        public override IAchievementGroupSnapshot CaptureStateTo(IAchievementGroupSnapshot snapshot)
        {
            base.CaptureStateTo(snapshot);

            if (snapshot is not ITimedAchievementGroupSnapshot timedSnapshot)
                throw new ArgumentException($"Invalid snapshot type: {snapshot.GetType().Name}. Expected: {nameof(ITimedAchievementGroupSnapshot)}");

            timedSnapshot.LastRefreshDate = _lastRefreshTime.ToString("o");
            return snapshot;
        }



        public override void Init()
        {
            if (IsInited)
                throw new InvalidOperationException("Group is already inited.");

            IsInited = true;
        }



        public TimeSpan CalculateTimeFromLastRefresh()
        {
            return DateTime.UtcNow - _lastRefreshTime;
        }

        public TimeSpan CalculateTimeToRefresh()
        {
            return _lastRefreshTime.AddSeconds(_config.RefreshTimeRate) - DateTime.UtcNow;
        }



        public void ForceRefresh()
        {
            if (_config.SaveUnreceivedAchievements)
                ClearNonRewardDispenseAchievements();
            else ClearAllAchievements();

            var candidates = _config.Achievements
                .OrderBy(_ => Random.value)
                .Take(_config.ActiveCount);

            foreach (var config in candidates)
            {
                var achievement = _achievementFactory.Create(config);
                _achievements.Add(achievement.Config.Id, achievement);
            }

            _lastRefreshTime = DateTime.UtcNow;
            Refreshed?.Invoke();
        }

        private void ClearNonRewardDispenseAchievements()
        {
            var achievementsToRemove = _achievements.Values.Where(a => !a.IsCompleted || a.IsRewardDispensed);

            foreach (var achievement in achievementsToRemove)
                _achievements.Remove(achievement.Config.Id);
        }



        void ITickableAchievementGroup.Tick(float deltaTime)
        {
            if (CalculateTimeFromLastRefresh().TotalSeconds >= _config.RefreshTimeRate)
            {
                _lastRefreshTime = DateTime.UtcNow;
                ForceRefresh();
            }
        }
    }
}