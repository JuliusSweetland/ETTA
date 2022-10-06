// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using JuliusSweetland.OptiKey.Enums;
using JuliusSweetland.OptiKey.Extensions;
using JuliusSweetland.OptiKey.Models;
using JuliusSweetland.OptiKey.Observables.PointSources;

namespace JuliusSweetland.OptiKey.Observables.TriggerSources
{
    /// <summary>
    /// Aggregates fixations on keys, allowing you to move away and return to complete a key fixation.
    /// Point fixations have to be completed in one go, i.e. if you move away from a point fixation it is lost.
    /// </summary>
    public class KeyFixationSource : ITriggerSource, IFixationTriggerSource
    {
        #region Fields

        // KeyValues with these FunctionKeys will have their String values ignored when looking them up in
        // timeToCompleteTriggerByKey. This allows functions like "Select Voice" whose exact keys aren't known
        // at design-time to share a single time-to-complete setting. 
        private static readonly ISet<FunctionKeys> FunctionKeysWithIgnoredStringValue = new HashSet<FunctionKeys>
        {
            FunctionKeys.SelectVoice
        };

        private readonly TimeSpan defaultLockOnTime;
        private readonly bool resumeRequiresLockOn;
        private readonly string defaultTimeToCompleteTrigger;
        private readonly IDictionary<KeyValue, string> timeToCompleteTriggerByKey;
        private readonly ConcurrentDictionary<KeyValue, TimeSpanOverrides> overrideTimesByKey;
        private readonly TimeSpan incompleteFixationTtl;
        private readonly ConcurrentDictionary<KeyValue, long> incompleteFixationProgress;
        private readonly ConcurrentDictionary<KeyValue, IDisposable> incompleteFixationTimeouts;
        
        private IPointSource pointSource;
        private IObservable<TriggerSignal> sequence;

        #endregion

        #region Ctor

        public KeyFixationSource(
            TimeSpan defaultLockOnTime,
            bool resumeRequiresLockOn,
            string defaultTimeToCompleteTrigger,
            IDictionary<KeyValue, string> timeToCompleteTriggerByKey,
            TimeSpan incompleteFixationTtl,
            IPointSource pointSource)
        {
            this.defaultLockOnTime = defaultLockOnTime;
            this.resumeRequiresLockOn = resumeRequiresLockOn;
            this.defaultTimeToCompleteTrigger = defaultTimeToCompleteTrigger;
            this.timeToCompleteTriggerByKey = timeToCompleteTriggerByKey ?? new Dictionary<KeyValue, string>();
            this.incompleteFixationTtl = incompleteFixationTtl;
            this.overrideTimesByKey = new ConcurrentDictionary<KeyValue, TimeSpanOverrides>();
            this.pointSource = pointSource;

            incompleteFixationProgress = new ConcurrentDictionary<KeyValue, long>();
            incompleteFixationTimeouts = new ConcurrentDictionary<KeyValue, IDisposable>();
        }

        #endregion

        #region Properties

        public RunningStates State { get; set; }

        public KeyEnabledStates KeyEnabledStates { get; set; }

        public IDictionary<KeyValue, TimeSpanOverrides> OverrideTimesByKey { get { return overrideTimesByKey; } }

        /// <summary>
        /// Change the point and key value source. N.B. After setting this any existing subscription 
        /// to the sequence must be disposed and the getter called again to recreate the sequence again.
        /// </summary>
        public IPointSource PointSource
        {
            get { return pointSource; }
            set { pointSource = value; }
        }

