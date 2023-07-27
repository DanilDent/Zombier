using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Prototype.Timer
{
    public class TimerService : IInitializable, ITickable, IDisposable
    {
        public void Initialize()
        {
            _timers = new List<Timer>();
        }

        public void Tick()
        {
            foreach (var timer in _timers)
            {
                timer.Tick(Time.deltaTime);
            }
        }

        public void Dispose()
        {
            _timers.Clear();
        }

        public void AddTimer(TimerConfig config)
        {
            _timers.Add(new Timer(config.Duration, config.TickInterval, config.OnInit, config.OnTick, config.OnDispose));
        }

        private List<Timer> _timers;

        private void HandleTimerExpired(object sender, EventArgs e)
        {
            if (sender is Timer timer)
            {
                _timers.Remove(timer);
            }
        }

        private class Timer
        {
            public static event EventHandler TimerExpired;

            public float Duration { get; }
            public float TickInterval { get; }

            public Timer(
              float duration,
              float tickInterval,
              Action onInit,
              Action onTick,
              Action onDispose)
            {
                Duration = duration;
                TickInterval = tickInterval;
                _remainingTime = Duration;

                _timer = TickInterval;
                _onInit = onInit;
                _onTick = onTick;
                _onDispose = onDispose;

                _onInit?.Invoke();
            }

            private float _timer;
            private float _remainingTime;
            private Action _onInit;
            private Action _onTick;
            private Action _onDispose;

            public void Tick(float deltaTime)
            {
                _timer -= deltaTime;
                _remainingTime -= deltaTime;
                if (_timer < 0 || Mathf.Approximately(_timer, 0f))
                {
                    _timer = TickInterval;
                    _onTick?.Invoke();
                }

                if (_remainingTime < 0f || Mathf.Approximately(_remainingTime, 0f))
                {
                    _onDispose?.Invoke();
                    TimerExpired?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }

    public class TimerConfig
    {
        public TimerConfig(float duration, float tickInterval, Action onInit, Action onTick, Action onDispose)
        {
            Duration = duration;
            TickInterval = tickInterval;
            OnInit = onInit;
            OnTick = onTick;
            OnDispose = onDispose;
        }

        public float Duration { get; }
        public float TickInterval { get; }
        public Action OnInit { get; }
        public Action OnTick { get; }
        public Action OnDispose { get; }
    }


}
