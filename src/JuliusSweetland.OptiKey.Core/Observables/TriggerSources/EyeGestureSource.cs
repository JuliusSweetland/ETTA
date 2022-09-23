// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using JuliusSweetland.OptiKey.Enums;
using JuliusSweetland.OptiKey.Models;
using JuliusSweetland.OptiKey.Observables.PointSources;
using JuliusSweetland.OptiKey.Properties;
using JuliusSweetland.OptiKey.Static;

namespace JuliusSweetland.OptiKey.Observables.TriggerSources
{
    /// <summary>
    /// Monitors point source for input patterns. If the pattern is completed an action is triggered
    /// </summary>
    public class EyeGestureSource : ITriggerSource
    {
        #region Fields

        private IPointSource pointSource;
        private IObservable<TriggerSignal> sequence;

        #endregion

        #region Ctor

        public EyeGestureSource(IPointSource pointSource)
        {
            this.pointSource = pointSource;
        }

        #endregion

        #region Properties

        public RunningStates State { get; set; }

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
                        Action disposeAllSubscriptions = null;
                        List<EyeGesture> gestureList = null;
                        double moveDelta = 0;

                        // Load gestures
                        try
                        {
                            gestureList = XmlEyeGestures.ReadFromString(Settings.Default.EyeGestureString).GestureList.ToList();
                        }
                        catch {
                            gestureList = null;
                        }

