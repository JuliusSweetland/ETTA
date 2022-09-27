// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using JuliusSweetland.OptiKey.Enums;
using JuliusSweetland.OptiKey.Extensions;
using JuliusSweetland.OptiKey.Models;
using JuliusSweetland.OptiKey.Services;
using JuliusSweetland.OptiKey.UI.Controls;
using JuliusSweetland.OptiKey.UI.ValueConverters;
using JuliusSweetland.OptiKey.UI.Windows;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;

namespace JuliusSweetland.OptiKey.UI.Views.Keyboards.Common
{
    /// <summary>
    /// Interaction logic for DynamicKeyboard.xaml
    /// </summary>
    public partial class DynamicKeyboard : KeyboardView
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly MainWindow mainWindow;
        private readonly string inputFilename;
        private readonly XmlKeyboard keyboard;
        private readonly IList<Tuple<KeyValue, KeyValue>> keyFamily;
        private readonly IDictionary<string, List<KeyValue>> keyValueByGroup;
        private readonly IDictionary<KeyValue, TimeSpanOverrides> overrideTimesByKey;
        private readonly IWindowManipulationService windowManipulationService;

        public DynamicKeyboard(
            MainWindow parentWindow, 
            string inputFile, 
            IList<Tuple<KeyValue, KeyValue>> keyFamily, 
            IDictionary<string, List<KeyValue>> keyValueByGroup, 
            IDictionary<KeyValue, TimeSpanOverrides> overrideTimesByKey,
            IWindowManipulationService windowManipulationService)
        {
            InitializeComponent();
            this.mainWindow = parentWindow;
            inputFilename = inputFile;
            this.keyFamily = keyFamily;
            this.keyValueByGroup = keyValueByGroup;
            this.overrideTimesByKey = overrideTimesByKey;
            this.windowManipulationService = windowManipulationService;

            // Read in XML file, exceptions get displayed to user
            if (string.IsNullOrEmpty(inputFilename))
            {
                Log.Error("No file specified for dynamic keyboard");
                SetupErrorLayout("Error loading file", "No file specified. Please choose a startup file in Management Console.");
                return;
            }
            try
            {
                keyboard = XmlKeyboard.ReadFromFile(inputFilename);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                SetupErrorLayout("Error loading file", SplitAndWrapExceptionInfo(e.ToString()));
                return;
            }

            if (!ValidateKeyboard()) { return; }

            // New logic for content keyboard
            if (keyboard.Content != null)
            {
                SetupGrid(); // Setup all the UI components
                if (!SetupDynamicItems()) { return; }
            }
            // Legacy logic
            else
            {
                if (!ValidateKeys()) { return; }
                SetupGrid(); // Setup all the UI components 
                SetupKeys();
            }

            SetupStyle(); // Set the override border and background colors 
        }

        private bool ValidateKeyboard()
        {
            string errorMessage = null;
            if (!string.IsNullOrWhiteSpace(keyboard.WindowState) && Enum.TryParse(keyboard.WindowState, out WindowStates validWindowState)
                                                                 && validWindowState != WindowStates.Docked && validWindowState != WindowStates.Floating && validWindowState != WindowStates.Maximised)
                errorMessage = "WindowState not valid";
            else if (!string.IsNullOrWhiteSpace(keyboard.Position) && !Enum.TryParse<MoveToDirections>(keyboard.Position, out _))
                errorMessage = "Position not valid";
            else if (!string.IsNullOrWhiteSpace(keyboard.DockSize) && !Enum.TryParse<DockSizes>(keyboard.DockSize, out var validDockSize))
                errorMessage = "DockSize not valid";
            else if (!string.IsNullOrWhiteSpace(keyboard.Width) &&
                !(double.TryParse(keyboard.Width.Replace("%", ""), out var validNumber) && validNumber >= -9999 && validNumber <= 9999))
                errorMessage = "Width must be between -9999 and 9999";
            else if (!string.IsNullOrWhiteSpace(keyboard.Height) &&
                !(double.TryParse(keyboard.Height.Replace("%", ""), out validNumber) && validNumber >= -9999 && validNumber <= 9999))
                errorMessage = "Height must be between -9999 and 9999";
            else if (!string.IsNullOrWhiteSpace(keyboard.HorizontalOffset) &&
                !(double.TryParse(keyboard.HorizontalOffset.Replace("%", ""), out validNumber) && validNumber >= -9999 && validNumber <= 9999))
                errorMessage = "Offset must be between -9999 and 9999";
            else if (!string.IsNullOrWhiteSpace(keyboard.VerticalOffset) &&
                !(double.TryParse(keyboard.VerticalOffset.Replace("%", ""), out validNumber) && validNumber >= -9999 && validNumber <= 9999))
                errorMessage = "Offset must be between -9999 and 9999";

            else if (keyboard.Grid == null)
                errorMessage = "No grid definition found";
            else if (keyboard.Grid.Rows < 1 || keyboard.Grid.Cols < 1)
                errorMessage = "Grid size is " + keyboard.Grid.Rows + " rows and " + keyboard.Grid.Cols + " columns";
            else if ((keyboard.Keys == null || keyboard.Keys.Count == 0) && keyboard.Content == null)
                errorMessage = "No key or content definitions found";

            if (errorMessage != null)
            {
                SetupErrorLayout("Invalid keyboard file", errorMessage);
                return false;
            }

            // If the keyboard overrides any size/position values, tell the windowsManipulationService that it shouldn't be persisting state changes
            if (!string.IsNullOrWhiteSpace(keyboard.WindowState)
                || !string.IsNullOrWhiteSpace(keyboard.Position)
                || !string.IsNullOrWhiteSpace(keyboard.DockSize)
                || !string.IsNullOrWhiteSpace(keyboard.Width)
                || !string.IsNullOrWhiteSpace(keyboard.Height)
                || !string.IsNullOrWhiteSpace(keyboard.HorizontalOffset)
                || !string.IsNullOrWhiteSpace(keyboard.VerticalOffset))
            {
                Log.InfoFormat("Overriding size and position for dynamic keyboard");
                if (windowManipulationService != null)
                windowManipulationService.OverridePersistedState(keyboard.PersistNewState, keyboard.WindowState,
                    keyboard.Position, keyboard.DockSize, keyboard.Width, keyboard.Height, keyboard.HorizontalOffset,
                    keyboard.VerticalOffset);
            }

            return true;
        }

        private bool ValidateKeys()
        {
            var allKeys = keyboard.Keys.ActionKeys.Cast<IXmlKey>()
                .Concat(keyboard.Keys.ChangeKeyboardKeys)
                .Concat(keyboard.Keys.DynamicKeys)
                .Concat(keyboard.Keys.PluginKeys)
                .Concat(keyboard.Keys.TextKeys)
                .ToList();

            var duplicates = allKeys
                .GroupBy(key => new Tuple<int, int>(key.Row, key.Col))
                .Where(group => group.Count() > 1)
                .Select(group => group.ToList())
                .ToList();

            if (duplicates.Count == 0)
                return true;

            var errorMsg = duplicates.Select(keys =>
            {
                var keyStrings = keys.Select(GetKeyString).Aggregate((seq, next) => $"{seq}, {next}");
                return $"{keyStrings} ({keys.First().Row}, {keys.First().Col})";
            }).Aggregate((msg, key) => $"{msg}, {key}");

            SetupErrorLayout("Duplicate row/column values for keys", errorMsg);
            return false;
        }

