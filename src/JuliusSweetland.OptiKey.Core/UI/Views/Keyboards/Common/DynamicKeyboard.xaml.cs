// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using JuliusSweetland.OptiKey.Enums;
using JuliusSweetland.OptiKey.Extensions;
using JuliusSweetland.OptiKey.Models;
using JuliusSweetland.OptiKey.Properties;
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
using System.Windows.Data;
using System.Windows.Media;

namespace JuliusSweetland.OptiKey.UI.Views.Keyboards.Common
{
    /// <summary>
    /// Interaction logic for DynamicKeyboard.xaml
    /// </summary>
    public partial class DynamicKeyboard : KeyboardView
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string inputFilename;
        private readonly XmlKeyboard keyboard;
        private readonly IList<Tuple<KeyValue, KeyValue>> keyFamily;
        private readonly IDictionary<string, List<KeyValue>> keyValueByGroup;
        private readonly IDictionary<KeyValue, TimeSpanOverrides> overrideTimesByKey;
        private readonly IWindowManipulationService windowManipulationService;

        public DynamicKeyboard(
            string inputFile,
            IList<Tuple<KeyValue, KeyValue>> keyFamily,
            IDictionary<string, List<KeyValue>> keyValueByGroup,
            IDictionary<KeyValue, TimeSpanOverrides> overrideTimesByKey,
            IWindowManipulationService windowManipulationService)
        {
            InitializeComponent();
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

            if (!ValidateKeyboard()) return;

            SetupMainGrid(keyboard.Grid.Rows, keyboard.Grid.Cols);

            if (!SetupDynamicItems()) return;

            SetupStyle();
        }

        public DynamicKeyboard(XmlKeyboard xmlKeyboard) 
        {
            InitializeComponent();
            keyFamily = new List<Tuple<KeyValue, KeyValue>>();
            keyValueByGroup = new Dictionary<string, List<KeyValue>>();
            overrideTimesByKey = new Dictionary<KeyValue, TimeSpanOverrides>();
            keyboard = xmlKeyboard;

            if (!ValidateKeyboard()) return;

            SetupMainGrid(keyboard.Grid.Rows, keyboard.Grid.Cols);

            if (!SetupDynamicItems()) return;

            SetupStyle();
        }

        private bool ValidateKeyboard()
        {
            string errorMessage = null;
            if (keyboard.Grid == null)
                errorMessage = "No grid definition found";
            else if (keyboard.Grid.Rows < 1 || keyboard.Grid.Cols < 1)
                errorMessage = "Grid size is " + keyboard.Grid.Rows + " rows and " + keyboard.Grid.Cols + " columns";
            else if (!keyboard.Interactors.Any())
                errorMessage = "No content definitions found";
            else if (keyboard.ErrorMessage != null)
                errorMessage = keyboard.ErrorMessage;
            
            if (errorMessage != null)
            {
                SetupErrorLayout("Invalid keyboard file", errorMessage);
                return false;
            }

            // If the keyboard overrides any size/position values, tell the windowsManipulationService that it shouldn't be persisting state changes
            if (windowManipulationService == null
                || !keyboard.WindowStateN.HasValue
                && !keyboard.PositionN.HasValue
                && !keyboard.DockSizeN.HasValue
                && !keyboard.WidthN.HasValue
                && !keyboard.HeightN.HasValue
                && !keyboard.HorizontalOffsetN.HasValue
                && !keyboard.VerticalOffsetN.HasValue)
            {
                return true;
            }
            
            Log.InfoFormat("Overriding size and position for dynamic keyboard");
            windowManipulationService.OverridePersistedState(
                keyboard.PersistNewStateN ?? false,
                keyboard.WindowStateN.HasValue ? keyboard.WindowState : null,
                keyboard.PositionN.HasValue ? keyboard.Position : null,
                keyboard.DockSizeN.HasValue ? keyboard.DockSize : null,
                keyboard.WidthN.HasValue ? keyboard.Width : null,
                keyboard.HeightN.HasValue ? keyboard.Height : null,
                keyboard.HorizontalOffsetN.HasValue ? keyboard.HorizontalOffset : null,
                keyboard.VerticalOffsetN.HasValue ? keyboard.VerticalOffset : null);

            return true;
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
            SetupMainGrid(4, 4);

            // Top middle two cells are main error message
            PlaceKeyInPosition(MainGrid, new Key { Text = heading }, 0, 1, 1, 2);

            // Middle row is detailed error message
            PlaceKeyInPosition(MainGrid, new Key { Text = content }, 1, 0, 2, 4);

            // Back key
            var backKey = new Key
            {
                SymbolGeometry = (Geometry)Application.Current.Resources["BackIcon"],
                Text = Properties.Resources.BACK,
                Value = KeyValues.BackFromKeyboardKey
            };
            PlaceKeyInPosition(MainGrid, backKey, 3, 3);

            // Fill in empty keys
            PlaceKeyInPosition(MainGrid, new Key(), 0, 0, 1, 1);
            PlaceKeyInPosition(MainGrid, new Key(), 0, 3, 1, 1);
            PlaceKeyInPosition(MainGrid, new Key(), 3, 0, 1, 1);
            PlaceKeyInPosition(MainGrid, new Key(), 3, 1, 1, 2);
        }

