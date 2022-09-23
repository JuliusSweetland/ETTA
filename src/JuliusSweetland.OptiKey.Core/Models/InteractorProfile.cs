// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved

using JuliusSweetland.OptiKey.Extensions;
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
            if (inheritenceProfile != null && finalProfile != null)
                BuildProfiles();
        }

        private ObservableCollection<InteractorProfileMap> profiles;
        [XmlIgnore] public ObservableCollection<InteractorProfileMap> Profiles { get { return profiles; } set { profiles = value; } }

        private InteractorProfile inheritenceProfile;
        [XmlIgnore] public InteractorProfile InheritenceProfile { get { return inheritenceProfile; } set { inheritenceProfile = value; } }

        private InteractorProfile finalProfile;
        [XmlIgnore] public InteractorProfile FinalProfile { get { return finalProfile; } set { finalProfile = value; } }

        private string name;
        [XmlAttribute] public string Name
        {
            get { return name; }
            set { name = name == "All" ? "All" : value; OnPropertyChanged(); }
        }

        private SolidColorBrush backgroundBrush;
        [XmlIgnore] public SolidColorBrush BackgroundBrush
        {
            get { return backgroundBrush; }
            set { backgroundBrush = value; OnPropertyChanged(); }
        }

        private string backgroundColor;
        [XmlAttribute] public string BackgroundColor
        {
            get { return backgroundColor; }
            set { 
                var brush = new SolidColorBrush();
                backgroundColor = string.IsNullOrWhiteSpace(value) ? null : value;
                backgroundBrush = ValidColor(value, out brush) ? brush : null;
                OnPropertyChanged(); }
        }

        private SolidColorBrush borderBrush;
        [XmlIgnore] public SolidColorBrush BorderBrush
        {
            get { return borderBrush; }
            set { borderBrush = value; OnPropertyChanged(); }
        }

        private string borderColor;
        [XmlAttribute] public string BorderColor
        {
            get { return borderColor; }
            set
            {
                var brush = new SolidColorBrush();
                borderColor = string.IsNullOrWhiteSpace(value) ? null : value;
                borderBrush = ValidColor(value, out brush) ? brush : null;
                OnPropertyChanged();
            }
        }

        private Thickness borderThickness;
        [XmlIgnore] public Thickness BorderThickness
        {
            get { return borderThickness; }
            set { borderThickness = value; OnPropertyChanged(); }
        }

        private SolidColorBrush foregroundBrush;
        [XmlIgnore] public SolidColorBrush ForegroundBrush
        {
            get { return foregroundBrush; }
            set { foregroundBrush = value; OnPropertyChanged(); }
        }

        private string foregroundColor;
        [XmlAttribute] public string ForegroundColor
        {
            get { return foregroundColor; }
            set
            {
                var brush = new SolidColorBrush();
                foregroundColor = string.IsNullOrWhiteSpace(value) ? null : value;
                foregroundBrush = ValidColor(value, out brush) ? brush : null;
                OnPropertyChanged();
            }
        }

        [XmlAttribute("Opacity")] public string XmlOpacity
        {
            get { return opacity?.ToString(); }
            set { opacity = double.TryParse(value, out double newValue) ? (double?)newValue : null; }
        }
        private double? opacity;
        [XmlIgnore] public double? Opacity
        {
            get { return opacity; }
            set
            {
                try { opacity = ((double)value).Clamp(0, 1); }
                catch { opacity = null; }
                OnPropertyChanged();
            }
        }

        [XmlAttribute("LockOnTime")] public string XmlLockOnTime
        {
            get { return lockOnTime?.ToString(); }
            set { lockOnTime = double.TryParse(value, out double newValue) ? (double?)newValue : null; }
        }
        private double? lockOnTime;
        [XmlIgnore] public double? LockOnTime
        {
            get { return lockOnTime; }
            set {
                try { lockOnTime = ((double)value).Clamp(0, 1000); }
                catch { lockOnTime = null; }
                OnPropertyChanged();
            }
        }

        private string completionTimes;
        [XmlAttribute] public string CompletionTimes
        {
            get { return completionTimes; }
            set
            {
                completionTimes = value;
                OnPropertyChanged();
            }
        }

        [XmlAttribute("TimeRequiredToLockDown")]
        public string XmlTimeRequiredToLockDown
        {
            get { return timeRequiredToLockDown?.ToString(); }
            set { timeRequiredToLockDown = double.TryParse(value, out double newValue) ? (double?)newValue : null; }
        }
        private double? timeRequiredToLockDown;
        [XmlIgnore] public double? TimeRequiredToLockDown
        {
            get { return timeRequiredToLockDown; }
            set
            {
                try { timeRequiredToLockDown = ((double)value).Clamp(0, 10000); }
                catch { timeRequiredToLockDown = null; }
                OnPropertyChanged();
            }
        }

        public void BuildProfiles()
        {
            inheritenceProfile = new InteractorProfile();
            var brush = new SolidColorBrush();
            foreach (var p in profiles.Where(x => x.IsMember).Select(y => y.Profile))
            {
                if (p.backgroundBrush != null)
                {
                    inheritenceProfile.BackgroundBrush = p.BackgroundBrush;
                    inheritenceProfile.BackgroundColor = p.BackgroundColor;
                }
                if (p.foregroundBrush != null)
                {
                    inheritenceProfile.ForegroundBrush = p.ForegroundBrush;
                    inheritenceProfile.ForegroundColor = p.ForegroundColor;
                }
                if (p.borderBrush != null)
                {
                    inheritenceProfile.BorderBrush = p.BorderBrush;
                    inheritenceProfile.BorderColor = p.BorderColor;
                }
                if (p.Opacity != null)
                    inheritenceProfile.Opacity = p.Opacity;
            }

            FinalProfile.BackgroundBrush = BackgroundBrush ?? inheritenceProfile.BackgroundBrush ?? Brushes.Black;
            FinalProfile.ForegroundBrush = ForegroundBrush ?? inheritenceProfile.ForegroundBrush ?? Brushes.White;
            FinalProfile.BorderBrush = BorderBrush ?? inheritenceProfile.BorderBrush ?? Brushes.Gray;
            FinalProfile.Opacity = Opacity ?? inheritenceProfile.Opacity ?? 1;
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
    }
}