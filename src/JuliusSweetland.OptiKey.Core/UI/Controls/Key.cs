// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using JuliusSweetland.OptiKey.Enums;
using JuliusSweetland.OptiKey.Extensions;
using JuliusSweetland.OptiKey.Models;
using JuliusSweetland.OptiKey.Properties;
using JuliusSweetland.OptiKey.UI.Utilities;
using JuliusSweetland.OptiKey.UI.ViewModels;

namespace JuliusSweetland.OptiKey.UI.Controls
{
    public class Key : UserControl, INotifyPropertyChanged
    {
        #region Private Member Vars

        private CompositeDisposable onUnloaded = null;

        #endregion

        #region Ctor

        public Key()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        #endregion

        #region On Loaded

        protected void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            onUnloaded = new CompositeDisposable();

            var keyboardHost = VisualAndLogicalTreeHelper.FindVisualParent<KeyboardHost>(this);

            // If key isn't visible, it won't have a visual parent and this is okay.
            if (keyboardHost == null || !(keyboardHost.DataContext is MainViewModel))
                return;

            var mainViewModel = keyboardHost.DataContext as MainViewModel;
            var keyStateService = mainViewModel.KeyStateService;
            var capturingStateManager = mainViewModel.CapturingStateManager;

            if (GazeRegion != null && GazeRegion != new Rect())
            {
                HorizontalOffset = GazeRegion.Left * SystemParameters.VirtualScreenWidth;
                VerticalOffset = GazeRegion.Top * SystemParameters.VirtualScreenHeight;
                Width = GazeRegion.Width * SystemParameters.VirtualScreenWidth;
                Height = GazeRegion.Height * SystemParameters.VirtualScreenHeight;
            }

            //Calculate KeyDownState
            if (Value != null)
            {
                var keyStateSubscription = keyStateService.KeyDownStates[Value]
                    .OnPropertyChanges(kds => kds.Value)
                    .Subscribe(value => KeyDownState = value);
                onUnloaded.Add(keyStateSubscription);
            }
            KeyDownState = (Value == null) ? KeyDownStates.Up : keyStateService.KeyDownStates[Value].Value;

            //Calculate SelectionProgress and SelectionInProgress
            if (Value != null)
            {
                var keySelectionProgressSubscription = keyStateService.KeySelectionProgress[Value]
                    .OnPropertyChanges(ksp => ksp.Value)
                    .Subscribe(value =>
                    {
                        SelectionProgress = value;
                        SelectionInProgress = value > 0d;
                    });
                onUnloaded.Add(keySelectionProgressSubscription);
            }
            var progress = (Value == null) ? 0 : keyStateService.KeySelectionProgress[Value].Value;
            SelectionProgress = progress;
            SelectionInProgress = progress > 0d;

            //Calculate IsEnabled
            Action calculateIsEnabled = () => IsEnabled = keyStateService.KeyEnabledStates[Value];
            var keyEnabledSubscription = keyStateService.KeyEnabledStates
                .OnAnyPropertyChanges()
                .Subscribe(_ => calculateIsEnabled());
            onUnloaded.Add(keyEnabledSubscription);
            calculateIsEnabled();

            //Calculate IsCurrent
            Action<KeyValue> calculateIsCurrent = value => IsCurrent = value != null && Value != null && value.Equals(Value);
            var currentPositionSubscription = mainViewModel.OnPropertyChanges(vm => vm.CurrentPositionKey)
                .Subscribe(calculateIsCurrent);
            onUnloaded.Add(currentPositionSubscription);
            calculateIsCurrent(mainViewModel.CurrentPositionKey);

            //Calculate IsHighlighted
            if (Value != null)
            {
                var keyHighlightedSubscription = keyStateService.KeyHighlightStates[Value]
                    .OnPropertyChanges(ksp => ksp.Value)
                    .Subscribe(value => IsHighlighted = value);
                onUnloaded.Add(keyHighlightedSubscription);
            }
            IsHighlighted = Value != null && keyStateService.KeyHighlightStates[Value].Value;

            //Calculate DisplayShiftDownText
            //Display shift down text (upper case text) if shift is locked down, or down (but NOT when we are capturing a multi key selection)
            Action<KeyDownStates, bool> calculateDisplayShiftDownText = (shiftDownState, capturingMultiKeySelection) =>
                    DisplayShiftDownText = shiftDownState == KeyDownStates.LockedDown
                    || (shiftDownState == KeyDownStates.Down && !capturingMultiKeySelection);