        private bool SetupDynamicItems()
        {
            var minKeyWidth = 1;
            var minKeyHeight = 1;
            if (keyboard.Interactors.Exists(x => x is DynamicKey))
            {
                minKeyWidth = keyboard.Interactors.Where(x => x is DynamicKey).Select(k => k.WidthN).Min();
                minKeyHeight = keyboard.Interactors.Where(x => x is DynamicKey).Select(k => k.HeightN).Min();
            }

            foreach (Interactor dynamicItem in keyboard.Interactors)
            {
                dynamicItem.BuildProfiles();
                var profile = dynamicItem.Expressed;

                if (dynamicItem is DynamicKey || dynamicItem is DynamicPopup)
                {
                    AddDynamicKey(MainGrid, dynamicItem, minKeyWidth, minKeyHeight);
                }
                else if (dynamicItem is DynamicOutputPanel)
                {
                    var newItem = new Output();
                    MainGrid.Children.Add(newItem);
                    Grid.SetColumn(newItem, dynamicItem.ColN);
                    Grid.SetRow(newItem, dynamicItem.RowN);
                    Grid.SetColumnSpan(newItem, dynamicItem.WidthN);
                    Grid.SetRowSpan(newItem, dynamicItem.HeightN);
                }
                else if (dynamicItem is DynamicScratchpad)
                {
                    var newItem = new Scratchpad();
                    var rs = new RelativeSource() { AncestorType = typeof(KeyboardHost) };
                    newItem.SetBinding(FlowDirectionProperty, new Binding("UiLanguageFlowDirection") { Source = Settings.Default });
                    newItem.SetBinding(Scratchpad.TextProperty, new Binding("DataContext.KeyboardOutputService.Text") { RelativeSource = rs });
                    MainGrid.Children.Add(newItem);
                    Grid.SetColumn(newItem, dynamicItem.ColN);
                    Grid.SetRow(newItem, dynamicItem.RowN);
                    Grid.SetColumnSpan(newItem, dynamicItem.WidthN);
                    Grid.SetRowSpan(newItem, dynamicItem.HeightN);
                    newItem.BackgroundColourOverride = profile.BackgroundBrush;
                    newItem.ForegroundColourOverride = profile.ForegroundBrush;
                    newItem.BorderColourOverride = profile.BorderBrush;
                    if (profile.BorderThicknessN.HasValue)
                        newItem.BorderThicknessOverride = profile.BorderThicknessN.Value;
                    if (profile.CornerRadiusN.HasValue)
                        newItem.CornerRadiusOverride = profile.CornerRadiusN.Value;
                    if (profile.OpacityN.HasValue)
                        newItem.OpacityOverride = profile.OpacityN.Value;
                    newItem.DisabledBackgroundColourOverride = profile.KeyDisabledBackgroundBrush;
                    newItem.DisabledForegroundColourOverride = profile.KeyDisabledForegroundBrush;
                }
                else if (dynamicItem is DynamicSuggestionRow)
                {
                    var grid = new Grid();
                    AddRowsToGrid(grid, 1);
                    AddColsToGrid(grid, 4);
                    for (int i = 0; i < 4; i++)
                    {
                        var dk = new DynamicKey() { WidthN = 1, HeightN = 1, ColN = i, Expressed = dynamicItem.Expressed };
                        var fk = i < 1 ? FunctionKeys.Suggestion1 : i < 2 ? FunctionKeys.Suggestion2 : i < 3 ? FunctionKeys.Suggestion3 : FunctionKeys.Suggestion4;
                        var newKey = CreateDynamicKey(dk, new KeyValue(fk), 1, 1);
                        PlaceKeyInPosition(grid, newKey, 0, i, 1, 1);
                    }
                    MainGrid.Children.Add(grid);
                    Grid.SetColumn(grid, dynamicItem.ColN);
                    Grid.SetRow(grid, dynamicItem.RowN);
                    Grid.SetColumnSpan(grid, dynamicItem.WidthN);
                    Grid.SetRowSpan(grid, dynamicItem.HeightN);
                }
                else if (dynamicItem is DynamicSuggestionCol)
                {
                    var grid = new Grid();
                    AddRowsToGrid(grid, 4);
                    AddColsToGrid(grid, 1);
                    for (int i = 0; i < 4; i++)
                    {
                        var dk = new DynamicKey() { WidthN = 1, HeightN = 1, ColN = i, Expressed = dynamicItem.Expressed };
                        var fk = i < 1 ? FunctionKeys.Suggestion1 : i < 2 ? FunctionKeys.Suggestion2 : i < 3 ? FunctionKeys.Suggestion3 : FunctionKeys.Suggestion4;
                        var newKey = CreateDynamicKey(dk, new KeyValue(fk), 1, 1);
                        PlaceKeyInPosition(grid, newKey, i, 0, 1, 1);
                    }
                    MainGrid.Children.Add(grid);
                    Grid.SetColumn(grid, dynamicItem.ColN);
                    Grid.SetRow(grid, dynamicItem.RowN);
                    Grid.SetColumnSpan(grid, dynamicItem.WidthN);
                    Grid.SetRowSpan(grid, dynamicItem.HeightN);
                }
            }

            return true;
        }

