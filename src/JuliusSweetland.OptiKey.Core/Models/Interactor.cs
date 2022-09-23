// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using JuliusSweetland.OptiKey.Enums;
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
            if (name == "Width")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BorderWidth"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Margin"));
            }
            if (name == "Height")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BorderHeight"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Margin"));
            }

            if (name == "Row" || name == "Col" || name == "Left" || name == "Top")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Location"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Margin"));
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
            InheritenceProfile = new InteractorProfile();
            FinalProfile = new InteractorProfile();
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
        [XmlIgnore] public Output Output { get; set; } = new Output();
        [XmlIgnore] public ScratchpadUserControl Scratchpad { get; set; } = new ScratchpadUserControl();
        [XmlIgnore] public SuggestionCol SuggestionCol { get; set; } = new SuggestionCol();
        [XmlIgnore] public SuggestionRow SuggestionRow { get; set; } = new SuggestionRow();

        [XmlIgnore]
        public string TypeAsString
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

        private double row;
        [XmlIgnore] public double Row { get { return row; } set { row = value; OnPropertyChanged(); } }
        [XmlAttribute("Row")] public string RowString { get { return this is DynamicPopup ? null : row.ToString(); } set { row = double.TryParse(value, out double result) ? result : 0; } }

        private double col;
        [XmlIgnore] public double Col { get { return col; } set { col = value; OnPropertyChanged(); } }
        [XmlAttribute("Col")] public string ColString { get { return this is DynamicPopup ? null : col.ToString(); } set { col = double.TryParse(value, out double result) ? result : 0; } }

        private double left;
        [XmlIgnore] public double Left { get { return left; } set { left = value; OnPropertyChanged(); } }
        [XmlAttribute("Left")] public string LeftString { get { return this is DynamicPopup ? left.ToString() : null; } set { left = double.TryParse(value, out double result) ? result : 0; } }

        private double top;
        [XmlIgnore] public double Top { get { return top; } set { top = value; OnPropertyChanged(); } }
        [XmlAttribute("Top")] public string TopString { get { return this is DynamicPopup ? top.ToString() : null; } set { top = double.TryParse(value, out double result) ? result : 0; } }

        [XmlAttribute] public double Width { get { return Key.WidthSpan; } set { Key.WidthSpan = value; OnPropertyChanged(); } }

        [XmlAttribute] public double Height { get { return Key.HeightSpan; } set { Key.HeightSpan = value; OnPropertyChanged(); } }

        [XmlElement] public string Label { get { return Key.ShiftUpText; }
            set {
                foreach (var c in commands.Where(x => x.Value == Key.ShiftUpText))
                    c.Value = value;
                Key.ShiftUpText = value;
                OnPropertyChanged(); } }

        [XmlElement] public string ShiftDownLabel { get { return Key.ShiftDownText; } set { Key.ShiftDownText = value; OnPropertyChanged(); } }

        private string symbol;
        [XmlElement] public string Symbol { get { return symbol; } set { symbol = value; OnPropertyChanged();
                Geometry geometry;
                try
                {
                    geometry = (Geometry)new ResourceDictionary() { Source = new Uri("/OptiKey;component/Resources/Icons/KeySymbols.xaml", UriKind.RelativeOrAbsolute) }[symbol];
                }
                catch { geometry = Geometry.Empty; }
                Key.SymbolGeometry = geometry;
            }
        }
        
        [XmlAttribute] public string SharedSizeGroup { get { return Key.SharedSizeGroup; } set { Key.SharedSizeGroup = value; OnPropertyChanged(); } }

        [XmlIgnore] public double BorderWidth { get { return Width * Layout.Width / Layout.Columns; } }
        [XmlIgnore] public double BorderHeight { get { return Height * Layout.Height / Layout.Rows; } }
        [XmlIgnore] public string Location { get { return this is DynamicPopup
                    ? "X:" + Left.ToString() + " Y:" + Top.ToString()
                    : "R:" + Row.ToString() + " C:" + Col.ToString(); } }
        [XmlIgnore] public Thickness Margin
        { get { return this is DynamicPopup
                    ? new Thickness(Layout.ScreenLeft + left * Layout.Width / Layout.Columns, Layout.ScreenTop + top * Layout.Height / Layout.Rows, 0, 0)
                    : new Thickness(Layout.Left + col * Layout.Width / Layout.Columns, Layout.Top + row * Layout.Height / Layout.Rows, 0, 0); } }

        [XmlIgnore] public ILayout Layout { get; set; }

        [XmlElement("KeyGroup")] public List<string> ProfileNames
        { get { return Profiles.Where(x => x.IsMember && x.Profile.Name != "All").Select(y => y.Profile.Name).ToList(); } }

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

        private KeyCommands selectedCommandType;
        [XmlIgnore]
        public string SelectedCommandType
        {
            get { return selectedCommandType.ToString(); }
            set { selectedCommandType = Enum.TryParse(value, out KeyCommands kc) ? kc : KeyCommands.Text; }
        }

        public void AddCommand()
        {
            switch (selectedCommandType)
            {
                case KeyCommands.Action:
                    Commands.Add(new ActionCommand());
                    break;
                case KeyCommands.ChangeKeyboard:
                    Commands.Add(new ChangeKeyboardCommand());
                    break;
                case KeyCommands.KeyDown:
                    Commands.Add(new KeyDownCommand() { Value = Label });
                    break;
                case KeyCommands.KeyToggle:
                    Commands.Add(new KeyTogglCommand() { Value = Label });
                    break;
                case KeyCommands.KeyUp:
                    Commands.Add(new KeyUpCommand() { Value = Label });
                    break;
                case KeyCommands.Loop:
                    Commands.Add(new LoopCommand());
                    break;
                case KeyCommands.MoveWindow:
                    Commands.Add(new MoveWindowCommand());
                    break;
                case KeyCommands.Plugin:
                    Commands.Add(new PluginCommand());
                    break;
                case KeyCommands.Text:
                    Commands.Add(new TextCommand() { Value = Label });
                    break;
                case KeyCommands.Wait:
                    Commands.Add(new WaitCommand());
                    break;
            }
        }
    }

    public class DynamicKey : Interactor { }
    public class DynamicOutputPanel : Interactor { }
    public class DynamicPopup : DynamicKey { }
    public class DynamicScratchpad : Interactor { }
    public class DynamicSuggestionRow : Interactor { }
    public class DynamicSuggestionCol : Interactor { }

}