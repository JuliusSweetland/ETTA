using JuliusSweetland.OptiKey.Enums;
using log4net;
// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace JuliusSweetland.OptiKey.Models
{
    [XmlRoot(ElementName = "Keyboard")]
    public class XmlKeyboard : INotifyPropertyChanged
    {
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public XmlKeyboard() { }

        [XmlIgnore] public string ErrorMessage { get; private set; }

        public string Name { get; set; }

        [XmlIgnore] public bool? ShowOutputPanelN { get; set; }
        public string ShowOutputPanel
        {
            get { return ShowOutputPanelN.HasValue ? ShowOutputPanelN.Value.ToString() : null; }
            set { ShowOutputPanelN = bool.TryParse(value, out bool result) ? (bool?)result : null; OnPropertyChanged(); }
        }

        public XmlKeyStates InitialKeyStates { get; set; }

        [XmlIgnore] public bool? PersistNewStateN { get; set; }
        public string PersistNewState
        {
            get { return PersistNewStateN.HasValue ? PersistNewStateN.Value.ToString() : null; }
            set { PersistNewStateN = bool.TryParse(value, out bool result) ? (bool?)result : null; OnPropertyChanged(); }
        }

        [XmlIgnore] public WindowStates? WindowStateN { get; set; }
        public string WindowState
        {
            get { return WindowStateN.HasValue ? WindowStateN.Value.ToString() : null; ; }
            set { WindowStateN = Enum.TryParse(value, out WindowStates result) && (result == WindowStates.Docked || result == WindowStates.Floating || result == WindowStates.Maximised) ? (WindowStates?) result : null; OnPropertyChanged(); }
        }

        [XmlIgnore] public MoveToDirections? PositionN { get; set; }
        public string Position
        {
            get { return PositionN.HasValue ? PositionN.Value.ToString() : null; }
            set { PositionN = Enum.TryParse(value, out MoveToDirections result) ? (MoveToDirections?)result : null; OnPropertyChanged(); }
        }

        [XmlIgnore] public DockSizes? DockSizeN { get; set; }
        public string DockSize
        {
            get { return DockSizeN.HasValue && DockSizeN.Value == DockSizes.Collapsed ? DockSizeN.Value.ToString() : null; }
            set { DockSizeN = Enum.TryParse(value, out DockSizes result) ? (DockSizes?)result : null; OnPropertyChanged(); }
        }

        private double ScreenWidth { get { return SystemParameters.VirtualScreenWidth; } }
        private double ScreenHeight { get { return SystemParameters.VirtualScreenHeight; } }
        [XmlIgnore] public double? WidthN { get; set; }
        private string width;
        public string Width
        {
            get { return width; }
            set
            {
                width = value;
                WidthN = ValidDim(value, ScreenWidth);
                OnPropertyChanged();
            }
        }

        [XmlIgnore] public double? HeightN { get; set; }
        private string height;
        public string Height
        {
            get { return height; }
            set
            {
                height = value;
                HeightN = ValidDim(value, ScreenHeight);
                OnPropertyChanged();
            }
        }

        [XmlIgnore] public double? HorizontalOffsetN;
        private string horizontalOffset;
        public string HorizontalOffset{ get { return horizontalOffset; } set { horizontalOffset = value; HorizontalOffsetN = ValidOffset(value, ScreenWidth); OnPropertyChanged(); } }

        [XmlIgnore] public double? VerticalOffsetN;
        private string verticalOffset;
        [XmlElement("VerticalOffset")] public string VerticalOffset{ get { return verticalOffset; } set { verticalOffset = value; VerticalOffsetN = ValidOffset(value, ScreenHeight); OnPropertyChanged(); } }

        public string Symbol
        { get; set; }

        public double? SymbolMargin
        { get; set; }

        public string BackgroundColor
        { get; set; }

        public string BorderColor
        { get; set; }

        [XmlIgnore]
        public Thickness? BorderThickness
        { get; set; }

        [XmlElement("BorderThickness")]
        public string BorderThicknessAsString
        {
            get { return BorderThickness.ToString(); }
            set
            {
                try
                {
                    ThicknessConverter thicknessConverter = new ThicknessConverter();
                    BorderThickness = (Thickness)thicknessConverter.ConvertFromString(value);
                }
                catch (System.FormatException)
                {
                    Log.ErrorFormat("Cannot interpret \"{0}\" as thickness", value);
                }
            }
        }

        [XmlIgnore]
        public bool Hidden
        { get; set; }

        [XmlElement("HideFromKeyboardMenu")]
        public string HiddenBoolAsString
        {
            get { return this.Hidden ? "True" : "False"; }
            set { this.Hidden = bool.TryParse(value, out bool result) && result; }
        }

        [XmlIgnore]
        public bool IsShiftAware
        { get; set; }

        [XmlElement("IsShiftAware")]
        public string IsShiftAwareAsString
        {
            get { return this.IsShiftAware ? "True" : "False"; }
            set { this.IsShiftAware = bool.TryParse(value, out bool result) && result; }
        }

        public XmlGrid Grid { get; set; } = new XmlGrid();
        [XmlIgnore] public int Rows { get { return Grid.Rows; } set { Grid.Rows = value; OnPropertyChanged(); } }
        [XmlIgnore] public int Cols { get { return Grid.Cols; } set { Grid.Cols = value; OnPropertyChanged(); } }

        [XmlElement("KeyGroup")] public List<InteractorProfile> Profiles { get; set; } = new List<InteractorProfile>();

        [XmlElement("Content")] public XmlInteractors Content { get; set; } = new XmlInteractors();
        [XmlIgnore] public List<Interactor> Interactors { get { return Content.Interactors; } set { Content.Interactors = value; } }

        public static XmlKeyboard ReadFromFile(string inputFilename)
        {
            XmlKeyboard keyboard;

            // If no extension given, try ".xml"
            string ext = Path.GetExtension(inputFilename);
            bool exists = File.Exists(inputFilename);
            if (!File.Exists(inputFilename) &&
                string.IsNullOrEmpty(Path.GetExtension(inputFilename)))
            {
                inputFilename += ".xml";
            }

            // Read in XML file (may throw)
            XmlSerializer serializer = new XmlSerializer(typeof(XmlKeyboard));
            using (FileStream readStream = new FileStream(@inputFilename, FileMode.Open))
            {
                keyboard = (XmlKeyboard)serializer.Deserialize(readStream);
            }

            keyboard.PostProcessXml();
            return keyboard;
        }

        public static XmlKeyboard ReadFromString(string xmlString)
        {
            if (string.IsNullOrEmpty(xmlString)) { return null; }

            var keyboard = new XmlKeyboard();
            var serializer = new XmlSerializer(typeof(XmlKeyboard));
            try
            {
                keyboard = (XmlKeyboard)serializer.Deserialize(new StringReader(xmlString));
                keyboard.PostProcessXml();                
                return keyboard;
            }
            catch
            {
                Log.ErrorFormat("Error reading keyboard from string: '{0}'", xmlString);
                return null;
            }
        }

        public void WriteToFile(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                return;

            foreach (var interactor in Interactors)
            {
                interactor.ProfileNames = new List<XmlElementValue>();
                foreach (var p in interactor.Profiles.Where(x => x.IsMember && x.Profile.Name != "All").Select(y => y.Profile.Name))
                {
                    interactor.ProfileNames.Add(new XmlElementValue() { Value = p });
                }
            }

            var serializer = new XmlSerializer(typeof(XmlKeyboard));
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var sw = new StreamWriter(filename);
            var xmlWriter = XmlWriter.Create(sw, new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true });

            serializer.Serialize(xmlWriter, this, ns);
            sw.Close();
        }

        public string WriteToString()
        {
            foreach (var interactor in Interactors)
            {
                interactor.ProfileNames.Clear();
                foreach (var p in interactor.Profiles.Where(x => x.IsMember && x.Profile.Name != "All").Select(y => y.Profile.Name))
                {
                    interactor.ProfileNames.Add(new XmlElementValue() { Value = p });
                }
            }

            var serializer = new XmlSerializer(typeof(XmlKeyboard));

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var sw = new StringWriter();
            var xmlWriter = XmlWriter.Create(sw, new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true });

            serializer.Serialize(xmlWriter, this, ns);
            return sw.ToString();
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

        private void ErrorPositioningItem(Interactor interactor)
        {
            var line1 = "Insufficient space to position item "
                + (Interactors.IndexOf(interactor) + 1)
                + " of " + Interactors.Count;

            var line2 = (interactor is DynamicKey dynamicKey)
                ? (!string.IsNullOrEmpty(dynamicKey.Label)) ? " with label '" + dynamicKey.Label + "'"
                    : (!string.IsNullOrEmpty(dynamicKey.Symbol)) ? " with symbol '" + dynamicKey.Symbol + "'"
                    : " with no label or symbol"
                : (interactor is DynamicScratchpad) ? " with type of Scratchpad"
                : " with type of Suggestion";

            var line3 = (interactor.RowN > -1 && interactor.ColN > -1)
                    ? " at row " + interactor.RowN + " column " + interactor.ColN
                    : " having width " + interactor.WidthN + " and height " + interactor.HeightN;

            ErrorMessage = line1 + line2 + line3;
            Log.ErrorFormat("Invalid keyboard file", ErrorMessage);
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

        private Tuple<int, int> FindItemPositions(List<List<int>> list, Interactor interactor, Tuple<int, int> rowCol)
        {
            var row = rowCol.Item1;
            var col = rowCol.Item2;
            if (interactor.RowN > -1 && interactor.RowN != row)
            {
                row = interactor.RowN;
                col = 0;
            }

            if (interactor.ColN > -1)
                col = interactor.ColN;

            if (ListContainsWidthAndHeight(list, row, col, interactor.WidthN, interactor.HeightN))
                rowCol = new Tuple<int, int>(row, col);

            else if (interactor.ColN < 0)
            {
                var newCol = FindCol(list, row, col + 1, interactor.WidthN, interactor.HeightN);
                if (newCol > -1)
                    rowCol = new Tuple<int, int>(row, newCol);
                else if (interactor.RowN < 0)
                {
                    for (int newRow = row + 1; newRow < list.Count; newRow++)
                    {
                        newCol = FindCol(list, newRow, 0, interactor.WidthN, interactor.HeightN);
                        if (newCol > -1)
                        {
                            rowCol = new Tuple<int, int>(newRow, newCol);
                            break;
                        }
                    }
                }
            }
            else if (interactor.RowN < 0)
                rowCol = new Tuple<int, int>(FindRow(list, row + 1, col, interactor.WidthN, interactor.HeightN), col);
            else
                rowCol = new Tuple<int, int>(-1, -1);

            //if enough empty space is not found then report an error
            if (rowCol.Item1 < 0 || rowCol.Item2 < 0)
            {
                ErrorPositioningItem(interactor);
                return rowCol;
            }

            interactor.RowN = rowCol.Item1;
            interactor.ColN = rowCol.Item2;
            for (int i = interactor.RowN; i < interactor.RowN + interactor.HeightN; i++)
            {
                list[i].RemoveAll(x => x >= interactor.ColN && x < interactor.ColN + interactor.WidthN);
            }
            return new Tuple<int, int>(interactor.RowN, interactor.ColN + interactor.WidthN);
        }

        private void PostProcessXml()
        {
            if (!Profiles.Exists(x => x.Name.ToUpper() == "ALL"))
                Profiles.Insert(0, new InteractorProfile() { Name="All"});
            else
            {
                var all = Profiles.Find(x => x.Name.ToUpper() == "ALL");
                if (Profiles.IndexOf(all) != 0)
                {
                    Profiles.Remove(all);
                    Profiles.Insert(0, all);
                }
            }

            UpgradeKeyboardXml();

            //start with a list of all grid cells marked empty
            var openGrid = new List<List<int>>();
            for (int r = 0; r < Rows; r++)
            {
                var gridRow = new List<int>();
                for (int c = 0; c < Cols; c++)
                {
                    gridRow.Add(c);
                }
                openGrid.Add(gridRow);
            }

            var rowCol = new Tuple<int, int>(0, 0);
            foreach (Interactor interactor in Interactors.Where(x => x.RowN >= 0 && x.ColN >= 0))
            {
                rowCol = FindItemPositions(openGrid, interactor, rowCol);
                if (rowCol.Item1 < 0 || rowCol.Item2 < 0)
                    return;
            }

            //process all items in the order listed in the xml file and place them in position
            //if an item has a row or column designation it is treated as an
            //indication to jump to that position and continue from there
            rowCol = new Tuple<int, int>(0, 0);
            foreach (Interactor interactor in Interactors)
            {
                if (interactor.RowN < 0 || interactor.ColN < 0)
                {
                    rowCol = FindItemPositions(openGrid, interactor, rowCol);
                    if (rowCol.Item1 < 0 || rowCol.Item2 < 0)
                        return;
                }
            }
        }

        private void UpgradeKeyboardXml()
        {
            if (LegacyKeys != null)
            {
                if (LegacyKeys.Interactors != null && LegacyKeys.Interactors.Any())
                {
                    Interactors.AddRange(LegacyKeys.Interactors);
                }
                LegacyKeys = null;
            }
            foreach (var profile in Profiles)
            {
                //Move any legacy XmlElement values into XmlAttribute properties
                UpgradeProfileXml(profile);
            }
            foreach (Interactor interactor in Interactors)
            {
                //Move any legacy XmlElement values into XmlAttribute properties
                UpgradeInteractorXml(interactor);

                foreach (var profile in Profiles)
                {
                    interactor.Profiles.Add(new InteractorProfileMap(profile,
                        interactor.ProfileNames.Exists(x => x.Value == profile.Name)));
                }
            }
        }

        private void UpgradeInteractorXml(Interactor interactor)
        {
            if (interactor.LegacyRow != null)
            {
                interactor.Row = interactor.LegacyRow;
                interactor.LegacyRow = null;
            }
            if (interactor.LegacyCol != null)
            {
                interactor.Col = interactor.LegacyCol;
                interactor.LegacyCol = null;
            }
            if (interactor.LegacyWidth != null)
            {
                interactor.Width = interactor.LegacyWidth;
                interactor.LegacyWidth = null;
            }
            if (interactor.LegacyHeight != null)
            {
                interactor.Height = interactor.LegacyHeight;
                interactor.LegacyHeight = null;
            }
            if (interactor.LegacyLabel != null)
            {
                interactor.Label = interactor.LegacyLabel;
                interactor.LegacyLabel = null;
            }
            if (interactor.LegacyShiftUpLabel != null)
            {
                interactor.Label = interactor.LegacyShiftUpLabel;
                interactor.LegacyShiftUpLabel = null;
            }
            if (interactor.LegacyShiftDownLabel != null)
            {
                interactor.ShiftDownLabel = interactor.LegacyShiftDownLabel;
                interactor.LegacyShiftDownLabel = null;
            }
            if (interactor.LegacySymbol != null)
            {
                interactor.Symbol = interactor.LegacySymbol;
                interactor.LegacySymbol = null;
            }
            if (interactor.LegacyDestinationKeyboard != null)
            {
                interactor.Commands.Add(new ChangeKeyboardCommand() { Value = interactor.LegacyDestinationKeyboard, BackReturnsHereAsString = interactor.LegacyReturnToThisKeyboard });
                interactor.LegacyDestinationKeyboard = null;
                interactor.LegacyReturnToThisKeyboard = null;
            }
            /* Update schema for Plugins from this:
            <PluginKey>
                <Plugin>NameOfPlugin</Plugin>
                <Method>CalledMethod</Method>
                <Arguments>
                      <Argument>
                            <Name>One</Name>
                            <Value>1</Value>
                      </Argument>
                      <Argument>
                            <Name>Two</Name>
                            <Value>2</Value>
                      </Argument>
                </Arguments>
            </PluginKey>

            or this:
            <DynamicKey>
                <Plugin>
                    <Name>NameOfPlugin</Name>
                    <Method>CalledMethod</Method>
                        <Argument>
                            <Name>One</Name>
                            <Value>1</Value>
                        </Argument>
                        <Argument>
                            <Name>Two</Name>
                            <Value>2</Value>
                      </Argument>
                </Plugin>
            </DynamicKey>

            to this:
            <DynamicKey>
                <Plugin Name="NameOfPlugin" Method="CalledMethod">
                    <Argument Name="One" Value="1"/>
                    <Argument Name="Two" Value="2"/>
                </Plugin>
            </DynamicKey>
            */
            foreach (PluginCommand command in interactor.Commands.Where(x => x is PluginCommand))
            {
                if (command.Value != null)
                {
                    command.Name = command.Value;
                    command.Value = null;
                }
                if (interactor.LegacyMethod != null)
                {
                    command.Method = interactor.LegacyMethod;
                    interactor.LegacyMethod = null;
                }
                if (interactor.LegacyArguments != null && interactor.LegacyArguments.Any())
                {
                    command.Arguments = interactor.LegacyArguments;
                    interactor.LegacyArguments = null;
                }
                if (command.LegacyName != null)
                {
                    command.Name = command.LegacyName;
                    command.LegacyName = null;
                }
                if (command.LegacyMethod != null)
                {
                    command.Method = command.LegacyMethod;
                    command.LegacyMethod = null;
                }

                if (command.Arguments != null && command.Arguments.Any()
                    && command.Arguments.First().LegacyArgumentList != null
                    && command.Arguments.First().LegacyArgumentList.Any())
                {
                    command.Arguments = command.Arguments.First().LegacyArgumentList;
                }
                foreach (var argument in command.Arguments)
                {
                    argument.LegacyArgumentList = null;
                    if (argument.LegacyName != null)
                    {
                        argument.Name = argument.LegacyName;
                        argument.LegacyName = null;
                    }
                    if (argument.LegacyValue != null)
                    {
                        argument.Value = argument.LegacyValue;
                        argument.LegacyValue = null;
                    }
                }
            }

            UpgradeProfileXml(interactor);
        }

        private void UpgradeProfileXml(Interactor profile)
        {
            if (profile.LegacySharedSizeGroup != null)
            {
                profile.SharedSizeGroup = profile.LegacySharedSizeGroup;
                profile.LegacySharedSizeGroup = null;
            }
            if (profile.LegacyAutoScaleToOneKeyWidth != null)
            {
                profile.AutoScaleToOneKeyWidth = profile.LegacyAutoScaleToOneKeyWidth;
                profile.LegacyAutoScaleToOneKeyWidth = null;
            }
            if (profile.LegacyAutoScaleToOneKeyHeight != null)
            {
                profile.AutoScaleToOneKeyHeight = profile.LegacyAutoScaleToOneKeyHeight;
                profile.LegacyAutoScaleToOneKeyHeight = null;
            }
            if (profile.LegacyUsePersianCompatibilityFont != null)
            {
                profile.UsePersianCompatibilityFont = profile.LegacyUsePersianCompatibilityFont;
                profile.LegacyUsePersianCompatibilityFont = null;
            }
            if (profile.LegacyUseUnicodeCompatibilityFont != null)
            {
                profile.UseUnicodeCompatibilityFont = profile.LegacyUseUnicodeCompatibilityFont;
                profile.LegacyUseUnicodeCompatibilityFont = null;
            }
            if (profile.LegacyUseUrduCompatibilityFont != null)
            {
                profile.UseUrduCompatibilityFont = profile.LegacyUseUrduCompatibilityFont;
                profile.LegacyUseUrduCompatibilityFont = null;
            }
            if (profile.LegacyBackgroundColor != null)
            {
                profile.BackgroundColor = profile.LegacyBackgroundColor;
                profile.LegacyBackgroundColor = null;
            }
        }

        //Legacy
        [XmlElement("Keys")] public XmlInteractors LegacyKeys { get; set; }
    }
}