            var capturingMultiKeySelectionSubscription = capturingStateManager
                .OnPropertyChanges(csm => csm.CapturingMultiKeySelection)
                .Subscribe(value => calculateDisplayShiftDownText(keyStateService.KeyDownStates[KeyValues.LeftShiftKey].Value, value));
            onUnloaded.Add(capturingMultiKeySelectionSubscription);

            var leftShiftKeyStateSubscription = keyStateService.KeyDownStates[KeyValues.LeftShiftKey]
                .OnPropertyChanges(sds => sds.Value)
                .Subscribe(value => calculateDisplayShiftDownText(value, capturingStateManager.CapturingMultiKeySelection));

            onUnloaded.Add(leftShiftKeyStateSubscription);
            calculateDisplayShiftDownText(keyStateService.KeyDownStates[KeyValues.LeftShiftKey].Value, capturingStateManager.CapturingMultiKeySelection);

            //Publish own version of KeySelection event
            var keySelectionSubscription = Observable.FromEventPattern<KeyValue>(
                handler => mainViewModel.KeySelection += handler,
                handler => mainViewModel.KeySelection -= handler)
                .Subscribe(pattern =>
                {
                    if (Value != null &&
                        pattern.EventArgs.Equals(Value)
                        && Selection != null)
                    {
                        Selection(this, null);
                    }
                });
            onUnloaded.Add(keySelectionSubscription);
        }

