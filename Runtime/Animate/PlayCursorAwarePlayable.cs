using UnityEngine;
using UnityEngine.Playables;
using System;
using DuksGames.Argon.Shared;
using System.Linq;

namespace DuksGames.Argon.Animate
{

    public class CursorDataListener : INotificationReceiver
    {
        public Action<LimitNotifyPlayable.PlayCursorData> callback;
        public PlayableOutput output;


        public void OnNotify(Playable origin, INotification notification, object context)
        {
            this.callback((LimitNotifyPlayable.PlayCursorData)context);
        }

        public void StopListening()
        {
            if (!this.output.IsOutputValid())
            {
                return;
            }
            this.output.RemoveNotificationReceiver(this);
        }

        public static CursorDataListener Create(
            PlayableOutput outputn,
            Action<LimitNotifyPlayable.PlayCursorData> callback
        )
        {
            var listener = new CursorDataListener
            {
                output = outputn,
                callback = callback
            };

            outputn.AddNotificationReceiver(listener);
            return listener;
        }

    }

    public class CursorDataOnceListener : INotificationReceiver
    {
        public Action<LimitNotifyPlayable.PlayCursorData> callback;
        public PlayableOutput output;
        public void OnNotify(Playable origin, INotification notification, object context)
        {
            this.output.RemoveNotificationReceiver(this);
            this.callback((LimitNotifyPlayable.PlayCursorData)context);
        }

        private CursorDataOnceListener() { }

        public static CursorDataOnceListener Create(
            PlayableOutput outputn,
            Action<LimitNotifyPlayable.PlayCursorData> callback
        )
        {
            var listener = new CursorDataOnceListener
            {
                output = outputn,
                callback = callback
            };
            outputn.AddNotificationReceiver(listener);
            return listener;
        }
    }

    public class LimitNotifyPlayable : PlayableBehaviour
    {
        private static readonly Notification CursorNotification;

        [Flags]
        public enum PlayCursorEventType
        {
            None = 0,
            LimitReached = 1,
            Seek = 2,
            LimitReachedAndSeek = 3,
        }

        public struct PlayCursorData
        {
            public PlayCursorEventType type;
            public double eventTime;
        }

        PlayCursorEventType FrameEventFlags = PlayCursorEventType.None;
        bool PlayBeganFlag;
        double LastGetTime;


        void Spam(string s)
        {
            // Debug.Log($"SNAPBACK {s}");
        }

        void CheckReachedLimit(Playable playable, FrameData info)
        {
            double targetTime = playable.GetSpeed() < 0d ? 0d : playable.GetDuration() - .04d;

            if ((targetTime < this.LastGetTime) != (targetTime < playable.GetTime()))
            {
                this.Spam($" LIM RCHD {playable.GetInput(0).GetPlayableType()} ({playable.GetSpeed()}) LastTime: {this.LastGetTime} | targTime: {targetTime} | thisTime {playable.GetTime()}  ");
                this.FrameEventFlags |= PlayCursorEventType.LimitReached;
            }
            this.LastGetTime = playable.GetTime();
        }

        // Raise limit-reached flag in the case where the playable started
        //   out of bounds and was headed even further out of bounds 
        void CheckOverplay(Playable playable)
        {
            if (playable.GetSpeed() > 0d)
            {
                if (playable.GetTime() > playable.GetDuration() - .04d)
                {
                    this.Spam($"forwards overplay LIM RCHD ");
                    this.FrameEventFlags |= PlayCursorEventType.LimitReached;
                }
            }
            if (playable.GetSpeed() < 0d)
            {
                if (playable.GetTime() < .0001d)
                {
                    this.FrameEventFlags |= PlayCursorEventType.LimitReached;
                }
            }
        }

        void OnSeek(Playable playable, FrameData info)
        {
            if (!info.seekOccurred)
            {
                return;
            }
            this.Spam("Seek Occurred");
            this.CheckOverplay(playable);
            this.LastGetTime = playable.GetTime();
        }

        void Notify(Playable playable, FrameData info)
        {
            if (this.FrameEventFlags == PlayCursorEventType.None)
            {
                return;
            }

            info.output.PushNotification(playable, CursorNotification, new PlayCursorData
            {
                type = this.FrameEventFlags,
                eventTime = playable.GetTime()
            });
        }

        void ResetFlags()
        {
            this.FrameEventFlags = PlayCursorEventType.None;
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            this.ResetFlags();

            if (info.timeHeld) { }
            if (info.timeLooped) { }

            if (this.PlayBeganFlag)
            {
                this.PlayBeganFlag = false;
                this.CheckOverplay(playable);
            }

            this.OnSeek(playable, info);
            this.CheckReachedLimit(playable, info);
            this.Notify(playable, info);
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            this.PlayBeganFlag = true;
            base.OnBehaviourPlay(playable, info);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            base.OnBehaviourPause(playable, info);
        }

        public override void OnGraphStop(Playable playable)
        {
            base.OnGraphStop(playable);
        }

        public override void PrepareData(Playable playable, FrameData info)
        {
            base.PrepareData(playable, info);
        }
    }

}

