// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using JuliusSweetland.OptiKey.Enums;
using JuliusSweetland.OptiKey.UI.Controls;
using JuliusSweetland.OptiKey.UI.Utilities;
using log4net;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;
using Key = JuliusSweetland.OptiKey.UI.Controls.Key;

namespace JuliusSweetland.OptiKey.Models
{
    [XmlRoot(ElementName = "Keyboard")]
    public class Layout : ILayout, INotifyPropertyChanged
    {
        private PropertyChangedEventHandler descendantPropertyChanged;
        [XmlIgnore] public ObservableCollection<Interactor> Descendants { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            if (name == "Width" || name == "Height" || name == "Rows" || name == "Columns"
                || name == "HorizontalOffset" || name == "VerticalOffset" || name == "Position")
            {
                CreateView();
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        protected void DescendantPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Width")
            {
                var newMin = Interactors.Select(k => k.WidthN).Min() > 0 ? Interactors.Select(k => k.WidthN).Min() : 1;
                if (newMin != MinKeyWidth)
                {
                    foreach (var i in Interactors)
                    {
                        i.Key.WidthSpan = i.WidthN / newMin;
                    }
                    CreateView();
                }
            }

            if (e.PropertyName == "Height")
            {
                var newMin = Interactors.Select(k => k.HeightN).Min() > 0 ? Interactors.Select(k => k.HeightN).Min() : 1;
                if (newMin != MinKeyHeight)
                {
                    foreach (var i in Interactors)
                    {
                        i.Key.HeightSpan = i.HeightN / newMin;
                    }
                    CreateView();
                }
            }
        }

        public Layout()
        {
            Profiles = new ObservableCollection<InteractorProfile>();
            InteractorCommand = new DelegateCommand<object>(SelectInteractor);
            descendantPropertyChanged = new PropertyChangedEventHandler(DescendantPropertyChanged);

            Interactors = new ObservableCollection<Interactor>();
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
        }

        private string name = "New Keyboard";
        [XmlElement] public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }

        [XmlIgnore] public double Width { get; set; } = SystemParameters.VirtualScreenWidth;
        private string widthString;
        [XmlElement("Width")]
        public string WidthString
        {
            get { return widthString; }
            set
            {
                widthString = value;
                Width = ValidDim(value, ScreenWidth); OnPropertyChanged("Width");
            }
        }

        [XmlIgnore] public double Height { get; set; } = SystemParameters.VirtualScreenHeight;
        private string heightString;
        [XmlElement("Height")]
        public string HeightString
        {
            get { return heightString; }
            set
            {
                heightString = value;
                Height = ValidDim(value, ScreenHeight); OnPropertyChanged("Height");
            }
        }

        [XmlElement("Grid")] public LayoutGrid Grid { get; set; } = new LayoutGrid();

        [XmlIgnore] public int Columns { get { return Grid.Cols; } set { Grid.Cols = value; OnPropertyChanged(); } }

        [XmlIgnore] public int Rows { get { return Grid.Rows; } set { Grid.Rows = value; OnPropertyChanged(); } }

        private string windowState;
        [XmlElement] public string WindowState { get { return windowState; } set { windowState = value; OnPropertyChanged(); } }

        private string position;
        [XmlElement] public string Position { get { return position; } set { position = value; OnPropertyChanged(); } }

        private string dockSize;
        [XmlElement] public string DockSize { get { return dockSize; } set { dockSize = value; OnPropertyChanged(); } }

        private double horizontalOffset;
        private string horizontalOffsetString;
        [XmlElement] public string HorizontalOffset { get { return horizontalOffsetString; } set { horizontalOffsetString = value; horizontalOffset = ValidOffset(value, ScreenWidth); OnPropertyChanged(); } }

        private double verticalOffset;
        private string verticalOffsetString;
        [XmlElement] public string VerticalOffset { get { return verticalOffsetString; } set { verticalOffsetString = value; verticalOffset = ValidOffset(value, ScreenHeight); OnPropertyChanged(); } }