                        // Create subscription
                        var eyeGestureSubscription = pointSource.Sequence
                        .Where(_ => disposed == false)
                        .Where(_ => State == RunningStates.Running)
                        .Where(tp => tp.Value != null) //Filter out stale indicators - the fixation progress is not reset by the points sequence being stale
                        .Select(tp => new Timestamped<PointAndKeyValue>(tp.Value, tp.Timestamp))
                        .Buffer(2, 1) //Sliding buffer of 2 (last & current) that moves by 1 value at a time
                        .Subscribe(tps =>
                        {

                            // Exit early if nothing to do
                            // (note that the source is still up and running in this case as
                            //  it may be modified on the fly from the Management Console)
                            if (!Settings.Default.EyeGesturesEnabled ||
                                 String.IsNullOrEmpty(Settings.Default.EyeGestureString))
                            { 
                                return;
                            }
                            
                            var checkPoint = tps.Last().Value.Point;
                            moveDelta = (2 * moveDelta + (checkPoint - tps.First().Value.Point).Length) / 3;

                            // If gesture list was updated via Management Console, load the new gestures now
                            if (Settings.Default.EyeGestureUpdated)
                            {
                                Settings.Default.EyeGestureUpdated = false;
                                try
                                {
                                    gestureList = XmlEyeGestures.ReadFromString(Settings.Default.EyeGestureString).GestureList.ToList();
                                }
                                catch { gestureList = null; }
                            }

                            // Process gestures
                            if (gestureList != null && Settings.Default.EyeGesturesEnabled)
                            {
                                for(int i = 0; i < gestureList.Count; i++)
                                {
                                    var gesture = gestureList[i];
                                    if (gesture.Enabled)
                                    {
                                        if (gesture.StepIndex >= gesture.Steps.Count
                                            && DateTimeOffset.Now > gesture.TimeStamp + TimeSpan.FromMilliseconds(gesture.Cooldown))
                                        {
                                            gesture.StepIndex = 0;
                                            gesture.TimeStamp = DateTimeOffset.Now;
                                        }
                                        else if (gesture.StepIndex < gesture.Steps.Count)
                                        {
                                            var step = gesture.Steps[gesture.StepIndex];

                                            if (gesture.StepIndex > 0 && DateTimeOffset.Now > gesture.TimeStamp + TimeSpan.FromMilliseconds(step.TimeLimit))
                                            {
                                                gesture.StepIndex = 0;
                                                gesture.TimeStamp = DateTimeOffset.Now;
                                                step = gesture.Steps[0];
                                            }

                                            if (PointAdvancesGesture(checkPoint, moveDelta, ref gesture, ref step))
                                            {
                                                gesture.StepIndex++;
                                                if (step.Commands != null && step.Commands.Any())
                                                {
                                                    var keyCommands = new List<KeyCommand>();
                                                    foreach (var command in step.Commands)
                                                    {
                                                        switch (command.Name)
                                                        {
                                                            case KeyCommands.Action:
                                                                keyCommands.Add(new ActionCommand { Value = command.Value });
                                                                break;
                                                            case KeyCommands.ChangeKeyboard:
                                                                keyCommands.Add(new ChangeKeyboardCommand { Value = command.Value, BackAction = command.BackAction });
                                                                break;
                                                            case KeyCommands.KeyDown:
                                                                keyCommands.Add(new KeyDownCommand { Value = command.Value });
                                                                break;
                                                            case KeyCommands.KeyUp:
                                                                keyCommands.Add(new KeyUpCommand { Value = command.Value });
                                                                break;
                                                            case KeyCommands.KeyToggle:
                                                                keyCommands.Add(new KeyTogglCommand { Value = command.Value });
                                                                break;
                                                            case KeyCommands.MoveWindow:
                                                                keyCommands.Add(new MoveWindowCommand { Value = command.Value });
                                                                break;
                                                            case KeyCommands.Plugin:
                                                                keyCommands.Add(new PluginCommand { Method = command.Method, Arguments = command.Argument });
                                                                break;
                                                            case KeyCommands.Text:
                                                                keyCommands.Add(new TextCommand { Value = command.Value });
                                                                break;
                                                            case KeyCommands.Wait:
                                                                keyCommands.Add(new WaitCommand { Value = command.Value });
                                                                break;
                                                        }
                                                   }
                                                    var pakv = new PointAndKeyValue(checkPoint, new KeyValue(gesture.Name) { Commands = keyCommands });
                                                    observer.OnNext(new TriggerSignal(1, 1, pakv));
                                                    observer.OnNext(new TriggerSignal(null, 0, null));
                                                }
                                            }
                                            else if (gesture.StepIndex == 1 && gesture.Steps.Count > 1)
                                            {
                                                step = gesture.Steps[0];
                                                //advance the expiration if we still meet the parameters of the starting step
                                                _ = PointAdvancesGesture(checkPoint, moveDelta, ref gesture, ref step);
                                                gesture.Steps[0] = step;
                                            }
                                        }
                                    }
                                    gestureList[i] = gesture;
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

                            if (eyeGestureSubscription != null)
                            {
                                eyeGestureSubscription.Dispose();
                                eyeGestureSubscription = null;
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

        private bool PointAdvancesGesture(Point checkpoint, double moveDelta, ref EyeGesture gesture, ref EyeGestureStep step)
        {
            var result = false;

            if (step.type == EyeGestureStepTypes.Fixation)
            {
                if (moveDelta > step.Radius)
                    step.DwellStart = DateTimeOffset.Now;
                else if (DateTimeOffset.Now > step.DwellStart + TimeSpan.FromMilliseconds(step.DwellTime))
                {
                    result = true;
                    gesture.FixationPoint = checkpoint;
                }
            }

            else if (step.type == EyeGestureStepTypes.LookInDirection)
            {
                result = (step.X == 0 || (checkpoint.X - gesture.PointStamp.X) / (Graphics.PrimaryScreenWidthInPixels * step.X / 100d) > 1)
                      && (step.Y == 0 || (checkpoint.Y - gesture.PointStamp.Y) / (Graphics.PrimaryScreenHeightInPixels * step.Y /100d) > 1);
            }
            else if (step.type == EyeGestureStepTypes.LookAtArea)
            {
                var left = step.Left * Graphics.PrimaryScreenWidthInPixels / 100d;
                var top = step.Top * Graphics.PrimaryScreenHeightInPixels / 100d;

                if (step.Round)
                {
                    var xRadius = step.Width * Graphics.PrimaryScreenWidthInPixels / 50d;
                    var yRadius = step.Height * Graphics.PrimaryScreenHeightInPixels / 50d;
                    var xLength = checkpoint.X - (left + xRadius);
                    var yLength = checkpoint.Y - (top + yRadius);

                    if (Math.Pow(xLength, 2) / Math.Pow(xRadius, 2) + Math.Pow(yLength, 2) / Math.Pow(yRadius, 2) > 1)
                        step.DwellStart = DateTimeOffset.Now;
                    else if (DateTimeOffset.Now > step.DwellStart + TimeSpan.FromMilliseconds(step.DwellTime))
                        result = true;
                }
                else
                {
                    var width = step.Width * Graphics.PrimaryScreenWidthInPixels / 100d;
                    var height = step.Height * Graphics.PrimaryScreenHeightInPixels / 100d;

                    if (checkpoint.X < left || checkpoint.X > left + width || checkpoint.Y < top || checkpoint.Y > top + height)
                        step.DwellStart = DateTimeOffset.Now;
                    else
                    {
                        if (DateTimeOffset.Now > step.DwellStart + TimeSpan.FromMilliseconds(step.DwellTime))
                            result = true;
                    }
                }
            }
            else
            {
                if ((checkpoint - gesture.FixationPoint).Length > Graphics.PrimaryScreenHeightInPixels * step.Radius / 100d)
                    step.DwellStart = DateTimeOffset.Now;
                else if (DateTimeOffset.Now > step.DwellStart + TimeSpan.FromMilliseconds(step.DwellTime))
                    result = true;
            }

            if (result)
            {
                gesture.PointStamp = checkpoint;
                gesture.TimeStamp = DateTimeOffset.Now;
            }
            return result;
        }

        #endregion
    }
}