        private string GetKeyString(IXmlKey xmlKey)
        {
            if (xmlKey is XmlTextKey textKey)
                return textKey.Text;

            return xmlKey.Label ?? xmlKey.Symbol;
        }

        private Key CreateKeyWithBasicProps(XmlKey xmlKey, int minKeyWidth, int minKeyHeight)
        {
            // Add the core properties from XML to a new key
            Key newKey = new Key();
            if (xmlKey.ShiftDownLabel != null && xmlKey.ShiftUpLabel != null)
            {
                newKey.ShiftUpText = xmlKey.ShiftUpLabel.ToStringWithValidNewlines();
                newKey.ShiftDownText = xmlKey.ShiftDownLabel.ToStringWithValidNewlines();
            }
            else if (xmlKey.Label != null)
            {
                string xmlKeyLabel = xmlKey.Label;
                string oldValue;
                string newValue;
                while (xmlKeyLabel.Contains("{Resource:"))
                {
                    oldValue = xmlKeyLabel.Substring(xmlKeyLabel.IndexOf("{Resource:"), xmlKeyLabel.IndexOf("}", xmlKeyLabel.IndexOf("{Resource:")) - xmlKeyLabel.IndexOf("{Resource:") + 1);
                    newValue = Properties.Resources.ResourceManager.GetString(oldValue.Substring(10, oldValue.Length - 11).Trim());
                    xmlKeyLabel = xmlKeyLabel.Replace(oldValue, newValue);
                }
                while (xmlKeyLabel.Contains("{Setting:"))
                {
                    oldValue = xmlKeyLabel.Substring(xmlKeyLabel.IndexOf("{Setting:"), xmlKeyLabel.IndexOf("}", xmlKeyLabel.IndexOf("{Setting:")) - xmlKeyLabel.IndexOf("{Setting:") + 1);
                    newValue = Properties.Settings.Default[oldValue.Substring(9, oldValue.Length - 10).Trim()].ToString();
                    xmlKeyLabel = xmlKeyLabel.Replace(oldValue, newValue);
                }

                newKey.Text = xmlKeyLabel.ToStringWithValidNewlines();
            }
            else if (xmlKey.Label != null)
            {
                newKey.Text = xmlKey.Label.ToStringWithValidNewlines();
            }

            if (xmlKey.Symbol != null)
            {
                Geometry geom = (Geometry)this.Resources[xmlKey.Symbol];
                if (geom != null)
                {
                    newKey.SymbolGeometry = geom;
                }
                else
                {
                    Log.ErrorFormat("Could not parse {0} as symbol geometry", xmlKey.Symbol);
                }
            }

            // Add same symbol margin to all keys
            if (keyboard.SymbolMargin.HasValue)
            {
                newKey.SymbolMargin = keyboard.SymbolMargin.Value;
            }

            // Set shared size group
            if (!string.IsNullOrEmpty(xmlKey.SharedSizeGroup))
            {
                newKey.SharedSizeGroup = xmlKey.SharedSizeGroup;
            }
            else
            {
                bool hasSymbol = newKey.SymbolGeometry != null;
                bool hasString = xmlKey.Label != null || xmlKey.ShiftUpLabel != null || xmlKey.ShiftDownLabel != null;
                if (hasSymbol && hasString)
                {
                    newKey.SharedSizeGroup = "KeyWithSymbolAndText";
                }
                else if (hasSymbol)
                {
                    newKey.SharedSizeGroup = "KeyWithSymbol";
                }
                else if (hasString)
                {
                    var text = newKey.Text != null ? newKey.Text.Compose()
                        : newKey.ShiftDownText != null ? newKey.ShiftDownText.Compose()
                        : newKey.ShiftUpText?.Compose();

                    //Strip out circle character used to show diacritic marks
                    text = text?.Replace("\x25CC", string.Empty);

                    newKey.SharedSizeGroup = text != null && text.Length > 5
                        ? "KeyWithLongText" : text != null && text.Length > 1
                        ? "KeyWithShortText" : "KeyWithSingleLetter";
                }
            }

            //Auto set width span and height span
            if (xmlKey.AutoScaleToOneKeyWidth)
            {
                newKey.WidthSpan = (double)xmlKey.Width / (double)minKeyWidth;
            }

            if (xmlKey.AutoScaleToOneKeyHeight)
            {
                newKey.HeightSpan = (double)xmlKey.Height / (double)minKeyHeight;
            }

            newKey.UsePersianCompatibilityFont = xmlKey.UsePersianCompatibilityFont;
            newKey.UseUnicodeCompatibilityFont = xmlKey.UseUnicodeCompatibilityFont;
            newKey.UseUrduCompatibilityFont = xmlKey.UseUrduCompatibilityFont;

            if (!string.IsNullOrEmpty(xmlKey.BackgroundColor)
               && (Regex.IsMatch(xmlKey.BackgroundColor, "^(#[0-9A-Fa-f]{3})$|^(#[0-9A-Fa-f]{6})$")
                   || System.Drawing.Color.FromName(xmlKey.BackgroundColor).IsKnownColor))
            {
                newKey.BackgroundColourOverride = (SolidColorBrush)new BrushConverter().ConvertFrom(xmlKey.BackgroundColor);
            }

            return newKey;
        }

        private string SplitAndWrapExceptionInfo(string info)
        {
            // Take first line of error message
            info = info.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)[0];

            // Wrap to (approx) three lines
            var len = info.Length;
            var maxLineLength = len / 3.5;
            Log.Info(maxLineLength);
            char[] space = { ' ' };

            var charCount = 0;
            var allLines = info.Split(space)
                .GroupBy(w => (int)((charCount += w.Length + 1) / maxLineLength))
                .Select(g => string.Join(" ", g));