        public IObservable<TriggerSignal> Sequence
        {
            get
            {
                if (sequence == null)
                { 
                    sequence = Observable.Create<TriggerSignal>(observer =>
                    {
                        bool disposed = false;

                        Timestamped<PointAndKeyValue>? lockOnStart = null; 
                        DateTimeOffset fixationStart = DateTimeOffset.MinValue;
                        PointAndKeyValue fixationCentrePointAndKeyValue = null;
                        KeyValue lastKeyValue = null;
                        int keystroke = 1;
                        
                        Action disposeAllSubscriptions = null;

                        var pointAndKeyValueSubscription = pointSource.Sequence
                            .Where(_ => disposed == false)
                            .Where(_ => State == RunningStates.Running)
                            .Where(tp => tp.Value != null) //Filter out stale indicators - the fixation progress is not reset by the points sequence being stale
                            .Select(tp => new Timestamped<PointAndKeyValue>(tp.Value, tp.Timestamp))
                            .Buffer(2, 1) //Sliding buffer of 2 (last & current) that moves by 1 value at a time
                            .Subscribe(tps =>
                            {
                                Timestamped<PointAndKeyValue> latestPointAndKeyValue = tps.Last(); //Store latest timeStampedPointAndKeyValue

                                //if the latest key is not the same as the last key press then reset values
                                if (lastKeyValue != null && !lastKeyValue.Equals(latestPointAndKeyValue.Value.KeyValue))
                                {
                                    keystroke = 1;
                                    lastKeyValue = null;
                                }

                                if (fixationCentrePointAndKeyValue == null
                                    && !resumeRequiresLockOn)
                                {
                                    //Does the latest point continue an incomplete fixation? Continue it immediately, i.e. don't require a lock on again
                                    if (latestPointAndKeyValue.Value.KeyValue != null
                                        && incompleteFixationProgress.TryGetValue(latestPointAndKeyValue.Value.KeyValue, out _))
                                    {
                                        fixationStart = latestPointAndKeyValue.Timestamp;
                                        fixationCentrePointAndKeyValue = new PointAndKeyValue(latestPointAndKeyValue.Value.Point, latestPointAndKeyValue.Value.KeyValue);
                                        lockOnStart = null;
                                    }
                                }

                                if (fixationCentrePointAndKeyValue == null)
                                {
                                    //No fixation in progress - we are in the "lock on" phase
                                    if (lockOnStart == null)
                                    {
                                        //We have no current lock-on - start a new one
                                        if (latestPointAndKeyValue.Value.KeyValue != null
                                            && (KeyEnabledStates == null || KeyEnabledStates[latestPointAndKeyValue.Value.KeyValue]))
                                        {
                                            lockOnStart = latestPointAndKeyValue;
                                        }
                                    }
                                    else
                                    {
                                        //Lock-on in progress, but latest point breaks the lock-on
                                        if (latestPointAndKeyValue.Value.KeyValue == null
                                            || !lockOnStart.Value.Value.KeyValue.Equals(latestPointAndKeyValue.Value.KeyValue)
                                            || (KeyEnabledStates != null && !KeyEnabledStates[latestPointAndKeyValue.Value.KeyValue]))
                                        {
                                            lockOnStart = null;
                                        }

                                        //Check if the current lock-on is complete - if so start a new fixation
                                        if (lockOnStart != null
                                            && latestPointAndKeyValue.Value.KeyValue != null
                                            && latestPointAndKeyValue.Timestamp.Subtract(lockOnStart.Value.Timestamp) >= GetTimeToLockOn(latestPointAndKeyValue.Value.KeyValue))
                                        {
                                            fixationStart = latestPointAndKeyValue.Timestamp;
                                            fixationCentrePointAndKeyValue = new PointAndKeyValue(latestPointAndKeyValue.Value.Point, latestPointAndKeyValue.Value.KeyValue);
                                            lockOnStart = null;
                                        }
                                    }
                                }
                                else
                                {
                                    //We have a current fixation
                                    if (latestPointAndKeyValue.Value.KeyValue == null
                                        || !fixationCentrePointAndKeyValue.KeyValue.Equals(latestPointAndKeyValue.Value.KeyValue)
                                        || (KeyEnabledStates != null && !KeyEnabledStates[latestPointAndKeyValue.Value.KeyValue]))
                                    {
                                        //Latest pointAndKeyValue is not over the fixation key, or the key is now disabled
                                        if (tps.Count > 1)
                                        {
                                            //We have a buffer which contains the previous (over fixation key) point
                                            Timestamped<PointAndKeyValue>? previousPointAndKeyValue = tps[tps.Count - 2];

                                            //Calculate the span of the fixation up to this point and store the aggregate progress (so that we can resume progress later)
                                            var fixationSpan = previousPointAndKeyValue.Value.Timestamp.Subtract(fixationStart);
                                            incompleteFixationProgress.AddOrUpdate(fixationCentrePointAndKeyValue.KeyValue,
                                                    _ => fixationSpan.Ticks,
                                                    (_, existingProgress) => existingProgress + fixationSpan.Ticks);

                                            //Dispose of existing incomplete fixation timeout for this key
                                            if (incompleteFixationTimeouts.TryGetValue(fixationCentrePointAndKeyValue.KeyValue, out var timeout))
                                            {
                                                timeout?.Dispose();
                                            }

                                            //Setup new incomplete fixation timeout
                                            PointAndKeyValue fixationCentrePointAndKeyValueCopy = fixationCentrePointAndKeyValue; //Access to modified closure

                                            //set the timeout to either the override or default value
                                            var fixationTimeout = (overrideTimesByKey != null
                                                && overrideTimesByKey.TryGetValue(fixationCentrePointAndKeyValueCopy.KeyValue, out TimeSpanOverrides timeSpanOverrides) && timeSpanOverrides.LockDownAttemptTimeout.HasValue)
                                                ? timeSpanOverrides.LockDownAttemptTimeout.Value : incompleteFixationTtl;

                                            incompleteFixationTimeouts[fixationCentrePointAndKeyValue.KeyValue] =
                                                Observable.Timer(fixationTimeout).Subscribe(_ =>
                                                {
                                                    if (overrideTimesByKey != null 
                                                        && overrideTimesByKey.TryGetValue(fixationCentrePointAndKeyValueCopy.KeyValue, out TimeSpanOverrides tmpOverride))
                                                    {
                                                        tmpOverride.LockDownCancelTime = DateTimeOffset.MinValue;
                                                    }
                                                    
                                                    incompleteFixationProgress.TryRemove(fixationCentrePointAndKeyValueCopy.KeyValue, out var _);
                                                    incompleteFixationTimeouts.TryRemove(fixationCentrePointAndKeyValueCopy.KeyValue, out var _);
                                                    observer.OnNext(new TriggerSignal(null, 0, fixationCentrePointAndKeyValueCopy));
                                                });
                                        }

                                        //Clear the current fixation
                                        fixationCentrePointAndKeyValue = null;
                                    }
                                    else
                                    {
                                        //We have created or added to a fixation - update state vars, publish our progress and reset if necessary
                                        var fixationSpan = latestPointAndKeyValue.Timestamp.Subtract(fixationStart);

                                        incompleteFixationProgress.TryGetValue(fixationCentrePointAndKeyValue.KeyValue, out var storedProgress);

                                        //Dispose of the timeout for the current fixation as it is in progress again
                                        if (incompleteFixationTimeouts.TryGetValue(fixationCentrePointAndKeyValue.KeyValue, out var timeout))
                                        {
                                            timeout?.Dispose();
                                        }

                                        var timeToCompleteTrigger = GetTimeToCompleteTrigger(fixationCentrePointAndKeyValue.KeyValue, lastKeyValue, keystroke);
                                        var progress = (((double)(storedProgress + fixationSpan.Ticks)) / (double)timeToCompleteTrigger.Ticks);

                                        //Publish a high signal if progress is 1 (100%), otherwise just publish progress (filter out 0 as this is a progress reset signal)
                                        if (progress > 0)
                                        {
                                            observer.OnNext(new TriggerSignal(
                                                progress >= 1 ? 1 : (double?)null, progress >= 1 ? 1 : progress, fixationCentrePointAndKeyValue));
                                        }

                                        //Reset if we've just published a high signal
                                        if (progress >= 1)
                                        {
                                            foreach (var t in incompleteFixationTimeouts.Values)
                                            {
                                                t.Dispose();
                                            }

                                            keystroke = (fixationCentrePointAndKeyValue.KeyValue.Equals(lastKeyValue)) ? keystroke + 1 : 1;
                                            lastKeyValue = fixationCentrePointAndKeyValue.KeyValue;
                                            fixationStart = latestPointAndKeyValue.Timestamp;

                                            //Temporary logic: if override times exist then we can presume that a dynamic keyboard with overrides is being user
                                            //The logic here would be to NOT require lock on again, hence why fixationCentrePointAndKeyValue is not cleared out.
                                            if (overrideTimesByKey != null &&
                                                overrideTimesByKey.TryGetValue(fixationCentrePointAndKeyValue.KeyValue, out TimeSpanOverrides tmpOverride) && tmpOverride.TimeRequiredToLockDown.HasValue)
                                            {
                                                tmpOverride.LockDownCancelTime = DateTimeOffset.Now + tmpOverride.TimeRequiredToLockDown.Value; 
                                            }
                                            incompleteFixationProgress.Clear();
                                            incompleteFixationTimeouts.Clear();
                                            lockOnStart = null;

                                            observer.OnNext(new TriggerSignal(null, 0, null));
                                            return;
                                        }
                                    }
                                }
                            },
                            ex =>
                            {
                                observer.OnError(ex);
                                disposeAllSubscriptions();
                            });

                        disposeAllSubscriptions = () =>
                        {
                            disposed = true;

                            if (pointAndKeyValueSubscription != null)
                            {
                                pointAndKeyValueSubscription.Dispose();
                                pointAndKeyValueSubscription = null;
                            }

                            sequence = null;
                        };

                        return disposeAllSubscriptions;
                    })
                    .Publish()
                    .RefCount();
                }

                return sequence;
            }
        }

