using System;

namespace WhiteArrow.GameAchievements
{
    public class Achievement : IDisposable
    {
        private readonly AchievementConfig _config;
        private readonly IAchievementHandler _handler;
        private readonly IAchievementRewardDispencer _rewardDispencer;

        private int _progress;
        private bool _isRewardDispensed;



        public AchievementConfig Config => _config;

        public bool IsCompleted => _progress >= _config.TargetProgress;
        public int Progress => _progress;

        public bool IsRewardDispensed => _isRewardDispensed;



        public event Action ProgressChanged;
        public event Action Completed;
        public event Action RewardDispensed;



        public Achievement(AchievementConfig config, IAchievementHandler handler, IAchievementRewardDispencer rewardDispencer)
        {
            if (UnityCheck.IsDestroyed(handler))
                throw new ArgumentNullException(nameof(handler));

            if (UnityCheck.IsDestroyed(rewardDispencer))
                throw new ArgumentNullException(nameof(rewardDispencer));

            _config = config ?? throw new ArgumentNullException(nameof(config));
            _handler = handler;
            _rewardDispencer = rewardDispencer;
        }



        public T GetConfigAs<T>() where T : AchievementConfig
        {
            if (_config is T tConfig)
                return tConfig;
            else throw new InvalidCastException(nameof(T));
        }



        public void RestoreState(IAchievementSnapshot snapshot)
        {
            if (UnityCheck.IsDestroyed(snapshot))
                throw new ArgumentNullException(nameof(snapshot));

            if (snapshot.Id != _config.Id)
                throw new ArgumentException($"Snapshot ID {snapshot.Id} does not match achievement config ID {_config.Id}.");

            _progress = snapshot.Progress;
            _isRewardDispensed = snapshot.IsRewardDispensed;
        }

        public IAchievementSnapshot CaptureStateTo(IAchievementSnapshot snapshot)
        {
            if (UnityCheck.IsDestroyed(snapshot))
                throw new ArgumentNullException(nameof(snapshot));

            snapshot.Id = _config.Id;
            snapshot.Progress = _progress;
            snapshot.IsRewardDispensed = _isRewardDispensed;

            return snapshot;
        }



        public void Init()
        {
            _handler.AddAchievement(this);
        }



        public void AddProgress(int value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value));

            if (IsCompleted)
                throw new InvalidOperationException("Already completed");

            value = Math.Min(value, _config.TargetProgress - _progress);
            _progress += value;

            ProgressChanged?.Invoke();

            if (IsCompleted)
                Completed?.Invoke();
        }

        public void SetProgress(int value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value));

            value = Math.Min(value, _config.TargetProgress - _progress);
            _progress += value;

            ProgressChanged?.Invoke();

            if (IsCompleted)
                Completed?.Invoke();
        }

        public void ResetProgress()
        {
            _progress = 0;
            _isRewardDispensed = false;
            ProgressChanged?.Invoke();
        }



        public void DispenseReward()
        {
            if (!IsCompleted)
                throw new InvalidOperationException("Not completed");

            _rewardDispencer.DispenseReward(_config.Reward);
            _isRewardDispensed = true;
            RewardDispensed?.Invoke();
        }



        public void Dispose()
        {
            _handler.RemoveAchievement(this);
        }
    }
}