            return string.Join(Environment.NewLine, allLines);
        }

        private void SetupErrorLayout(string heading, string content)
        {
            if (MainGrid.Children.Count > 0)
                MainGrid.Children.RemoveRange(0, MainGrid.Children.Count);
            if (MainGrid.ColumnDefinitions.Count > 0)
                MainGrid.ColumnDefinitions.RemoveRange(0, MainGrid.ColumnDefinitions.Count);
            if (MainGrid.RowDefinitions.Count > 0)
                MainGrid.RowDefinitions.RemoveRange(0, MainGrid.RowDefinitions.Count);
            AddRowsToGrid(4);
            AddColsToGrid(4);

            // Top middle two cells are main error message
            {
                var newKey = new Key {Text = heading};
                PlaceKeyInPosition(newKey, 0, 1, 1, 2);
            }

            // Middle row is detailed error message
            {
                var newKey = new Key {Text = content};
                PlaceKeyInPosition(newKey, 1, 0, 2, 4);
            }

            // Back key
            {
                var newKey = new Key
                {
                    SymbolGeometry = (Geometry)Application.Current.Resources["BackIcon"],
                    Text = Properties.Resources.BACK,
                    Value = KeyValues.BackFromKeyboardKey
                };
                PlaceKeyInPosition(newKey, 3, 3);
            }

            // Fill in empty keys
            {
                var newKey = new Key();
                PlaceKeyInPosition(newKey, 0, 0, 1, 1);
            }
            {
                var newKey = new Key();
                PlaceKeyInPosition(newKey, 0, 3, 1, 1);
            }
            {
                var newKey = new Key();
                PlaceKeyInPosition(newKey, 3, 0, 1, 1);
            }
            {
                var newKey = new Key();
                PlaceKeyInPosition(newKey, 3, 1, 1, 2);
            }
        }

        private void DynamicItemError(XmlDynamicItem dynamicItem)
        {
            var line1 = "Insufficient space to position item "
                + (keyboard.Content.Items.IndexOf(dynamicItem) + 1)
                + " of " + keyboard.Content.Items.Count;

            var line2 = (dynamicItem is XmlDynamicKey dynamicKey)
                ? (!string.IsNullOrEmpty(dynamicKey.Label)) ? " with label '" + dynamicKey.Label + "'"
                    : (!string.IsNullOrEmpty(dynamicKey.Symbol)) ? " with symbol '" + dynamicKey.Symbol + "'"
                    : " with no label or symbol"
                : (dynamicItem is XmlDynamicScratchpad) ? " with type of Scratchpad"
                : " with type of Suggestion";

            var line3 = (dynamicItem.Row > -1 && dynamicItem.Col > -1)
                    ? " at row " + dynamicItem.Row + " column " + dynamicItem.Col
                    : " having width " + dynamicItem.Width + " and height " + dynamicItem.Height;

            SetupErrorLayout("Invalid keyboard file", line1 + line2 + line3);
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

        private Tuple<int, int> FindItemPositions(List<List<int>> list, XmlDynamicItem dynamicItem, int row, int col)
        {
            if (dynamicItem.Row > -1 && dynamicItem.Row != row)
            {
                row = dynamicItem.Row;
                col = 0;
            }

            if (dynamicItem.Col > -1)
                col = dynamicItem.Col;

            if (ListContainsWidthAndHeight(list, row, col, dynamicItem.Width, dynamicItem.Height))
                return new Tuple<int, int>(row, col);

            if (dynamicItem.Col < 0)
            {
                var newCol = FindCol(list, row, col + 1, dynamicItem.Width, dynamicItem.Height);
                if (newCol > -1)
                    return new Tuple<int, int>(row, newCol);
                else if (dynamicItem.Row < 0)
                {
                    for (int newRow = row + 1; newRow < list.Count; newRow++)
                    {
                        newCol = FindCol(list, newRow, 0, dynamicItem.Width, dynamicItem.Height);
                        if (newCol > -1)
                            return new Tuple<int, int>(newRow, newCol);
                    }
                }
            }
            else if (dynamicItem.Row < 0)
                return new Tuple<int, int>(FindRow(list, row + 1, col, dynamicItem.Width, dynamicItem.Height), col);

            return new Tuple<int, int>(-1, -1);
        }

        private bool SetupDynamicItems()
        {
            //create an item list that excludes popups
            var itemList = keyboard.Content.Items.Where(x => !(x is XmlDynamicPopup)).ToList();
            var minKeyWidth = itemList.Select(k => k.Width).Min() > 0 ? itemList.Select(k => k.Width).Min() : 1;
            var minKeyHeight = itemList.Select(k => k.Height).Min() > 0 ? itemList.Select(k => k.Height).Min() : 1;

            //start with a list of all grid cells marked empty
            var openGrid = new List<List<int>>();
            for (int r = 0; r < keyboard.Grid.Rows; r++)
            {
                var gridRow = new List<int>();
                for (int c = 0; c < keyboard.Grid.Cols; c++)
                {
                    gridRow.Add(c);
                }
                openGrid.Add(gridRow);
            }

            //process all items in the order listed in the xml file and place them in position
            //if an item has a row or column designation it is treated as an
            //indication to jump to that position and continue from there
            var rowCol = new Tuple<int, int>(0, 0);
            foreach (XmlDynamicItem dynamicItem in itemList)
            {
                rowCol = FindItemPositions(openGrid, dynamicItem, rowCol.Item1, rowCol.Item2);

                //if there is not enough empty space is not found then return an error
                if (rowCol.Item1 < 0 || rowCol.Item2 < 0)
                {
                    DynamicItemError(dynamicItem);
                    return false;
                }

                dynamicItem.Row = rowCol.Item1;
                dynamicItem.Col = rowCol.Item2;
                for (int i = dynamicItem.Row; i < dynamicItem.Row + dynamicItem.Height; i++)
                {
                    openGrid[i].RemoveAll(x => x >= dynamicItem.Col && x < dynamicItem.Col + dynamicItem.Width);
                }

                SetupDynamicItem(dynamicItem, minKeyWidth, minKeyHeight);
                rowCol = new Tuple<int, int>(rowCol.Item1, rowCol.Item2 + dynamicItem.Width);
            }

            //process popups
            var popupList = keyboard.Content.Items.Where(x => x is XmlDynamicPopup).ToList();
            foreach (XmlDynamicKey popup in popupList)
            {
                SetupDynamicItem(popup, minKeyWidth, minKeyHeight);
            }
            return true;
        }

        private void SetupDynamicItem(XmlDynamicItem dynamicItem, int minKeyWidth, int minKeyHeight)
        {
            if (dynamicItem is XmlDynamicKey xmlDynamicKey)
            {
                AddDynamicKey(xmlDynamicKey, minKeyWidth, minKeyHeight);
            }
            else if (dynamicItem is XmlDynamicOutputPanel)
            {
                var outputPanel = new Output();
                MainGrid.Children.Add(outputPanel);
                Grid.SetColumn(outputPanel, dynamicItem.Col);
                Grid.SetRow(outputPanel, dynamicItem.Row);
                Grid.SetColumnSpan(outputPanel, dynamicItem.Width);
                Grid.SetRowSpan(outputPanel, dynamicItem.Height);
                if (ValidColor(dynamicItem.BackgroundColor, out var colorBrush))
                    outputPanel.Background = colorBrush;
                if (ValidColor(dynamicItem.ForegroundColor, out colorBrush))
                    outputPanel.Foreground = colorBrush;
                if (!string.IsNullOrEmpty(dynamicItem.Opacity) && double.TryParse(dynamicItem.Opacity, out var opacity))
                    outputPanel.Opacity = opacity;
            }
            else if (dynamicItem is XmlDynamicSuggestionRow)
            {
                var suggestionRow = new SuggestionRow();
                MainGrid.Children.Add(suggestionRow);
                Grid.SetColumn(suggestionRow, dynamicItem.Col);
                Grid.SetRow(suggestionRow, dynamicItem.Row);
                Grid.SetColumnSpan(suggestionRow, dynamicItem.Width);
                Grid.SetRowSpan(suggestionRow, dynamicItem.Height);

                if (ValidColor(dynamicItem.BackgroundColor, out var colorBrush))
                {
                    suggestionRow.Background = colorBrush;
                    suggestionRow.DisabledBackgroundColourOverride = colorBrush;
                }
                if (ValidColor(dynamicItem.ForegroundColor, out colorBrush))
                    suggestionRow.Foreground = colorBrush;

                if (!string.IsNullOrEmpty(dynamicItem.Opacity) && double.TryParse(dynamicItem.Opacity, out var opacity))
                    suggestionRow.OpacityOverride = opacity;
            }
            else if (dynamicItem is XmlDynamicSuggestionCol)
            {
                var suggestionCol = new SuggestionCol();
                MainGrid.Children.Add(suggestionCol);
                Grid.SetColumn(suggestionCol, dynamicItem.Col);
                Grid.SetRow(suggestionCol, dynamicItem.Row);
                Grid.SetColumnSpan(suggestionCol, dynamicItem.Width);
                Grid.SetRowSpan(suggestionCol, dynamicItem.Height);

                if (ValidColor(dynamicItem.BackgroundColor, out var colorBrush))
                {
                    suggestionCol.Background = colorBrush;
                    suggestionCol.DisabledBackgroundColourOverride = colorBrush;
                }
                if (ValidColor(dynamicItem.ForegroundColor, out colorBrush))
                    suggestionCol.Foreground = colorBrush;

                if (!string.IsNullOrEmpty(dynamicItem.Opacity) && double.TryParse(dynamicItem.Opacity, out var suggestionColOpacity))
                    suggestionCol.Opacity = suggestionColOpacity;
            }
        }

        private void AddDynamicKey(XmlDynamicKey xmlDynamicKey, int minKeyWidth, int minKeyHeight)
        {
            if (xmlDynamicKey.Commands.Any())
            {
                var addCommandList = AddCommandList(xmlDynamicKey, xmlDynamicKey.Commands, minKeyWidth, minKeyHeight);
                if (addCommandList != null && addCommandList.Any())
                {
                    var xmlKeyValue = new KeyValue($"R{xmlDynamicKey.Row}-C{xmlDynamicKey.Col}")
                    {
                        Commands = addCommandList
                    };
                    CreateDynamicKey(xmlDynamicKey, xmlKeyValue, minKeyWidth, minKeyHeight);
                }
            }
            //place a key that performs no action
            else
                CreateDynamicKey(xmlDynamicKey, null, minKeyWidth, minKeyHeight);
        }

        private List<KeyCommand> AddCommandList(XmlDynamicKey xmlDynamicKey, List<KeyCommand> commands , int minKeyWidth, int minKeyHeight)
        {
            var xmlKeyValue = new KeyValue($"R{xmlDynamicKey.Row}-C{xmlDynamicKey.Col}");
            var commandList = new List<KeyCommand>();
            if (xmlDynamicKey.Commands.Any())
            {
                var rootDir = Path.GetDirectoryName(inputFilename);
                foreach (KeyCommand keyCommand in xmlDynamicKey.Commands)
                {
                    KeyValue commandKeyValue;
                    if (keyCommand is ActionCommand dynamicAction)
                    {
                        if (!Enum.TryParse(dynamicAction.Value, out FunctionKeys actionEnum))
                            Log.ErrorFormat("Could not parse {0} as function key", dynamicAction.Value);
                        else
                        {
                            commandKeyValue = new KeyValue(actionEnum);
                            if (xmlDynamicKey.Commands.Count == 1 && KeyValues.KeysWhichCanBeLockedDown.Contains(commandKeyValue))
                            {
                                CreateDynamicKey(xmlDynamicKey, commandKeyValue, minKeyWidth, minKeyHeight);
                                return null;
                            }
                            else
                                commandList.Add(new ActionCommand() { FunctionKey = actionEnum });

                            if (KeyValues.KeysWhichCanBeLockedDown.Contains(commandKeyValue) 
                                && !keyFamily.Contains(new Tuple<KeyValue, KeyValue>(xmlKeyValue, commandKeyValue)))
                            {
                                keyFamily.Add(new Tuple<KeyValue, KeyValue>(xmlKeyValue, commandKeyValue));
                            }
                        }
                    }
                    else if (keyCommand is ChangeKeyboardCommand dynamicLink)
                    {
                        if (string.IsNullOrEmpty(dynamicLink.Value))
                            Log.ErrorFormat("Destination Keyboard not found for {0} ", dynamicLink.Value);
                        else
                        {
                            var kb_link = Enum.TryParse(dynamicLink.Value, out Enums.Keyboards keyboardEnum)
                                ? dynamicLink.Value : Path.Combine(rootDir, dynamicLink.Value);

                            commandList.Add(new ChangeKeyboardCommand() { Value = kb_link, BackAction = dynamicLink.BackAction });
                        }
                    }
                    else if (keyCommand is KeyDownCommand dynamicKeyDown)
                    {
                        if (string.IsNullOrEmpty(dynamicKeyDown.Value))
                            Log.ErrorFormat("KeyDown text not found for {0} ", dynamicKeyDown.Value);
                        else
                        {
                            commandKeyValue = new KeyValue(dynamicKeyDown.Value);
                            commandList.Add(new KeyDownCommand() { Value = dynamicKeyDown.Value });
                            if (!keyFamily.Contains(new Tuple<KeyValue, KeyValue>(xmlKeyValue, commandKeyValue)))
                                keyFamily.Add(new Tuple<KeyValue, KeyValue>(xmlKeyValue, commandKeyValue));
                        }
                    }
                    else if (keyCommand is KeyTogglCommand dynamicKeyToggle)
                    {
                        if (string.IsNullOrEmpty(dynamicKeyToggle.Value))
                            Log.ErrorFormat("KeyToggle text not found for {0} ", dynamicKeyToggle.Value);
                        else
                        {
                            commandKeyValue = new KeyValue(dynamicKeyToggle.Value);
                            commandList.Add(new KeyTogglCommand() { Value = dynamicKeyToggle.Value }); ;
                            if (!keyFamily.Contains(new Tuple<KeyValue, KeyValue>(xmlKeyValue, commandKeyValue)))
                                keyFamily.Add(new Tuple<KeyValue, KeyValue>(xmlKeyValue, commandKeyValue));
                        }
                    }
                    else if (keyCommand is KeyUpCommand dynamicKeyUp)
                    {
                        if (string.IsNullOrEmpty(dynamicKeyUp.Value))
                            Log.ErrorFormat("KeyUp text not found for {0} ", dynamicKeyUp.Value);
                        else
                            commandList.Add(new KeyUpCommand() { Value = dynamicKeyUp.Value });
                    }
                    else if (keyCommand is MoveWindowCommand dynamicBounds)
                    {
                        commandList.Add(new MoveWindowCommand() { Value = dynamicBounds.Value } );
                    }
                    else if (keyCommand is TextCommand dynamicText)
                    {
                        if (string.IsNullOrEmpty(dynamicText.Value))
                            Log.ErrorFormat("Text not found for {0} ", dynamicText.Value);
                        else
                            commandList.Add(new TextCommand() { Value = dynamicText.Value });
                    }
                    else if (keyCommand is WaitCommand dynamicWait)
                    {
                        if (!int.TryParse(dynamicWait.Value, out _))
                            Log.ErrorFormat("Could not parse wait {0} as int value", dynamicWait.Value);
                        else
                            commandList.Add(new WaitCommand() { Value = dynamicWait.Value } );
                    }
                    else if (keyCommand is PluginCommand dynamicPlugin)
                    {
                        if (string.IsNullOrWhiteSpace(dynamicPlugin.Name.ToString()))
                            Log.ErrorFormat("Plugin not found for {0} ", dynamicPlugin.Value);
                        else if (string.IsNullOrWhiteSpace(dynamicPlugin.Method))
                            Log.ErrorFormat("Method not found for {0} ", dynamicPlugin.Value);
                        else
                            commandList.Add(new PluginCommand() { Value = dynamicPlugin.Name,
                                Method = dynamicPlugin.Method, Arguments = dynamicPlugin.Arguments } );
                    }
                    else if (keyCommand is LoopCommand dynamicLoop)
                    {
                        var vReturn = AddCommandList(xmlDynamicKey, dynamicLoop.Commands, minKeyWidth, minKeyHeight);
                        if (vReturn != null && vReturn.Any())
                            commandList.Add(new LoopCommand() { Value = dynamicLoop.Count.ToString(), Commands = vReturn } );
                        else
                            return null;
                    }
                }
            }
            else
            {
                Log.ErrorFormat("No value found in dynamic key with label {0}", xmlDynamicKey.Label);
            }
            return commandList;
        }

        private void CreateDynamicKey(XmlDynamicKey xmlKey, KeyValue xmlKeyValue, int minKeyWidth, int minKeyHeight)
        {
            // Add the core properties from XML to a new key
            var newKey = new Key { Value = xmlKeyValue };
            if (xmlKey is XmlDynamicPopup)
                newKey = new KeyPopup { Value = xmlKeyValue, GazeRegion = xmlKey.GazeRegion };

            //add this item's KeyValue to the 'ALL' KeyGroup list
            if (!keyValueByGroup.ContainsKey("ALL"))
                keyValueByGroup.Add("ALL", new List<KeyValue> { xmlKeyValue });
            else if (!keyValueByGroup["ALL"].Contains(xmlKeyValue))
                keyValueByGroup["ALL"].Add(xmlKeyValue);

            //add this item's KeyValue to each KeyGroup referenced in its definition
            foreach (KeyGroup vKeyGroup in xmlKey.KeyGroups)
            {
                if (!keyValueByGroup.ContainsKey(vKeyGroup.Value.ToUpper()))
                    keyValueByGroup.Add(vKeyGroup.Value.ToUpper(), new List<KeyValue> { xmlKeyValue });
                else if (!keyValueByGroup[vKeyGroup.Value.ToUpper()].Contains(xmlKeyValue))
                    keyValueByGroup[vKeyGroup.Value.ToUpper()].Add(xmlKeyValue);
            }

            if (xmlKey.Label != null)
            {
                string label = xmlKey.Label;
                string vText;
                string vLookup;
                while (label.Contains("{Resource:"))
                {
                    vText = label.Substring(label.IndexOf("{Resource:"), label.IndexOf("}", label.IndexOf("{Resource:")) - label.IndexOf("{Resource:") + 1);
                    vLookup = Properties.Resources.ResourceManager.GetString(vText.Substring(10, vText.Length - 11).Trim());
                    label = label.Replace(vText, vLookup);
                }
                while (label.Contains("{Setting:"))
                {
                    vText = label.Substring(label.IndexOf("{Setting:"), label.IndexOf("}", label.IndexOf("{Setting:")) - label.IndexOf("{Setting:") + 1);
                    vLookup = Properties.Settings.Default[vText.Substring(9, vText.Length - 10).Trim()].ToString();
                    label = label.Replace(vText, vLookup);
                }

                newKey.Text = label.ToStringWithValidNewlines();
            }

            if (xmlKey.Label != null && xmlKey.ShiftDownLabel != null)
            {
                newKey.ShiftUpText = xmlKey.Label.ToStringWithValidNewlines();
                newKey.ShiftDownText = xmlKey.ShiftDownLabel.ToStringWithValidNewlines();
            }

            if (xmlKey.Symbol != null)
            {
                Geometry geom = (Geometry)this.Resources[xmlKey.Symbol];
                if (geom != null)
                    newKey.SymbolGeometry = geom;
                else
                    Log.ErrorFormat("Could not parse {0} as symbol geometry", xmlKey.Symbol);
            }

            // Add same symbol margin to all keys
            if (keyboard.SymbolMargin.HasValue)
                newKey.SymbolMargin = keyboard.SymbolMargin.Value;

            //Create a list and add all the keyboard's attribute KeyGroup that are referenced by this key
            List<XmlKeyGroup> keyGroupList = new List<XmlKeyGroup>();
            keyGroupList.AddRange(keyboard.KeyGroups.Where(x => x.Name.ToUpper() == "ALL" || xmlKey.KeyGroups.Exists(y => y.Value == x.Name)));

            // Set shared size group
            if (!string.IsNullOrEmpty(xmlKey.SharedSizeGroup))
                newKey.SharedSizeGroup = xmlKey.SharedSizeGroup;
            else if (keyGroupList != null && keyGroupList.Exists(x => x.SharedSizeGroup != null))
                newKey.SharedSizeGroup = keyGroupList.Find(x => x.SharedSizeGroup != null).SharedSizeGroup;
            else
            {
                bool hasSymbol = newKey.SymbolGeometry != null;
                bool hasString = xmlKey.Label != null || xmlKey.ShiftDownLabel != null;
                if (hasSymbol && hasString)
                    newKey.SharedSizeGroup = "KeyWithSymbolAndText";
                else if (hasSymbol)
                    newKey.SharedSizeGroup = "KeyWithSymbol";
                else if (hasString)
                {
                    var text = newKey.Text != null ? newKey.Text.Compose() : newKey.ShiftDownText?.Compose();

                    //Strip out circle character used to show diacritic marks
                    text = text?.Replace("\x25CC", string.Empty);

                    newKey.SharedSizeGroup = text != null && text.Length > 5
                        ? "KeyWithLongText" : text != null && text.Length > 1
                        ? "KeyWithShortText" : "KeyWithSingleLetter";
                }
            }

            //Auto set width span and height span
            if (xmlKey.AutoScaleToOneKeyWidth.HasValue && xmlKey.AutoScaleToOneKeyWidth.Value)
                newKey.WidthSpan = (double)xmlKey.Width / (double)minKeyWidth;
            else if (!xmlKey.AutoScaleToOneKeyWidth.HasValue
                && (keyGroupList == null || keyGroupList.Exists(x => x.AutoScaleToOneKeyWidth.HasValue
                    && !x.AutoScaleToOneKeyWidth.Value)))
                newKey.WidthSpan = (double)xmlKey.Width / (double)minKeyWidth;
            if (xmlKey.AutoScaleToOneKeyHeight.HasValue && xmlKey.AutoScaleToOneKeyHeight.Value)
                newKey.WidthSpan = (double)xmlKey.Width / (double)minKeyWidth;
            else if (!xmlKey.AutoScaleToOneKeyHeight.HasValue
                && (keyGroupList == null || keyGroupList.Exists(x => x.AutoScaleToOneKeyHeight.HasValue
                    && !x.AutoScaleToOneKeyHeight.Value)))
                newKey.WidthSpan = (double)xmlKey.Width / (double)minKeyWidth;


            if (xmlKey.UsePersianCompatibilityFont)
                newKey.UsePersianCompatibilityFont = true;
            else if (keyGroupList != null && keyGroupList.Exists(x => x.UsePersianCompatibilityFont))
                newKey.UsePersianCompatibilityFont = true;

            if (xmlKey.UseUnicodeCompatibilityFont)
                newKey.UseUnicodeCompatibilityFont = true;
            else if (keyGroupList != null && keyGroupList.Exists(x => x.UseUnicodeCompatibilityFont))
                newKey.UseUnicodeCompatibilityFont = true;

            if (xmlKey.UseUrduCompatibilityFont)
                newKey.UseUrduCompatibilityFont = true;
            else if (keyGroupList != null && keyGroupList.Exists(x => x.UseUrduCompatibilityFont))
                newKey.UseUrduCompatibilityFont = true;

            if (ValidColor(xmlKey.ForegroundColor, out var colorBrush))
                newKey.ForegroundColourOverride = colorBrush;
            else if (keyGroupList != null && keyGroupList.Exists(x => ValidColor(x.ForegroundColor, out colorBrush)))
                newKey.ForegroundColourOverride = colorBrush;

            if (ValidColor(xmlKey.KeyDisabledForeground, out colorBrush))
                newKey.DisabledForegroundColourOverride = colorBrush;
            else if (keyGroupList != null && keyGroupList.Exists(x => ValidColor(x.KeyDisabledForeground, out colorBrush)))
                newKey.DisabledForegroundColourOverride = colorBrush;
            else if (newKey.ForegroundColourOverride != null)
                newKey.DisabledForegroundColourOverride = new SolidColorBrush(HlsColor.Fade(((SolidColorBrush)newKey.ForegroundColourOverride).Color, .15)); 

            if (ValidColor(xmlKey.KeyDownForeground, out colorBrush))
                newKey.KeyDownForegroundOverride = colorBrush;
            else if (keyGroupList != null && keyGroupList.Exists(x => ValidColor(x.KeyDownForeground, out colorBrush)))
                newKey.KeyDownForegroundOverride = colorBrush;
            else if (newKey.ForegroundColourOverride != null)
                newKey.KeyDownForegroundOverride = new SolidColorBrush(HlsColor.Fade(((SolidColorBrush)newKey.ForegroundColourOverride).Color, .15));

            if (ValidColor(xmlKey.BackgroundColor, out colorBrush))
                newKey.BackgroundColourOverride = colorBrush;
            else if (keyGroupList != null && keyGroupList.Exists(x => ValidColor(x.BackgroundColor, out colorBrush)))
                newKey.BackgroundColourOverride = colorBrush;

            if (ValidColor(xmlKey.KeyDisabledBackground, out colorBrush))
                newKey.DisabledBackgroundColourOverride = colorBrush;
            else if (keyGroupList != null && keyGroupList.Exists(x => ValidColor(x.KeyDisabledBackground, out colorBrush)))
                newKey.DisabledBackgroundColourOverride = colorBrush;
            else if (newKey.BackgroundColourOverride != null)
                newKey.DisabledBackgroundColourOverride = new SolidColorBrush(HlsColor.Fade(((SolidColorBrush)newKey.BackgroundColourOverride).Color, .15));

            if (ValidColor(xmlKey.KeyDownBackground, out colorBrush))
                newKey.KeyDownBackgroundOverride = colorBrush;
            else if (keyGroupList != null && keyGroupList.Exists(x => ValidColor(x.KeyDownBackground, out colorBrush)))
                newKey.KeyDownBackgroundOverride = colorBrush;
            else if (newKey.BackgroundColourOverride != null)
                newKey.KeyDownBackgroundOverride = new SolidColorBrush(HlsColor.Fade(((SolidColorBrush)newKey.BackgroundColourOverride).Color, .15));

            if (ValidColor(xmlKey.BorderColor, out colorBrush))
                newKey.BorderColourOverride = colorBrush;
            else if (keyGroupList != null && keyGroupList.Exists(x => ValidColor(x.BorderColor, out colorBrush)))
                newKey.BorderColourOverride = colorBrush;

            int borderThickness = 1;
            if (!string.IsNullOrEmpty(xmlKey.BorderThickness) && int.TryParse(xmlKey.BorderThickness, out borderThickness))
                newKey.BorderThicknessOverride = borderThickness;
            else if (keyGroupList != null && keyGroupList.Exists(x => !string.IsNullOrEmpty(x.BorderThickness) && int.TryParse(x.BorderThickness, out borderThickness)))
                newKey.BorderThicknessOverride = borderThickness;

            int cornerRadius = 0;
            if (!string.IsNullOrEmpty(xmlKey.CornerRadius) && int.TryParse(xmlKey.CornerRadius, out cornerRadius))
                newKey.CornerRadiusOverride = cornerRadius;
            else if (keyGroupList != null && keyGroupList.Exists(x => !string.IsNullOrEmpty(x.CornerRadius) && int.TryParse(x.CornerRadius, out cornerRadius)))
                newKey.CornerRadiusOverride = cornerRadius;

            int margin = 0;
            if (!string.IsNullOrEmpty(xmlKey.Margin) && int.TryParse(xmlKey.Margin, out margin))
                newKey.MarginOverride = margin;
            else if (keyGroupList != null && keyGroupList.Exists(x => !string.IsNullOrEmpty(x.Margin) && int.TryParse(x.Margin, out margin)))
                newKey.MarginOverride = margin;

            double opacity = 1;
            if (!string.IsNullOrEmpty(xmlKey.Opacity) && double.TryParse(xmlKey.Opacity, out opacity))
                newKey.OpacityOverride = opacity;
            else if (keyGroupList != null && keyGroupList.Exists(x => !string.IsNullOrEmpty(x.Opacity) && double.TryParse(x.Opacity, out opacity)))
                newKey.OpacityOverride = opacity;

            if (!string.IsNullOrEmpty(xmlKey.KeyDisabledOpacity) && double.TryParse(xmlKey.KeyDisabledOpacity, out opacity))
                newKey.DisabledBackgroundOpacity = opacity;
            else if (keyGroupList != null && keyGroupList.Exists(x => !string.IsNullOrEmpty(x.KeyDisabledOpacity) && double.TryParse(x.KeyDisabledOpacity, out opacity)))
                newKey.DisabledBackgroundOpacity = opacity;
            else if (newKey.OpacityOverride < 1d)
                newKey.DisabledBackgroundOpacity = newKey.OpacityOverride;

            if (!string.IsNullOrEmpty(xmlKey.KeyDownOpacity) && double.TryParse(xmlKey.KeyDownOpacity, out opacity))
                newKey.KeyDownOpacityOverride = opacity;
            else if (keyGroupList != null && keyGroupList.Exists(x => !string.IsNullOrEmpty(x.KeyDownOpacity) && double.TryParse(x.KeyDownOpacity, out opacity)))
                newKey.KeyDownOpacityOverride = opacity;
            else if (newKey.OpacityOverride < 1d)
                newKey.KeyDownOpacityOverride = newKey.OpacityOverride;

            if (xmlKeyValue != null && overrideTimesByKey != null)
            {
                TimeSpanOverrides timeSpanOverrides;
                if (xmlKey.LockOnTime >= 0)
                {
                    if (overrideTimesByKey.TryGetValue(xmlKeyValue, out timeSpanOverrides))
                    {
                        timeSpanOverrides.LockOnTime = TimeSpan.FromMilliseconds(Convert.ToDouble(xmlKey.LockOnTime));
                        overrideTimesByKey[xmlKeyValue] = timeSpanOverrides;
                    }
                    else
                    {
                        timeSpanOverrides = new TimeSpanOverrides() { LockOnTime = TimeSpan.FromMilliseconds(Convert.ToDouble(xmlKey.LockOnTime)) };
                        overrideTimesByKey.Add(xmlKeyValue, timeSpanOverrides);
                    }
                }
                else if (keyGroupList != null && keyGroupList.Exists(x => x.LockOnTime >= 0))
                {
                    if (overrideTimesByKey.TryGetValue(xmlKeyValue, out timeSpanOverrides))
                    {
                        timeSpanOverrides.LockOnTime = TimeSpan.FromMilliseconds(Convert.ToDouble(keyGroupList.Find(x => x.LockOnTime >= 0).LockOnTime));
                        overrideTimesByKey[xmlKeyValue] = timeSpanOverrides;
                    }
                    else
                    {
                        timeSpanOverrides = new TimeSpanOverrides() { LockOnTime = TimeSpan.FromMilliseconds(Convert.ToDouble(keyGroupList.Find(x => x.LockOnTime >= 0).LockOnTime)) };
                        overrideTimesByKey.Add(xmlKeyValue, timeSpanOverrides);
                    }
                }

                if (!string.IsNullOrEmpty (xmlKey.CompletionTimes))
                {
                    if (overrideTimesByKey.TryGetValue(xmlKeyValue, out timeSpanOverrides))
                    {
                        timeSpanOverrides.CompletionTimes = xmlKey.CompletionTimes.Split(',').ToList(); 
                        overrideTimesByKey[xmlKeyValue] = timeSpanOverrides;
                    }
                    else
                    {
                        timeSpanOverrides = new TimeSpanOverrides() { CompletionTimes = xmlKey.CompletionTimes.Split(',').ToList() };
                        overrideTimesByKey.Add(xmlKeyValue, timeSpanOverrides);
                    }
                }
                else if (keyGroupList != null && keyGroupList.Exists(x => !string.IsNullOrEmpty(x.CompletionTimes)))
                {
                    if (overrideTimesByKey.TryGetValue(xmlKeyValue, out timeSpanOverrides))
                    {
                        timeSpanOverrides.CompletionTimes = keyGroupList.Find(x => !string.IsNullOrEmpty(x.CompletionTimes)).CompletionTimes.Split(',').ToList();
                    overrideTimesByKey[xmlKeyValue] = timeSpanOverrides;
                    }
                    else
                    {
                        timeSpanOverrides = new TimeSpanOverrides() { CompletionTimes = keyGroupList.Find(x => !string.IsNullOrEmpty(x.CompletionTimes)).CompletionTimes.Split(',').ToList() };
                        overrideTimesByKey.Add(xmlKeyValue, timeSpanOverrides);
                    }
                }

                if (xmlKey.TimeRequiredToLockDown > 0)
                {
                    if (overrideTimesByKey.TryGetValue(xmlKeyValue, out timeSpanOverrides))
                    {
                        timeSpanOverrides.TimeRequiredToLockDown = TimeSpan.FromMilliseconds(Convert.ToDouble(xmlKey.TimeRequiredToLockDown));
                        overrideTimesByKey[xmlKeyValue] = timeSpanOverrides;
                    }
                    else
                    {
                        timeSpanOverrides = new TimeSpanOverrides() { TimeRequiredToLockDown = TimeSpan.FromMilliseconds(Convert.ToDouble(xmlKey.TimeRequiredToLockDown)) };
                        overrideTimesByKey.Add(xmlKeyValue, timeSpanOverrides);
                    }
                }
                else if (keyGroupList != null && keyGroupList.Exists(x => x.TimeRequiredToLockDown > 0))
                {
                    if (overrideTimesByKey.TryGetValue(xmlKeyValue, out timeSpanOverrides))
                    {
                        timeSpanOverrides.TimeRequiredToLockDown = TimeSpan.FromMilliseconds(Convert.ToDouble(keyGroupList.Find(x => x.TimeRequiredToLockDown > 0).TimeRequiredToLockDown));
                        overrideTimesByKey[xmlKeyValue] = timeSpanOverrides;
                    }
                    else
                    {
                        timeSpanOverrides = new TimeSpanOverrides() { TimeRequiredToLockDown = TimeSpan.FromMilliseconds(Convert.ToDouble(keyGroupList.Find(x => x.TimeRequiredToLockDown > 0).TimeRequiredToLockDown)) };
                        overrideTimesByKey.Add(xmlKeyValue, timeSpanOverrides);
                    }
                }

                if (xmlKey.LockDownAttemptTimeout > 0)
                {
                    if (overrideTimesByKey.TryGetValue(xmlKeyValue, out timeSpanOverrides))
                    {
                        timeSpanOverrides.LockDownAttemptTimeout = TimeSpan.FromMilliseconds(Convert.ToDouble(xmlKey.LockDownAttemptTimeout));
                        overrideTimesByKey[xmlKeyValue] = timeSpanOverrides;
                    }
                    else
                    {
                        timeSpanOverrides = new TimeSpanOverrides() { LockDownAttemptTimeout = TimeSpan.FromMilliseconds(Convert.ToDouble(xmlKey.LockDownAttemptTimeout)) };
                        overrideTimesByKey.Add(xmlKeyValue, timeSpanOverrides);
                    }
                }
                else if (keyGroupList != null && keyGroupList.Exists(x => x.LockDownAttemptTimeout > 0))
                {
                    if (overrideTimesByKey.TryGetValue(xmlKeyValue, out timeSpanOverrides))
                    {
                        timeSpanOverrides.LockDownAttemptTimeout = TimeSpan.FromMilliseconds(Convert.ToDouble(keyGroupList.Find(x => x.LockDownAttemptTimeout > 0).LockDownAttemptTimeout));
                        overrideTimesByKey[xmlKeyValue] = timeSpanOverrides;
                    }
                    else
                    {
                        timeSpanOverrides = new TimeSpanOverrides() { LockDownAttemptTimeout = TimeSpan.FromMilliseconds(Convert.ToDouble(keyGroupList.Find(x => x.LockDownAttemptTimeout > 0).LockDownAttemptTimeout)) };
                        overrideTimesByKey.Add(xmlKeyValue, timeSpanOverrides);
                    }
                }
            }

            PlaceKeyInPosition(newKey, xmlKey.Row, xmlKey.Col, xmlKey.Height, xmlKey.Width);
        }

        private void SetupKeys()
        {
            XmlKeys keys = keyboard.Keys;

            var allKeys = keys.ActionKeys.Cast<IXmlKey>()
                .Concat(keys.ChangeKeyboardKeys)
                .Concat(keys.DynamicKeys)
                .Concat(keys.PluginKeys)
                .Concat(keys.TextKeys)
                .ToList();

            var minKeyWidth = allKeys.Select(k => k.Width).Min();
            var minKeyHeight = allKeys.Select(k => k.Height).Min();

            // Iterate over each possible type of key and add to keyboard
            foreach (XmlActionKey key in keys.ActionKeys)
            {
                AddActionKey(key, minKeyWidth, minKeyHeight);
            }

            foreach (XmlChangeKeyboardKey key in keys.ChangeKeyboardKeys)
            {
                AddChangeKeyboardKey(key, minKeyWidth, minKeyHeight);
            }

            foreach (XmlDynamicKey key in keys.DynamicKeys)
            {
                AddDynamicKey(key, minKeyWidth, minKeyHeight);
            }

            foreach (XmlPluginKey key in keys.PluginKeys)
            {
                AddPluginKey(key, minKeyWidth, minKeyHeight);
            }

            foreach (XmlTextKey key in keys.TextKeys)
            {
                AddTextKey(key, minKeyWidth, minKeyHeight);
            }
        }

        void AddActionKey(XmlActionKey xmlKey, int minKeyWidth, int minKeyHeight)
        {
            Key newKey = CreateKeyWithBasicProps(xmlKey, minKeyWidth, minKeyHeight);

            if (xmlKey.Action.HasValue)
            {
                newKey.Value = new KeyValue(xmlKey.Action.Value);
            }
            else
            {
                Log.ErrorFormat("No FunctionKey found for key with label {0}", xmlKey.Label);
            }

            PlaceKeyInPosition(newKey, xmlKey.Row, xmlKey.Col, xmlKey.Height, xmlKey.Width);
        }

        void AddChangeKeyboardKey(XmlChangeKeyboardKey xmlKey, int minKeyWidth, int minKeyHeight)
        {
            Key newKey = CreateKeyWithBasicProps(xmlKey, minKeyWidth, minKeyHeight);

            if (xmlKey.DestinationKeyboard != null)
            {
                var rootDir = Path.GetDirectoryName(inputFilename);
                bool replaceCurrKeyboard = !xmlKey.ReturnToThisKeyboard;
                newKey.Value = Enum.TryParse(xmlKey.DestinationKeyboard, out Enums.Keyboards keyboardEnum)
                    ? new ChangeKeyboardKeyValue(keyboardEnum, replaceCurrKeyboard)
                    : new ChangeKeyboardKeyValue(Path.Combine(rootDir, xmlKey.DestinationKeyboard), replaceCurrKeyboard);
            }
            else
            {
                Log.ErrorFormat("No destination keyboard found for changekeyboard key with label {0}", xmlKey.Label);
            }

            PlaceKeyInPosition(newKey, xmlKey.Row, xmlKey.Col, xmlKey.Height, xmlKey.Width);
        }
        
        void AddPluginKey(XmlPluginKey xmlKey, int minKeyWidth, int minKeyHeight)
        {
            Key newKey = CreateKeyWithBasicProps(xmlKey, minKeyWidth, minKeyHeight);

            if (xmlKey.Plugin != null && xmlKey.Method != null)
            {
                // FIXME: Saving the XML of the xmlKey itself probably is not the best option. It is done this way to avoid messing with
                // other pieces of code deep within OptiKey.
                XmlSerializer xmlSer = new XmlSerializer(typeof(XmlPluginKey));
                using (var sww = new StringWriter())
                {
                    XmlTextWriter writer = new XmlTextWriter(sww) { Formatting = Formatting.Indented };
                    xmlSer.Serialize(writer, xmlKey);
                    newKey.Value = new KeyValue(FunctionKeys.Plugin, sww.ToString());
                }
            }
            else
            {
                Log.ErrorFormat("Incomplete plugin key configuration in key {0}", xmlKey.Label ?? xmlKey.Symbol);
            }

            PlaceKeyInPosition(newKey, xmlKey.Row, xmlKey.Col, xmlKey.Height, xmlKey.Width);
        }

        void AddTextKey(XmlTextKey xmlKey, int minKeyWidth, int minKeyHeight)
        {
            Key newKey = CreateKeyWithBasicProps(xmlKey, minKeyWidth, minKeyHeight);

            if (xmlKey.Text != null)
            {
                newKey.Value = new KeyValue(xmlKey.Text);
            }
            else
            {
                Log.ErrorFormat("No value found in text key with label {0}", xmlKey.Label);
            }

            PlaceKeyInPosition(newKey, xmlKey.Row, xmlKey.Col, xmlKey.Height, xmlKey.Width);
        }

        private void SetupStyle()
        {
            // Get border and background values, if specified, to override
            if (keyboard.BorderThickness.HasValue)
            {
                Log.InfoFormat("Setting border thickness for custom keyboard: {0}", keyboard.BorderThickness.Value);
                this.BorderThickness = keyboard.BorderThickness.Value;
            }
            if (ValidColor(keyboard.BorderColor, out var colorBrush))
            {
                Log.InfoFormat("Setting border color for custom keyboard: {0}", keyboard.BorderColor);
                this.BorderBrush = colorBrush;
                if(mainWindow != null)
                {
                    mainWindow.BorderBrushOverride = colorBrush;
                }
            }
            if (ValidColor(keyboard.BackgroundColor, out colorBrush))
            {
                Log.InfoFormat("Setting background color for custom keyboard: {0}", keyboard.BackgroundColor);
                this.Background = colorBrush;
                if (mainWindow != null)
                {
                    mainWindow.BackgroundColourOverride = colorBrush;
                }
            }
        }
        private void SetupGrid()
        {
            XmlGrid grid = keyboard.Grid;
            AddRowsToGrid(grid.Rows);
            AddColsToGrid(grid.Cols);
        }

        private void AddRowsToGrid(int nRows)
        {
            for (int i = 0; i < nRows; i++)
            {
                MainGrid.RowDefinitions.Add(new RowDefinition());
            }

            if (keyboard != null && keyboard.ShowOutputPanel)
            {
                // make sure top controls and main grid are scaled appropriately
                TopGrid.RowDefinitions[1].Height = new GridLength(nRows, GridUnitType.Star);
            }
            else
            {
                // hide the output control
                TopGrid.RowDefinitions[0].Height = new GridLength(0);
                OutputPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void AddColsToGrid(int nCols)
        {
            for (int i = 0; i < nCols; i++)
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }

        private void PlaceKeyInPosition(Key key, int row, int col, int rowSpan = 1, int colSpan = 1)
        {
            MainGrid.Children.Add(key);
            if (!(key is KeyPopup))
            {
                Grid.SetColumn(key, col);
                Grid.SetRow(key, row);
                Grid.SetColumnSpan(key, colSpan);
                Grid.SetRowSpan(key, rowSpan);
            }
        }

        public static string StringWithValidNewlines(string s)
        {
            if (s.Contains("\\r\\n"))
                s = s.Replace("\\r\\n", Environment.NewLine);

            if (s.Contains("\\n"))
                s = s.Replace("\\n", Environment.NewLine);

            return s;
        }

        protected override void OnLoaded(object sender, RoutedEventArgs e)
        {
            base.OnLoaded(sender, e);
            ShiftAware = keyboard != null && keyboard.IsShiftAware;
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
