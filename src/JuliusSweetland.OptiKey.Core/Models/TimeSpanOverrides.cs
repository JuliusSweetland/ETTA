// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using System;
using System.Collections.Generic;

namespace JuliusSweetland.OptiKey.Models
{
    public class TimeSpanOverrides
    {
        public TimeSpan? LockOnTime { get; set; }

        //This is a list of times required to trigger keystrokes
        //When used the time to trigger the first keystroke is overridden and the time to trigger repetitive keystrokes can be shortened
        public List<string> CompletionTimes { get; set; }

        //The amount of time a continuous gaze is required to lock down a key
        //When used the key will be pressed down when triggerred and remain down as long as gaze is fixed on key
        public TimeSpan? TimeRequiredToLockDown { get; set; }

        //The purpose of this is to allow the key to be quickly released if TimeRequiredToLockDown is used
        //It is the amount of time focus can be lost before resetting the key
        //When used this will override the keyFixationTimeout
        public TimeSpan? LockDownAttemptTimeout { get; set; }

        //The time the last lock down attempt will expire if gaze is not maintained
        public DateTimeOffset LockDownCancelTime { get; set; }
    }
}