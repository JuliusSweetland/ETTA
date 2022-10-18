// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using JuliusSweetland.OptiKey.Enums;
using JuliusSweetland.OptiKey.Extensions;
using JuliusSweetland.OptiKey.Models;
using JuliusSweetland.OptiKey.Properties;
using JuliusSweetland.OptiKey.Static;
using JuliusSweetland.OptiKey.UI.Controls;
using JuliusSweetland.OptiKey.UI.Utilities;
using JuliusSweetland.OptiKey.UI.ViewModels.Keyboards;
using log4net;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;
using Key = JuliusSweetland.OptiKey.UI.Controls.Key;

namespace JuliusSweetland.OptiKey.UI.ViewModels.Management
{
    public class LayoutViewModel : INotifyPropertyChanged
    {
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public LayoutViewModel()
        {
            OpenFileCommand = new DelegateCommand(OpenFile);
            SaveFileCommand = new DelegateCommand(SaveFile);
            AddLayoutCommand = new DelegateCommand(AddLayout);
            AddBuiltInCommand = new DelegateCommand(AddBuiltIn);
            AddProfileCommand = new DelegateCommand(AddProfile);
            DeleteProfileCommand = new DelegateCommand(DeleteProfile);
            AddInteractorCommand = new DelegateCommand(AddInteractor);
            CloneInteractorCommand = new DelegateCommand(CloneInteractor);
            DeleteInteractorCommand = new DelegateCommand(DeleteInteractor);
            InteractorCommand = new DelegateCommand<object>(SelectInteractor);

            XmlKeyboards = new ObservableCollection<XmlKeyboard>();
            Profiles = new ObservableCollection<InteractorProfile>();
            Interactors = new ObservableCollection<Interactor>();
            descendantPropertyChanged = new PropertyChangedEventHandler(DescendantPropertyChanged);

            XmlKeyboards.CollectionChanged += delegate (object sender, NotifyCollectionChangedEventArgs e)
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
            Interactors.CollectionChanged += delegate (object sender, NotifyCollectionChangedEventArgs e)
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
            Load();
            AddLayout();
        }

        #region Properties

        public static List<string> KeyboardList = new List<string>()
        {
            "Alpha1", "Alpha2", "Alpha3",
            "ConversationAlpha1", "ConversationAlpha2", "ConversationAlpha3",
            "ConversationNumericAndSymbols", "Currencies1", "Currencies2",
            "Diacritics1", "Diacritics2", "Diacritics3", "Language", "Menu", "Mouse",
            "NumericAndSymbols1", "NumericAndSymbols2", "NumericAndSymbols3", "PhysicalKeys",
            "SimplifiedAlpha", "SimplifiedConversationAlpha", "SizeAndPosition", "WebBrowsing"
        };

        public static List<string> WindowStates = new List<string>() { { "" }, { Enums.WindowStates.Docked.ToString() }, { Enums.WindowStates.Floating.ToString() }, { Enums.WindowStates.Maximised.ToString() } };

        public static List<string> PositionList = Enum.GetNames(typeof(MoveToDirections)).ToList();
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private PropertyChangedEventHandler descendantPropertyChanged;
        protected void DescendantPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CreateViewbox();
        }

        public DelegateCommand OpenFileCommand { get; private set; }
        public DelegateCommand SaveFileCommand { get; private set; }
        public DelegateCommand AddLayoutCommand { get; private set; }
        public DelegateCommand AddBuiltInCommand { get; private set; }
        public DelegateCommand AddProfileCommand { get; private set; }
        public DelegateCommand DeleteProfileCommand { get; private set; }
        public DelegateCommand AddInteractorCommand { get; private set; }
        public DelegateCommand CloneInteractorCommand { get; private set; }
        public DelegateCommand DeleteInteractorCommand { get; private set; }
        public ICommand InteractorCommand { get; private set; }

        public string KeyboardFile { get; set; }
        
