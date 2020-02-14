
// Copyright (c) 2019 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using JuliusSweetland.OptiKey.UI.Controls;
using JuliusSweetland.OptiKey.Models;
using JuliusSweetland.OptiKey.Extensions;
using JuliusSweetland.OptiKey.Enums;
using System.Windows.Controls;
using System.Xml.Serialization;
using System.IO;
using System;
using System.Linq;
using System.Windows.Media;
using System.Reflection;
using log4net;
using System.Xml;
using System.Windows;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using JuliusSweetland.OptiKey.UI.Utilities;
using JuliusSweetland.OptiKey.UI.Windows;

namespace JuliusSweetland.OptiKey.UI.Views.Keyboards.Common
{
    /// <summary>
    /// Interaction logic for DynamicKeyboard.xaml
    /// </summary>
    public partial class DynamicKeyboard : KeyboardView
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly MainWindow mainWindow;
        private string inputFilename;
        private XmlKeyboard keyboard;
        private IDictionary<string, List<KeyValue>> keyValueByRef;

        public DynamicKeyboard(MainWindow parentWindow, string inputFile, IDictionary<string, List<KeyValue>> keyValueByRef = null)
        {
            InitializeComponent();
            this.mainWindow = parentWindow;
            inputFilename = inputFile;
            this.keyValueByRef = keyValueByRef;

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
            double validNumber;
            WindowStates validWindowState;
            MoveToDirections validPosition;
            DockSizes validDockSize;
            if (!string.IsNullOrWhiteSpace(keyboard.WindowState) && Enum.TryParse(keyboard.WindowState, out validWindowState)
                && validWindowState != WindowStates.Docked && validWindowState != WindowStates.Floating && validWindowState != WindowStates.Maximised)
                errorMessage = "WindowState not valid";
            else if (!string.IsNullOrWhiteSpace(keyboard.Position) && !Enum.TryParse<MoveToDirections>(keyboard.Position, out validPosition))
                errorMessage = "Position not valid";
            else if (!string.IsNullOrWhiteSpace(keyboard.DockSize) && !Enum.TryParse<DockSizes>(keyboard.DockSize, out validDockSize))
                errorMessage = "DockSize not valid";
            else if (!string.IsNullOrWhiteSpace(keyboard.Width) &&
                !(double.TryParse(keyboard.Width.Replace("%", ""), out validNumber) && validNumber >= -9999 && validNumber <= 9999))
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

            return true;
        }
        