        [XmlIgnore] public double ScreenWidth { get { return SystemParameters.VirtualScreenWidth; } }
        [XmlIgnore] public double ScreenHeight { get { return SystemParameters.VirtualScreenHeight; } }
        [XmlIgnore] public double ScreenLeft { get { return .2 * ScreenWidth; } }
        [XmlIgnore] public double ScreenTop { get { return .2 * ScreenHeight; } }
        [XmlIgnore]
        public double Left
        {
            get
            {
                return Enum.TryParse(position, out MoveToDirections newMovePosition)
                    ? newMovePosition == MoveToDirections.Left || newMovePosition == MoveToDirections.TopLeft || newMovePosition == MoveToDirections.BottomLeft
                        ? ScreenLeft + horizontalOffset
                        : newMovePosition == MoveToDirections.Right || newMovePosition == MoveToDirections.TopRight || newMovePosition == MoveToDirections.BottomRight
                        ? ScreenLeft + ScreenWidth - Width + horizontalOffset
                        : ScreenLeft + ScreenWidth / 2 - Width / 2 + horizontalOffset
                    : ScreenLeft + horizontalOffset;
            }
        }
        [XmlIgnore]
        public double Top
        {
            get
            {
                return Enum.TryParse(position, out MoveToDirections newMovePosition)
                    ? newMovePosition == MoveToDirections.Top || newMovePosition == MoveToDirections.TopLeft || newMovePosition == MoveToDirections.TopRight
                        ? ScreenTop + verticalOffset
                        : (newMovePosition == MoveToDirections.Bottom || newMovePosition == MoveToDirections.BottomLeft || newMovePosition == MoveToDirections.BottomRight)
                        ? ScreenTop + ScreenHeight - Height + verticalOffset
                        : ScreenTop + ScreenHeight / 2 - Height / 2 + verticalOffset
                    : ScreenTop + verticalOffset;
            }
        }
        [XmlIgnore] public Thickness Margin { get { return new Thickness(Left, Top, 0, 0); } }

        private InteractorProfile selectedProfile;
        [XmlIgnore]
        public InteractorProfile SelectedProfile
        {
            get { return selectedProfile; }
            set { selectedProfile = value; OnPropertyChanged(); }
        }

        private Interactor selectedInteractor;
        [XmlIgnore]
        public Interactor SelectedInteractor
        {
            get { return selectedInteractor; }
            set
            {
                if (selectedInteractor != null)
                    selectedInteractor.Key.IsCurrent = false;
                selectedInteractor = value;
                if (selectedInteractor != null)
                {
                    selectedInteractor.Key.IsCurrent = true;
                    selectedInteractor.BuildProfiles();
                }
                OnPropertyChanged();
            }
        }

        private InteractorTypes selectedInteractorType;
        [XmlIgnore]
        public string SelectedInteractorType
        {
            get { return selectedInteractorType.ToString(); }
            set
            {
                selectedInteractorType = Enum.TryParse(value, out InteractorTypes type) ? type : InteractorTypes.Key;
            }
        }

        [XmlIgnore] public ICommand InteractorCommand { get; private set; }

        [XmlElement("KeyGroup")] public ObservableCollection<InteractorProfile> Profiles { get; set; }

        [XmlElement("Content")] public LayoutContent Content { get; set; } = new LayoutContent();
        [XmlIgnore]
        public ObservableCollection<Interactor> Interactors
        {
            get { return Content.Interactors; }
            set { Content.Interactors = value; }
        }

        private Canvas canvas;
        private Viewbox view = new Viewbox();
        [XmlIgnore] public Viewbox View { get { return view; } set { view = value; OnPropertyChanged(); } }
        [XmlIgnore] public Border KeyboardView { get; set; }
        [XmlIgnore] public string KeyboardFile { get; set; }
        [XmlIgnore] public double MinKeyWidth { get; set; }
        [XmlIgnore] public double MinKeyHeight { get; set; }

