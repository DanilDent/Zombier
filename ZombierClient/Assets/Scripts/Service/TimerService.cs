using Prototype.Data;
using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace Prototype.Service
{
    public class TimerService
    {
        private Dictionary<IdData, TaggedTimer> _activeTimers = new Dictionary<IdData, TaggedTimer>();

        public void SetTimer(TaggedTimer timer)
        {
            timer.Elapsed += DisposeTimerHandler;
            timer.AutoReset = false;
            timer.Enabled = true;
            _activeTimers.Add(timer.InternalId, timer);
        }

        private void DisposeTimerHandler(object sender, ElapsedEventArgs e)
        {
            if (sender is TaggedTimer cast)
            {
#if DEBUG
                Debug.Log($"Timer {cast} elapsed at {e.SignalTime}");
#endif
                var timer = _activeTimers[cast.InternalId];
                try
                {
                    _activeTimers.Remove(cast.InternalId);
                    timer.Dispose();
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
        }
    }

    public class TaggedTimer : Timer
    {
        public readonly IdData InternalId;
        public TaggedTimer(IdData id)
        {
            InternalId = id;
        }
    }

    public class Timer<T> : TaggedTimer
    {
        public readonly T Value;

        public Timer(T value)
            : base((IdData)Guid.NewGuid().ToString())
        {
            Value = value;
        }
    }

    public class TimerIdData : Timer<IdData>
    {
        public TimerIdData(IdData value)
            : base(value)
        { }
    }
}