        private bool ValidateKeys()
        {
            var allKeys = keyboard.Keys.ActionKeys.Cast<XmlKey>()
                .Concat(keyboard.Keys.ChangeKeyboardKeys)
                .Concat(keyboard.Keys.MultiStepKeys)
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
                var keyStrings = keys.Select(key => GetKeyString(key)).Aggregate((seq, next) => $"{seq}, {next}");
                return $"{keyStrings} ({keys.First().Row}, {keys.First().Col})";
            }).Aggregate((msg, key) => $"{msg}, {key}");

            SetupErrorLayout("Duplicate row/column values for keys", errorMsg);
            return false;
        }

        private string GetKeyString(XmlKey xmlKey)
        {
            var textKey = xmlKey as XmlTextKey;
            if (textKey != null)
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
                string vLabel = xmlKey.Label.ToString();
                string vText;
                string vLookup;
                while (vLabel.Contains("{Resource:"))
                {
                    vText = vLabel.Substring(vLabel.IndexOf("{Resource:"), vLabel.IndexOf("}", vLabel.IndexOf("{Resource:")) - vLabel.IndexOf("{Resource:") + 1);
                    vLookup = Properties.Resources.ResourceManager.GetString(vText.Substring(10, vText.Length - 11).Trim());
                    vLabel = vLabel.Replace(vText, vLookup);
                }
                while (vLabel.Contains("{Setting:"))
                {
                    vText = vLabel.Substring(vLabel.IndexOf("{Setting:"), vLabel.IndexOf("}", vLabel.IndexOf("{Setting:")) - vLabel.IndexOf("{Setting:") + 1);
                    vLookup = Properties.Settings.Default[vText.Substring(9, vText.Length - 10).Trim()].ToString();
                    vLabel = vLabel.Replace(vText, vLookup);
                }

                newKey.Text = vLabel.ToStringWithValidNewlines();
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
            info = info.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)[0];

            // Wrap to (approx) three lines
            var len = info.Length;
            var maxLineLength = len / 3.5;
            Log.Info(maxLineLength);
            char[] space = new char[] { ' ' };

            var charCount = 0;
            var allLines = info.Split(space)
                .GroupBy(w => (int)((charCount += w.Length + 1) / maxLineLength))
                .Select(g => string.Join(" ", g));

            return String.Join(Environment.NewLine, allLines);
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
                Key newKey = new Key();
                newKey.Text = heading;
                this.PlaceKeyInPosition(newKey, 0, 1, 1, 2);
            }

            // Middle row is detailed error message
            {
                Key newKey = new Key();
                newKey.Text = content;
                this.PlaceKeyInPosition(newKey, 1, 0, 2, 4);
            }

            // Back key
            {
                Key newKey = new Key();
                newKey.SymbolGeometry = (System.Windows.Media.Geometry)Application.Current.Resources["BackIcon"];
                newKey.Text = JuliusSweetland.OptiKey.Properties.Resources.BACK;
                newKey.Value = KeyValues.BackFromKeyboardKey;
                this.PlaceKeyInPosition(newKey, 3, 3);
            }

            // Fill in empty keys
            {
                Key newKey = new Key();
                this.PlaceKeyInPosition(newKey, 0, 0, 1, 1);
            }
            {
                Key newKey = new Key();
                this.PlaceKeyInPosition(newKey, 0, 3, 1, 1);
            }
            {
                Key newKey = new Key();
                this.PlaceKeyInPosition(newKey, 3, 0, 1, 1);
            }
            {
                Key newKey = new Key();
                this.PlaceKeyInPosition(newKey, 3, 1, 1, 2);
            }
        }

        private bool SetupDynamicItems()
        {
            var minKeyWidth = keyboard.Content.Items.Select(k => k.Width).Min() > 0 ? keyboard.Content.Items.Select(k => k.Width).Min() : 1;
            var minKeyHeight = keyboard.Content.Items.Select(k => k.Height).Min() > 0 ? keyboard.Content.Items.Select(k => k.Height).Min() : 1;

            //start with a list of all grid cells marked empty
            List<List<int>> openGrid = new List<List<int>>();
            for (int r = 0; r < keyboard.Grid.Rows; r++)
            {
                List<int> gridRow = new List<int>();
                for (int c = 0; c < keyboard.Grid.Cols; c++)
                {
                    gridRow.Add(c);
                }
                openGrid.Add(gridRow);
            }

            //begin section 1: processing items with a reserved row and column
            var itemPosition = keyboard.Content.Items.ToList();
            foreach (XmlDynamicItem dynamicItem in itemPosition.Where(x => x.Row > -1 && x.Col > -1))
            {
                var vIndex = keyboard.Content.Items.IndexOf(dynamicItem);
                var vLabel = " with type of Suggestion";
                if (dynamicItem is XmlDynamicKey dynamicKey)
                {
                    vLabel = (!string.IsNullOrEmpty(dynamicKey.Label)) ? " with label '" + dynamicKey.Label + "'"
                        : (!string.IsNullOrEmpty(dynamicKey.Symbol)) ? " with symbol '" + dynamicKey.Symbol + "'"
                        : " with no label or symbol";
                }
                else if (dynamicItem is XmlDynamicScratchpad)
                {
                    vLabel = " with type of Scratchpad";
                }

                if (dynamicItem.Col + dynamicItem.Width > keyboard.Grid.Cols || dynamicItem.Row + dynamicItem.Height > keyboard.Grid.Rows)
                {
                    SetupErrorLayout("Invalid keyboard file", "Insufficient space to position item "
                        + (vIndex + 1) + " of " + itemPosition.Count + vLabel
                        + " at row " + dynamicItem.Row + " column " + dynamicItem.Col);
                    return false;
                }
                //find space to allocate and remove it from available
                for (int row = dynamicItem.Row; row < (dynamicItem.Row + dynamicItem.Height); row++)
                {
                    for (int col = dynamicItem.Col; col < (dynamicItem.Col + dynamicItem.Width); col++)
                    {
                        if (!openGrid.ElementAt(row).Exists(x => x.Equals(col))) //if the column is unavailable
                        {
                            SetupErrorLayout("Invalid keyboard file", "Insufficient space to position item "
                                + (vIndex + 1) + " of " + itemPosition.Count + vLabel
                                + " at row " + dynamicItem.Row + " column " + dynamicItem.Col);
                            return false;
                        }
                        else
                            openGrid.ElementAt(row).Remove(col);
                    }
                }
                SetupDynamicItem(dynamicItem, minKeyWidth, minKeyHeight);
            }
            //end section 1: processing items with a reserved row and column

            //begin section 2: processing items that need a row and/or column assigned
            //the items are processed in the same order they were listed in the xml file
            //if an item has a row or column designation it is treated as an indication to jump forward 
            //to that row or column and mark any/all skipped spaces as empty
            foreach (XmlDynamicItem dynamicItem in itemPosition.Where(x => !(x.Row > -1 && x.Col > -1)))
            {
                var vIndex = itemPosition.IndexOf(dynamicItem);
                var vLabel = " with type of Suggestion";
                if (dynamicItem is XmlDynamicKey dynamicKey)
                {
                    vLabel = (!string.IsNullOrEmpty(dynamicKey.Label)) ? " with label '" + dynamicKey.Label + "'"
                        : (!string.IsNullOrEmpty(dynamicKey.Symbol)) ? " with symbol '" + dynamicKey.Symbol + "'"
                        : " with no label or symbol";
                }
                else if (dynamicItem is XmlDynamicScratchpad)
                {
                    vLabel = " with type of Scratchpad";
                }

                bool itemPositionConfirmed = false;
                while (!itemPositionConfirmed)
                {
                    var vItemColumn = 0;
                    var vRowsConfirmed = 0;

                    //set start row to first row with enough available width for the item
                    var startRow = dynamicItem.Row > -1 ? (openGrid.ElementAt(dynamicItem.Row).Count >= dynamicItem.Width ? dynamicItem.Row : -1)
                        : openGrid.FindIndex(x => (x.Count() >= dynamicItem.Width));
                    //if a start row with enough empty space is not found then return an error
                    if (startRow < 0)
                    {
                        SetupErrorLayout("Invalid keyboard file", "Insufficient space to position item "
                            + (vIndex + 1) + " of " + itemPosition.Count + vLabel + " having width "
                            + dynamicItem.Width + " and height " + dynamicItem.Height);
                        return false;
                    }
                    //block all preceding columns in all preceding rows
                    for (int row = 0; row < startRow; row++)
                    {
                        if (openGrid.ElementAt(row).Any())
                            openGrid.ElementAt(row).Clear();
                    }
                    for (int row = startRow; row < keyboard.Grid.Rows; row++)
                    {
                        if (dynamicItem.Col > -1)
                        {
                            //if column exists then block all preceding columns in the row
                            if (openGrid.ElementAt(row).Exists(x => x == dynamicItem.Col))
                                openGrid.ElementAt(row).RemoveAll(x => x < dynamicItem.Col);
                            //else block the whole row and check another 
                            else
                            {
                                openGrid.ElementAt(row).Clear();
                                break;
                            }
                        }
                        //if height > 1 and we are searching subsequent rows then we need to start at the confirmed start column
                        var vColsConfirmed = 0;
                        var startColumn = (vRowsConfirmed > 0) ? vItemColumn : (dynamicItem.Col > -1) ? dynamicItem.Col : openGrid.ElementAt(row).First();
                        while (openGrid.ElementAt(row).Any())
                        {
                            //if the next open space is adjacent then increment columns confirmed
                            if (openGrid.ElementAt(row).First() == startColumn + vColsConfirmed)
                            {
                                vColsConfirmed++;
                                openGrid.ElementAt(row).RemoveAt(0);
                                //stop searching if we meet the width requiement
                                if (vColsConfirmed == dynamicItem.Width)
                                    break;
                            }
                            //else if this row does not have enough additional space then block what remains and break
                            else if (openGrid.ElementAt(row).Count < dynamicItem.Width || dynamicItem.Col > -1)
                            {
                                openGrid.ElementAt(row).Clear();
                                break;
                            }
                            //else if the next open space is not adjacent then we need to reset
                            else
                            {
                                vColsConfirmed = 0;
                                startColumn = openGrid.ElementAt(row).First();
                            }
                        }

                        if (vColsConfirmed == dynamicItem.Width)
                        {
                            vItemColumn = startColumn;
                            vRowsConfirmed++;

                            if (vRowsConfirmed == dynamicItem.Height)
                            {
                                dynamicItem.Col = vItemColumn;
                                dynamicItem.Row = 1 + row - dynamicItem.Height;
                                itemPositionConfirmed = true;
                                break;
                            }
                        }
                    } //loop back to process the next row
                }

                if (itemPositionConfirmed)
                {
                    SetupDynamicItem(dynamicItem, minKeyWidth, minKeyHeight);
                }
                else
                {
                    SetupErrorLayout("Invalid keyboard file", "Insufficient space to position item "
                        + (vIndex + 1) + " of " + itemPosition.Count + vLabel + " having width "
                        + dynamicItem.Width + " and height " + dynamicItem.Height);
                    return false;
                }
            }
            //end section 2: processing items that need a row and/or column assigned

            return true;
        }

        private void SetupDynamicItem(XmlDynamicItem dynamicItem, int minKeyWidth, int minKeyHeight)
        {
            if (dynamicItem is XmlDynamicKey xmlDynamicKey)
            {
                AddDynamicKey(xmlDynamicKey, minKeyWidth, minKeyHeight);
            }
            else if (dynamicItem is XmlDynamicScratchpad xmlScratchpadItem)
            {
                var scratchpad = new XmlScratchpad();
                MainGrid.Children.Add(scratchpad);
                Grid.SetColumn(scratchpad, dynamicItem.Col);
                Grid.SetRow(scratchpad, dynamicItem.Row);
                Grid.SetColumnSpan(scratchpad, dynamicItem.Width);
                Grid.SetRowSpan(scratchpad, dynamicItem.Height);

                SolidColorBrush colorBrush;
                if (ValidColor(dynamicItem.BackgroundColor, out colorBrush))
                {
                    scratchpad.Scratchpad.BackgroundColourOverride = colorBrush;
                }
                if (ValidColor(dynamicItem.ForegroundColor, out colorBrush))
                {
                    scratchpad.Scratchpad.Foreground = colorBrush;
                }

                double vOpacity = 1;
                if (!string.IsNullOrEmpty(dynamicItem.Opacity) && double.TryParse(dynamicItem.Opacity, out vOpacity))
                {
                    scratchpad.Scratchpad.OpacityOverride = vOpacity;
                }
            }
            else if (dynamicItem is XmlDynamicSuggestionRow xmlSuggestionRow)
            {
                var suggestionRow = new XmlSuggestionRow();
                MainGrid.Children.Add(suggestionRow);
                Grid.SetColumn(suggestionRow, dynamicItem.Col);
                Grid.SetRow(suggestionRow, dynamicItem.Row);
                Grid.SetColumnSpan(suggestionRow, dynamicItem.Width);
                Grid.SetRowSpan(suggestionRow, dynamicItem.Height);

                SolidColorBrush colorBrush;
                if (ValidColor(dynamicItem.BackgroundColor, out colorBrush))
                {
                    suggestionRow.Background = colorBrush;
                    suggestionRow.DisabledBackgroundColourOverride = colorBrush;
                }
                if (ValidColor(dynamicItem.ForegroundColor, out colorBrush))
                {
                    suggestionRow.Foreground = colorBrush;
                }

                double vOpacity = 1;
                if (!string.IsNullOrEmpty(dynamicItem.Opacity) && double.TryParse(dynamicItem.Opacity, out vOpacity))
                {
                    suggestionRow.OpacityOverride = vOpacity;
                }
            }
            else if (dynamicItem is XmlDynamicSuggestionCol xmlSuggestionCol)
            {
                var suggestionCol = new XmlSuggestionCol();
                MainGrid.Children.Add(suggestionCol);
                Grid.SetColumn(suggestionCol, dynamicItem.Col);
                Grid.SetRow(suggestionCol, dynamicItem.Row);
                Grid.SetColumnSpan(suggestionCol, dynamicItem.Width);
                Grid.SetRowSpan(suggestionCol, dynamicItem.Height);

                SolidColorBrush colorBrush;
                if (ValidColor(dynamicItem.BackgroundColor, out colorBrush))
                {
                    suggestionCol.Background = colorBrush;
                    suggestionCol.DisabledBackgroundColourOverride = colorBrush;
                }
                if (ValidColor(dynamicItem.ForegroundColor, out colorBrush))
                {
                    suggestionCol.Foreground = colorBrush;
                }

                double vOpacity = 1;
                if (!string.IsNullOrEmpty(dynamicItem.Opacity) && double.TryParse(dynamicItem.Opacity, out vOpacity))
                {
                    suggestionCol.Opacity = vOpacity;
                }
            }
        }

        private void AddDynamicKey(XmlDynamicKey xmlDynamicKey, int minKeyWidth, int minKeyHeight)
        {
            if (xmlDynamicKey.KeySteps.Any())
            {
                var vStepList = AddDynamicStepList(xmlDynamicKey, minKeyWidth, minKeyHeight);
                if (vStepList != "")
                {
                    vStepList = (vStepList == "") ? vStepList : "<StepList>" + vStepList;
                    Key newKey = CreateDynamicKey(xmlDynamicKey, minKeyWidth, minKeyHeight);
                    newKey.Value = new KeyValue(FunctionKeys.StepList, vStepList);
                    PlaceKeyInPosition(newKey, xmlDynamicKey.Row, xmlDynamicKey.Col, xmlDynamicKey.Height, xmlDynamicKey.Width);
                    var keyValueAsList = new List<KeyValue>();
                    keyValueAsList.Add(newKey.Value);

                    if (!keyValueByRef.ContainsKey("ALL"))
                        keyValueByRef.Add("ALL", keyValueAsList);
                    else if (!keyValueByRef["ALL"].Contains(newKey.Value))
                        keyValueByRef["ALL"].Add(newKey.Value);

                    foreach (XmlDynamicKey vXmlKey in xmlDynamicKey.KeySteps)
                    {
                        if (vXmlKey is DynamicKeyRef vKeyRef)
                        {
                            if (!keyValueByRef.ContainsKey(vKeyRef.Value.ToUpper()))
                                keyValueByRef.Add(vKeyRef.Value.ToUpper(), keyValueAsList);
                            else if (!keyValueByRef[vKeyRef.Value.ToUpper()].Contains(newKey.Value))
                                keyValueByRef[vKeyRef.Value.ToUpper()].Add(newKey.Value);
                        }
                    }
                }
            }
            //place a key that performs no action
            else
            {
                Key newKey = CreateDynamicKey(xmlDynamicKey, minKeyWidth, minKeyHeight);
                PlaceKeyInPosition(newKey, xmlDynamicKey.Row, xmlDynamicKey.Col, xmlDynamicKey.Height, xmlDynamicKey.Width);
            }
        }

        private string AddDynamicStepList(XmlDynamicKey xmlDynamicKey, int minKeyWidth, int minKeyHeight)
        {
            var vStepList = "";
            if (xmlDynamicKey.KeySteps.Any())
            {
                var rootDir = Path.GetDirectoryName(inputFilename);

                foreach (XmlDynamicKey vStep in xmlDynamicKey.KeySteps)
                {
                    if (vStep is DynamicAction vAction)
                    {
                        //TODO: add code to enable proper functionality 
                        //of keydown an keyup states executed from a dynamic keyboard
                        //also dwell duration and cooldown
                        //also locking behavior
                        FunctionKeys actionEnum;
                        if (!Enum.TryParse(vAction.Value, out actionEnum))
                            Log.ErrorFormat("Could not parse {0} as function key", vAction.Value);
                        else
                        {
                            var actionKeyValue = new KeyValue(actionEnum);
                            if (KeyValues.KeysWhichCanBeLockedDown.Contains(actionKeyValue))
                            {
                                Key newKey = CreateDynamicKey(xmlDynamicKey, minKeyWidth, minKeyHeight);
                                newKey.Value = actionKeyValue;
                                PlaceKeyInPosition(newKey, xmlDynamicKey.Row, xmlDynamicKey.Col, xmlDynamicKey.Height, xmlDynamicKey.Width);
                                return "";
                            }
                            else
                                vStepList += "<Action>" + vAction.Value;
                        }
                    }
                    else if (vStep is DynamicLink vLink)
                    {
                        if (string.IsNullOrEmpty(vLink.Value))
                            Log.ErrorFormat("Destination Keyboard not found for {0} ", vLink.Label);
                        else
                        {
                            string vDestinationKeyboard = Enum.TryParse(vLink.Value, out Enums.Keyboards keyboardEnum)
                            ? keyboardEnum.ToString()
                            : Path.Combine(rootDir, vLink.Value);

                                vStepList += (vLink.BackReturnsHere)
                                    ? "<KeyboardAndReturn>" + vDestinationKeyboard
                                    : "<Keyboard>" + vDestinationKeyboard;
                        }
                    }
                    else if (vStep is DynamicKeyDown vKeyDown)
                    {
                        if (string.IsNullOrEmpty(vKeyDown.Value))
                            Log.ErrorFormat("KeyDown text not found for {0} ", vKeyDown.Label);
                        else
                            vStepList += "<KeyDown>" + vKeyDown.Value;
                    }
                    else if (vStep is DynamicKeyUp vKeyUp)
                    {
                        if (string.IsNullOrEmpty(vKeyUp.Value))
                            Log.ErrorFormat("KeyUp text not found for {0} ", vKeyUp.Label);
                        else
                            vStepList += "<KeyUp>" + vKeyUp.Value;
                    }
                    else if (vStep is DynamicKeyUpByRef vKeyUpByRef)
                    {
                        if (string.IsNullOrEmpty(vKeyUpByRef.Value))
                            Log.ErrorFormat("KeyUpByRef text not found for {0} ", vKeyUpByRef.Label);
                        else
                            vStepList += "<KeyUpByRef>" + vKeyUpByRef.Value.ToUpper();
                    }
                    else if (vStep is DynamicKeyToggle vKeyToggle)
                    {
                        if (string.IsNullOrEmpty(vKeyToggle.Value))
                            Log.ErrorFormat("KeyToggle text not found for {0} ", vKeyToggle.Label);
                        else
                            vStepList += "<KeyToggle>" + vKeyToggle.Value;
                    }
                    else if (vStep is DynamicText vText)
                    {
                        if (string.IsNullOrEmpty(vText.Value))
                            Log.ErrorFormat("Text not found for {0} ", vText.Label);
                        else
                            vStepList += "<Text>" + vText.Value;
                    }
                    else if (vStep is DynamicWait vWait)
                    {
                        int waitInt;
                        if (!int.TryParse(vWait.Value, out waitInt))
                            Log.ErrorFormat("Could not parse wait {0} as int value", vWait.Label);
                        else
                            vStepList += "<Wait>" + vWait.Value;
                    }
                    else if (vStep is DynamicPlugin vPlugin)
                    {
                        if (string.IsNullOrWhiteSpace(vPlugin.Name))
                            Log.ErrorFormat("Plugin not found for {0} ", vPlugin.Label);
                        else if (string.IsNullOrWhiteSpace(vPlugin.Method))
                            Log.ErrorFormat("Method not found for {0} ", vPlugin.Label);
                        else
                        {
                            vStepList += "<Plugin>" + vPlugin.Name + "<Method>" + vPlugin.Method;
                            foreach (var vArg in vPlugin.Argument)
                            {
                                vStepList += "<Argument>";
                                if (!string.IsNullOrWhiteSpace(vArg.Name))
                                    vStepList += "<Name>" + vArg.Name;
                                if (!string.IsNullOrWhiteSpace(vArg.Value))
                                    vStepList += "<Value>" + vArg.Value;
                            }
                            vStepList += "</Plugin>";
                        }
                    }
                    else if (vStep is DynamicLoop vkeyLoop) //this is last because all types are XmlDynamicKey, but we only want to go deeper if we hit a Loop flag
                    {
                        var vReturn = AddDynamicStepList(vkeyLoop, minKeyWidth, minKeyHeight);
                        if (vReturn != "")
                            vStepList += "<Loop>" + vkeyLoop.Count.ToString() + vReturn + "</Loop>";
                        else
                            return "";
                    }
                }
            }
            else
            {
                Log.ErrorFormat("No value found in dynamic key with label {0}", xmlDynamicKey.Label);
            }
            return vStepList;
        }

        private Key CreateDynamicKey(XmlDynamicKey xmlKey, int minKeyWidth, int minKeyHeight)
        {
            // Add the core properties from XML to a new key
            Key newKey = new Key();
            if (xmlKey.Label != null)
            {
                string vLabel = xmlKey.Label.ToString();
                string vText;
                string vLookup;
                while (vLabel.Contains("{Resource:"))
                {
                    vText = vLabel.Substring(vLabel.IndexOf("{Resource:"), vLabel.IndexOf("}", vLabel.IndexOf("{Resource:")) - vLabel.IndexOf("{Resource:") + 1);
                    vLookup = Properties.Resources.ResourceManager.GetString(vText.Substring(10, vText.Length - 11).Trim());
                    vLabel = vLabel.Replace(vText, vLookup);
                }
                while (vLabel.Contains("{Setting:"))
                {
                    vText = vLabel.Substring(vLabel.IndexOf("{Setting:"), vLabel.IndexOf("}", vLabel.IndexOf("{Setting:")) - vLabel.IndexOf("{Setting:") + 1);
                    vLookup = Properties.Settings.Default[vText.Substring(9, vText.Length - 10).Trim()].ToString();
                    vLabel = vLabel.Replace(vText, vLookup);
                }

                newKey.Text = vLabel.ToStringWithValidNewlines();
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

            // Set style group
            XmlStyleGroup vStyleGroup = new XmlStyleGroup();
            if (!string.IsNullOrEmpty(xmlKey.StyleGroup))
            {
                vStyleGroup = keyboard.StyleGroups.Find(x => x.Name == xmlKey.StyleGroup);
            }

            // Set shared size group
            if (!string.IsNullOrEmpty(xmlKey.SharedSizeGroup))
            {
                newKey.SharedSizeGroup = xmlKey.SharedSizeGroup;
            }
            else if (vStyleGroup != null && !string.IsNullOrEmpty(vStyleGroup.SharedSizeGroup))
            {
                newKey.SharedSizeGroup = vStyleGroup.SharedSizeGroup;
            }
            else
            {
                bool hasSymbol = newKey.SymbolGeometry != null;
                bool hasString = xmlKey.Label != null || xmlKey.ShiftDownLabel != null;
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
                    var text = newKey.Text != null ? newKey.Text.Compose() : newKey.ShiftDownText?.Compose();

                    //Strip out circle character used to show diacritic marks
                    text = text?.Replace("\x25CC", string.Empty);

                    newKey.SharedSizeGroup = text != null && text.Length > 5
                        ? "KeyWithLongText" : text != null && text.Length > 1
                        ? "KeyWithShortText" : "KeyWithSingleLetter";
                }
            }

            //Auto set width span and height span
            if ((xmlKey.AutoScaleToOneKeyWidth) || (vStyleGroup != null && vStyleGroup.AutoScaleToOneKeyWidth))
            {
                newKey.WidthSpan = (double)xmlKey.Width / (double)minKeyWidth;
            }

            if ((xmlKey.AutoScaleToOneKeyHeight) || (vStyleGroup != null && vStyleGroup.AutoScaleToOneKeyHeight))
            {
                newKey.HeightSpan = (double)xmlKey.Height / (double)minKeyHeight;
            }

            if ((xmlKey.UsePersianCompatibilityFont) || (vStyleGroup != null && vStyleGroup.UsePersianCompatibilityFont))
                newKey.UsePersianCompatibilityFont = true;
            if ((xmlKey.UseUnicodeCompatibilityFont) || (vStyleGroup != null && vStyleGroup.UseUnicodeCompatibilityFont))
                newKey.UseUnicodeCompatibilityFont = true;
            if ((xmlKey.UseUrduCompatibilityFont) || (vStyleGroup != null && vStyleGroup.UseUrduCompatibilityFont))
                newKey.UseUrduCompatibilityFont = true;

            SolidColorBrush colorBrush;
            if (ValidColor(xmlKey.BackgroundColor, out colorBrush))
            {
                newKey.BackgroundColourOverride = colorBrush;
            }
            else if (vStyleGroup != null && ValidColor(vStyleGroup.BackgroundColor, out colorBrush))
            {
                newKey.BackgroundColourOverride = colorBrush;
            }

            if (ValidColor(xmlKey.ForegroundColor, out colorBrush))
            {
                newKey.ForegroundColourOverride = colorBrush;
                newKey.DisabledForegroundColourOverride = colorBrush;
            }
            else if (vStyleGroup != null && ValidColor(vStyleGroup.ForegroundColor, out colorBrush))
            {
                newKey.ForegroundColourOverride = colorBrush;
                newKey.DisabledForegroundColourOverride = colorBrush;
            }

            if (ValidColor(xmlKey.KeyDisabledBackground, out colorBrush))
            {
                newKey.DisabledBackgroundColourOverride = colorBrush;
            }
            else if (vStyleGroup != null && ValidColor(vStyleGroup.KeyDisabledBackground, out colorBrush))
            {
                newKey.DisabledBackgroundColourOverride = colorBrush;
            }

            if (ValidColor(xmlKey.KeyDownBackground, out colorBrush))
            {
                newKey.KeyDownBackgroundOverride = colorBrush;
            }
            else if (vStyleGroup != null && ValidColor(vStyleGroup.KeyDownBackground, out colorBrush))
            {
                newKey.KeyDownBackgroundOverride = colorBrush;
            }

            double vOpacity = 1;
            if (!string.IsNullOrEmpty(xmlKey.KeyDownOpacity) && double.TryParse(xmlKey.KeyDownOpacity, out vOpacity))
            {
                newKey.KeyDownOpacityOverride = vOpacity;
            }
            else if (vStyleGroup != null && !string.IsNullOrEmpty(vStyleGroup.KeyDownOpacity) && double.TryParse(vStyleGroup.KeyDownOpacity, out vOpacity))
            {
                newKey.KeyDownOpacityOverride = vOpacity;
            }

            if (!string.IsNullOrEmpty(xmlKey.Opacity) && double.TryParse(xmlKey.Opacity, out vOpacity))
            {
                newKey.OpacityOverride = vOpacity;
                newKey.DisabledBackgroundOpacity = vOpacity;
            }
            else if (vStyleGroup != null && !string.IsNullOrEmpty(vStyleGroup.Opacity) && double.TryParse(vStyleGroup.Opacity, out vOpacity))
            {
                newKey.OpacityOverride = vOpacity;
                newKey.DisabledBackgroundOpacity = vOpacity;
            }

            return newKey;
        }

        private void SetupKeys()
        {
            XmlKeys keys = keyboard.Keys;

            var allKeys = keys.ActionKeys.Cast<XmlKey>()
                .Concat(keys.ChangeKeyboardKeys.Cast<XmlKey>())
                .Concat(keys.MultiStepKeys.Cast<XmlKey>())
                .Concat(keys.PluginKeys.Cast<XmlKey>())
                .Concat(keys.TextKeys.Cast<XmlKey>())
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

            foreach (XmlMultiStepKey key in keys.MultiStepKeys)
            {
                AddMultiStepKey(key, minKeyWidth, minKeyHeight);
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
                Enums.Keyboards keyboardEnum;
                newKey.Value = System.Enum.TryParse(xmlKey.DestinationKeyboard, out keyboardEnum)
                    ? new ChangeKeyboardKeyValue(keyboardEnum, replaceCurrKeyboard)
                    : new ChangeKeyboardKeyValue(Path.Combine(rootDir, xmlKey.DestinationKeyboard), replaceCurrKeyboard);
            }
            else
            {
                Log.ErrorFormat("No destination keyboard found for changekeyboard key with label {0}", xmlKey.Label);
            }

            PlaceKeyInPosition(newKey, xmlKey.Row, xmlKey.Col, xmlKey.Height, xmlKey.Width);
        }

        void AddMultiStepKey(XmlMultiStepKey xmlKey, int minKeyWidth, int minKeyHeight)
        {
            Key newKey = CreateKeyWithBasicProps(xmlKey, minKeyWidth, minKeyHeight);
            if (xmlKey.Steps != null)
            {
                var vStepList = "<StepList>";
                var rootDir = Path.GetDirectoryName(inputFilename);
                string vDestinationKeyboard;
                Enums.Keyboards keyboardEnum;

                foreach (var vStep in xmlKey.Steps)
                {
                    if (!String.IsNullOrWhiteSpace(vStep.Action))
                    {
                        vStepList += "<Action>" + vStep.Action;
                    }
                    if (!String.IsNullOrWhiteSpace(vStep.DestinationKeyboard))
                    {
                        vDestinationKeyboard = System.Enum.TryParse(vStep.DestinationKeyboard, out keyboardEnum)
                            ? keyboardEnum.ToString()
                            : Path.Combine(rootDir, vStep.DestinationKeyboard);

                        vStepList += (vStep.ReturnToThisKeyboard)
                            ? "<KeyboardAndReturn>" + vDestinationKeyboard
                            : "<Keyboard>" + vDestinationKeyboard;
                    }
                    if (!String.IsNullOrWhiteSpace(vStep.Text))
                    {
                        vStepList += "<Text>" + vStep.Text;
                    }
                    if (!String.IsNullOrWhiteSpace(vStep.Wait))
                    {
                        vStepList += "<Wait>" + vStep.Wait;
                    }
                }

                newKey.Value = new KeyValue(FunctionKeys.StepList, vStepList);
            }
            else
            {
                Log.ErrorFormat("No value found in dynamic key with label {0}", xmlKey.Label);
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
            SolidColorBrush colorBrush;
            // Get border and background values, if specified, to override
            if (keyboard.BorderThickness.HasValue)
            {
                Log.InfoFormat("Setting border thickness for custom keyboard: {0}", keyboard.BorderThickness.Value);
                this.BorderThickness = keyboard.BorderThickness.Value;
            }
            if (ValidColor(keyboard.BorderColor, out colorBrush))
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

        private void PlaceKeyInPosition(Key key, int row, int col, int rowspan = 1, int colspan = 1)
        {
            MainGrid.Children.Add(key);
            Grid.SetColumn(key, col);
            Grid.SetRow(key, row);
            Grid.SetColumnSpan(key, colspan);
            Grid.SetRowSpan(key, rowspan);
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