        private void SelectInteractor(object obj)
        {
            SelectedInteractor = (Interactor)obj;
        }

        private double ValidDim(string dim, double screenDim)
        {
            if (!double.TryParse(dim.Replace("%", ""), out double numericDim))
                return screenDim;

            numericDim = dim.Contains("%")
                ? numericDim > 0
                    ? numericDim / 100 * screenDim : screenDim + (numericDim / 100 * screenDim)
                : numericDim > 0 ? numericDim : screenDim + numericDim;

            if (numericDim > -.97 * screenDim
                && !(numericDim > -.03 * screenDim && numericDim < .03 * screenDim)
                && numericDim < 1.03 * screenDim)
                return numericDim;

            return screenDim;
        }
        private double ValidOffset(string dim, double screenDim)
        {
            if (!double.TryParse(dim.Replace("%", ""), out double numericDim))
                return 0;

            numericDim = dim.Contains("%") ? numericDim / 100 * screenDim : numericDim;

            return numericDim > -.97 * screenDim && numericDim < .97 * screenDim ? numericDim : 0;
        }

        public void Load()
        {
            if (!Profiles.Any())
                Profiles.Add(new InteractorProfile() { Name = "All" });

            if (Interactors.Any())
                SelectedInteractor = Interactors[0];

            SelectedProfile = Profiles[0];
            CreateView();
        }

        public void AddProfile()
        {
            var profile = new InteractorProfile() { Name = "Profile" + Profiles.Count.ToString() };
            Profiles.Add(profile);
            foreach (var i in Interactors)
            {
                i.Profiles.Add(new InteractorProfileMap(profile, false));
            }
            SelectedProfile = profile;
        }

        public void DeleteProfile()
        {
            var index = Profiles.IndexOf(SelectedProfile);
            if (index > 0)
            {
                foreach (var i in Interactors)
                {
                    var map = i.Profiles.Where(x => x.Profile == SelectedProfile).FirstOrDefault();
                    if (map != null)
                        i.Profiles.Remove(map);
                }
                Profiles.RemoveAt(index);
                SelectedProfile = Profiles[index - 1];
            }
        }

        public void AddInteractor()
        {
            var interactor = new Interactor();
            switch (selectedInteractorType)
            {
                case InteractorTypes.Key:
                    interactor = new DynamicKey();
                    interactor.Commands.Add(new TextCommand() { Value = null });
                    break;
                case InteractorTypes.OutputPanel:
                    interactor = new DynamicOutputPanel();
                    break;
                case InteractorTypes.Popup:
                    interactor = new DynamicPopup();
                    interactor.Commands.Add(new TextCommand() { Value = null });
                    break;
                case InteractorTypes.Scratchpad:
                    interactor = new DynamicScratchpad();
                    break;
                case InteractorTypes.SuggestionRow:
                    interactor = new DynamicSuggestionRow();
                    break;
                case InteractorTypes.SuggestionColumn:
                    interactor = new DynamicSuggestionCol();
                    break;
            }

            foreach (var p in Profiles)
            {
                interactor.Profiles.Add(new InteractorProfileMap(p, p.Name == "All"));
            }

            var index = SelectedInteractor != null ? Interactors.IndexOf(SelectedInteractor) + 1 : 0;
            Interactors.Insert(index, interactor);
            AddInteractorToView(interactor);
            SelectedInteractor = interactor;
        }

        public void DeleteInteractor()
        {
            if (selectedInteractor == null)
                return;

            canvas.Children.Remove(SelectedInteractor.Key);
            Interactors.Remove(SelectedInteractor);
            SelectedInteractor = null;
        }