        private void AddDynamicKey(Grid grid, Interactor xmlDynamicKey, int minKeyWidth, int minKeyHeight)
        {
            var xmlKeyValue = new KeyValue($"R{xmlDynamicKey.RowN}-C{xmlDynamicKey.ColN}");
            if (xmlDynamicKey.Commands.Count == 1
                && Enum.TryParse(xmlDynamicKey.Commands.First().Value, out FunctionKeys actionEnum)
                && KeyValues.KeysWhichCanBeLockedDown.Contains(new KeyValue(actionEnum)))
            {
                var newKey = CreateDynamicKey(xmlDynamicKey, new KeyValue(actionEnum), minKeyWidth, minKeyHeight);
                PlaceKeyInPosition(grid, newKey, xmlDynamicKey.RowN, xmlDynamicKey.ColN, xmlDynamicKey.HeightN, xmlDynamicKey.WidthN);

            }
            else
            {
                var addCommandList = AddCommandList(xmlDynamicKey, xmlDynamicKey.Commands.ToList());
                if (addCommandList != null && addCommandList.Any())
                    xmlKeyValue.Commands = addCommandList;
                else
                    xmlKeyValue = null; //create a key that performs no action

                var newKey = CreateDynamicKey(xmlDynamicKey, xmlKeyValue, minKeyWidth, minKeyHeight);
                PlaceKeyInPosition(grid, newKey, xmlDynamicKey.RowN, xmlDynamicKey.ColN, xmlDynamicKey.HeightN, xmlDynamicKey.WidthN);
            }
        }

