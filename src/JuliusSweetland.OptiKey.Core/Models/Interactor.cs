// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using JuliusSweetland.OptiKey.Enums;
using JuliusSweetland.OptiKey.Extensions;
using JuliusSweetland.OptiKey.UI.Controls;
using JuliusSweetland.OptiKey.UI.ValueConverters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace JuliusSweetland.OptiKey.Models
{
    public class Interactor : INotifyPropertyChanged
    {
        private PropertyChangedEventHandler descendantPropertyChanged;
        [XmlIgnore] public ObservableCollection<InteractorProfile> Descendants { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            if (name == "Row" || name == "Col" || name.StartsWith("Gaze"))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InteractorLocation"));
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        protected void DescendantPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Profiles"));
        }

        public Interactor()
        {
            if (!(this is InteractorProfile))
            {
                Inherited = new InteractorProfile();
                Expressed = new InteractorProfile();
                commands = new ObservableCollection<KeyCommand>();
                descendantPropertyChanged = new PropertyChangedEventHandler(DescendantPropertyChanged);

                Profiles = new ObservableCollection<InteractorProfileMap>();
                Profiles.CollectionChanged += delegate (object sender, NotifyCollectionChangedEventArgs e)
                {
                    if (e.Action == NotifyCollectionChangedAction.Add)
                    {
                        foreach (INotifyPropertyChanged propertyChanged in e.NewItems)
                            propertyChanged.PropertyChanged += descendantPropertyChanged;
                    }
                    else if (e.Action == NotifyCollectionChangedAction.Remove)
                    {
                        foreach (INotifyPropertyChanged propertyChanged in e.OldItems)
                            propertyChanged.PropertyChanged -= descendantPropertyChanged;
                    }
                };
            }
        }
        
        [XmlIgnore] public Key Key { get; set; } = new Key();
        [XmlIgnore] public string TypeAsString
        {
            get
            {
                return
                    this is DynamicKey ? InteractorTypes.Key.ToString() :
                    this is DynamicOutputPanel ? InteractorTypes.OutputPanel.ToString() :
                    this is DynamicPopup ? InteractorTypes.Popup.ToString() :
                    this is DynamicScratchpad ? InteractorTypes.Scratchpad.ToString() :
                    this is DynamicSuggestionRow ? InteractorTypes.SuggestionRow.ToString() :
                    this is DynamicSuggestionCol ? InteractorTypes.SuggestionColumn.ToString() :
                    "InteractorProfile";
            }
        }

        [XmlIgnore] public Visibility IsKey { get { return this is DynamicPopup || this is DynamicKey ? Visibility.Visible : Visibility.Collapsed; } }
        [XmlIgnore] public Visibility IsPopup { get { return this is DynamicPopup ? Visibility.Visible : Visibility.Collapsed; } }
        [XmlIgnore] public Visibility IsNotPopup { get { return this is DynamicPopup ? Visibility.Collapsed : Visibility.Visible; } }

        [XmlIgnore] public int RowN { get; set; } = -1;
        [XmlAttribute] public string Row { get { return (this is DynamicPopup || this is InteractorProfile) ? null : RowN.ToString(); } set { RowN = int.TryParse(value, out int result) ? result : -1; OnPropertyChanged(); } }

        [XmlIgnore] public int ColN { get; set; } = -1;
        [XmlAttribute] public string Col { get { return (this is DynamicPopup || this is InteractorProfile) ? null : ColN.ToString(); } set { ColN = int.TryParse(value, out int result) ? result : -1; OnPropertyChanged(); } }

        [XmlIgnore] public int WidthN { get; set; } = 1;
        [XmlAttribute] public string Width { get { return (this is DynamicPopup || this is InteractorProfile) ? null : WidthN.ToString(); } set { WidthN = int.TryParse(value, out int result) && result > 0 ? result : 1; OnPropertyChanged(); } }

        [XmlIgnore] public int HeightN { get; set; } = 1;
        [XmlAttribute] public string Height { get { return (this is DynamicPopup || this is InteractorProfile) ? null : HeightN.ToString(); } set { HeightN = int.TryParse(value, out int result) && result > 0 ? result : 1; OnPropertyChanged(); } }
        
        private double gazeLeft = -.1;
        [XmlIgnore] public double GazeLeft { get { return gazeLeft; } set { gazeLeft = value; OnPropertyChanged(); } }
        private double gazeTop = 1;
        [XmlIgnore] public double GazeTop { get { return gazeTop; } set { gazeTop = value; OnPropertyChanged(); } }
        private double gazeWidth = .1;
        [XmlIgnore] public double GazeWidth { get { return gazeWidth; } set { gazeWidth = value; OnPropertyChanged(); } }
        private double gazeHeight = .1;
        [XmlIgnore] public double GazeHeight { get { return gazeHeight; } set { gazeHeight = value; OnPropertyChanged(); } }
        [XmlAttribute] public string GazeRegion
        {
            get { return this is DynamicPopup ? string.Concat(GazeLeft.ToString(), ",", GazeTop.ToString(), ",", GazeWidth.ToString(), ",", GazeHeight.ToString()) : null; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var list = new List<double>();
                    foreach (var item in value.Split(',').ToList())
                    {
                        if (double.TryParse(item, out double result))
                            list.Add(result);
                    }
                    if (list.Count == 4)
                    {
                        GazeLeft = list[0];
                        GazeTop = list[1];
                        GazeWidth = list[2];
                        GazeHeight = list[3];
                    }
                }
            }
        }

        private string label;
        [XmlIgnore] public string LabelEdit
        {
            get { return label; }
            set
            {
                if (string.IsNullOrEmpty(label) && !Commands.Any())
                    Commands.Add(new TextCommand() { Value = value });
                else if (string.IsNullOrEmpty(value) && Commands.Count == 1 && Commands[0] is TextCommand && Commands[0].Value == label)
                    Commands.Clear();
                foreach (var c in commands.Where(x => x.Value == Label))
                    c.Value = value;
                Label = value;
                ShiftDownLabel = Label != null && Label != Label.ToUpper() && Label.Length == 1 ? Label.ToUpper() : null;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InteractorLabel"));
                OnPropertyChanged();
            }
        }
        [XmlAttribute] public string Label { get { return label; } set { label = value; } }

        private string shiftDownLabel;
        [XmlAttribute] public string ShiftDownLabel { get { return shiftDownLabel; } set { shiftDownLabel = value; OnPropertyChanged(); } }

        private string symbol;
        [XmlAttribute] public string Symbol { get { return symbol; } set { symbol = value; OnPropertyChanged(); } }

        [XmlIgnore] public string InteractorLabel
        {
            get
            {
                return this is DynamicOutputPanel ? "Output Panel"
                    : this is DynamicScratchpad ? "Scratchpad"
                    : this is DynamicSuggestionRow ? "Suggestion Row"
                    : this is DynamicSuggestionCol ? "Suggestion Col"
                    : Label;
            }
        }
        [XmlIgnore] public string InteractorLocation
        { get { return this is DynamicPopup
                    ? "XY (" + GazeLeft.ToString() + ", " + GazeTop.ToString() + ")"
                    : "RC (" + Row + ", " + Col + ")"; } }

        [XmlElement("KeyGroup")] public List<XmlElementValue> ProfileNames { get; set; }

        private ObservableCollection<KeyCommand> commands;
        [XmlElement("Action", typeof(ActionCommand))]
        [XmlElement("ChangeKeyboard", typeof(ChangeKeyboardCommand))]
        [XmlElement("KeyDown", typeof(KeyDownCommand))]
        [XmlElement("KeyUp", typeof(KeyUpCommand))]
        [XmlElement("KeyToggle", typeof(KeyTogglCommand))]
        [XmlElement("Loop", typeof(LoopCommand))]
        [XmlElement("Plugin", typeof(PluginCommand))]
        [XmlElement("MoveWindow", typeof(MoveWindowCommand))]
        [XmlElement("Switch", typeof(SwitchCommand))]
        [XmlElement("Text", typeof(TextCommand))]
        [XmlElement("Wait", typeof(WaitCommand))]
        public ObservableCollection<KeyCommand> Commands { get { return commands; } set { commands = value; } }

        private ObservableCollection<InteractorProfileMap> profiles;
        [XmlIgnore] public ObservableCollection<InteractorProfileMap> Profiles
        { get { return profiles; } set { profiles = value; } }

        private InteractorProfile inherited;
        [XmlIgnore] public InteractorProfile Inherited
        { get { return inherited; } set { inherited = value; } }

        [XmlIgnore] public InteractorProfile Expressed { get; set; }

        private string name;
        [XmlAttribute] public string Name
        {
            get { return name; }
            set { name = name == "All" ? "All" : value; OnPropertyChanged(); }
        }

        [XmlIgnore] public TimeSpan? LockOnTimeN { get; set; }
        private string lockOnTime;
        [XmlAttribute]
        public string LockOnTime
        {
            get { return lockOnTime; }
            set
            {
                lockOnTime = double.TryParse(value, out double result) ? value : null;
                LockOnTimeN = lockOnTime != null
                    ? (TimeSpan?)TimeSpan.FromMilliseconds(result.Clamp(0, 1000)) : null;
                OnPropertyChanged();
            }
        }

        [XmlIgnore] public List<string> CompletionTimesN { get; set; }
        private string completionTimes;
        [XmlAttribute]
        public string CompletionTimes
        {
            get { return completionTimes; }
            set
            {
                completionTimes = string.IsNullOrWhiteSpace(value) ? null : value;
                var list = new List<string>();
                if (completionTimes != null)
                {
                    foreach (var item in completionTimes.Split(',').ToList().Where(x => int.TryParse(x, out _)))
                    {
                        list.Add(item);
                    }
                }
                CompletionTimesN = list.Any() ? list : null;
                OnPropertyChanged();
            }
        }

        [XmlIgnore] public TimeSpan? TimeRequiredToLockDownN { get; set; }
        private string timeRequiredToLockDown;
        [XmlAttribute]
        public string TimeRequiredToLockDown
        {
            get { return timeRequiredToLockDown; }
            set
            {
                timeRequiredToLockDown = double.TryParse(value, out double result) ? value : null;
                TimeRequiredToLockDownN = timeRequiredToLockDown != null
                    ? (TimeSpan?)TimeSpan.FromMilliseconds(result.Clamp(0, 10000)) : null;
                OnPropertyChanged();
            }
        }

        [XmlIgnore] public TimeSpan? LockDownAttemptTimeoutN { get; set; }
        private string lockDownAttemptTimeout;
        [XmlAttribute]
        public string LockDownAttemptTimeout
        {
            get { return lockDownAttemptTimeout; }
            set
            {
                lockDownAttemptTimeout = double.TryParse(value, out double result) ? value : null;
                LockDownAttemptTimeoutN = lockDownAttemptTimeout != null
                    ? (TimeSpan?)TimeSpan.FromMilliseconds(result.Clamp(0, 1000)) : null;
                OnPropertyChanged();
            }
        }

        [XmlIgnore] public double? OpacityN { get; set; }
        private string opacity;
        [XmlAttribute]
        public string Opacity
        {
            get { return opacity; }
            set
            {
                opacity = string.IsNullOrWhiteSpace(value) ? null : value;
                OpacityN = double.TryParse(value, out double result) ? (double?)result : null;
                OnPropertyChanged();
            }
        }

        [XmlIgnore] public SolidColorBrush BackgroundBrush { get; set; }
        private string backgroundColor;
        [XmlAttribute] public string BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                backgroundColor = string.IsNullOrWhiteSpace(value) ? null : value;
                if (HlsColor.TryParse(value, out SolidColorBrush brush))
                {
                    BackgroundBrush = brush;
                    OnPropertyChanged();
                }
                else if (BackgroundBrush != null)
                {
                    BackgroundBrush = null;
                    OnPropertyChanged();
                }
            }
        }

        [XmlIgnore] public SolidColorBrush ForegroundBrush { get; set; }
        private string foregroundColor;
        [XmlAttribute]
        public string ForegroundColor
        {
            get { return foregroundColor; }
            set
            {
                foregroundColor = string.IsNullOrWhiteSpace(value) ? null : value;
                if (HlsColor.TryParse(value, out SolidColorBrush brush))
                {
                    ForegroundBrush = brush;
                    OnPropertyChanged();
                }
                else if (ForegroundBrush != null)
                {
                    ForegroundBrush = null;
                    OnPropertyChanged();
                }
            }
        }

        [XmlIgnore] public SolidColorBrush BorderBrush { get; set; }
        private string borderColor;
        [XmlAttribute] public string BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = string.IsNullOrWhiteSpace(value) ? null : value;
                if (HlsColor.TryParse(value, out SolidColorBrush brush))
                {
                    BorderBrush = brush;
                    OnPropertyChanged();
                }
                else if (BorderBrush != null)
                {
                    BorderBrush = null;
                    OnPropertyChanged();
                }
            }
        }

        [XmlIgnore] public int? BorderThicknessN;
        private string borderThickness;
        [XmlAttribute] public string BorderThickness
        {
            get { return borderThickness; }
            set
            {
                borderThickness = value;
                BorderThicknessN = int.TryParse(value, out int result) ? (int?)result : null;
                OnPropertyChanged();
            }
        }

        [XmlIgnore] public int? CornerRadiusN;
        private string cornerRadius;
        [XmlAttribute] public string CornerRadius
        {
            get { return cornerRadius; }
            set
            {
                cornerRadius = value;
                CornerRadiusN = int.TryParse(value, out int result) ? (int?)result : null;
                OnPropertyChanged();
            }
        }

        [XmlIgnore] public SolidColorBrush KeyDisabledBackgroundBrush { get; set; }
        private string keyDisabledBackground;
        [XmlAttribute] public string KeyDisabledBackground
        {
            get { return keyDisabledBackground; }
            set
            {
                keyDisabledBackground = string.IsNullOrWhiteSpace(value) ? null : value;
                KeyDisabledBackgroundBrush = HlsColor.TryParse(value, out SolidColorBrush brush) ? brush : null;
            }
        }

        [XmlIgnore] public SolidColorBrush KeyDisabledForegroundBrush { get; set; }
        private string keyDisabledForeground;
        [XmlAttribute] public string KeyDisabledForeground
        {
            get { return keyDisabledForeground; }
            set
            {
                keyDisabledForeground = string.IsNullOrWhiteSpace(value) ? null : value;
                KeyDisabledForegroundBrush = HlsColor.TryParse(value, out SolidColorBrush brush) ? brush : null;
            }
        }

        [XmlIgnore] public double? KeyDisabledOpacityN { get; set; }
        private string keyDisabledOpacity;
        [XmlAttribute]
        public string KeyDisabledOpacity
        {
            get { return keyDisabledOpacity; }
            set
            {
                keyDisabledOpacity = string.IsNullOrWhiteSpace(value) ? null : value;
                KeyDisabledOpacityN = double.TryParse(value, out double result) ? (double?)result : null;
            }
        }

        [XmlIgnore] public SolidColorBrush KeyDownBackgroundBrush { get; set; }
        private string keyDownBackground;
        [XmlAttribute] public string KeyDownBackground
        {
            get { return keyDownBackground; }
            set
            {
                keyDownBackground = string.IsNullOrWhiteSpace(value) ? null : value;
                KeyDownBackgroundBrush = HlsColor.TryParse(value, out SolidColorBrush brush) ? brush : null;
            }
        }

        [XmlIgnore] public SolidColorBrush KeyDownForegroundBrush { get; set; }
        private string keyDownForeground;
        [XmlAttribute] public string KeyDownForeground
        {
            get { return keyDownForeground; }
            set
            {
                keyDownForeground = string.IsNullOrWhiteSpace(value) ? null : value;
                KeyDownForegroundBrush = HlsColor.TryParse(value, out SolidColorBrush brush) ? brush : null;
            }
        }

        [XmlIgnore] public double? KeyDownOpacityN { get; set; }
        private string keyDownOpacity;
        [XmlAttribute] public string KeyDownOpacity
        {
            get { return keyDownOpacity; }
            set
            {
                keyDownOpacity = string.IsNullOrWhiteSpace(value) ? null : value;
                KeyDownOpacityN = double.TryParse(value, out double result) ? (double?)result : null;
            }
        }

        private string sharedSizeGroup;
        [XmlAttribute] public string SharedSizeGroup
        { get { return sharedSizeGroup; } set { sharedSizeGroup = value; OnPropertyChanged(); } }

        [XmlIgnore] public bool? AutoScaleToOneKeyWidthN;
        [XmlAttribute] public string AutoScaleToOneKeyWidth
        {
            get { return AutoScaleToOneKeyWidthN.HasValue ? AutoScaleToOneKeyWidthN.Value.ToString() : null; }
            set { AutoScaleToOneKeyWidthN = bool.TryParse(value, out bool result) ? (bool?)result : null; OnPropertyChanged(); }
        }

        [XmlIgnore] public bool? AutoScaleToOneKeyHeightN;
        [XmlAttribute] public string AutoScaleToOneKeyHeight
        {
            get { return AutoScaleToOneKeyHeightN.HasValue ? AutoScaleToOneKeyHeightN.Value.ToString() : null; }
            set { AutoScaleToOneKeyHeightN = bool.TryParse(value, out bool result) ? (bool?)result : null; OnPropertyChanged(); }
        }

        private string compatibilityFont;
        [XmlAttribute] public string CompatibilityFont
        {
            get { return string.IsNullOrWhiteSpace(compatibilityFont) ? null : compatibilityFont; }
            set
            {
                compatibilityFont = value;
                if (value != "" && value != "Persian" && value != "Unicode" && value != "Urdu")
                {
                    UsePersianCompatibilityFont = "False";
                    UseUnicodeCompatibilityFont = "False";
                    UseUrduCompatibilityFont = "False";
                }
                else
                {
                    UsePersianCompatibilityFont = value == "Persian" ? "True" : null;
                    UseUnicodeCompatibilityFont = value == "Unicode" ? "True" : null;
                    UseUrduCompatibilityFont = value == "Urdu" ? "True" : null;
                }
            }
        }
        [XmlIgnore] public bool? UsePersianCompatibilityFontN { get; set; }
        [XmlAttribute] public string UsePersianCompatibilityFont
        {
            get { return UsePersianCompatibilityFontN.HasValue ? UsePersianCompatibilityFontN.Value.ToString() : null; }
            set { UsePersianCompatibilityFontN = bool.TryParse(value, out bool result) ? (bool?)result : null; OnPropertyChanged(); }
        }

        [XmlIgnore] public bool? UseUnicodeCompatibilityFontN { get; set; }
        [XmlAttribute] public string UseUnicodeCompatibilityFont
        {
            get { return UseUnicodeCompatibilityFontN.HasValue ? UseUnicodeCompatibilityFontN.Value.ToString() : null; }
            set { UseUnicodeCompatibilityFontN = bool.TryParse(value, out bool result) ? (bool?)result : null; OnPropertyChanged(); }
        }

        [XmlIgnore] public bool? UseUrduCompatibilityFontN { get; set; }
        [XmlAttribute] public string UseUrduCompatibilityFont
        {
            get { return UseUrduCompatibilityFontN.HasValue ? UseUrduCompatibilityFontN.Value.ToString() : null; }
            set { UseUrduCompatibilityFontN = bool.TryParse(value, out bool result) ? (bool?)result : null; OnPropertyChanged(); }
        }

        public void BuildProfiles()
        {
            inherited = new InteractorProfile();
            foreach (var p in profiles.Where(x => x.IsMember).Select(y => y.Profile))
            {
                if (p.LockOnTimeN.HasValue)
                    inherited.LockOnTime = p.LockOnTime;
                if (p.CompletionTimesN != null)
                    inherited.CompletionTimes = p.CompletionTimes;
                if (p.TimeRequiredToLockDownN.HasValue)
                    inherited.TimeRequiredToLockDown = p.TimeRequiredToLockDown;
                if (p.LockDownAttemptTimeoutN.HasValue)
                    inherited.LockDownAttemptTimeout = p.LockDownAttemptTimeout;

                if (p.OpacityN.HasValue)
                    inherited.Opacity = p.Opacity;
                if (p.BackgroundBrush != null)
                    inherited.BackgroundColor = p.BackgroundColor;
                if (p.ForegroundBrush != null)
                    inherited.ForegroundColor = p.ForegroundColor;
                if (p.BorderBrush != null)
                    inherited.BorderColor = p.BorderColor;
                if (p.BorderThicknessN.HasValue)
                    inherited.BorderThickness = p.BorderThickness;
                if (p.CornerRadiusN.HasValue)
                    inherited.CornerRadius = p.CornerRadius;

                if (p.KeyDisabledBackgroundBrush != null)
                    inherited.KeyDisabledBackgroundBrush = p.KeyDisabledBackgroundBrush;
                if (p.KeyDisabledForegroundBrush != null)
                    inherited.KeyDisabledForegroundBrush = p.KeyDisabledForegroundBrush;
                if (p.KeyDisabledOpacityN.HasValue)
                    inherited.KeyDisabledOpacityN = p.KeyDisabledOpacityN;

                if (p.KeyDownBackgroundBrush != null)
                    inherited.KeyDownBackgroundBrush = p.KeyDownBackgroundBrush;
                if (p.KeyDownForegroundBrush != null)
                    inherited.KeyDownForegroundBrush = p.KeyDownForegroundBrush;
                if (p.KeyDownOpacityN.HasValue)
                    inherited.KeyDownOpacityN = p.KeyDownOpacityN;

                if (!string.IsNullOrWhiteSpace(p.SharedSizeGroup))
                    inherited.SharedSizeGroup = p.SharedSizeGroup;
                if (!string.IsNullOrWhiteSpace(p.AutoScaleToOneKeyWidth))
                    inherited.AutoScaleToOneKeyWidth = p.AutoScaleToOneKeyWidth;
                if (!string.IsNullOrWhiteSpace(p.AutoScaleToOneKeyHeight))
                    inherited.AutoScaleToOneKeyHeight = p.AutoScaleToOneKeyHeight;
                if (!string.IsNullOrWhiteSpace(p.CompatibilityFont))
                    inherited.CompatibilityFont = p.CompatibilityFont;
            }

            Expressed.LockOnTimeN = LockOnTimeN ?? inherited.LockOnTimeN;
            Expressed.CompletionTimesN = CompletionTimesN ?? inherited.CompletionTimesN;
            Expressed.TimeRequiredToLockDownN = TimeRequiredToLockDownN ?? inherited.TimeRequiredToLockDownN;
            Expressed.LockDownAttemptTimeoutN = LockDownAttemptTimeoutN ?? inherited.LockDownAttemptTimeoutN;

            Expressed.OpacityN = OpacityN ?? inherited.OpacityN;
            Expressed.BackgroundBrush = BackgroundBrush ?? inherited.BackgroundBrush;
            Expressed.ForegroundBrush = ForegroundBrush ?? inherited.ForegroundBrush;
            Expressed.BorderBrush = BorderBrush ?? inherited.BorderBrush;
            Expressed.BorderThicknessN = BorderThicknessN ?? inherited.BorderThicknessN;
            Expressed.CornerRadiusN = CornerRadiusN ?? inherited.CornerRadiusN;

            Expressed.KeyDisabledBackgroundBrush = KeyDisabledBackgroundBrush ?? inherited.KeyDisabledBackgroundBrush ?? (Expressed.BackgroundBrush != null ? new SolidColorBrush(HlsColor.Fade(Expressed.BackgroundBrush.Color, .15)) : null);
            Expressed.KeyDisabledForegroundBrush = KeyDisabledForegroundBrush ?? inherited.KeyDisabledForegroundBrush ?? (Expressed.ForegroundBrush != null ? new SolidColorBrush(HlsColor.Fade(Expressed.ForegroundBrush.Color, .15)) : null);
            Expressed.KeyDisabledOpacityN = KeyDisabledOpacityN ?? inherited.KeyDisabledOpacityN ?? Expressed.OpacityN;

            Expressed.KeyDownBackgroundBrush = KeyDownBackgroundBrush ?? inherited.KeyDownBackgroundBrush ??(Expressed.BackgroundBrush != null ? new SolidColorBrush(HlsColor.Fade(Expressed.BackgroundBrush.Color, .15)) : null);
            Expressed.KeyDownForegroundBrush = KeyDownForegroundBrush ?? inherited.KeyDownForegroundBrush ?? (Expressed.ForegroundBrush != null ? new SolidColorBrush(HlsColor.Fade(Expressed.ForegroundBrush.Color, .15)) : null);
            Expressed.KeyDownOpacityN = KeyDownOpacityN ?? inherited.KeyDownOpacityN ?? Expressed.OpacityN;

            Expressed.SharedSizeGroup = SharedSizeGroup ?? inherited.SharedSizeGroup;
            Expressed.AutoScaleToOneKeyWidthN = AutoScaleToOneKeyWidthN ?? inherited.AutoScaleToOneKeyWidthN ?? true;
            Expressed.AutoScaleToOneKeyHeightN = AutoScaleToOneKeyHeightN ?? inherited.AutoScaleToOneKeyHeightN ?? true;
            Expressed.UsePersianCompatibilityFontN = UsePersianCompatibilityFontN ?? inherited.UsePersianCompatibilityFontN ?? false;
            Expressed.UseUnicodeCompatibilityFontN = UseUnicodeCompatibilityFontN ?? inherited.UseUnicodeCompatibilityFontN ?? false;
            Expressed.UseUrduCompatibilityFontN = UseUrduCompatibilityFontN ?? inherited.UseUrduCompatibilityFontN ?? false;
        }
        //Legacy elements

        [XmlElement("Row")] public string LegacyRow { get; set; }
        [XmlElement("Col")] public string LegacyCol { get; set; }
        [XmlElement("Width")] public string LegacyWidth { get; set; }
        [XmlElement("Height")] public string LegacyHeight { get; set; }
        [XmlElement("Label")] public string LegacyLabel { get; set; }
        [XmlElement("ShiftUpLabel")] public string LegacyShiftUpLabel { get; set; }
        [XmlElement("ShiftDownLabel")] public string LegacyShiftDownLabel { get; set; }
        [XmlElement("Symbol")] public string LegacySymbol { get; set; }
        [XmlElement("DestinationKeyboard")] public string LegacyDestinationKeyboard { get; set; }
        [XmlElement("ReturnToThisKeyboard")] public string LegacyReturnToThisKeyboard { get; set; }
        [XmlElement("Method")] public string LegacyMethod { get; set; }
        [XmlElement("Arguments")] public List<PluginArgument> LegacyArguments { get; set; }
        [XmlElement("SharedSizeGroup")] public string LegacySharedSizeGroup { get; set; }
        [XmlElement("AutoScaleToOneKeyWidth")] public string LegacyAutoScaleToOneKeyWidth { get; set; }
        [XmlElement("AutoScaleToOneKeyHeight")] public string LegacyAutoScaleToOneKeyHeight { get; set; }
        [XmlElement("UsePersianCompatibilityFont")] public string LegacyUsePersianCompatibilityFont { get; set; }
        [XmlElement("UseUnicodeCompatibilityFont")] public string LegacyUseUnicodeCompatibilityFont { get; set; }
        [XmlElement("UseUrduCompatibilityFont")] public string LegacyUseUrduCompatibilityFont { get; set; }
        [XmlElement("BackgroundColor")] public string LegacyBackgroundColor { get; set; }
    }

    public class ActionKey : DynamicKey { }
    public class ChangeKeyboardKey : DynamicKey { }
    public class PluginKey : DynamicKey { }
    public class TextKey : DynamicKey { }
    public class DynamicKey : Interactor { }
    public class DynamicOutputPanel : Interactor { }
    public class DynamicPopup : Interactor { }
    public class DynamicScratchpad : Interactor { }
    public class DynamicSuggestionRow : Interactor { }
    public class DynamicSuggestionCol : Interactor { }
    public class InteractorProfile : Interactor { }

}