// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using JuliusSweetland.OptiKey.Enums;
using JuliusSweetland.OptiKey.Extensions;
using JuliusSweetland.OptiKey.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace JuliusSweetland.OptiKey.Models
{
    public class Interactor : InteractorProfile, INotifyPropertyChanged
    {
        private PropertyChangedEventHandler descendantPropertyChanged;
        [XmlIgnore] public ObservableCollection<InteractorProfile> Descendants { get; private set; }

        new public event PropertyChangedEventHandler PropertyChanged;
        new protected void OnPropertyChanged([CallerMemberName] string name = null)
        {

            if (name == "Row" || name == "Col" || name.StartsWith("Gaze"))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Location"));
            }

            if (name == "Label")
            {
                ShiftDownLabel = Label != null && Label != Label.ToUpper() ? Label.ToUpper() : null;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        protected void DescendantPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            BuildProfiles();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        public Interactor()
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
                    InteractorTypes.SuggestionColumn.ToString();
            }
        }

        [XmlIgnore] public Visibility IsPopup { get { return this is DynamicPopup ? Visibility.Visible : Visibility.Collapsed; } }
        [XmlIgnore] public Visibility IsNotPopup { get { return this is DynamicPopup ? Visibility.Collapsed : Visibility.Visible; } }

        [XmlIgnore] public int RowN { get; set; } = -1;
        [XmlAttribute] public string Row { get { return this is DynamicPopup ? null : RowN.ToString(); } set { RowN = int.TryParse(value, out int result) ? result : -1; OnPropertyChanged(); } }

        [XmlIgnore] public int ColN { get; set; } = -1;
        [XmlAttribute] public string Col { get { return this is DynamicPopup ? null : ColN.ToString(); } set { ColN = int.TryParse(value, out int result) ? result : -1; OnPropertyChanged(); } }

        [XmlIgnore] public int WidthN { get; set; } = 1;
        [XmlAttribute] public string Width { get { return this is DynamicPopup ? null : WidthN.ToString(); } set { WidthN = int.TryParse(value, out int result) && result > 0 ? result : 1; OnPropertyChanged(); } }

        [XmlIgnore] public int HeightN { get; set; } = 1;
        [XmlAttribute] public string Height { get { return this is DynamicPopup ? null : HeightN.ToString(); } set { HeightN = int.TryParse(value, out int result) && result > 0 ? result : 1; OnPropertyChanged(); } }
        
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
        [XmlAttribute] public string Label { get { return label; }
            set {
                foreach (var c in commands.Where(x => x.Value == label))
                    c.Value = value;
                label = value;
                OnPropertyChanged(); } }

        [XmlAttribute] public string ShiftDownLabel { get { return Key.ShiftDownText; } set { Key.ShiftDownText = value; OnPropertyChanged(); } }

        private string symbol;
        [XmlAttribute] public string Symbol { get { return symbol; } set { symbol = value; OnPropertyChanged(); } }

        [XmlIgnore] public string Location { get { return this is DynamicPopup
                    ? "(" + GazeRegion + ")"
                    : "R:" + RowN.ToString() + " C:" + ColN.ToString(); } }

        [XmlElement("KeyGroup")] public List<KeyGroup> ProfileNames { get; set; }

        private ObservableCollection<KeyCommand> commands;
        [XmlElement("Action", typeof(ActionCommand))]
        [XmlElement("ChangeKeyboard", typeof(ChangeKeyboardCommand))]
        [XmlElement("KeyDown", typeof(KeyDownCommand))]
        [XmlElement("KeyUp", typeof(KeyUpCommand))]
        [XmlElement("KeyToggle", typeof(KeyTogglCommand))]
        [XmlElement("Loop", typeof(LoopCommand))]
        [XmlElement("Plugin", typeof(PluginCommand))]
        [XmlElement("MoveWindow", typeof(MoveWindowCommand))]
        [XmlElement("Text", typeof(TextCommand))]
        [XmlElement("Wait", typeof(WaitCommand))]
        public ObservableCollection<KeyCommand> Commands { get { return commands; } set { commands = value; } }

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
        [XmlElement("Arguments")] public List<DynamicArgument> LegacyArguments { get; set; }
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

}