        public void CloneInteractor()
        {
            if (selectedInteractor == null)
                return;

            var serializer = new XmlSerializer(SelectedInteractor.GetType());
            var sw = new StringWriter();
            var xmlWriter = XmlWriter.Create(sw, new XmlWriterSettings());
            serializer.Serialize(xmlWriter, SelectedInteractor, new XmlSerializerNamespaces());
            var newInteractor = (Interactor)serializer.Deserialize(new StringReader(sw.ToString()));
            foreach (var p in SelectedInteractor.Profiles)
            {
                newInteractor.Profiles.Add(new InteractorProfileMap(p.Profile, p.IsMember));
            }
            newInteractor.ColN += newInteractor.WidthN;
            Interactors.Insert(Interactors.IndexOf(SelectedInteractor) + 1, newInteractor);
            AddInteractorToView(newInteractor);
            SelectedInteractor = newInteractor;
        }

        private void CreateView()
        {
            var thickness = 20;
            if (canvas != null && canvas.Children != null)
                canvas.Children.Clear();

            canvas = new Canvas()
            {
                Width = 2 * ScreenLeft + ScreenWidth,
                Height = 2 * ScreenTop + ScreenHeight,
            };
            canvas.Children.Add(new Border()
            {
                Background = Brushes.Black,
                BorderBrush = Brushes.White,
                BorderThickness = new Thickness(thickness),
                Width = ScreenWidth + 2 * thickness,
                Height = ScreenHeight + 2 * thickness,
                Margin = new Thickness(ScreenLeft - thickness, ScreenTop - thickness, 0, 0),
            }); ;
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
            KeyboardView = new Border() { Background = Brushes.Transparent, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(2) };
            KeyboardView.SetBinding(Border.WidthProperty, new Binding("Width") { Source = this });
            KeyboardView.SetBinding(Border.HeightProperty, new Binding("Height") { Source= this });
            KeyboardView.SetBinding(Border.MarginProperty, new Binding("Margin") { Source = this });
            canvas.Children.Add(KeyboardView);

            //KeyboardView.Child = new UI.Views.Keyboards.Common.DynamicKeyboard(
            //    null, KeyboardFile, new List<Tuple<KeyValue, KeyValue>>(), new Dictionary<string, List<KeyValue>>(), new Dictionary<KeyValue, TimeSpanOverrides>(), null);

            //var allKeys = VisualAndLogicalTreeHelper.FindLogicalChildren<Key>(KeyboardView.Child).ToList();
            //if (allKeys != null && allKeys.Any() && Interactors != null && Interactors.Any())
            //{
            //    foreach (Key userControl in allKeys)
            //    {
            //        var interactor = Interactors.FirstOrDefault(x => (int)x.Row == System.Windows.Controls.Grid.GetRow(userControl)
            //        && (int)x.Col == System.Windows.Controls.Grid.GetColumn(userControl));
            //        if (userControl.InputBindings.Count == 0)
            //            userControl.InputBindings.Add(new MouseBinding()
            //            {
            //                MouseAction = MouseAction.LeftClick,
            //                Command = InteractorCommand,
            //                CommandParameter = interactor
            //            });
            //    }
            //}
            if (Interactors.Any())
            SetupInteractors();

            canvas.ClipToBounds = true;
            View.Child = canvas;
        }

        public void AddInteractorToView(Interactor interactor)
        {
            if (interactor is DynamicKey || interactor is DynamicPopup)
            {
                interactor.Key = (Key)BindInteractor(interactor, interactor.Key);
                canvas.Children.Add(interactor.Key);
            }
        }

