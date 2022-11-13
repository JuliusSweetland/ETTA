// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved

using JuliusSweetland.OptiKey.Enums;
using log4net;
using SharpDX.XInput;
using System;

namespace JuliusSweetland.OptiKey.Properties {

    public abstract class Settings : global::System.Configuration.ApplicationSettingsBase {

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Derived classes must specify what app they are
        public abstract AppType GetApp();

        private static Settings defaultInstance;

        // Settings singleton used by core library is configured by each executable with their own derived settings.       
        public static Settings Default {
            get {
                if (null == defaultInstance)
                {
                    throw new System.ArgumentNullException("Base settings class is null - Settings need initialising from a derived class");
                }
                return defaultInstance;
            }
        }

        public static void InitialiseWithDerivedSettings(Settings instance)
        {
            defaultInstance = instance;
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Insert")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Enums.Keys KeySelectionTriggerKeyboardKeyDownUpKey {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.Keys)(this["KeySelectionTriggerKeyboardKeyDownUpKey"]));
            }
            set {
                this["KeySelectionTriggerKeyboardKeyDownUpKey"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("A")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public GamepadButtonFlags KeySelectionTriggerGamepadXInputButtonDownUpButton
        {
            get
            {
                return ((GamepadButtonFlags)(this["KeySelectionTriggerGamepadXInputButtonDownUpButton"]));
            }
            set
            {
                this["KeySelectionTriggerGamepadXInputButtonDownUpButton"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Any")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public UserIndex KeySelectionTriggerGamepadXInputController
        {
            get
            {
                return ((UserIndex)(this["KeySelectionTriggerGamepadXInputController"]));
            }
            set
            {
                this["KeySelectionTriggerGamepadXInputController"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Any")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public UserIndex PointSelectionTriggerGamepadXInputController
        {
            get
            {
                return ((UserIndex)(this["PointSelectionTriggerGamepadXInputController"]));
            }
            set
            {
                this["PointSelectionTriggerGamepadXInputController"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("00000000-0000-0000-0000-000000000000")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public System.Guid KeySelectionTriggerGamepadDirectInputController
        {
            get
            {
                return ((System.Guid)(this["KeySelectionTriggerGamepadDirectInputController"]));
            }
            set
            {
                this["KeySelectionTriggerGamepadDirectInputController"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("00000000-0000-0000-0000-000000000000")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public System.Guid PointSelectionTriggerGamepadDirectInputController
        {
            get
            {
                return ((System.Guid)(this["PointSelectionTriggerGamepadDirectInputController"]));
            }
            set
            {
                this["PointSelectionTriggerGamepadDirectInputController"] = value;
            }
        }


        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int KeySelectionTriggerGamepadDirectInputButtonDownUpButton
        {
            get
            {
                return ((int)(this["KeySelectionTriggerGamepadDirectInputButtonDownUpButton"]));
            }
            set
            {
                this["KeySelectionTriggerGamepadDirectInputButtonDownUpButton"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("NextHigh")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Enums.TriggerStopSignals MultiKeySelectionTriggerStopSignal {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.TriggerStopSignals)(this["MultiKeySelectionTriggerStopSignal"]));
            }
            set {
                this["MultiKeySelectionTriggerStopSignal"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Left")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Enums.MouseButtons KeySelectionTriggerMouseDownUpButton {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.MouseButtons)(this["KeySelectionTriggerMouseDownUpButton"]));
            }
            set {
                this["KeySelectionTriggerMouseDownUpButton"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Fixations")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Enums.TriggerSources KeySelectionTriggerSource {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.TriggerSources)(this["KeySelectionTriggerSource"]));
            }
            set {
                this["KeySelectionTriggerSource"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool SettingsUpgradeRequired {
            get {
                return ((bool)(this["SettingsUpgradeRequired"]));
            }
            set {
                this["SettingsUpgradeRequired"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool LimitBackOne
        {
            get
            {
                return ((bool)(this["LimitBackOne"]));
            }
            set
            {
                this["LimitBackOne"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("00:00:00.0130000")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::System.TimeSpan PointsMousePositionSampleInterval {
            get {
                return ((global::System.TimeSpan)(this["PointsMousePositionSampleInterval"]));
            }
            set {
                this["PointsMousePositionSampleInterval"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool PointsMousePositionHideCursor {
            get {
                return ((bool)(this["PointsMousePositionHideCursor"]));
            }
            set {
                this["PointsMousePositionHideCursor"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MousePosition")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Enums.PointsSources PointsSource {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.PointsSources)(this["PointsSource"]));
            }
            set {
                this["PointsSource"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("00:00:00.1500000")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::System.TimeSpan PointTtl {
            get {
                return ((global::System.TimeSpan)(this["PointTtl"]));
            }
            set {
                this["PointTtl"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("40")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public double PointSelectionTriggerLockOnRadiusInPixels {
            get {
                return ((double)(this["PointSelectionTriggerLockOnRadiusInPixels"]));
            }
            set {
                this["PointSelectionTriggerLockOnRadiusInPixels"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("60")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public double PointSelectionTriggerFixationRadiusInPixels {
            get {
                return ((double)(this["PointSelectionTriggerFixationRadiusInPixels"]));
            }
            set {
                this["PointSelectionTriggerFixationRadiusInPixels"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("00:00:01.2500000")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::System.TimeSpan PointSelectionTriggerFixationCompleteTime {
            get {
                return ((global::System.TimeSpan)(this["PointSelectionTriggerFixationCompleteTime"]));
            }
            set {
                this["PointSelectionTriggerFixationCompleteTime"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("6666")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int GazeTrackerUdpPort {
            get {
                return ((int)(this["GazeTrackerUdpPort"]));
            }
            set {
                this["GazeTrackerUdpPort"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool MultiKeySelectionLockedDownWhenSimulatingKeyStrokes {
            get {
                return ((bool)(this["MultiKeySelectionLockedDownWhenSimulatingKeyStrokes"]));
            }
            set {
                this["MultiKeySelectionLockedDownWhenSimulatingKeyStrokes"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("00:00:00.2500000")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::System.TimeSpan MultiKeySelectionFixationMinDwellTime {
            get {
                return ((global::System.TimeSpan)(this["MultiKeySelectionFixationMinDwellTime"]));
            }
            set {
                this["MultiKeySelectionFixationMinDwellTime"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("20")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int MaxDictionaryMatchesOrSuggestions {
            get {
                return ((int)(this["MaxDictionaryMatchesOrSuggestions"]));
            }
            set {
                this["MaxDictionaryMatchesOrSuggestions"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("00:01:00")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::System.TimeSpan MultiKeySelectionMaxDuration {
            get {
                return ((global::System.TimeSpan)(this["MultiKeySelectionMaxDuration"]));
            }
            set {
                this["MultiKeySelectionMaxDuration"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("EnglishUK")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Enums.Languages UiLanguage {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.Languages)(this["UiLanguage"]));
            }
            set {
                this["UiLanguage"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("00:00:01.2500000")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::System.TimeSpan KeySelectionTriggerFixationDefaultCompleteTime
        {
            get
            {
                return ((global::System.TimeSpan)(this["KeySelectionTriggerFixationDefaultCompleteTime"]));
            }
            set
            {
                this["KeySelectionTriggerFixationDefaultCompleteTime"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(null)]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string KeySelectionTriggerFixationDefaultCompleteTimes {
            get {
                return ((string)(this["KeySelectionTriggerFixationDefaultCompleteTimes"]));
            }
            set {
                this["KeySelectionTriggerFixationDefaultCompleteTimes"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0,0,0,0")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::System.Windows.Rect MainWindowFloatingSizeAndPosition {
            get {
                return ((global::System.Windows.Rect)(this["MainWindowFloatingSizeAndPosition"]));
            }
            set {
                this["MainWindowFloatingSizeAndPosition"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("75")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int ToastNotificationVerticalFillPercentage {
            get {
                return ((int)(this["ToastNotificationVerticalFillPercentage"]));
            }
            set {
                this["ToastNotificationVerticalFillPercentage"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.02")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public decimal ToastNotificationSecondsPerCharacter {
            get {
                return ((decimal)(this["ToastNotificationSecondsPerCharacter"]));
            }
            set {
                this["ToastNotificationSecondsPerCharacter"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/Resources/Themes/Android_Dark.xaml")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string Theme {
            get {
                return ((string)(this["Theme"]));
            }
            set {
                this["Theme"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool Debug {
            get {
                return ((bool)(this["Debug"]));
            }
            set {
                this["Debug"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/Resources/Fonts/#Roboto")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string FontFamily {
            get {
                return ((string)(this["FontFamily"]));
            }
            set {
                this["FontFamily"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Condensed")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string FontStretch {
            get {
                return ((string)(this["FontStretch"]));
            }
            set {
                this["FontStretch"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Light")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string FontWeight {
            get {
                return ((string)(this["FontWeight"]));
            }
            set {
                this["FontWeight"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int ScratchpadNumberOfLines {
            get {
                return ((int)(this["ScratchpadNumberOfLines"]));
            }
            set {
                this["ScratchpadNumberOfLines"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool AutoAddSpace {
            get {
                return ((bool)(this["AutoAddSpace"]));
            }
            set {
                this["AutoAddSpace"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool AutoCapitalise {
            get {
                return ((bool)(this["AutoCapitalise"]));
            }
            set {
                this["AutoCapitalise"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int SpeechRate {
            get {
                return ((int)(this["SpeechRate"]));
            }
            set {
                this["SpeechRate"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string SpeechVoice {
            get {
                return ((string)(this["SpeechVoice"]));
            }
            set {
                this["SpeechVoice"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int SpeechVolume {
            get {
                return ((int)(this["SpeechVolume"]));
            }
            set {
                this["SpeechVolume"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Resources\\Sounds\\Tone2.wav")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string ErrorSoundFile {
            get {
                return ((string)(this["ErrorSoundFile"]));
            }
            set {
                this["ErrorSoundFile"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Resources\\Sounds\\Tone1.wav")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string InfoSoundFile {
            get {
                return ((string)(this["InfoSoundFile"]));
            }
            set {
                this["InfoSoundFile"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Resources\\Sounds\\Click1.wav")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string KeySelectionSoundFile {
            get {
                return ((string)(this["KeySelectionSoundFile"]));
            }
            set {
                this["KeySelectionSoundFile"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Resources\\Sounds\\Falling1.wav")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string MultiKeySelectionCaptureEndSoundFile {
            get {
                return ((string)(this["MultiKeySelectionCaptureEndSoundFile"]));
            }
            set {
                this["MultiKeySelectionCaptureEndSoundFile"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Resources\\Sounds\\Rising1.wav")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string MultiKeySelectionCaptureStartSoundFile {
            get {
                return ((string)(this["MultiKeySelectionCaptureStartSoundFile"]));
            }
            set {
                this["MultiKeySelectionCaptureStartSoundFile"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Fixations")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Enums.TriggerSources PointSelectionTriggerSource {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.TriggerSources)(this["PointSelectionTriggerSource"]));
            }
            set {
                this["PointSelectionTriggerSource"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("00:00:00.7500000")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::System.TimeSpan KeySelectionTriggerIncompleteFixationTtl {
            get {
                return ((global::System.TimeSpan)(this["KeySelectionTriggerIncompleteFixationTtl"]));
            }
            set {
                this["KeySelectionTriggerIncompleteFixationTtl"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("00:00:00.2500000")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::System.TimeSpan KeySelectionTriggerFixationLockOnTime {
            get {
                return ((global::System.TimeSpan)(this["KeySelectionTriggerFixationLockOnTime"]));
            }
            set {
                this["KeySelectionTriggerFixationLockOnTime"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("00:00:00.2500000")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::System.TimeSpan PointSelectionTriggerFixationLockOnTime {
            get {
                return ((global::System.TimeSpan)(this["PointSelectionTriggerFixationLockOnTime"]));
            }
            set {
                this["PointSelectionTriggerFixationLockOnTime"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual bool ConversationOnlyMode {
            get {
                return ((bool)(this["ConversationOnlyMode"]));
            }
            set {
                this["ConversationOnlyMode"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual bool MouseOnlyMode
        {
            get
            {
                return ((bool)(this["MouseOnlyMode"]));
            }
            set
            {
                this["MouseOnlyMode"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("25")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int MoveAndResizeAdjustmentAmountInPixels {
            get {
                return ((int)(this["MoveAndResizeAdjustmentAmountInPixels"]));
            }
            set {
                this["MoveAndResizeAdjustmentAmountInPixels"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("NextHigh")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string SelectionTriggerStopSignal {
            get {
                return ((string)(this["SelectionTriggerStopSignal"]));
            }
            set {
                this["SelectionTriggerStopSignal"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool CheckForUpdates {
            get {
                return ((bool)(this["CheckForUpdates"]));
            }
            set {
                this["CheckForUpdates"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool ShowSplashScreen {
            get {
                return ((bool)(this["ShowSplashScreen"]));
            }
            set {
                this["ShowSplashScreen"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("00:00:03")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::System.TimeSpan TETCalibrationCheckTimeSpan {
            get {
                return ((global::System.TimeSpan)(this["TETCalibrationCheckTimeSpan"]));
            }
            set {
                this["TETCalibrationCheckTimeSpan"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int MouseScrollAmountInClicks {
            get {
                return ((int)(this["MouseScrollAmountInClicks"]));
            }
            set {
                this["MouseScrollAmountInClicks"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Resources\\Sounds\\MouseClick1.wav")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string MouseClickSoundFile {
            get {
                return ((string)(this["MouseClickSoundFile"]));
            }
            set {
                this["MouseClickSoundFile"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Resources\\Sounds\\MouseDoubleClick1.wav")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string MouseDoubleClickSoundFile {
            get {
                return ((string)(this["MouseDoubleClickSoundFile"]));
            }
            set {
                this["MouseDoubleClickSoundFile"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Resources\\Sounds\\MouseScroll1.wav")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string MouseScrollSoundFile {
            get {
                return ((string)(this["MouseScrollSoundFile"]));
            }
            set {
                this["MouseScrollSoundFile"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("60")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int CursorWidthInPixels {
            get {
                return ((int)(this["CursorWidthInPixels"]));
            }
            set {
                this["CursorWidthInPixels"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("90")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int CursorHeightInPixels {
            get {
                return ((int)(this["CursorHeightInPixels"]));
            }
            set {
                this["CursorHeightInPixels"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool MouseMagnifierLockedDown {
            get {
                return ((bool)(this["MouseMagnifierLockedDown"]));
            }
            set {
                this["MouseMagnifierLockedDown"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool MouseMagneticCursorLockedDown
        {
            get {
                return ((bool)(this["MouseMagneticCursorLockedDown"]));
            }
            set {
                this["MouseMagneticCursorLockedDown"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("90")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int GazeIndicatorSize {
            get {
                return ((int)(this["GazeIndicatorSize"]));
            }
            set {
                this["GazeIndicatorSize"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("None")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public GazeIndicatorStyles GazeIndicatorStyle {
            get {
                return ((GazeIndicatorStyles)(this["GazeIndicatorStyle"]));
            }
            set {
                this["GazeIndicatorStyle"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public double MagnifySourcePercentageOfScreen {
            get {
                return ((double)(this["MagnifySourcePercentageOfScreen"]));
            }
            set {
                this["MagnifySourcePercentageOfScreen"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("60")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public double MagnifyDestinationPercentageOfScreen {
            get {
                return ((double)(this["MagnifyDestinationPercentageOfScreen"]));
            }
            set {
                this["MagnifyDestinationPercentageOfScreen"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("75")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int ToastNotificationHorizontalFillPercentage {
            get {
                return ((int)(this["ToastNotificationHorizontalFillPercentage"]));
            }
            set {
                this["ToastNotificationHorizontalFillPercentage"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("25")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int CrashMessageFontSize {
            get {
                return ((int)(this["CrashMessageFontSize"]));
            }
            set {
                this["CrashMessageFontSize"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool SuggestWords {
            get {
                return ((bool)(this["SuggestWords"]));
            }
            set {
                this["SuggestWords"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("00:01:00")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::System.TimeSpan CalibrationMaxDuration {
            get {
                return ((global::System.TimeSpan)(this["CalibrationMaxDuration"]));
            }
            set {
                this["CalibrationMaxDuration"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Insert")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Enums.Keys PointSelectionTriggerKeyboardKeyDownUpKey {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.Keys)(this["PointSelectionTriggerKeyboardKeyDownUpKey"]));
            }
            set {
                this["PointSelectionTriggerKeyboardKeyDownUpKey"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Left")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Enums.MouseButtons PointSelectionTriggerMouseDownUpButton {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.MouseButtons)(this["PointSelectionTriggerMouseDownUpButton"]));
            }
            set {
                this["PointSelectionTriggerMouseDownUpButton"] = value;
            }
        }


        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("A")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public GamepadButtonFlags PointSelectionTriggerGamepadXInputButtonDownUpButton
        {
            get
            {
                return ((GamepadButtonFlags)(this["PointSelectionTriggerGamepadXInputButtonDownUpButton"]));
            }
            set
            {
                this["PointSelectionTriggerGamepadXInputButtonDownUpButton"] = value;
            }
        }


        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int PointSelectionTriggerGamepadDirectInputButtonDownUpButton
        {
            get
            {
                return ((int)(this["PointSelectionTriggerGamepadDirectInputButtonDownUpButton"]));
            }
            set
            {
                this["PointSelectionTriggerGamepadDirectInputButtonDownUpButton"] = value;
            }
        }


        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool SuppressModifierKeysWhenInMouseKeyboard {
            get {
                return ((bool)(this["SuppressModifierKeysWhenInMouseKeyboard"]));
            }
            set {
                this["SuppressModifierKeysWhenInMouseKeyboard"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int AutoCloseCrashMessageSeconds {
            get {
                return ((int)(this["AutoCloseCrashMessageSeconds"]));
            }
            set {
                this["AutoCloseCrashMessageSeconds"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int ErrorSoundVolume {
            get {
                return ((int)(this["ErrorSoundVolume"]));
            }
            set {
                this["ErrorSoundVolume"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int InfoSoundVolume {
            get {
                return ((int)(this["InfoSoundVolume"]));
            }
            set {
                this["InfoSoundVolume"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("50")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int KeySelectionSoundVolume {
            get {
                return ((int)(this["KeySelectionSoundVolume"]));
            }
            set {
                this["KeySelectionSoundVolume"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int MouseClickSoundVolume {
            get {
                return ((int)(this["MouseClickSoundVolume"]));
            }
            set {
                this["MouseClickSoundVolume"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int MouseDoubleClickSoundVolume {
            get {
                return ((int)(this["MouseDoubleClickSoundVolume"]));
            }
            set {
                this["MouseDoubleClickSoundVolume"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("75")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int MouseScrollSoundVolume {
            get {
                return ((int)(this["MouseScrollSoundVolume"]));
            }
            set {
                this["MouseScrollSoundVolume"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("50")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int MultiKeySelectionCaptureEndSoundVolume {
            get {
                return ((int)(this["MultiKeySelectionCaptureEndSoundVolume"]));
            }
            set {
                this["MultiKeySelectionCaptureEndSoundVolume"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("50")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int MultiKeySelectionCaptureStartSoundVolume {
            get {
                return ((int)(this["MultiKeySelectionCaptureStartSoundVolume"]));
            }
            set {
                this["MultiKeySelectionCaptureStartSoundVolume"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("FillPie")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Enums.ProgressIndicatorBehaviours ProgressIndicatorBehaviour {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.ProgressIndicatorBehaviours)(this["ProgressIndicatorBehaviour"]));
            }
            set {
                this["ProgressIndicatorBehaviour"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int ProgressIndicatorResizeStartProportion {
            get {
                return ((int)(this["ProgressIndicatorResizeStartProportion"]));
            }
            set {
                this["ProgressIndicatorResizeStartProportion"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("20")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int ProgressIndicatorResizeEndProportion {
            get {
                return ((int)(this["ProgressIndicatorResizeEndProportion"]));
            }
            set {
                this["ProgressIndicatorResizeEndProportion"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool KeySelectionTriggerFixationResumeRequiresLockOn {
            get {
                return ((bool)(this["KeySelectionTriggerFixationResumeRequiresLockOn"]));
            }
            set {
                this["KeySelectionTriggerFixationResumeRequiresLockOn"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("50")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public double MainWindowFullDockThicknessAsPercentageOfScreen {
            get {
                return ((double)(this["MainWindowFullDockThicknessAsPercentageOfScreen"]));
            }
            set {
                this["MainWindowFullDockThicknessAsPercentageOfScreen"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Top")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Enums.DockEdges MainWindowDockPosition {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.DockEdges)(this["MainWindowDockPosition"]));
            }
            set {
                this["MainWindowDockPosition"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Full")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Enums.DockSizes MainWindowDockSize {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.DockSizes)(this["MainWindowDockSize"]));
            }
            set {
                this["MainWindowDockSize"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public double MainWindowOpacity {
            get {
                return ((double)(this["MainWindowOpacity"]));
            }
            set {
                this["MainWindowOpacity"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Collapsed")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Enums.DockSizes MouseKeyboardDockSize {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.DockSizes)(this["MouseKeyboardDockSize"]));
            }
            set {
                this["MouseKeyboardDockSize"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Docked")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual global::JuliusSweetland.OptiKey.Enums.WindowStates MainWindowState {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.WindowStates)(this["MainWindowState"]));
            }
            set {
                this["MainWindowState"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("20")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public double MainWindowCollapsedDockThicknessAsPercentageOfFullDockThickness {
            get {
                return ((double)(this["MainWindowCollapsedDockThicknessAsPercentageOfFullDockThickness"]));
            }
            set {
                this["MainWindowCollapsedDockThicknessAsPercentageOfFullDockThickness"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Docked")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual global::JuliusSweetland.OptiKey.Enums.WindowStates MainWindowPreviousState {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.WindowStates)(this["MainWindowPreviousState"]));
            }
            set {
                this["MainWindowPreviousState"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual bool MultiKeySelectionEnabled {
            get {
                return ((bool)(this["MultiKeySelectionEnabled"]));
            }
            set {
                this["MultiKeySelectionEnabled"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("25")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int MouseMoveAmountInPixels {
            get {
                return ((int)(this["MouseMoveAmountInPixels"]));
            }
            set {
                this["MouseMoveAmountInPixels"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Resources\\Sounds\\MouseDown1.wav")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string MouseDownSoundFile {
            get {
                return ((string)(this["MouseDownSoundFile"]));
            }
            set {
                this["MouseDownSoundFile"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Resources\\Sounds\\MouseUp1.wav")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string MouseUpSoundFile {
            get {
                return ((string)(this["MouseUpSoundFile"]));
            }
            set {
                this["MouseUpSoundFile"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int MouseDownSoundVolume {
            get {
                return ((int)(this["MouseDownSoundVolume"]));
            }
            set {
                this["MouseDownSoundVolume"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int MouseUpSoundVolume {
            get {
                return ((int)(this["MouseUpSoundVolume"]));
            }
            set {
                this["MouseUpSoundVolume"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Alpha")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual global::JuliusSweetland.OptiKey.Enums.Keyboards StartupKeyboard {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.Keyboards)(this["StartupKeyboard"]));
            }
            set {
                this["StartupKeyboard"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool SuppressAutoCapitaliseIntelligently {
            get {
                return ((bool)(this["SuppressAutoCapitaliseIntelligently"]));
            }
            set {
                this["SuppressAutoCapitaliseIntelligently"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool MultiKeySelectionLockedDownWhenNotSimulatingKeyStrokes {
            get {
                return ((bool)(this["MultiKeySelectionLockedDownWhenNotSimulatingKeyStrokes"]));
            }
            set {
                this["MultiKeySelectionLockedDownWhenNotSimulatingKeyStrokes"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("SameAsDockedPosition")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Enums.MinimisedEdges MainWindowMinimisedPosition {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.MinimisedEdges)(this["MainWindowMinimisedPosition"]));
            }
            set {
                this["MainWindowMinimisedPosition"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Upper")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Enums.Case KeyCase {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.Case)(this["KeyCase"]));
            }
            set {
                this["KeyCase"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Medium")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Enums.DataStreamProcessingLevels TobiiEyeXProcessingLevel {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.DataStreamProcessingLevels)(this["TobiiEyeXProcessingLevel"]));
            }
            set {
                this["TobiiEyeXProcessingLevel"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("EnglishUK")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Enums.Languages KeyboardAndDictionaryLanguage {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.Languages)(this["KeyboardAndDictionaryLanguage"]));
            }
            set {
                this["KeyboardAndDictionaryLanguage"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool PublishVirtualKeyCodesForCharacters {
            get {
                return ((bool)(this["PublishVirtualKeyCodesForCharacters"]));
            }
            set {
                this["PublishVirtualKeyCodesForCharacters"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual bool MagnifySuppressedForScrollingActions {
            get {
                return ((bool)(this["MagnifySuppressedForScrollingActions"]));
            }
            set {
                this["MagnifySuppressedForScrollingActions"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1,1,1,1")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::System.Windows.Thickness ConversationBorderThickness {
            get {
                return ((global::System.Windows.Thickness)(this["ConversationBorderThickness"]));
            }
            set {
                this["ConversationBorderThickness"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int NGramAutoCompleteGramCount {
            get {
                return ((int)(this["NGramAutoCompleteGramCount"]));
            }
            set {
                this["NGramAutoCompleteGramCount"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int NGramAutoCompleteLeadingSpaceCount {
            get {
                return ((int)(this["NGramAutoCompleteLeadingSpaceCount"]));
            }
            set {
                this["NGramAutoCompleteLeadingSpaceCount"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int NGramAutoCompleteTrailingSpaceCount {
            get {
                return ((int)(this["NGramAutoCompleteTrailingSpaceCount"]));
            }
            set {
                this["NGramAutoCompleteTrailingSpaceCount"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool KeySelectionTriggerFixationCompleteTimesByIndividualKey {
            get {
                return ((bool)(this["KeySelectionTriggerFixationCompleteTimesByIndividualKey"]));
            }
            set {
                this["KeySelectionTriggerFixationCompleteTimesByIndividualKey"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("NGram")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Enums.SuggestionMethods SuggestionMethod {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.SuggestionMethods)(this["SuggestionMethod"]));
            }
            set {
                this["SuggestionMethod"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool SuppressModifierKeysForAllMouseActions {
            get {
                return ((bool)(this["SuppressModifierKeysForAllMouseActions"]));
            }
            set {
                this["SuppressModifierKeysForAllMouseActions"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Default")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Enums.KeyboardLayouts KeyboardLayout {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.KeyboardLayouts)(this["KeyboardLayout"]));
            }
            set {
                this["KeyboardLayout"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool UseAlphabeticalKeyboardLayout {
            get {
                return ((bool)(this["UseAlphabeticalKeyboardLayout"]));
            }
            set {
                this["UseAlphabeticalKeyboardLayout"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool UseSimplifiedKeyboardLayout {
            get {
                return ((bool)(this["UseSimplifiedKeyboardLayout"]));
            }
            set {
                this["UseSimplifiedKeyboardLayout"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual bool EnableCommuniKateKeyboardLayout {
            get {
                return ((bool)(this["EnableCommuniKateKeyboardLayout"]));
            }
            set {
                this["EnableCommuniKateKeyboardLayout"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual bool UseCommuniKateKeyboardLayoutByDefault {
            get {
                return ((bool)(this["UseCommuniKateKeyboardLayoutByDefault"]));
            }
            set {
                this["UseCommuniKateKeyboardLayoutByDefault"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("toppage")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string CommuniKateKeyboardCurrentContext {
            get {
                return ((string)(this["CommuniKateKeyboardCurrentContext"]));
            }
            set {
                this["CommuniKateKeyboardCurrentContext"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool CommuniKateStagedForDeletion {
            get {
                return ((bool)(this["CommuniKateStagedForDeletion"]));
            }
            set {
                this["CommuniKateStagedForDeletion"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool SuggestNextWords {
            get {
                return ((bool)(this["SuggestNextWords"]));
            }
            set {
                this["SuggestNextWords"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool ForceCapsLock {
            get {
                return ((bool)(this["ForceCapsLock"]));
            }
            set {
                this["ForceCapsLock"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int GazeSmoothingLevel {
            get {
                return ((int)(this["GazeSmoothingLevel"]));
            }
            set {
                this["GazeSmoothingLevel"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool SmoothWhenChangingGazeTarget {
            get {
                return ((bool)(this["SmoothWhenChangingGazeTarget"]));
            }
            set {
                this["SmoothWhenChangingGazeTarget"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool ConversationConfirmOnlyMode {
            get {
                return ((bool)(this["ConversationConfirmOnlyMode"]));
            }
            set {
                this["ConversationConfirmOnlyMode"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual bool ConversationConfirmEnable {
            get {
                return ((bool)(this["ConversationConfirmEnable"]));
            }
            set {
                this["ConversationConfirmEnable"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual bool EnableQuitKeys {
            get {
                return ((bool)(this["EnableQuitKeys"]));
            }
            set {
                this["EnableQuitKeys"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool MaryTTSEnabled {
            get {
                return ((bool)(this["MaryTTSEnabled"]));
            }
            set {
                this["MaryTTSEnabled"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("cmu-slt-hsmm en_US female hmm")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string MaryTTSVoice {
            get {
                return ((string)(this["MaryTTSVoice"]));
            }
            set {
                this["MaryTTSVoice"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string MaryTTSLocation {
            get {
                return ((string)(this["MaryTTSLocation"]));
            }
            set {
                this["MaryTTSLocation"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int SpeechDelay {
            get {
                return ((int)(this["SpeechDelay"]));
            }
            set {
                this["SpeechDelay"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("./Resources/CommuniKate/pageset.obz")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string CommuniKatePagesetLocation {
            get {
                return ((string)(this["CommuniKatePagesetLocation"]));
            }
            set {
                this["CommuniKatePagesetLocation"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string CommuniKateDefaultBoard {
            get {
                return ((string)(this["CommuniKateDefaultBoard"]));
            }
            set {
                this["CommuniKateDefaultBoard"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual bool UsingCommuniKateKeyboardLayout {
            get {
                return ((bool)(this["UsingCommuniKateKeyboardLayout"]));
            }
            set {
                this["UsingCommuniKateKeyboardLayout"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string CommuniKateKeyboardPrevious1Context {
            get {
                return ((string)(this["CommuniKateKeyboardPrevious1Context"]));
            }
            set {
                this["CommuniKateKeyboardPrevious1Context"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string CommuniKateKeyboardPrevious2Context {
            get {
                return ((string)(this["CommuniKateKeyboardPrevious2Context"]));
            }
            set {
                this["CommuniKateKeyboardPrevious2Context"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string CommuniKateKeyboardPrevious3Context {
            get {
                return ((string)(this["CommuniKateKeyboardPrevious3Context"]));
            }
            set {
                this["CommuniKateKeyboardPrevious3Context"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string CommuniKateKeyboardPrevious4Context {
            get {
                return ((string)(this["CommuniKateKeyboardPrevious4Context"]));
            }
            set {
                this["CommuniKateKeyboardPrevious4Context"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("50")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int CommuniKateSoundVolume {
            get {
                return ((int)(this["CommuniKateSoundVolume"]));
            }
            set {
                this["CommuniKateSoundVolume"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool CommuniKateSpeakSelected {
            get {
                return ((bool)(this["CommuniKateSpeakSelected"]));
            }
            set {
                this["CommuniKateSpeakSelected"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int CommuniKateSpeakSelectedVolume {
            get {
                return ((int)(this["CommuniKateSpeakSelectedVolume"]));
            }
            set {
                this["CommuniKateSpeakSelectedVolume"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int CommuniKateSpeakSelectedRate {
            get {
                return ((int)(this["CommuniKateSpeakSelectedRate"]));
            }
            set {
                this["CommuniKateSpeakSelectedRate"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string DynamicKeyboardsLocation {
            get {
                return ((string)(this["DynamicKeyboardsLocation"]));
            }
            set {
                this["DynamicKeyboardsLocation"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string StartupKeyboardFile {
            get {
                return ((string)(this["StartupKeyboardFile"]));
            }
            set {
                this["StartupKeyboardFile"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string PluginsLocation {
            get {
                return ((string)(this["PluginsLocation"]));
            }
            set {
                this["PluginsLocation"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual bool EnableAttentionKey {
            get {
                return ((bool)(this["EnableAttentionKey"]));
            }
            set {
                this["EnableAttentionKey"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Resources\\Sounds\\Attention.wav")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string AttentionSoundFile {
            get {
                return ((string)(this["AttentionSoundFile"]));
            }
            set {
                this["AttentionSoundFile"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int AttentionSoundVolume {
            get {
                return ((int)(this["AttentionSoundVolume"]));
            }
            set {
                this["AttentionSoundVolume"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public double ToastNotificationMinimumTimeSeconds {
            get {
                return ((double)(this["ToastNotificationMinimumTimeSeconds"]));
            }
            set {
                this["ToastNotificationMinimumTimeSeconds"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("00:00:00.0500000")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::System.TimeSpan MouseDragDelayAfterLeftMouseButtonDownBeforeMove {
            get {
                return ((global::System.TimeSpan)(this["MouseDragDelayAfterLeftMouseButtonDownBeforeMove"]));
            }
            set {
                this["MouseDragDelayAfterLeftMouseButtonDownBeforeMove"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("00:00:00.0500000")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::System.TimeSpan MouseDragDelayAfterMoveBeforeLeftMouseButtonUp {
            get {
                return ((global::System.TimeSpan)(this["MouseDragDelayAfterMoveBeforeLeftMouseButtonUp"]));
            }
            set {
                this["MouseDragDelayAfterMoveBeforeLeftMouseButtonUp"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int MouseDragNumberOfSteps {
            get {
                return ((int)(this["MouseDragNumberOfSteps"]));
            }
            set {
                this["MouseDragNumberOfSteps"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("00:00:00.0050000")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::System.TimeSpan MouseDragDelayBetweenEachStep {
            get {
                return ((global::System.TimeSpan)(this["MouseDragDelayBetweenEachStep"]));
            }
            set {
                this["MouseDragDelayBetweenEachStep"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool DisplayVoicesWhenChangingKeyboardLanguage {
            get {
                return ((bool)(this["DisplayVoicesWhenChangingKeyboardLanguage"]));
            }
            set {
                this["DisplayVoicesWhenChangingKeyboardLanguage"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual bool LookToScrollIsDefault {
            get {
                return ((bool)(this["LookToScrollIsDefault"]));
            }
            set {
                this["LookToScrollIsDefault"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Free")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Enums.LookToScrollModes LookToScrollMode {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.LookToScrollModes)(this["LookToScrollMode"]));
            }
            set {
                this["LookToScrollMode"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool LookToScrollShowOverlayWindow
        {
            get
            {
                return ((bool)(this["LookToScrollShowOverlayWindow"]));
            }
            set
            {
                this["LookToScrollShowOverlayWindow"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ScreenPoint")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Enums.LookToScrollBounds LookToScrollBounds {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.LookToScrollBounds)(this["LookToScrollBounds"]));
            }
            set {
                this["LookToScrollBounds"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("50")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int LookToScrollHorizontalDeadzone {
            get {
                return ((int)(this["LookToScrollHorizontalDeadzone"]));
            }
            set {
                this["LookToScrollHorizontalDeadzone"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("50")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int LookToScrollVerticalDeadzone {
            get {
                return ((int)(this["LookToScrollVerticalDeadzone"]));
            }
            set {
                this["LookToScrollVerticalDeadzone"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Medium")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public LookToScrollSpeeds LookToScrollSpeed {
            get {
                return ((LookToScrollSpeeds)(this["LookToScrollSpeed"]));
            }
            set {
                this["LookToScrollSpeed"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool LookToScrollCentreMouseWhenActivated {
            get {
                return ((bool)(this["LookToScrollCentreMouseWhenActivated"]));
            }
            set {
                this["LookToScrollCentreMouseWhenActivated"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool LookToScrollBringWindowToFrontWhenActivated {
            get {
                return ((bool)(this["LookToScrollBringWindowToFrontWhenActivated"]));
            }
            set {
                this["LookToScrollBringWindowToFrontWhenActivated"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool LookToScrollDeactivateUponSwitchingKeyboards {
            get {
                return ((bool)(this["LookToScrollDeactivateUponSwitchingKeyboards"]));
            }
            set {
                this["LookToScrollDeactivateUponSwitchingKeyboards"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Yellow")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string LookToScrollOverlayBoundsColour {
            get {
                return ((string)(this["LookToScrollOverlayBoundsColour"]));
            }
            set {
                this["LookToScrollOverlayBoundsColour"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Gray")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string LookToScrollOverlayDeadzoneColour {
            get {
                return ((string)(this["LookToScrollOverlayDeadzoneColour"]));
            }
            set {
                this["LookToScrollOverlayDeadzoneColour"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("4")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int LookToScrollOverlayBoundsThickness {
            get {
                return ((int)(this["LookToScrollOverlayBoundsThickness"]));
            }
            set {
                this["LookToScrollOverlayBoundsThickness"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("4")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int LookToScrollOverlayDeadzoneThickness {
            get {
                return ((int)(this["LookToScrollOverlayDeadzoneThickness"]));
            }
            set {
                this["LookToScrollOverlayDeadzoneThickness"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool LookToScrollResumeAfterChoosingPointForMouse
        {
            get
            {
                return ((bool)(this["LookToScrollResumeAfterChoosingPointForMouse"]));
            }
            set
            {
                this["LookToScrollResumeAfterChoosingPointForMouse"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Home")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Enums.SimplifiedKeyboardContexts SimplifiedKeyboardContext {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.SimplifiedKeyboardContexts)(this["SimplifiedKeyboardContext"]));
            }
            set {
                this["SimplifiedKeyboardContext"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public string PresageDatabaseLocation {
            get {
                return ((string)(this["PresageDatabaseLocation"]));
            }
            set {
                this["PresageDatabaseLocation"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("11")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int PresageNumberOfSuggestions {
            get {
                return ((int)(this["PresageNumberOfSuggestions"]));
            }
            set {
                this["PresageNumberOfSuggestions"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<dictionary>\r\n  <item>\r\n    <key>\r\n     " +
            " <keyValue>\r\n        <functionKey>Alpha1Keyboard</functionKey>\r\n      </keyValue" +
            ">\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item" +
            ">\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>Alpha2Keyboard</f" +
            "unctionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</t" +
            "icks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <" +
            "functionKey>BackFromKeyboard</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <" +
            "value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <ke" +
            "y>\r\n      <keyValue>\r\n        <functionKey>ConversationAlpha1Keyboard</functionK" +
            "ey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n " +
            "   </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <function" +
            "Key>ConversationAlpha2Keyboard</functionKey>\r\n      </keyValue>\r\n    </key>\r\n   " +
            " <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <" +
            "key>\r\n      <keyValue>\r\n        <functionKey>ConversationNumericAndSymbolsKeyboa" +
            "rd</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.7" +
            "5S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n    " +
            "    <functionKey>Currencies1Keyboard</functionKey>\r\n      </keyValue>\r\n    </key" +
            ">\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r" +
            "\n    <key>\r\n      <keyValue>\r\n        <functionKey>Currencies2Keyboard</function" +
            "Key>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n" +
            "    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functio" +
            "nKey>Diacritic1Keyboard</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value" +
            ">\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n " +
            "     <keyValue>\r\n        <functionKey>Diacritic2Keyboard</functionKey>\r\n      </" +
            "keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n" +
            "  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>Diacritic" +
            "3Keyboard</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <tick" +
            "s>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue" +
            ">\r\n        <functionKey>LanguageKeyboard</functionKey>\r\n      </keyValue>\r\n    <" +
            "/key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <it" +
            "em>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>MenuKeyboard</functionKey" +
            ">\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n   " +
            " </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKe" +
            "y>MouseKeyboard</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n     " +
            " <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <ke" +
            "yValue>\r\n        <functionKey>NumericAndSymbols1Keyboard</functionKey>\r\n      </" +
            "keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n" +
            "  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>NumericAn" +
            "dSymbols2Keyboard</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n   " +
            "   <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <" +
            "keyValue>\r\n        <functionKey>NumericAndSymbols3Keyboard</functionKey>\r\n      " +
            "</keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>" +
            "\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>Dynamic" +
            "Keyboard</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks" +
            ">PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>" +
            "\r\n        <functionKey>WebBrowsingKeyboard</functionKey>\r\n      </keyValue>\r\n   " +
            " </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <" +
            "item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>PhysicalKeysKeyboard</f" +
            "unctionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</t" +
            "icks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <" +
            "functionKey>SizeAndPositionKeyboard</functionKey>\r\n      </keyValue>\r\n    </key>" +
            "\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n" +
            "    <key>\r\n      <keyValue>\r\n        <functionKey>CatalanSpain</functionKey>\r\n  " +
            "    </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </va" +
            "lue>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>Cro" +
            "atianCroatia</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <t" +
            "icks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyVa" +
            "lue>\r\n        <functionKey>CzechCzechRepublic</functionKey>\r\n      </keyValue>\r\n" +
            "    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n" +
            "  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>DanishDenmark</funct" +
            "ionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks" +
            ">\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <func" +
            "tionKey>DutchBelgium</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n" +
            "      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n    " +
            "  <keyValue>\r\n        <functionKey>DutchNetherlands</functionKey>\r\n      </keyVa" +
            "lue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </i" +
            "tem>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>EnglishCanada<" +
            "/functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S<" +
            "/ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n       " +
            " <functionKey>EnglishUK</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value" +
            ">\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n " +
            "     <keyValue>\r\n        <functionKey>EnglishUS</functionKey>\r\n      </keyValue>" +
            "\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>" +
            "\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>FrenchCanada</func" +
            "tionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</tick" +
            "s>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <fun" +
            "ctionKey>FrenchFrance</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r" +
            "\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n   " +
            "   <keyValue>\r\n        <functionKey>GeorgianGeorgia</functionKey>\r\n      </keyVa" +
            "lue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </i" +
            "tem>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>GermanGermany<" +
            "/functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S<" +
            "/ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n       " +
            " <functionKey>GreekGreece</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <val" +
            "ue>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n       " +
            " <functionKey>HebrewIsrael</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <val" +
            "ue>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n       " +
            " <functionKey>HindiIndia</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <val" +
            "ue>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n       " +
            " <functionKey>HungarianHungary</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <val" +
            "ue>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r" +
            "\n      <keyValue>\r\n        <functionKey>ItalianItaly</functionKey>\r\n      </keyV" +
            "alue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </" +
            "item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>JapaneseJapan" +
            "</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S" +
            "</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n      " +
            "  <functionKey>KoreanKorea</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <va" +
            "lue>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>" +
            "\r\n      <keyValue>\r\n        <functionKey>PolishPoland</functionKey>\r\n      </key" +
            "Value>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  <" +
            "/item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>PortuguesePo" +
            "rtugal</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>P" +
            "T1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n" +
            "        <functionKey>RussianRussia</functionKey>\r\n      </keyValue>\r\n    </key>\r" +
            "\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n " +
            "   <key>\r\n      <keyValue>\r\n        <functionKey>SerbianSerbia</functionKey>\r\n  " +
            "    </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </va" +
            "lue>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>Slo" +
            "vakSlovakia</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ti" +
            "cks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyVal" +
            "ue>\r\n        <functionKey>SlovenianSlovenia</functionKey>\r\n      </keyValue>\r\n  " +
            "  </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  " +
            "<item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>SpanishSpain</function" +
            "Key>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n" +
            "    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functio" +
            "nKey>TurkishTurkey</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n  " +
            "    <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      " +
            "<keyValue>\r\n        <functionKey>UkrainianUkraine</functionKey>\r\n      </keyValu" +
            "e>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </ite" +
            "m>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>UrduPakistan</fu" +
            "nctionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ti" +
            "cks>\r\n    </value>\r\n  </item>  \r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        " +
            "<functionKey>AddToDictionary</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <" +
            "value>\r\n      <ticks>PT2S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r" +
            "\n      <keyValue>\r\n        <functionKey>Attention</functionKey>\r\n      </keyValu" +
            "e>\r\n    </key>\r\n    <value>\r\n      <ticks>PT2S</ticks>\r\n    </value>\r\n  </item>\r" +
            "\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>CommuniKateKeyboard" +
            "</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT2S</t" +
            "icks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <" +
            "functionKey>BackMany</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n" +
            "      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n    " +
            "  <keyValue>\r\n        <functionKey>BackOne</functionKey>\r\n      </keyValue>\r\n   " +
            " </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <" +
            "item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>Calibrate</functionKey>" +
            "\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    " +
            "</value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey" +
            ">ClearScratchpad</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n    " +
            "  <ticks>PT2S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyV" +
            "alue>\r\n        <functionKey>DecreaseOpacity</functionKey>\r\n      </keyValue>\r\n  " +
            "  </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  " +
            "<item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>IncreaseOpacity</funct" +
            "ionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks" +
            ">\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <func" +
            "tionKey>Minimise</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n    " +
            "  <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <k" +
            "eyValue>\r\n        <functionKey>MultiKeySelectionIsOn</functionKey>\r\n      </keyV" +
            "alue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </" +
            "item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>NoQuestionRes" +
            "ult</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1." +
            "75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n   " +
            "     <functionKey>Quit</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>" +
            "\r\n      <ticks>PT2S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n     " +
            " <keyValue>\r\n        <functionKey>Restart</functionKey>\r\n      </keyValue>\r\n    " +
            "</key>\r\n    <value>\r\n      <ticks>PT2S</ticks>\r\n    </value>\r\n  </item>\r\n  <item" +
            ">\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>Sleep</functionKey>\r\n      " +
            "</keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>" +
            "\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>Speak</" +
            "functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</" +
            "ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        " +
            "<functionKey>YesQuestionResult</functionKey>\r\n      </keyValue>\r\n    </key>\r\n   " +
            " <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <" +
            "key>\r\n      <keyValue>\r\n        <functionKey>LeftAlt</functionKey>\r\n      </keyV" +
            "alue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.5S</ticks>\r\n    </value>\r\n  </i" +
            "tem>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>LeftCtrl</func" +
            "tionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.5S</ticks" +
            ">\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <func" +
            "tionKey>LeftShift</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n   " +
            "   <ticks>PT1.5S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <k" +
            "eyValue>\r\n        <functionKey>LeftWin</functionKey>\r\n      </keyValue>\r\n    </k" +
            "ey>\r\n    <value>\r\n      <ticks>PT1.5S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>" +
            "\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>NextSuggestions</functionKey" +
            ">\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n   " +
            " </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKe" +
            "y>PreviousSuggestions</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r" +
            "\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n   " +
            "   <keyValue>\r\n        <functionKey>Suggestion1</functionKey>\r\n      </keyValue>" +
            "\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>" +
            "\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>Suggestion2</funct" +
            "ionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks" +
            ">\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <func" +
            "tionKey>Suggestion3</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n " +
            "     <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n     " +
            " <keyValue>\r\n        <functionKey>Suggestion4</functionKey>\r\n      </keyValue>\r\n" +
            "    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n" +
            "  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>Suggestion5</functio" +
            "nKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r" +
            "\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <keyValue>\r\n        <functi" +
            "onKey>Suggestion6</functionKey>\r\n      </keyValue>\r\n    </key>\r\n    <value>\r\n   " +
            "   <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  <item>\r\n    <key>\r\n      <" +
            "keyValue>\r\n        <functionKey>SelectVoice</functionKey>\r\n      </keyValue>\r\n  " +
            "  </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </value>\r\n  </item>\r\n  " +
            "<item>\r\n    <key>\r\n      <keyValue>\r\n        <functionKey>More</functionKey>\r\n  " +
            "    </keyValue>\r\n    </key>\r\n    <value>\r\n      <ticks>PT1.75S</ticks>\r\n    </va" +
            "lue>\r\n  </item>\r\n</dictionary>")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Models.SerializableDictionaryOfTimeSpanByKeyValues KeySelectionTriggerFixationCompleteTimesByKeyValues {
            get {
                return ((global::JuliusSweetland.OptiKey.Models.SerializableDictionaryOfTimeSpanByKeyValues)(this["KeySelectionTriggerFixationCompleteTimesByKeyValues"]));
            }
            set {
                this["KeySelectionTriggerFixationCompleteTimesByKeyValues"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("None")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::JuliusSweetland.OptiKey.Enums.DataStreamProcessingLevels IrisbondProcessingLevel {
            get {
                return ((global::JuliusSweetland.OptiKey.Enums.DataStreamProcessingLevels)(this["IrisbondProcessingLevel"]));
            }
            set {
                this["IrisbondProcessingLevel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("9")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public int IrisbondCalibrationTargetCount {
            get {
                return ((int)(this["IrisbondCalibrationTargetCount"]));
            }
            set {
                this["IrisbondCalibrationTargetCount"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual bool EnableCopyAllScratchpadKey {
            get {
                return ((bool)(this["EnableCopyAllScratchpadKey"]));
            }
            set {
                this["EnableCopyAllScratchpadKey"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual bool EnableTranslationKey
        {
            get
            {
                return ((bool)(this["EnableTranslationKey"]));
            }
            set
            {
                this["EnableTranslationKey"] = value;
            }
        }    
            
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("en")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual string TranslationTargetLanguage
        {
            get
            {
                return (string)(this["TranslationTargetLanguage"]);
            }
            set
            {
                this["TranslationTargetLanguage"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual bool EnableOverrideTranslationApiKey
        {
            get
            {
                return ((bool)(this["EnableOverrideTranslationApiKey"]));
            }
            set
            {
                this["EnableOverrideTranslationApiKey"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual string OverriddenTranslationApiKey
        {
            get
            {
                return ((string)(this["OverriddenTranslationApiKey"]));
            }
            set
            {
                this["OverriddenTranslationApiKey"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool TypeDiacriticsAfterLetters {
            get {
                return ((bool)(this["TypeDiacriticsAfterLetters"]));
            }
            set {
                this["TypeDiacriticsAfterLetters"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("LeftToRight")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::System.Windows.FlowDirection UiLanguageFlowDirection {
            get {
                return ((global::System.Windows.FlowDirection)(this["UiLanguageFlowDirection"]));
            }
            set {
                this["UiLanguageFlowDirection"] = value;
            }
        }

	    [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool EnablePlugins {
            get {
                return ((bool)(this["EnablePlugins"]));
            }
            set {
                this["EnablePlugins"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool MagnifierCenterOnScreen
        {
            get
            {
                return ((bool)(this["MagnifierCenterOnScreen"]));
            }
            set
            {
                this["MagnifierCenterOnScreen"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public bool SuppressTriggerWithoutPositionError
        {
            get
            {
                return ((bool)(this["SuppressTriggerWithoutPositionError"]));
            }
            set
            {
                this["SuppressTriggerWithoutPositionError"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual bool AllowMultipleInstances
        {
            get
            {
                return ((bool)(this["AllowMultipleInstances"]));
            }
            set
            {
                this["AllowMultipleInstances"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("false")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual bool CleanShutdown
        {
            get
            {
                return ((bool)(this["CleanShutdown"]));
            }
            set
            {
                this["CleanShutdown"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual bool AttemptRestartUponCrash
        {
            get
            {
                return ((bool)(this["AttemptRestartUponCrash"]));
            }
            set
            {
                this["AttemptRestartUponCrash"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual bool EnableResizeWithMouse
        {
            get
            {
                return ((bool)(this["EnableResizeWithMouse"]));
            }
            set
            {
                this["EnableResizeWithMouse"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual string EyeGestureFile
        {
            get
            {
                return ((string)(this["EyeGestureFile"]));
            }
            set
            {
                this["EyeGestureFile"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual string EyeGestureString
        {
            get
            {
                return ((string)(this["EyeGestureString"]));
            }
            set
            {
                this["EyeGestureString"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual bool EyeGesturesEnabled
        {
            get
            {
                return ((bool)(this["EyeGesturesEnabled"]));
            }
            set
            {
                this["EyeGesturesEnabled"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual bool EyeGestureUpdated
        {
            get
            {
                return ((bool)(this["EyeGestureUpdated"]));
            }
            set
            {
                this["EyeGestureUpdated"] = value;
            }
        }


        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public virtual string KeyboardFile
        {
            get
            {
                return ((string)(this["KeyboardFile"]));
            }
            set
            {
                this["KeyboardFile"] = value;
            }
        }

        public bool IsOverridden(string propName)
        {
            var propActual = this.GetType().GetProperty(propName);
            var propBase = typeof(Settings).GetProperty(propName);

            if (propActual != null && propBase != null)
            {
                return !(propActual.DeclaringType == propBase.DeclaringType);
            }
            else
            {
                Log.ErrorFormat("No property named {0}", propName);
                return false;
            }
        }
    }
}