        #endregion
        
        #region Methods

        TimeSpan GetTimeToCompleteTrigger(KeyValue keyValue, KeyValue lastKeyValue, int keystroke)
        { 
            if (keyValue.FunctionKey.HasValue)
            {
                FunctionKeys functionKey = keyValue.FunctionKey.Value;

                if (FunctionKeysWithIgnoredStringValue.Contains(functionKey))
                {
                    keyValue = new KeyValue(functionKey);
                }
            }

            //check if this key has any override times
            var hasOverride = overrideTimesByKey.TryGetValue(keyValue, out TimeSpanOverrides timeSpanOverrides);
            if (hasOverride && timeSpanOverrides.TimeRequiredToLockDown.HasValue && keystroke == 2)
            {
                return timeSpanOverrides.TimeRequiredToLockDown.Value;
            }

            var completionTimes = hasOverride && timeSpanOverrides.CompletionTimes != null && timeSpanOverrides.CompletionTimes.Any()
                ? timeSpanOverrides.CompletionTimes
                : timeToCompleteTriggerByKey.GetValueOrDefault(keyValue, defaultTimeToCompleteTrigger).Split(',').ToList();

            if (lastKeyValue == null || keyValue != lastKeyValue)
            {
                return TimeSpan.FromMilliseconds(double.Parse(completionTimes.First()));
            }

            if (completionTimes.Count > keystroke)
            {
                return TimeSpan.FromMilliseconds(double.Parse(completionTimes[keystroke]));
            }

            return TimeSpan.FromMilliseconds(double.Parse(completionTimes.Last()));
        }

        TimeSpan GetTimeToLockOn(KeyValue keyValue)
        {
            if (keyValue.FunctionKey.HasValue)
            {
                FunctionKeys functionKey = keyValue.FunctionKey.Value;

                if (FunctionKeysWithIgnoredStringValue.Contains(functionKey))
                {
                    keyValue = new KeyValue(functionKey);
                }
            }

            if (overrideTimesByKey.TryGetValue(keyValue, out var timeSpanOverrides))
            {
                TimeSpan? lockonTime = timeSpanOverrides.LockOnTime;
                if (lockonTime.HasValue)
                {
                    return lockonTime.Value;
                }
            }
            
            return defaultLockOnTime;
        }

        #endregion
    }
}