        public ObservableCollection<XmlKeyboard> XmlKeyboards { get; private set; }
        
        private XmlKeyboard xmlKeyboard = new XmlKeyboard();
        public XmlKeyboard XmlKeyboard
        {
            get { return xmlKeyboard; }
            set
            {
                xmlKeyboard = value;
                XmlKeyboards.Clear();
                XmlKeyboards.Add(value);
                Profiles.Clear();
                Profiles.AddRange(XmlKeyboard.Profiles);
                Profile = Profiles[0];
                Interactors.Clear();
                Interactors.AddRange(XmlKeyboard.Interactors);
                CreateViewbox();
                OnPropertyChanged();
            }
        }

        private ObservableCollection<InteractorProfile> profiles;
        public ObservableCollection<InteractorProfile> Profiles
        { get { return profiles; } set { profiles = value; OnPropertyChanged(); } }
        
        private InteractorProfile profile = new InteractorProfile();
        public InteractorProfile Profile
        { get { return profile; } set { profile = value; OnPropertyChanged(); } }

        private ObservableCollection<Interactor> interactors;
        public ObservableCollection<Interactor> Interactors
        { get { return interactors; } set { interactors = value; OnPropertyChanged(); } }

        private Interactor interactor;
        public Interactor Interactor
        {
            get { return interactor; }
            set
            {
                if (interactor != null && interactor.Key != null)
                {
                    interactor.Key.IsCurrent = false;
                    if (interactor is DynamicPopup)
                    {
                        canvas.Children.Remove(gazeRegion);
                        interactor.Key.Margin = new Thickness(ScreenLeft + interactor.GazeLeft * ScreenWidth, ScreenTop + interactor.GazeTop * ScreenHeight, 0, 0);
                    }
                }
                interactor = value;
                if (interactor != null)
                {
                    if (interactor.Key != null)
                    {
                        interactor.Key.IsCurrent = true;
                        if (interactor is DynamicPopup)
                        {
                            gazeRegion.Margin = interactor.Key.Margin;
                            gazeRegion.Width = interactor.Key.Width;
                            gazeRegion.Height = interactor.Key.Height;
                            canvas.Children.Add(gazeRegion);
                            interactor.Key.Margin = new Thickness((ScreenLeft + interactor.GazeLeft * ScreenWidth).Clamp(ScreenLeft, ScreenLeft + ScreenWidth - interactor.GazeWidth * ScreenWidth), (ScreenTop + interactor.GazeTop * ScreenHeight).Clamp(ScreenTop, ScreenTop + ScreenHeight - interactor.GazeHeight * ScreenHeight), 0, 0);
                        }
                    }
                    SelectedInteractorType = interactor.TypeAsString;
                }
                OnPropertyChanged();
            }
        }

        private InteractorTypes newType;
        private InteractorTypes selectedInteractorType;
        public string SelectedInteractorType
        {
            get { return selectedInteractorType.ToString(); }
            set
            {
                selectedInteractorType = Enum.TryParse(value, out InteractorTypes type) ? type : InteractorTypes.Key;
                if (Interactor != null && Interactor.TypeAsString != value)
                    ReplaceInteractor();
            }
        }

        private string keyboardName;
        public string KeyboardName { get { return keyboardName; } set { keyboardName = value; OnPropertyChanged(); } }

        private Viewbox viewbox;
        public Viewbox Viewbox
        {
            get { return viewbox; }
            set { viewbox = value; OnPropertyChanged(); }
        }

        #endregion

        #region Methods

        public void ApplyChanges()
        {
            Settings.Default.KeyboardFile = KeyboardFile;
        }

        private void Load()
        {
            KeyboardFile = Settings.Default.KeyboardFile;
        }