        protected void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (onUnloaded != null
                && !onUnloaded.IsDisposed)
            {
                onUnloaded.Dispose();
                onUnloaded = null;
            }
        }

        #endregion

        #region Events

        public event EventHandler Selection;

        #endregion

        #region Properties

        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(Key), new PropertyMetadata(default(double)));
        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(Key), new PropertyMetadata(default(double)));
        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        public static readonly DependencyProperty KeyDownStateProperty =
            DependencyProperty.Register("KeyDownState", typeof(KeyDownStates), typeof(Key), new PropertyMetadata(default(KeyDownStates)));

        public KeyDownStates KeyDownState
        {
            get { return (KeyDownStates)GetValue(KeyDownStateProperty); }
            set { SetValue(KeyDownStateProperty, value); }
        }

        public static readonly DependencyProperty DisplayShiftDownTextProperty =
            DependencyProperty.Register("DisplayShiftDownText", typeof(bool), typeof(Key), new PropertyMetadata(default(bool)));

        public bool DisplayShiftDownText
        {
            get { return (bool)GetValue(DisplayShiftDownTextProperty); }
            set { SetValue(DisplayShiftDownTextProperty, value); }
        }

        public static readonly DependencyProperty IsCurrentProperty =
            DependencyProperty.Register("IsCurrent", typeof(bool), typeof(Key), new PropertyMetadata(default(bool)));

        public bool IsCurrent
        {
            get { return (bool)GetValue(IsCurrentProperty); }
            set { SetValue(IsCurrentProperty, value); }
        }

        //Should be true when key is the first selected key in multiKey sequence, and false otherwise.
        public static readonly DependencyProperty IsHighlightedProperty =
            DependencyProperty.Register("IsHighlighted", typeof(bool), typeof(Key), new PropertyMetadata(default(bool)));

        public bool IsHighlighted
        {
            get { return (bool)GetValue(IsHighlightedProperty); }
            set { SetValue(IsHighlightedProperty, value); }
        }

        public static readonly DependencyProperty SelectionProgressProperty =
            DependencyProperty.Register("SelectionProgress", typeof(double), typeof(Key), new PropertyMetadata(default(double)));

        public double SelectionProgress
        {
            get { return (double)GetValue(SelectionProgressProperty); }
            set { SetValue(SelectionProgressProperty, value); }
        }

        public static readonly DependencyProperty SelectionInProgressProperty =
            DependencyProperty.Register("SelectionInProgress", typeof(bool), typeof(Key), new PropertyMetadata(default(bool)));

        public bool SelectionInProgress
        {
            get { return (bool)GetValue(SelectionInProgressProperty); }
            set { SetValue(SelectionInProgressProperty, value); }
        }

        public static readonly DependencyProperty GazeRegionProperty = DependencyProperty.Register("GazeRegion", typeof(Rect), typeof(Key), new PropertyMetadata(default(Rect)));

        public Rect GazeRegion
        {
            get { return (Rect)GetValue(GazeRegionProperty); }
            set { SetValue(GazeRegionProperty, value); } 
        }

        //Specify if this key spans multiple keys horizontally - used to keep the contents proportional to other keys
        public static readonly DependencyProperty WidthSpanProperty =
            DependencyProperty.Register("WidthSpan", typeof(double), typeof(Key), new PropertyMetadata(1d));

        public double WidthSpan
        {
            get { return (double)GetValue(WidthSpanProperty); }
            set { SetValue(WidthSpanProperty, value); }
        }

        //Specify if this key spans multiple keys vertically - used to keep the contents proportional to other keys
        public static readonly DependencyProperty HeightSpanProperty =
            DependencyProperty.Register("HeightSpan", typeof(double), typeof(Key), new PropertyMetadata(1d));

        public double HeightSpan
        {
            get { return (double)GetValue(HeightSpanProperty); }
            set { SetValue(HeightSpanProperty, value); }
        }

        public static readonly DependencyProperty SymbolMarginProperty =
                   DependencyProperty.Register("SymbolMargin", typeof(double), typeof(Key), new PropertyMetadata(2d));

        public double SymbolMargin
        {
            get { return (double)GetValue(SymbolMarginProperty); }
            set { SetValue(SymbolMarginProperty, value); }
        }

        public static readonly DependencyProperty SharedSizeGroupProperty =
            DependencyProperty.Register("SharedSizeGroup", typeof(string), typeof(Key), new PropertyMetadata(default(string)));

        public string SharedSizeGroup
        {
            get { return (string)GetValue(SharedSizeGroupProperty); }
            set { SetValue(SharedSizeGroupProperty, value); }
        }

        public static readonly DependencyProperty SymbolGeometryProperty =
            DependencyProperty.Register("SymbolGeometry", typeof(Geometry), typeof(Key),
            new PropertyMetadata(default(Geometry), OnSymbolGeometryOrTextChanged));

        public Geometry SymbolGeometry
        {
            get { return (Geometry)GetValue(SymbolGeometryProperty); }
            set { SetValue(SymbolGeometryProperty, value); }
        }

        public static readonly DependencyProperty SymbolOrientationProperty =
            DependencyProperty.Register("SymbolOrientation", typeof(SymbolOrientations), typeof(Key), new PropertyMetadata(SymbolOrientations.Top));

        public SymbolOrientations SymbolOrientation
        {
            get { return (SymbolOrientations)GetValue(SymbolOrientationProperty); }
            set { SetValue(SymbolOrientationProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(Key), new PropertyMetadata(default(string), TextChanged));

        private static void TextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var key = dependencyObject as Key;

            if (key != null)
            {
                var value = dependencyPropertyChangedEventArgs.NewValue as string;
                //Depending on parametrized case, change the value
                if (value != null)
                {
                    TextInfo textInfo = Settings.Default.UiLanguage.ToCultureInfo().TextInfo;

                    //If Case is specifically set, use it, otherwise, use setting value
                    switch (key.Case == Case.Settings ? Settings.Default.KeyCase : key.Case)
                    {
                        case Case.Upper:
                            value = textInfo.ToUpper(value);
                            break;
                        case Case.Lower:
                            value = textInfo.ToLower(value);
                            break;
                        case Case.Title:
                            //Must be lowercased first because ToTitleCase consider uppercased string as abreviations
                            value = textInfo.ToTitleCase(textInfo.ToLower(value));
                            break;
                    }
                }
                key.ShiftDownText = value;
                key.ShiftUpText = value;
            }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty CaseProperty =
            DependencyProperty.Register("Case", typeof(Case), typeof(Key), new PropertyMetadata(Case.Settings));

        public Case Case
        {
            get { return (Case)GetValue(CaseProperty); }
            set { SetValue(CaseProperty, value); }
        }

        public static readonly DependencyProperty ShiftUpTextProperty =
            DependencyProperty.Register("ShiftUpText", typeof(string), typeof(Key),
            new PropertyMetadata(default(string), OnSymbolGeometryOrTextChanged));

        public string ShiftUpText
        {
            get { return (string)GetValue(ShiftUpTextProperty); }
            set { SetValue(ShiftUpTextProperty, value); }
        }

        public static readonly DependencyProperty ShiftDownTextProperty =
            DependencyProperty.Register("ShiftDownText", typeof(string), typeof(Key),
            new PropertyMetadata(default(string), OnSymbolGeometryOrTextChanged));

        public string ShiftDownText
        {
            get { return (string)GetValue(ShiftDownTextProperty); }
            set { SetValue(ShiftDownTextProperty, value); }
        }

        private static void OnSymbolGeometryOrTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var senderAsKey = sender as Key;
            if (senderAsKey != null && senderAsKey.PropertyChanged != null)
            {
                senderAsKey.OnPropertyChanged("HasSymbol");
                senderAsKey.OnPropertyChanged("HasText");
            }
        }

        public bool HasSymbol { get { return SymbolGeometry != null; } }
        public bool HasText
        {
            get
            {
                return !string.IsNullOrEmpty(ShiftUpText) ||
                       !string.IsNullOrEmpty(ShiftDownText);
            }
        }

        public static readonly DependencyProperty OnlyVisibleWhenInUseProperty =
            DependencyProperty.Register("OnlyVisibleWhenInUse", typeof(bool), typeof(Key), new PropertyMetadata(default(bool)));

        public bool OnlyVisibleWhenInUse
        {
            get { return (bool)GetValue(OnlyVisibleWhenInUseProperty); }
            set { SetValue(OnlyVisibleWhenInUseProperty, value); }
        }

        public static readonly DependencyProperty BackgroundColourOverrideProperty =
            DependencyProperty.Register("BackgroundColourOverride", typeof(Brush), typeof(Key), new PropertyMetadata(default(Brush)));

        public Brush BackgroundColourOverride
        {
            get { return (Brush)GetValue(BackgroundColourOverrideProperty); }
            set { SetValue(BackgroundColourOverrideProperty, value); }
        }

        public static readonly DependencyProperty BorderColourOverrideProperty =
            DependencyProperty.Register("BorderColourOverrideProperty", typeof(Brush), typeof(Key), new PropertyMetadata(default(Brush)));

        public Brush BorderColourOverride
        {
            get { return (Brush)GetValue(BorderColourOverrideProperty); }
            set { SetValue(BorderColourOverrideProperty, value); }
        }

        public static readonly DependencyProperty BorderThicknessOverrideProperty =
            DependencyProperty.Register("BorderThicknessOverrideProperty", typeof(int), typeof(Key), new PropertyMetadata(defaultValue:1));

        public int BorderThicknessOverride
        {
            get { return (int)GetValue(BorderThicknessOverrideProperty); }
            set { SetValue(BorderThicknessOverrideProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusOverrideProperty =
            DependencyProperty.Register("CornerRadiusOverrideProperty", typeof(int), typeof(Key), new PropertyMetadata(defaultValue: 0));

        public int CornerRadiusOverride
        {
            get { return (int)GetValue(CornerRadiusOverrideProperty); }
            set { SetValue(CornerRadiusOverrideProperty, value); }
        }

        public static readonly DependencyProperty MarginOverrideProperty =
            DependencyProperty.Register("MarginOverrideProperty", typeof(int), typeof(Key), new PropertyMetadata(defaultValue: 0));

        public int MarginOverride
        {
            get { return (int)GetValue(MarginOverrideProperty); }
            set { SetValue(MarginOverrideProperty, value); }
        }

        public static readonly DependencyProperty DisabledBackgroundColourOverrideProperty =
            DependencyProperty.Register("DisabledBackgroundColourOverride", typeof(Brush), typeof(Key), new PropertyMetadata(default(Brush)));

        public Brush DisabledBackgroundColourOverride
        {
            get { return (Brush)GetValue(DisabledBackgroundColourOverrideProperty); }
            set { SetValue(DisabledBackgroundColourOverrideProperty, value); }
        }

        public static readonly DependencyProperty KeyDownBackgroundOverrideProperty =
            DependencyProperty.Register("KeyDownBackgroundOverride", typeof(Brush), typeof(Key), new PropertyMetadata(default(Brush)));

        public Brush KeyDownBackgroundOverride
        {
            get { return (Brush)GetValue(KeyDownBackgroundOverrideProperty); }
            set { SetValue(KeyDownBackgroundOverrideProperty, value); }
        }

        public static readonly DependencyProperty KeyDownOpacityOverrideProperty =
            DependencyProperty.Register("KeyDownOpacityOverride", typeof(double), typeof(Key), new PropertyMetadata(defaultValue: 1.0));

        public double KeyDownOpacityOverride
        {
            get { return (double)GetValue(KeyDownOpacityOverrideProperty); }
            set { SetValue(KeyDownOpacityOverrideProperty, value); }
        }

        public static readonly DependencyProperty OpacityOverrideProperty =
            DependencyProperty.Register("OpacityOverride", typeof(double), typeof(Key), new PropertyMetadata(defaultValue: 1.0));

        public double OpacityOverride
        {
            get { return (double)GetValue(OpacityOverrideProperty); }
            set { SetValue(OpacityOverrideProperty, value); }
        }

        public static readonly DependencyProperty DisabledBackgroundOpacityProperty =
            DependencyProperty.Register("DisabledBackgroundOpacity", typeof(double), typeof(Key), new PropertyMetadata((1.0)));

        public double DisabledBackgroundOpacity
        {
            get { return (double)GetValue(DisabledBackgroundOpacityProperty); }
            set { SetValue(DisabledBackgroundOpacityProperty, value); }
        }

        public static readonly DependencyProperty ForegroundColourOverrideProperty =
            DependencyProperty.Register("ForegroundColourOverride", typeof(Brush), typeof(Key), new PropertyMetadata(default(Brush)));

        public Brush ForegroundColourOverride
        {
            get { return (Brush)GetValue(ForegroundColourOverrideProperty); }
            set { SetValue(ForegroundColourOverrideProperty, value); }
        }

        public static readonly DependencyProperty DisabledForegroundColourOverrideProperty =
            DependencyProperty.Register("DisabledForegroundColourOverride", typeof(Brush), typeof(Key), new PropertyMetadata(default(Brush)));

        public Brush DisabledForegroundColourOverride
        {
            get { return (Brush)GetValue(DisabledForegroundColourOverrideProperty); }
            set { SetValue(DisabledForegroundColourOverrideProperty, value); }
        }

        public static readonly DependencyProperty KeyDownForegroundOverrideProperty =
            DependencyProperty.Register("KeyDownForegroundOverride", typeof(Brush), typeof(Key), new PropertyMetadata(default(Brush)));

        public Brush KeyDownForegroundOverride
        {
            get { return (Brush)GetValue(KeyDownForegroundOverrideProperty); }
            set { SetValue(KeyDownForegroundOverrideProperty, value); }
        }

        public static readonly DependencyProperty HoverForegroundColourOverrideProperty =
            DependencyProperty.Register("HoverForegroundColourOverride", typeof(Brush), typeof(Key), new PropertyMetadata(default(Brush)));

        public Brush HoverForegroundColourOverride
        {
            get { return (Brush)GetValue(HoverForegroundColourOverrideProperty); }
            set { SetValue(HoverForegroundColourOverrideProperty, value); }
        }

        public static readonly DependencyProperty SelectionProgressColourOverrideProperty =
            DependencyProperty.Register("SelectionProgressColourOverride", typeof(Brush), typeof(Key), new PropertyMetadata(default(Brush)));

        public Brush SelectionProgressColourOverride
        {
            get { return (Brush)GetValue(SelectionProgressColourOverrideProperty); }
            set { SetValue(SelectionProgressColourOverrideProperty, value); }
        }

        public static readonly DependencyProperty SelectionForegroundColourOverrideProperty =
            DependencyProperty.Register("SelectionForegroundColourOverride", typeof(Brush), typeof(Key), new PropertyMetadata(default(Brush)));

        public Brush SelectionForegroundColourOverride
        {
            get { return (Brush)GetValue(SelectionForegroundColourOverrideProperty); }
            set { SetValue(SelectionForegroundColourOverrideProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(KeyValue), typeof(Key), new PropertyMetadata(default(KeyValue)));

        public KeyValue Value
        {
            get { return (KeyValue)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty UseUnicodeCompatibilityFontProperty =
            DependencyProperty.Register("UseUnicodeCompatibilityFont", typeof(bool), typeof(Key), new PropertyMetadata(default(bool)));

        public bool UseUnicodeCompatibilityFont
        {
            get { return (bool)GetValue(UseUnicodeCompatibilityFontProperty); }
            set { SetValue(UseUnicodeCompatibilityFontProperty, value); }
        }

        public static readonly DependencyProperty UsePersianCompatibilityFontProperty =
            DependencyProperty.Register("UsePersianCompatibilityFont", typeof(bool), typeof(Key), new PropertyMetadata(default(bool)));

        public bool UsePersianCompatibilityFont
        {
            get { return (bool)GetValue(UsePersianCompatibilityFontProperty); }
            set { SetValue(UsePersianCompatibilityFontProperty, value); }
        }

        public static readonly DependencyProperty UseUrduCompatibilityFontProperty =
            DependencyProperty.Register("UseUrduCompatibilityFont", typeof(bool), typeof(Key), new PropertyMetadata(default(bool)));

        public bool UseUrduCompatibilityFont
        {
            get { return (bool)GetValue(UseUrduCompatibilityFontProperty); }
            set { SetValue(UseUrduCompatibilityFontProperty, value); }
        }

        #endregion

        #region OnPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}