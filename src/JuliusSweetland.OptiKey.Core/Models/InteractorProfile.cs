// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved

using JuliusSweetland.OptiKey.Extensions;
using JuliusSweetland.OptiKey.UI.ValueConverters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace JuliusSweetland.OptiKey.Models
{
    public class InteractorProfile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private ObservableCollection<InteractorProfileMap> profiles;
        [XmlIgnore] public ObservableCollection<InteractorProfileMap> Profiles
        { get { return profiles; } set { profiles = value; } }

        private InteractorProfile inherited;
        [XmlIgnore] public InteractorProfile Inherited
        { get { return inherited; } set { inherited = value; } }

        [XmlIgnore] public InteractorProfile Expressed {get;set; }

        private string name;
        [XmlAttribute] public string Name
        {
            get { return name; }
            set { name = name == "All" ? "All" : value; OnPropertyChanged(); }
        }

        [XmlIgnore] public SolidColorBrush BackgroundBrush { get; set; }
        private string backgroundColor;
        [XmlAttribute] public string BackgroundColor
        {
            get { return backgroundColor; }
            set {
                backgroundColor = string.IsNullOrWhiteSpace(value) ? null : value;
                BackgroundBrush = ValidColor(value, out SolidColorBrush brush) ? brush : null;
                OnPropertyChanged(); }
        }

        [XmlIgnore] public SolidColorBrush BorderBrush { get; set; }
        private string borderColor;
        [XmlAttribute] public string BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = string.IsNullOrWhiteSpace(value) ? null : value;
                BorderBrush = ValidColor(value, out SolidColorBrush brush) ? brush : null;
                OnPropertyChanged();
            }
        }

        [XmlIgnore] public int? BorderThicknessN;
        private string borderThickness;
        [XmlAttribute] public string BorderThickness
        {
            get { return borderThickness; }
            set { borderThickness = value;
                BorderThicknessN = int.TryParse(value, out int result) ? (int?)result : null;
                OnPropertyChanged(); }
        }

        [XmlIgnore] public int? CornerRadiusN;
        private string cornerRadius;
        [XmlAttribute]
        public string CornerRadius
        {
            get { return cornerRadius; }
            set
            {
                cornerRadius = value;
                CornerRadiusN = int.TryParse(value, out int result) ? (int?)result : null;
                OnPropertyChanged();
            }
        }

        [XmlIgnore] public SolidColorBrush ForegroundBrush { get; set; }
        private string foregroundColor;
        [XmlAttribute] public string ForegroundColor
        {
            get { return foregroundColor; }
            set
            {
                foregroundColor = string.IsNullOrWhiteSpace(value) ? null : value;
                ForegroundBrush = ValidColor(value, out SolidColorBrush brush) ? brush : null;
                OnPropertyChanged();
            }
        }

        [XmlIgnore] public double? OpacityN { get; set; }
        private string opacity;
        [XmlAttribute] public string Opacity
        {
            get { return opacity; }
            set {
                opacity = string.IsNullOrWhiteSpace(value) ? null : value;
                OpacityN = double.TryParse(value, out double result) ? (double?)result : null;
            }
        }

        [XmlIgnore] public TimeSpan? LockOnTimeN { get; set; }
        private string lockOnTime;
        [XmlAttribute] public string LockOnTime
        {
            get { return lockOnTime; }
            set {
                lockOnTime = double.TryParse(value, out double result) ? value : null;
                LockOnTimeN = lockOnTime != null
                    ? (TimeSpan?)TimeSpan.FromMilliseconds(result.Clamp(0, 1000)) : null;
                OnPropertyChanged();
            }
        }

        [XmlIgnore] public List<string> CompletionTimesN { get; set; }
        private string completionTimes;
        [XmlAttribute] public string CompletionTimes
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
        [XmlAttribute] public string TimeRequiredToLockDown
        {
            get { return timeRequiredToLockDown; }
            set { timeRequiredToLockDown = double.TryParse(value, out double result) ? value : null;
                TimeRequiredToLockDownN = timeRequiredToLockDown != null
                    ? (TimeSpan?)TimeSpan.FromMilliseconds(result.Clamp(0, 10000)) : null;
                OnPropertyChanged();
            }
        }

        [XmlIgnore] public TimeSpan? LockDownAttemptTimeoutN { get; set; }
        private string lockDownAttemptTimeout;
        [XmlAttribute] public string LockDownAttemptTimeout
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

        [XmlIgnore] public SolidColorBrush KeyDisabledForegroundBrush { get; set; }
        private string keyDisabledForeground;
        [XmlAttribute] public string KeyDisabledForeground
        {
            get { return keyDisabledForeground; }
            set
            {
                keyDisabledForeground = string.IsNullOrWhiteSpace(value) ? null : value;
                KeyDisabledForegroundBrush = ValidColor(value, out SolidColorBrush brush) ? brush : null;
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
                KeyDownForegroundBrush = ValidColor(value, out SolidColorBrush brush) ? brush : null;
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
                KeyDisabledBackgroundBrush = ValidColor(value, out SolidColorBrush brush) ? brush : null;
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
                KeyDownBackgroundBrush = ValidColor(value, out SolidColorBrush brush) ? brush : null;
            }
        }

        [XmlIgnore] public double? KeyDisabledOpacityN { get; set; }
        private string keyDisabledOpacity;
        [XmlAttribute] public string KeyDisabledOpacity
        {
            get { return keyDisabledOpacity; }
            set
            {
                keyDisabledOpacity = string.IsNullOrWhiteSpace(value) ? null : value;
                KeyDisabledOpacityN = double.TryParse(value, out double result) ? (double?)result : null;
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

        [XmlIgnore] public bool? AutoScaleToOneKeyWidth;
        [XmlAttribute("AutoScaleToOneKeyWidth")]
        public string AutoScaleToOneKeyWidthString
        {
            get { return AutoScaleToOneKeyWidth.HasValue ? AutoScaleToOneKeyWidth.Value.ToString() : null; }
            set { AutoScaleToOneKeyWidth = bool.TryParse(value, out bool result) ? (bool?)result : null; }
        }
        
        [XmlIgnore] public bool? AutoScaleToOneKeyHeight;
        [XmlAttribute("AutoScaleToOneKeyHeight")]
        public string AutoScaleToOneKeyHeightString
        {
            get { return AutoScaleToOneKeyHeight.HasValue ? AutoScaleToOneKeyHeight.Value.ToString() : null; }
            set { AutoScaleToOneKeyHeight = bool.TryParse(value, out bool result) ? (bool?)result : null; }
        }

        [XmlIgnore] public bool? UsePersianCompatibilityFontN { get; set; }
        [XmlAttribute] public string UsePersianCompatibilityFont
        {
            get { return UsePersianCompatibilityFontN.HasValue ? UsePersianCompatibilityFontN.Value.ToString() : null; }
            set { UsePersianCompatibilityFontN = bool.TryParse(value, out bool result) ? (bool?)result : null; }
        }

        [XmlIgnore] public bool? UseUnicodeCompatibilityFontN { get; set; }
        [XmlAttribute] public string UseUnicodeCompatibilityFont
        {
            get { return UseUnicodeCompatibilityFontN.HasValue ? UseUnicodeCompatibilityFontN.Value.ToString() : null; }
            set { UseUnicodeCompatibilityFontN = bool.TryParse(value, out bool result) ? (bool?)result : null; }
        }

        [XmlIgnore] public bool? UseUrduCompatibilityFontN { get; set; }
        [XmlAttribute] public string UseUrduCompatibilityFont
        {
            get { return UseUrduCompatibilityFontN.HasValue ? UseUrduCompatibilityFontN.Value.ToString() : null; }
            set { UseUrduCompatibilityFontN = bool.TryParse(value, out bool result) ? (bool?)result : null; }
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

                if (!string.IsNullOrWhiteSpace(p.SharedSizeGroup))
                    inherited.SharedSizeGroup = p.SharedSizeGroup;
                if (!string.IsNullOrWhiteSpace(p.AutoScaleToOneKeyWidthString))
                    inherited.AutoScaleToOneKeyWidthString = p.AutoScaleToOneKeyWidthString;
                if (!string.IsNullOrWhiteSpace(p.AutoScaleToOneKeyHeightString))
                    inherited.AutoScaleToOneKeyHeightString = p.AutoScaleToOneKeyHeightString;
                if (!string.IsNullOrEmpty(p.UseUrduCompatibilityFont))
                    inherited.UsePersianCompatibilityFont = p.UsePersianCompatibilityFont;
                if (!string.IsNullOrEmpty(p.UseUnicodeCompatibilityFont))
                    inherited.UseUnicodeCompatibilityFont = p.UseUnicodeCompatibilityFont;
                if (!string.IsNullOrEmpty(p.UseUrduCompatibilityFont))
                    inherited.UseUrduCompatibilityFont = p.UseUrduCompatibilityFont;

                if (p.BackgroundBrush != null)
                    inherited.BackgroundColor = p.BackgroundColor;
                if (p.ForegroundBrush != null)
                    inherited.ForegroundColor = p.ForegroundColor;
                if (p.BorderBrush != null)
                    inherited.BorderColor = p.BorderColor;
                if (p.OpacityN.HasValue)
                    inherited.Opacity = p.Opacity;

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
                
                if (p.BorderThicknessN.HasValue)
                    inherited.BorderThickness = p.BorderThickness;
                if (p.CornerRadiusN.HasValue)
                    inherited.CornerRadius = p.CornerRadius;
            }

            Expressed.LockOnTimeN = LockOnTimeN ?? inherited.LockOnTimeN;
            Expressed.CompletionTimesN = CompletionTimesN ?? inherited.CompletionTimesN;
            Expressed.TimeRequiredToLockDownN = TimeRequiredToLockDownN ?? inherited.TimeRequiredToLockDownN;
            Expressed.LockDownAttemptTimeoutN = LockDownAttemptTimeoutN ?? inherited.LockDownAttemptTimeoutN;

            Expressed.SharedSizeGroup = SharedSizeGroup ?? inherited.SharedSizeGroup;

            Expressed.AutoScaleToOneKeyWidth = AutoScaleToOneKeyWidth.HasValue
                ? AutoScaleToOneKeyWidth : inherited.AutoScaleToOneKeyWidth.HasValue
                ? inherited.AutoScaleToOneKeyWidth : true;

            Expressed.AutoScaleToOneKeyHeight = AutoScaleToOneKeyHeight.HasValue
                ? AutoScaleToOneKeyHeight : inherited.AutoScaleToOneKeyHeight.HasValue
                ? inherited.AutoScaleToOneKeyHeight : true;

            Expressed.UsePersianCompatibilityFontN = UsePersianCompatibilityFontN.HasValue
                ? UsePersianCompatibilityFontN.Value : inherited.UsePersianCompatibilityFontN.HasValue
                ? inherited.UsePersianCompatibilityFontN.Value : false;

            Expressed.UseUnicodeCompatibilityFontN = UseUnicodeCompatibilityFontN.HasValue
                ? UseUnicodeCompatibilityFontN.Value : inherited.UseUnicodeCompatibilityFontN.HasValue
                ? inherited.UseUnicodeCompatibilityFontN.Value : false;

            Expressed.UseUrduCompatibilityFontN = UseUrduCompatibilityFontN.HasValue
                ? UseUrduCompatibilityFontN.Value : inherited.UseUrduCompatibilityFontN.HasValue
                ? inherited.UseUrduCompatibilityFontN.Value : false;

            Expressed.BackgroundBrush = BackgroundBrush ?? inherited.BackgroundBrush;
            Expressed.ForegroundBrush = ForegroundBrush ?? inherited.ForegroundBrush;
            Expressed.BorderBrush = BorderBrush ?? inherited.BorderBrush;
            Expressed.OpacityN = OpacityN ?? inherited.OpacityN;

            Expressed.KeyDisabledBackgroundBrush = KeyDisabledBackgroundBrush ?? (inherited.KeyDisabledBackgroundBrush != null ? Inherited.KeyDisabledBackgroundBrush : Expressed.BackgroundBrush != null ? new SolidColorBrush(HlsColor.Fade(Expressed.BackgroundBrush.Color, .15)) : null);

            Expressed.KeyDownBackgroundBrush = KeyDownBackgroundBrush ?? (inherited.KeyDownBackgroundBrush != null ? Inherited.KeyDownBackgroundBrush : Expressed.BackgroundBrush != null ? new SolidColorBrush(HlsColor.Fade(Expressed.BackgroundBrush.Color, .15)) : null);

            Expressed.KeyDisabledForegroundBrush = KeyDisabledForegroundBrush ?? (inherited.KeyDisabledForegroundBrush != null ? Inherited.KeyDisabledForegroundBrush : Expressed.ForegroundBrush != null ? new SolidColorBrush(HlsColor.Fade(Expressed.ForegroundBrush.Color, .15)) : null);

            Expressed.KeyDownForegroundBrush = KeyDownForegroundBrush ?? (inherited.KeyDownForegroundBrush != null ? Inherited.KeyDownForegroundBrush : Expressed.ForegroundBrush != null ? new SolidColorBrush(HlsColor.Fade(Expressed.ForegroundBrush.Color, .15)) : null);

            Expressed.KeyDisabledOpacityN = KeyDisabledOpacityN.HasValue ? KeyDisabledOpacityN : inherited.KeyDisabledOpacityN.HasValue ? Inherited.KeyDisabledOpacityN : Expressed.OpacityN;

            Expressed.KeyDownOpacityN = KeyDownOpacityN.HasValue ? KeyDownOpacityN : inherited.KeyDownOpacityN.HasValue ? Inherited.KeyDownOpacityN : Expressed.OpacityN;

            Expressed.BorderThicknessN = BorderThicknessN ?? inherited.BorderThicknessN;
            Expressed.CornerRadiusN = CornerRadiusN ?? inherited.CornerRadiusN;
        }

        private bool ValidColor(string color, out SolidColorBrush colorBrush)
        {
            if (!string.IsNullOrEmpty(color)
                && (Regex.IsMatch(color, "^(#[0-9A-Fa-f]{3})$|^(#[0-9A-Fa-f]{6})$")
                || System.Drawing.Color.FromName(color).IsKnownColor))
            {
                colorBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(color);
                return true;
            }
            colorBrush = null;
            return false;
        }

        //Legacy elements
        [XmlElement("SharedSizeGroup")] public string LegacySharedSizeGroup { get; set; }
        [XmlElement("AutoScaleToOneKeyWidth")] public string LegacyAutoScaleToOneKeyWidth { get; set; }
        [XmlElement("AutoScaleToOneKeyHeight")] public string LegacyAutoScaleToOneKeyHeight { get; set; }
        [XmlElement("UsePersianCompatibilityFont")] public string LegacyUsePersianCompatibilityFont { get; set; }
        [XmlElement("UseUnicodeCompatibilityFont")] public string LegacyUseUnicodeCompatibilityFont { get; set; }
        [XmlElement("UseUrduCompatibilityFont")] public string LegacyUseUrduCompatibilityFont { get; set; }
        [XmlElement("BackgroundColor")] public string LegacyBackgroundColor { get; set; }
    }
}