        private UserControl BindInteractor(Interactor interactor, UserControl userControl)
        {
            if (userControl.InputBindings.Count == 0)
                userControl.InputBindings.Add(new MouseBinding()
                {
                    MouseAction = MouseAction.LeftClick,
                    Command = InteractorCommand,
                    CommandParameter = interactor
                });
            userControl.SetBinding(Border.WidthProperty, new Binding("BorderWidth")
            { Source = interactor });
            userControl.SetBinding(Border.HeightProperty, new Binding("BorderHeight")
            { Source = interactor });
            userControl.SetBinding(Border.MarginProperty, new Binding("Margin")
            { Source = interactor });
            userControl.SetBinding(Key.BackgroundColourOverrideProperty, new Binding("BackgroundBrush")
            { Source = interactor.Expressed });
            userControl.SetBinding(Key.ForegroundColourOverrideProperty, new Binding("ForegroundBrush")
            { Source = interactor.Expressed });
            userControl.SetBinding(Key.BorderColourOverrideProperty, new Binding("BorderBrush")
            { Source = interactor.Expressed });
            userControl.SetBinding(Key.OpacityOverrideProperty, new Binding("Opacity")
            { Source = interactor.Expressed });
            userControl.SetBinding(Key.SharedSizeGroupProperty, new Binding("SharedSizeGroup")
            { Source = interactor });

            return userControl;
        }

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [XmlNamespaceDeclarations] public XmlSerializerNamespaces xmlns;

        public static Layout ReadFromFile(string inputFilename)
        {
            if (!File.Exists(inputFilename) && string.IsNullOrEmpty(Path.GetExtension(inputFilename)))
                inputFilename += ".xml";

            var layout = new Layout();
            var serializer = new XmlSerializer(typeof(Layout));

            using (var reader = new FileStream(@inputFilename, FileMode.Open))
            {
                layout = (Layout)serializer.Deserialize(reader);
            }

            return layout;
        }

        public static Layout ReadFromString(string xmlString)
        {
            if (string.IsNullOrEmpty(xmlString)) { return null; }

            var layout = new Layout();
            var serializer = new XmlSerializer(typeof(Layout));
            try
            {
                layout = (Layout)serializer.Deserialize(new StringReader(xmlString));
                return layout;
            }
            catch
            {
                Log.ErrorFormat("Error reading layout from string: '{0}'", xmlString);
                return null;
            }
        }

        public void WriteToFile(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                return;

            var serializer = new XmlSerializer(typeof(Layout));
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var sw = new StreamWriter(filename);
            var xmlWriter = XmlWriter.Create(sw, new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true });

            serializer.Serialize(xmlWriter, this, ns);
            sw.Close();
        }

        public string WriteToString()
        {
            var serializer = new XmlSerializer(typeof(Layout));

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var sw = new StringWriter();
            var xmlWriter = XmlWriter.Create(sw, new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true });