        private List<KeyCommand> AddCommandList(Interactor xmlDynamicKey, List<KeyCommand> commands)
        {
            if (!commands.Any())
            {
                Log.ErrorFormat("No value found in dynamic key with label {0}", xmlDynamicKey.Label);
                return commands;
            }

            var commandList = new List<KeyCommand>();
            var rootDir = Path.GetDirectoryName(inputFilename);
            var xmlKeyValue = new KeyValue($"R{xmlDynamicKey.RowN}-C{xmlDynamicKey.ColN}");

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
                        var kb_link = rootDir != null ? Enum.TryParse(dynamicLink.Value, out Enums.Keyboards keyboardEnum) ? dynamicLink.Value : Path.Combine(rootDir, dynamicLink.Value) : null;

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
                    if (string.IsNullOrWhiteSpace(dynamicPlugin.Name))
                        Log.ErrorFormat("Plugin not found for {0} ", dynamicPlugin.Name);
                    else if (string.IsNullOrWhiteSpace(dynamicPlugin.Method))
                        Log.ErrorFormat("Method not found for {0} ", dynamicPlugin.Name);
                    else
                        commandList.Add(new PluginCommand() { Name = dynamicPlugin.Name,
                            Method = dynamicPlugin.Method, Arguments = dynamicPlugin.Arguments } );
                }
                else if (keyCommand is LoopCommand dynamicLoop)
                {
                    var result = AddCommandList(xmlDynamicKey, dynamicLoop.Commands);
                    if (result != null && result.Any())
                        commandList.Add(new LoopCommand() { Value = dynamicLoop.Count.ToString(), Commands = result } );
                    else
                        return null;
                }
            }
            return commandList;
        }

        private Key CreateDynamicKey(Interactor xmlKey, KeyValue xmlKeyValue, int minKeyWidth, int minKeyHeight)
        {
            var profile = xmlKey.Expressed;
            // Add the core properties from XML to a new key
            var newKey = new Key { Value = xmlKeyValue };
            if (xmlKey is DynamicPopup)
            {
                if (inputFilename != null)
                    newKey = new KeyPopup { Value = xmlKeyValue, GazeRegion = Rect.Parse(xmlKey.GazeRegion) };
                else
                    newKey.GazeRegion = Rect.Parse(xmlKey.GazeRegion);
            }

            //add this item's KeyValue to each KeyGroup referenced in its definition
            foreach (var name in xmlKey.Profiles.Where(x => x.IsMember).Select(x => x.Profile.Name.ToUpper()))
            {
                if (!keyValueByGroup.ContainsKey(name))
                    keyValueByGroup.Add(name, new List<KeyValue> { xmlKeyValue });
                else if (!keyValueByGroup[name].Contains(xmlKeyValue))
                    keyValueByGroup[name].Add(xmlKeyValue);
            }

            if (xmlKey.Label != null)
            {
                var label = xmlKey.Label;
                while (label.Contains("{Resource:"))
                {
                    var start = label.IndexOf("{Resource:");
                    var fullText = label.Substring(start, label.IndexOf("}", start) - start + 1);
                    var propertyName = fullText.Substring(10, fullText.Length - 11).Trim();
                    var propertyValue = Properties.Resources.ResourceManager.GetString(propertyName);
                    label = label.Replace(fullText, propertyValue);
                }
                while (label.Contains("{Setting:"))
                {
                    var start = label.IndexOf("{Setting:");
                    var fullText = label.Substring(start, label.IndexOf("}", start) - start + 1);
                    var propertyName = fullText.Substring(9, fullText.Length - 10).Trim();
                    var propertyValue = Properties.Settings.Default[propertyName].ToString();
                    label = label.Replace(fullText, propertyValue);
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
            if (keyboard.SymbolMarginN.HasValue)
                newKey.SymbolMargin = keyboard.SymbolMarginN.Value;

            // Set shared size group
            if (!string.IsNullOrEmpty(profile.SharedSizeGroup))
                newKey.SharedSizeGroup = profile.SharedSizeGroup;
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

            void SetSuggestionProperties(int index)
            {
                var mb = new MultiBinding() { Converter = new SuggestionsPaged(), Mode = BindingMode.OneWay };
                var rs = new RelativeSource() { AncestorType = typeof(KeyboardHost) };
                mb.Bindings.Add(new Binding("DataContext.SuggestionService.Suggestions") { RelativeSource = rs });
                mb.Bindings.Add(new Binding("DataContext.SuggestionService.SuggestionsPage") { RelativeSource = rs });
                mb.Bindings.Add(new Binding("DataContext.SuggestionService.SuggestionsPerPage") { RelativeSource = rs });
                mb.Bindings.Add(new Binding() { Source = index });
                newKey.SharedSizeGroup = "KeyWithSuggestion";
                newKey.Case = Case.None;
                newKey.SetBinding(Key.TextProperty, mb);
            }
            if (xmlKeyValue != null && xmlKeyValue.FunctionKey.HasValue)
            {
                switch (xmlKeyValue.FunctionKey.Value)
                {
                    case FunctionKeys.Suggestion1:
                        SetSuggestionProperties(0);
                        break;
                    case FunctionKeys.Suggestion2:
                        SetSuggestionProperties(1);
                        break;
                    case FunctionKeys.Suggestion3:
                        SetSuggestionProperties(2);
                        break;
                    case FunctionKeys.Suggestion4:
                        SetSuggestionProperties(3);
                        break;
                    case FunctionKeys.Suggestion5:
                        SetSuggestionProperties(4);
                        break;
                    case FunctionKeys.Suggestion6:
                        SetSuggestionProperties(5);
                        break;
                }
            }

            //Auto set width span and height span
            if (profile.AutoScaleToOneKeyWidthN.HasValue && profile.AutoScaleToOneKeyWidthN.Value)
                newKey.WidthSpan = (double)xmlKey.WidthN / minKeyWidth;

            if (profile.AutoScaleToOneKeyHeightN.HasValue && profile.AutoScaleToOneKeyHeightN.Value)
                newKey.HeightSpan = (double)xmlKey.HeightN / minKeyHeight;

            if (profile.UseUrduCompatibilityFontN.HasValue)
                newKey.UsePersianCompatibilityFont = profile.UsePersianCompatibilityFontN.Value;
            if (profile.UseUnicodeCompatibilityFontN.HasValue)
                newKey.UseUnicodeCompatibilityFont = profile.UseUnicodeCompatibilityFontN.Value;
            if (profile.UseUrduCompatibilityFontN.HasValue)
                newKey.UseUrduCompatibilityFont = profile.UseUrduCompatibilityFontN.Value;

            newKey.ForegroundColourOverride = profile.ForegroundBrush;
            newKey.DisabledForegroundColourOverride = profile.KeyDisabledForegroundBrush;
            newKey.KeyDownForegroundOverride = profile.KeyDownForegroundBrush;

            newKey.BackgroundColourOverride = profile.BackgroundBrush;
            newKey.DisabledBackgroundColourOverride = profile.KeyDisabledBackgroundBrush;
            newKey.KeyDownBackgroundOverride = profile.KeyDownBackgroundBrush;

            newKey.BorderColourOverride = profile.BorderBrush;

            if (profile.BorderThicknessN.HasValue)
                newKey.BorderThicknessOverride = profile.BorderThicknessN.Value;
            if (profile.CornerRadiusN.HasValue)
                newKey.CornerRadiusOverride = profile.CornerRadiusN.Value;

            if (profile.OpacityN.HasValue)
                newKey.OpacityOverride = profile.OpacityN.Value;
            if (profile.KeyDisabledOpacityN.HasValue)
                newKey.DisabledOpacityOverride = profile.KeyDisabledOpacityN.Value;
            if (profile.KeyDownOpacityN.HasValue)
                newKey.KeyDownOpacityOverride = profile.KeyDownOpacityN.Value;

            if (xmlKeyValue != null && overrideTimesByKey != null)
            {
                if (profile.LockOnTimeN.HasValue || profile.CompletionTimesN != null
                    || profile.TimeRequiredToLockDownN.HasValue || profile.LockDownAttemptTimeoutN.HasValue)
                {
                    var timeSpanOverrides = new TimeSpanOverrides()
                    {
                        LockOnTime = profile.LockOnTimeN,
                        CompletionTimes = profile.CompletionTimesN,
                        TimeRequiredToLockDown = profile.TimeRequiredToLockDownN,
                        LockDownAttemptTimeout = profile.LockDownAttemptTimeoutN
                    };
                    if (overrideTimesByKey.ContainsKey(xmlKeyValue))
                        overrideTimesByKey[xmlKeyValue] = timeSpanOverrides;
                    else
                        overrideTimesByKey.Add(xmlKeyValue, timeSpanOverrides);
                }
            }

            xmlKey.Key = newKey;
            return newKey;
        }

        private void SetupStyle()
        {
            // Get border and background values, if specified, to override
            if (keyboard.BorderThicknessN.HasValue)
            {
                Log.InfoFormat("Setting border thickness for custom keyboard: {0}", keyboard.BorderThicknessN.Value);
                this.BorderThickness = new Thickness(keyboard.BorderThicknessN.Value);
            }
            if (keyboard.BorderBrush != null)
            {
                Log.InfoFormat("Setting border color for custom keyboard: {0}", keyboard.BorderColor);
                this.BorderBrush = keyboard.BorderBrush;
            }
            if (keyboard.BackgroundBrush != null)
            {
                Log.InfoFormat("Setting background color for custom keyboard: {0}", keyboard.BackgroundColor);
                this.Background = keyboard.BackgroundBrush;
            }
        }

        private void SetupMainGrid(int rows, int cols)
        {
            AddRowsToGrid(MainGrid, rows);
            AddColsToGrid(MainGrid, cols);

            if (keyboard != null && keyboard.ShowOutputPanelN.HasValue && keyboard.ShowOutputPanelN.Value)
            {
                // make sure top controls and main grid are scaled appropriately
                TopGrid.RowDefinitions[1].Height = new GridLength(keyboard.Grid.Rows, GridUnitType.Star);
            }
            else
            {
                // hide the output control
                TopGrid.RowDefinitions[0].Height = new GridLength(0);
                OutputPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void AddRowsToGrid(Grid grid, int nRows)
        {
            for (int i = 0; i < nRows; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }
        }

        private void AddColsToGrid(Grid grid, int nCols)
        {
            for (int i = 0; i < nCols; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }

        private void PlaceKeyInPosition(Grid grid, Key key, int row, int col, int rowSpan = 1, int colSpan = 1)
        {
            grid.Children.Add(key);
            Grid.SetColumn(key, col);
            Grid.SetRow(key, row);
            Grid.SetColumnSpan(key, colSpan);
            Grid.SetRowSpan(key, rowSpan);
        }

        protected override void OnLoaded(object sender, RoutedEventArgs e)
        {
            base.OnLoaded(sender, e);
            ShiftAware = keyboard != null && keyboard.IsShiftAware;
        }
    }
}