        private void OpenFile()
        {
            var fileDialog = new System.Windows.Forms.OpenFileDialog() { FileName = Path.GetFileName(KeyboardFile) };
            var result = fileDialog.ShowDialog();
            string tempFilename;
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    tempFilename = fileDialog.FileName; // we will commit it if loading is okay
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    return;
            }
            try
            {
                XmlKeyboard = XmlKeyboard.ReadFromFile(tempFilename);
            }
            catch (Exception e)
            {
                Log.Error($"Error reading from Layout file: {tempFilename} :");
                Log.Info(e.ToString());
                return;
            }
            KeyboardFile = tempFilename;
        }

        private void SaveFile()
        {
            var fp = Settings.Default.DynamicKeyboardsLocation;
            var fn = XmlKeyboard.Name + ".xml";
            try
            {
                fp = Path.GetDirectoryName(KeyboardFile);
                fn = Path.GetFileName(KeyboardFile);
            }
            catch { }
            var saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.InitialDirectory = fp;
            saveFileDialog.FileName = fn;
            saveFileDialog.Title = "Save File";
            saveFileDialog.CheckFileExists = false;
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.DefaultExt = "xml";
            saveFileDialog.Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;
            var result = saveFileDialog.ShowDialog();
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    KeyboardFile = saveFileDialog.FileName;
                    XmlKeyboard.WriteToFile(KeyboardFile);
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    break;
            }
        }

        private void AddBuiltIn()
        {
            var newKeyboard = new XmlKeyboard() { Name = KeyboardName };
            var profile = new InteractorProfile() { Name = "All" };
            newKeyboard.Profiles = new List<InteractorProfile> { profile };

            DependencyObject content = (DependencyObject)new Alpha1().GetContent();

            switch (KeyboardName)
            {
                case "Alpha2":
                    content = (DependencyObject)new Alpha2().GetContent();
                    break;
                case "Alpha3":
                    content = (DependencyObject)new Alpha3().GetContent();
                    break;
                case "ConversationAlpha1":
                    content = (DependencyObject)new ConversationAlpha1(null).GetContent();
                    break;
                case "ConversationAlpha2":
                    content = (DependencyObject)new ConversationAlpha1(null).GetContent();
                    break;
                case "ConversationAlpha3":
                    content = (DependencyObject)new ConversationAlpha3(null).GetContent();
                    break;
                case "ConversationNumericAndSymbols":
                    content = (DependencyObject)new ConversationNumericAndSymbols(null).GetContent();
                    break;
                case "Currencies1":
                    content = (DependencyObject)new Currencies1().GetContent();
                    break;
                case "Currencies2":
                    content = (DependencyObject)new Currencies2().GetContent();
                    break;
                case "Diacritics1":
                    content = (DependencyObject)new Diacritics1().GetContent();
                    break;
                case "Diacritics2":
                    content = (DependencyObject)new Diacritics2().GetContent();
                    break;
                case "Diacritics3":
                    content = (DependencyObject)new Diacritics3().GetContent();
                    break;
                case "Language":
                    content = (DependencyObject)new Language(null).GetContent();
                    break;
                case "Menu":
                    content = (DependencyObject)new Keyboards.Menu(null).GetContent();
                    break;
                case "Mouse":
                    content = (DependencyObject)new Keyboards.Mouse(null).GetContent();
                    break;
                case "NumericAndSymbols1":
                    content = (DependencyObject)new NumericAndSymbols1().GetContent();
                    break;
                case "NumericAndSymbols2":
                    content = (DependencyObject)new NumericAndSymbols2().GetContent();
                    break;
                case "NumericAndSymbols3":
                    content = (DependencyObject)new NumericAndSymbols3().GetContent();
                    break;
                case "PhysicalKeys":
                    content = (DependencyObject)new PhysicalKeys().GetContent();
                    break;
                case "SimplifiedAlpha":
                    content = (DependencyObject)new SimplifiedAlpha(null).GetContent();
                    break;
                case "SimplifiedConversationAlpha":
                    content = (DependencyObject)new SimplifiedConversationAlpha(null).GetContent();
                    break;
                case "SizeAndPosition":
                    content = (DependencyObject)new SizeAndPosition(null).GetContent();
                    break;
                case "WebBrowsing":
                    content = (DependencyObject)new WebBrowsing().GetContent();
                    break;
            }
            var symbols = new ResourceDictionary() { Source = new Uri("/OptiKey;component/Resources/Icons/KeySymbols.xaml", UriKind.RelativeOrAbsolute) };

            var symbolValues = symbols.Values.OfType<Geometry>().Select(x => x.ToString()).ToList();
            var symbolKeys = symbols.Keys.OfType<string>().ToList();
            var allKeys = VisualAndLogicalTreeHelper.FindLogicalChildren<Key>(content).ToList();
            var outputList = VisualAndLogicalTreeHelper.FindLogicalChildren<Output>(content).ToList();
            var outputRows = outputList.Any() ? 2 : 0;
            var maxRow = 0d;
            var maxCol = 0d;
            foreach (Key key in allKeys.Where(x => x is Key && VisualAndLogicalTreeHelper.FindLogicalParent<Output>(x) == null))
            {
                var i = new DynamicKey() { Label = key.ShiftUpText, ShiftDownLabel = key.ShiftDownText, ColN = Grid.GetColumn(key), RowN = Grid.GetRow(key) - outputRows, WidthN = Grid.GetColumnSpan(key), HeightN = Grid.GetRowSpan(key) };
                if (key.Value != null)
                {
                    var kv = key.Value;
                    if (kv.FunctionKey.HasValue)
                        i.Commands.Add(new ActionCommand() { Value = kv.FunctionKey.Value.ToString() });
                    else
                        i.Commands.Add(new TextCommand() { Value = kv.String });
                }
                if (key.SymbolGeometry != null)
                    i.Symbol = symbolKeys[symbolValues.IndexOf(key.SymbolGeometry.ToString())];
                i.SharedSizeGroup = key.SharedSizeGroup;
                i.Profiles.Add(new InteractorProfileMap(profile, true));
                newKeyboard.Interactors.Add(i);
                maxCol = Math.Max(maxCol, i.ColN + i.WidthN);
                maxRow = Math.Max(maxRow, i.RowN + i.HeightN);
            }
            newKeyboard.ShowOutputPanel = outputRows > 0 ? "True" : null;
            newKeyboard.Rows = (int)maxRow;
            newKeyboard.Cols = (int)maxCol;
            XmlKeyboard = newKeyboard;
        }

        private void AddLayout()
        {
            XmlKeyboard = new XmlKeyboard() {
                Name = "NewKeyboard",
                Grid = new XmlGrid() { Cols = 16, Rows = 14 },
                Profiles = new List<InteractorProfile> { new InteractorProfile() { Name = "All" } } };
        }

        private void AddProfile()
        {
            if (Profiles == null) { return; }

            var name = "Profile";
            for (int i = 1; i <= Profiles.Count() + 1; i++)
            {
                name = "Profile" + i.ToString();
                if (Profiles.Where(x => x.Name == name).Count() == 0)
                    break;
            }
            Profile = new InteractorProfile() { Name = name };
            Profiles.Add(Profile);
            XmlKeyboard.Profiles = Profiles.ToList();
            foreach (var i in Interactors)
            {
                i.Profiles.Add(new InteractorProfileMap(profile, false));
            }
        }

        private void DeleteProfile()
        {
            if (Profiles == null) { return; }
            
            var index = Profiles.IndexOf(Profile);
            if (index > 0)
            {
                foreach (var i in Interactors)
                {
                    var map = i.Profiles.Where(x => x.Profile == Profile).FirstOrDefault();
                    if (map != null)
                        i.Profiles.Remove(map);
                }
                Profiles.RemoveAt(index);
                XmlKeyboard.Profiles = Profiles.ToList();
                Profile = Profiles[index - 1];
            }
            CreateViewbox();
        }

        private void AddInteractor()
        {
            if (Interactors == null) { return; }

            var minKeyWidth = 1;
            var minKeyHeight = 1;
            if (Interactors.Where(x => x is DynamicKey).Any())
            {
                minKeyWidth = Interactors.Where(x => x is DynamicKey).Select(k => k.WidthN).Min();
                minKeyHeight = Interactors.Where(x => x is DynamicKey).Select(k => k.HeightN).Min();
            }

            var interactor = new Interactor();
            
            switch (newType)
            {
                case InteractorTypes.Key:
                    interactor = new DynamicKey() { RowN = 0, ColN = 0, WidthN = minKeyWidth, HeightN = minKeyHeight };
                    break;
                case InteractorTypes.Popup:
                    interactor = new DynamicPopup() { RowN = 0, ColN = 0 };
                    break;
                case InteractorTypes.OutputPanel:
                    interactor = new DynamicOutputPanel() { RowN = 0, ColN = 0, WidthN = XmlKeyboard.Cols, HeightN = 2 * minKeyHeight };
                    break;
                case InteractorTypes.Scratchpad:
                    interactor = new DynamicScratchpad() { RowN = 0, ColN = 0, WidthN = 8 * minKeyWidth, HeightN = minKeyHeight};
                    break;
                case InteractorTypes.SuggestionRow:
                    interactor = new DynamicSuggestionRow() { RowN = 0, ColN = 0, WidthN = 8 * minKeyWidth, HeightN = minKeyHeight };
                    break;
                case InteractorTypes.SuggestionColumn:
                    interactor = new DynamicSuggestionCol() { RowN = 0, ColN = 0, HeightN = 4 * minKeyHeight};
                    break;
            }

            foreach (var p in Profiles)
            {
                interactor.Profiles.Add(new InteractorProfileMap(p, p.Name == "All"));
            }

            var index = Interactor != null ? Interactors.IndexOf(Interactor) + 1 : 0;
            Interactors.Insert(index, interactor);
            XmlKeyboard.Interactors = Interactors.ToList();
            CreateViewbox();
            Interactor = interactor;
        }

        private void CloneInteractor()
        {
            if (Interactor == null) { return; }

            var serializer = new XmlSerializer(Interactor.GetType());
            var sw = new StringWriter();
            var xmlWriter = XmlWriter.Create(sw, new XmlWriterSettings());
            serializer.Serialize(xmlWriter, Interactor, new XmlSerializerNamespaces());
            var newInteractor = (Interactor)serializer.Deserialize(new StringReader(sw.ToString()));
            foreach (var p in Interactor.Profiles)
            {
                newInteractor.Profiles.Add(new InteractorProfileMap(p.Profile, p.IsMember));
            }
            newInteractor.ColN += newInteractor.WidthN;
            Interactors.Insert(Interactors.IndexOf(Interactor) + 1, newInteractor);
            XmlKeyboard.Interactors = Interactors.ToList();
            CreateViewbox();
            Interactor = newInteractor;
        }

        private void DeleteInteractor()
        {
            if (Interactor == null) { return; }

            Interactors.Remove(interactor);
            XmlKeyboard.Interactors = Interactors.ToList();
            CreateViewbox();
            Interactor = null;
        }

        private void ReplaceInteractor()
        {
            newType = selectedInteractorType;
            var index = Interactors.IndexOf(Interactor);
            Interactors.RemoveAt(index);
            if (index > 0)
                Interactor = Interactors[index - 1];
            AddInteractor();
        }

        private void SelectInteractor(object obj)
        {
            Interactor = (Interactor)obj;
        }

        private Canvas canvas;
        private Border gazeRegion = new Border() { Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FB6043"), BorderThickness = new Thickness(5), Child = new Viewbox() { Stretch = Stretch.Uniform,
        StretchDirection = StretchDirection.Both, Child = new TextBlock() { Text = "Gaze\nHere", TextAlignment=TextAlignment.Center, Foreground = Brushes.White } } };
        public double ScreenWidth { get { return Graphics.VirtualScreenWidthInPixels; } }
        public double ScreenHeight { get { return Graphics.VirtualScreenHeightInPixels; } }
        public double ScreenLeft { get { return .2 * ScreenWidth; } }
        public double ScreenTop { get { return .2 * ScreenHeight; } }
        public Thickness Margin { get { return new Thickness(Left, Top, 0, 0); } }
        public double Width { get { return XmlKeyboard.WidthN ?? ScreenWidth; } }
        public double Height { get { return XmlKeyboard.HeightN ?? ScreenHeight; } }
        public double Left
        {
            get
            {
                var offset = XmlKeyboard.HorizontalOffsetN ?? 0;
                return Enum.TryParse(XmlKeyboard.Position, out MoveToDirections newMovePosition)
                    ? newMovePosition == MoveToDirections.Left || newMovePosition == MoveToDirections.TopLeft || newMovePosition == MoveToDirections.BottomLeft
                        ? ScreenLeft + offset
                        : newMovePosition == MoveToDirections.Right || newMovePosition == MoveToDirections.TopRight || newMovePosition == MoveToDirections.BottomRight
                        ? ScreenLeft + ScreenWidth - Width + offset
                        : ScreenLeft + ScreenWidth / 2 - Width / 2 + offset
                    : ScreenLeft + offset;
            }
        }
        public double Top
        {
            get
            {
                var offset = XmlKeyboard.VerticalOffsetN ?? 0;
                return Enum.TryParse(XmlKeyboard.Position, out MoveToDirections newMovePosition)
                    ? newMovePosition == MoveToDirections.Top || newMovePosition == MoveToDirections.TopLeft || newMovePosition == MoveToDirections.TopRight
                        ? ScreenTop + offset
                        : (newMovePosition == MoveToDirections.Bottom || newMovePosition == MoveToDirections.BottomLeft || newMovePosition == MoveToDirections.BottomRight)
                        ? ScreenTop + ScreenHeight - Height + offset
                        : ScreenTop + ScreenHeight / 2 - Height / 2 + offset
                    : ScreenTop + offset;
            }
        }

        private void CreateViewbox()
        {
            Viewbox = null;

            var thickness = 40;
            if (canvas != null && canvas.Children != null)
                canvas.Children.Clear();

            canvas = new Canvas()
            {
                Width = 2 * ScreenLeft + ScreenWidth,
                Height = 2 * ScreenTop + ScreenHeight,
            };
            canvas.Children.Add(new Border()
            {
                Background = new SolidColorBrush(Color.FromRgb(24,24,24)),
                BorderBrush = Brushes.White,
                BorderThickness = new Thickness(thickness),
                Width = ScreenWidth + 2 * thickness,
                Height = ScreenHeight + 2 * thickness,
                Margin = new Thickness(ScreenLeft - thickness, ScreenTop - thickness, 0, 0),
            });
            canvas.Children.Add(new System.Windows.Shapes.Line()
            {
                X1 = ScreenLeft + .25 * ScreenWidth,
                Y1 = ScreenTop + 1.15 * ScreenHeight,
                X2 = ScreenLeft + .75 * ScreenWidth,
                Y2 = ScreenTop + 1.15 * ScreenHeight,
                SnapsToDevicePixels = true,
                Stroke = Brushes.White,
                StrokeThickness = 2 * thickness
            });
            canvas.Children.Add(new System.Windows.Shapes.Line()
            {
                X1 = ScreenLeft + .5 * ScreenWidth,
                Y1 = ScreenTop + ScreenHeight + thickness,
                X2 = ScreenLeft + .5 * ScreenWidth,
                Y2 = ScreenTop + 1.15 * ScreenHeight,
                SnapsToDevicePixels = true,
                Stroke = Brushes.White,
                StrokeThickness = 6 * thickness
            });
            
            var dynamicKeyboard = new Views.Keyboards.Common.DynamicKeyboard(XmlKeyboard);
            dynamicKeyboard.SetBinding(FrameworkElement.WidthProperty, new Binding("Width") { Source = this });
            dynamicKeyboard.SetBinding(FrameworkElement.HeightProperty, new Binding("Height") { Source = this });
            dynamicKeyboard.SetBinding(FrameworkElement.MarginProperty, new Binding("Margin") { Source = this });
            canvas.Children.Add(dynamicKeyboard);

            foreach (var i in Interactors.Where(x => x.Key != null))
            {
                i.Key.InputBindings.Add(new MouseBinding()
                {
                    MouseAction = MouseAction.LeftClick,
                    Command = InteractorCommand,
                    CommandParameter = i
                });

                if (i is DynamicPopup)
                {
                    var parent = VisualAndLogicalTreeHelper.FindVisualParent<Grid>(i.Key);
                    if (parent != null)
                        parent.Children.Remove(i.Key);
                    i.Key.Margin = new Thickness(ScreenLeft + i.Key.GazeRegion.Left * ScreenWidth, ScreenTop + i.Key.GazeRegion.Top * ScreenHeight, 0, 0);
                    i.Key.Width = i.Key.GazeRegion.Width * ScreenWidth;
                    i.Key.Height = i.Key.GazeRegion.Height * ScreenHeight;
                    canvas.Children.Add(i.Key);
                }
                if (i is DynamicOutputPanel)
                {
                    foreach (var item in VisualAndLogicalTreeHelper.FindLogicalChildren<Output>(dynamicKeyboard.MainGrid)
                        .Where(x => Grid.GetRow(x) == i.RowN && Grid.GetColumn(x) == i.ColN))
                    {
                        item.InputBindings.Add(new MouseBinding()
                        {
                            MouseAction = MouseAction.LeftClick,
                            Command = InteractorCommand,
                            CommandParameter = i
                        });
                    }
                }
                if (i is DynamicScratchpad)
                {
                    foreach (var item in VisualAndLogicalTreeHelper.FindLogicalChildren<XmlScratchpad>(dynamicKeyboard.MainGrid)
                        .Where(x => Grid.GetRow(x) == i.RowN && Grid.GetColumn(x) == i.ColN))
                    {
                        item.InputBindings.Add(new MouseBinding()
                        {
                            MouseAction = MouseAction.LeftClick,
                            Command = InteractorCommand,
                            CommandParameter = i
                        });
                    }
                }
                if (i is DynamicSuggestionCol)
                {
                    foreach (var item in VisualAndLogicalTreeHelper.FindLogicalChildren<XmlSuggestionCol>(dynamicKeyboard.MainGrid)
                        .Where(x => Grid.GetRow(x) == i.RowN && Grid.GetColumn(x) == i.ColN))
                    {
                        item.InputBindings.Add(new MouseBinding()
                        {
                            MouseAction = MouseAction.LeftClick,
                            Command = InteractorCommand,
                            CommandParameter = i
                        });
                    }
                }
                if (i is DynamicSuggestionRow)
                {
                    foreach (var item in VisualAndLogicalTreeHelper.FindLogicalChildren<XmlSuggestionRow>(dynamicKeyboard.MainGrid)
                        .Where(x => Grid.GetRow(x) == i.RowN && Grid.GetColumn(x) == i.ColN))
                    {
                        item.InputBindings.Add(new MouseBinding()
                        {
                            MouseAction = MouseAction.LeftClick,
                            Command = InteractorCommand,
                            CommandParameter = i
                        });
                    }
                }
                if (i == Interactor)
                {
                    Interactor = null;
                    Interactor = i;
                }
            }
            canvas.ClipToBounds = true;
            Viewbox = new Viewbox();
            Viewbox.Width = .7 * SystemParameters.VirtualScreenWidth;
            Viewbox.Height = .7 * SystemParameters.VirtualScreenHeight;
            Viewbox.Stretch = Stretch.Fill;
            Viewbox.StretchDirection = StretchDirection.Both;
            Viewbox.Child = canvas;
        }

        #endregion

    }
}