            serializer.Serialize(xmlWriter, this, ns);
            return sw.ToString();
        }

        private bool ListContainsWidth(List<int> list, int col, int width)
        {
            return list.Contains(col) && (width <= 1 || ListContainsWidth(list, col + 1, width - 1));
        }

        private bool ListContainsWidthAndHeight(List<List<int>> list, int row, int col, int width, int height)
        {
            return list[row] != null && ListContainsWidth(list[row], col, width)
                && ((height <= 1) || ListContainsWidthAndHeight(list, row + 1, col, width, height - 1));
        }

        private int FindCol(List<List<int>> list, int row, int col, int width, int height)
        {
            return list[row] == null || !list[row].Any() || col + width > list[row].Last() + 1 ? -1
                : ListContainsWidthAndHeight(list, row, col, width, height) ? col
                : FindCol(list, row, col + 1, width, height);
        }

        private int FindRow(List<List<int>> list, int row, int col, int width, int height)
        {
            return row + height > list.Count ? -1
                : ListContainsWidthAndHeight(list, row, col, width, height) ? row
                : FindRow(list, row + 1, col, width, height);
        }

        private Tuple<int, int> FindItemPositions(List<List<int>> list, Interactor dynamicItem, int row, int col)
        {
            if (dynamicItem.RowN > -1 && dynamicItem.RowN != row)
            {
                row = (int)dynamicItem.RowN;
                col = 0;
            }

            if (dynamicItem.ColN > -1)
                col = (int)dynamicItem.ColN;

            if (ListContainsWidthAndHeight(list, row, col, (int)dynamicItem.WidthN, (int)dynamicItem.HeightN))
                return new Tuple<int, int>(row, col);

            if (dynamicItem.ColN < 0)
            {
                var newCol = FindCol(list, row, col + 1, (int)dynamicItem.WidthN, (int)dynamicItem.HeightN);
                if (newCol > -1)
                    return new Tuple<int, int>(row, newCol);
                else if (dynamicItem.RowN < 0)
                {
                    for (int newRow = row + 1; newRow < list.Count; newRow++)
                    {
                        newCol = FindCol(list, newRow, 0, (int)dynamicItem.WidthN, (int)dynamicItem.HeightN);
                        if (newCol > -1)
                            return new Tuple<int, int>(newRow, newCol);
                    }
                }
            }
            else if (dynamicItem.RowN < 0)
                return new Tuple<int, int>(FindRow(list, row + 1, col, (int)dynamicItem.WidthN, (int)dynamicItem.HeightN), col);

            return new Tuple<int, int>(-1, -1);
        }

        private void SetupInteractors()
        {
            //create an item list that excludes popups
            var itemList = Interactors.Where(x => !(x is DynamicPopup)).ToList();
            var minKeyWidth = itemList.Select(k => k.WidthN).Min() > 0 ? itemList.Select(k => k.WidthN).Min() : 1;
            var minKeyHeight = itemList.Select(k => k.HeightN).Min() > 0 ? itemList.Select(k => k.HeightN).Min() : 1;

            //start with a list of all grid cells marked empty
            var openGrid = new List<List<int>>();
            for (int r = 0; r < Grid.Rows; r++)
            {
                var gridRow = new List<int>();
                for (int c = 0; c < Grid.Cols; c++)
                {
                    gridRow.Add(c);
                }
                openGrid.Add(gridRow);
            }

            //process all items in the order listed in the xml file and place them in position
            //if an item has a row or column designation it is treated as an
            //indication to jump to that position and continue from there
            var rowCol = new Tuple<int, int>(0, 0);
            foreach (var dynamicItem in itemList)
            {
                rowCol = FindItemPositions(openGrid, dynamicItem, rowCol.Item1, rowCol.Item2);

                //if there is not enough empty space is not found then return an error
                if (rowCol.Item1 < 0 || rowCol.Item2 < 0)
                {
                    return;
                }

                dynamicItem.RowN = rowCol.Item1;
                dynamicItem.ColN = rowCol.Item2;
                for (int i = (int)dynamicItem.RowN; i < dynamicItem.RowN + dynamicItem.HeightN; i++)
                {
                    openGrid[i].RemoveAll(x => x >= dynamicItem.ColN && x < dynamicItem.ColN + dynamicItem.WidthN);
                }

                //SetupDynamicItem(dynamicItem, minKeyWidth, minKeyHeight);
                rowCol = new Tuple<int, int>(rowCol.Item1, rowCol.Item2 + (int)dynamicItem.WidthN);

                AddInteractorToView(dynamicItem);
            }

            ////process popups
            //var popupList = keyboard.Content.Items.Where(x => x is XmlDynamicPopup).ToList();
            //foreach (XmlDynamicKey popup in popupList)
            //{
            //    SetupDynamicItem(popup, minKeyWidth, minKeyHeight);
            //}
            //return true;
        }
    }

    public class LayoutGrid
    {
        public int Rows { get; set; } = 14;
        public int Cols { get; set; } = 16;
    }

    public class LayoutContent
    {
        [XmlElement("DynamicKey", typeof(DynamicKey))]
        [XmlElement("OutputPanel", typeof(DynamicOutputPanel))]
        [XmlElement("Popup", typeof(DynamicPopup))]
        [XmlElement("Scratchpad", typeof(DynamicScratchpad))]
        [XmlElement("SuggestionRow", typeof(DynamicSuggestionRow))]
        [XmlElement("SuggestionCol", typeof(DynamicSuggestionCol))]
        public ObservableCollection<Interactor> Interactors { get; set; } = new ObservableCollection<Interactor>();
    }
}