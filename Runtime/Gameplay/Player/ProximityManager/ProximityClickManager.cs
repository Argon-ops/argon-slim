using UnityEngine;
using System.Collections.Generic;
using DuksGames.Argon.Adapters;
using System.Collections;
// using DuksGames.Argon.Shared;
// using DuksGames.Argon.Utils;


namespace DuksGames.Argon.Gameplay
{
    /// <summary>
    ///  Manage a collection of beacons and continually update their states
    /// </summary>
    public class ProximityClickManager : MonoBehaviour, 
        ISuspendable, 
        IProximityClickManager,
        ICurrentBeaconProvider 
    {
        HashSet<IChoosableClickBeacon> _candidates = new HashSet<IChoosableClickBeacon>();

        [SerializeField] Camera _camera;

        [SerializeField, Tooltip("Defines how frequently to update the state of nearby beacons")]
        float _pollIntervalSeconds = .25f;

        [SerializeField, Tooltip("Defines how directly the player needs to look at a beacon for it to be clickable")]
        float _dotTolerance = .05f;

        [SerializeField]
        float _dotDistanceNudgeThreshold = 1.2f;

        bool _shouldKeepPolling;
        IChoosableClickBeacon _currentClickable;

        public IChoosableClickBeacon GetCurrentBeacon() { return this._currentClickable; }

        void Start()
        {
            this.StartPolling();
        }

        void OnDestroy()
        {
            this.StopPolling();
        }

        public void Add(IChoosableClickBeacon clickBeacon)
        {
            if (this._isSuspended)
            {
                return;
            }
            if (this._candidates.Add(clickBeacon))
            {
                clickBeacon.GetClickBeacon().SetState(EClickBeaconState.Visible);
            }
        }

        public void Remove(IChoosableClickBeacon clickBeacon)
        {
            if (clickBeacon == this._currentClickable)
            {
                this._currentClickable.GetClickBeacon().SetState(EClickBeaconState.Off);
                this._currentClickable = null;
            }

            if (this._candidates.Remove(clickBeacon))
            {
                clickBeacon.GetClickBeacon().SetState(EClickBeaconState.Off);
            }
        }

        #region ISuspendable

        bool _isSuspended;

        public void Suspend()
        {
            this._isSuspended = true;
            this.StopPolling();
            this.EjectCandidates();
        }

        public void Restart()
        {
            this._isSuspended = false;
            this.StartPolling();
        }

        #endregion


        IEnumerator Poll()
        {
            this._shouldKeepPolling = true;
            while (this._shouldKeepPolling)
            {
                this.UpdateCurrent();
                yield return new WaitForSeconds(this._pollIntervalSeconds);
            }
        }

        void StartPolling()
        {
            if (!this._shouldKeepPolling)
            {
                StartCoroutine(this.Poll());
            }
        }

        void StopPolling() { this._shouldKeepPolling = false; }

        struct BeaconPickData
        {
            public IChoosableClickBeacon Candidate;
            public float CameraAlignmentRating;
            public float DistanceSquared;

            public static void Copy(BeaconPickData from, BeaconPickData to)
            {
                to.Candidate = from.Candidate;
                to.CameraAlignmentRating = from.CameraAlignmentRating;
            }

            public static BeaconPickData Empty()
            {
                return new BeaconPickData
                {
                    Candidate = null,
                    CameraAlignmentRating = Mathf.Infinity,
                    DistanceSquared = Mathf.Infinity
                };
            }
        }

        class TriMethodCandidate
        {
            public IChoosableClickBeacon Candidate;
            public Vector3 FromCamera;
            public float DistanceSquared;
            public float SignedDistanceToRefSquared;
            public float MedianDot;
            public float MedianDotRatio;

            public static TriMethodCandidate Empty()
            {
                return new TriMethodCandidate
                {
                    Candidate = null,
                    FromCamera = Vector3.positiveInfinity,
                    DistanceSquared = Mathf.Infinity,
                    SignedDistanceToRefSquared = 0f,
                };
            }
        }

        Vector3 _cachedForwardFaceVector;

        void UpdateCurrent()
        {
            // TODO: don't we want to check isSuspended here?
            //  OR: drop isSuspended
            BeaconPickData pick = BeaconPickData.Empty();
            BeaconPickData runnerUp = BeaconPickData.Empty();


            foreach (var candidate in this._candidates)
            {
                var deltaPos = candidate.GetLocatableBeacon().GetApparentPosition() - this._camera.transform.position;
                
                // NOT GOOD LOL => : find another way to prioritize close beacons // deltaPos.y *= 0f; // remove camera tilt. helps us pick beacons that are closer


                float camAlignmentRating = 1f - Vector3.Dot(deltaPos.normalized, this._camera.transform.forward);

                // forward face normal
                if (candidate.HasForwardFaceVector(ref this._cachedForwardFaceVector))
                {
                    // the forward normal should be opposed to deltaPos
                    if (Vector3.Dot(deltaPos, this._cachedForwardFaceVector) > 0f)
                    {
                        continue;
                    }
                }

                // TODO: dist factor can warp the cam alignmet metric in ways that we don't want
                //  we should keep track of raw cam alignment independen of dist factor
                //   User should never click/interact with something that isn't in the cameras field of view
                
                // distance
                var distanceSquared = deltaPos.sqrMagnitude;
                var sdistFactor = Mathf.Clamp01(distanceSquared / (this._dotDistanceNudgeThreshold * this._dotDistanceNudgeThreshold));
                camAlignmentRating *= sdistFactor;

                if (camAlignmentRating < this._dotTolerance && camAlignmentRating < pick.CameraAlignmentRating)
                {
                    BeaconPickData.Copy(pick, runnerUp); // Save the previous pick to runner up
                    pick.CameraAlignmentRating = camAlignmentRating;
                    pick.Candidate = candidate;
                    pick.DistanceSquared = distanceSquared;
                }

            }

            // half-hearted attempt to weed out cases where the pick doesn't make sense (runnerUp is a lot closer)
            if (pick.Candidate != null && runnerUp.Candidate != null)
            {
                if (pick.DistanceSquared * 4f < runnerUp.DistanceSquared)
                {
                    BeaconPickData.Copy(runnerUp, pick);
                }
            }

            this.UpdateWith(pick.CameraAlignmentRating < this._dotTolerance ? pick.Candidate : null);
        }

        void UpdateWith(IChoosableClickBeacon next)
        {

            if (this._currentClickable != null && !this._currentClickable.GetClickBeacon().Equals(null))
            {
                this._currentClickable.GetClickBeacon().SetState(EClickBeaconState.Visible);
            }

            if (next == null)
            {
                this._currentClickable = null;
                return;
            }

            this._currentClickable = next;

            this._currentClickable.GetClickBeacon().SetState(EClickBeaconState.IsNextClick);
        }


        void EjectCandidates()
        {
            foreach (var c in this._candidates)
            {
                c.GetClickBeacon().SetState(EClickBeaconState.Off);
            }
            this._candidates.Clear();
        }


    }

}