// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using JuliusSweetland.OptiKey.Enums;
using JuliusSweetland.OptiKey.Extensions;
using JuliusSweetland.OptiKey.Models;
using JuliusSweetland.OptiKey.Properties;
using JuliusSweetland.OptiKey.Services;
using JuliusSweetland.OptiKey.UI.Utilities;
using JuliusSweetland.OptiKey.UI.ViewModels.Keyboards.Base;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using JuliusSweetland.OptiKey.UI.Windows;
using ViewModelKeyboards = JuliusSweetland.OptiKey.UI.ViewModels.Keyboards;
using JuliusSweetland.OptiKey.Static;
using JuliusSweetland.OptiKey.UI.Views.Keyboards.Common;

namespace JuliusSweetland.OptiKey.UI.Controls
{
    public class KeyboardHost : ContentControl
    {
        #region Private member vars

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private MainWindow mainWindow;
        private IList<Tuple<KeyValue, KeyValue>> keyFamily;
        private IDictionary<string, List<KeyValue>> keyValueByGroup;
        private IDictionary<KeyValue, TimeSpanOverrides> overrideTimesByKey;
        private IWindowManipulationService windowManipulationService;
        private CompositeDisposable currentKeyboardKeyValueSubscriptions = new CompositeDisposable();

        #endregion

        #region Ctor

        public KeyboardHost()
        {
            Settings.Default.OnPropertyChanges(s => s.KeyboardAndDictionaryLanguage).Subscribe(_ => GenerateContent());
            Settings.Default.OnPropertyChanges(s => s.UiLanguage).Subscribe(_ => GenerateContent());
            Settings.Default.OnPropertyChanges(s => s.MouseKeyboardDockSize).Subscribe(_ => GenerateContent());
            Settings.Default.OnPropertyChanges(s => s.ConversationOnlyMode).Subscribe(_ => GenerateContent());
            Settings.Default.OnPropertyChanges(s => s.ConversationConfirmEnable).Subscribe(_ => GenerateContent());
            Settings.Default.OnPropertyChanges(s => s.ConversationConfirmOnlyMode).Subscribe(_ => GenerateContent());
            Settings.Default.OnPropertyChanges(s => s.UseAlphabeticalKeyboardLayout).Subscribe(_ => GenerateContent());
            Settings.Default.OnPropertyChanges(s => s.EnableCommuniKateKeyboardLayout).Subscribe(_ => GenerateContent());
            Settings.Default.OnPropertyChanges(s => s.UseCommuniKateKeyboardLayoutByDefault).Subscribe(_ => GenerateContent());
            Settings.Default.OnPropertyChanges(s => s.UseSimplifiedKeyboardLayout).Subscribe(_ => GenerateContent());
            Settings.Default.OnPropertyChanges(s => s.CommuniKateKeyboardCurrentContext).Subscribe(_ => GenerateContent());
            Settings.Default.OnPropertyChanges(s => s.SimplifiedKeyboardContext).Subscribe(_ => GenerateContent());

            Loaded += OnLoaded;

            var contentDp = DependencyPropertyDescriptor.FromProperty(ContentProperty, typeof(KeyboardHost));
            if (contentDp != null)
            {
                contentDp.AddValueChanged(this, ContentChangedHandler);
            }

            this.MouseEnter += this.OnMouseEnter;
        }

        #endregion

        #region Properties

        public static readonly DependencyProperty KeyboardProperty =
            DependencyProperty.Register("Keyboard", typeof(IKeyboard), typeof(KeyboardHost),
                new PropertyMetadata(default(IKeyboard),
                    (o, args) =>
                    {
                        var keyboardHost = o as KeyboardHost;
                        if (keyboardHost != null)
                        {
                            keyboardHost.GenerateContent();
                        }
                    }));
        
        public IKeyboard Keyboard
        {
            get { return (IKeyboard)GetValue(KeyboardProperty); }
            set { SetValue(KeyboardProperty, value); }
        }


        public static readonly DependencyProperty PointToKeyValueMapProperty =
            DependencyProperty.Register("PointToKeyValueMap", typeof(Dictionary<Rect, KeyValue>),
                typeof(KeyboardHost), new PropertyMetadata(default(Dictionary<Rect, KeyValue>)));

        public Dictionary<Rect, KeyValue> PointToKeyValueMap
        {
            get { return (Dictionary<Rect, KeyValue>)GetValue(PointToKeyValueMapProperty); }
            set { SetValue(PointToKeyValueMapProperty, value); }
        }

        public static readonly DependencyProperty ErrorContentProperty =
            DependencyProperty.Register("ErrorContent", typeof(object), typeof(KeyboardHost), new PropertyMetadata(default(object)));

        public object ErrorContent
        {
            get { return GetValue(ErrorContentProperty); }
            set { SetValue(ErrorContentProperty, value); }
        }

        #endregion

        #region OnLoaded - build key map

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Log.Debug("KeyboardHost loaded.");

            BuildPointToKeyMap();

            SubscribeToSizeChanges();

            mainWindow = VisualAndLogicalTreeHelper.FindVisualParent<MainWindow>(this);

            var parentWindow = Window.GetWindow(this);

            if (parentWindow == null)
            {
                var windowException = new ApplicationException(Properties.Resources.PARENT_WINDOW_COULD_NOT_BE_FOUND);

                Log.Error(windowException);

                throw windowException;
            }

            SubscribeToParentWindowMoves(parentWindow);
            SubscribeToParentWindowStateChanges(parentWindow);

            Loaded -= OnLoaded; //Ensure this logic only runs once
        }

        #endregion

        #region Generate Content

        private void GenerateContent()
        {
            Log.DebugFormat("GenerateContent called. Keyboard language is '{0}' and Keyboard type is '{1}'",
                Settings.Default.KeyboardAndDictionaryLanguage, Keyboard != null ? Keyboard.GetType() : null);

            //Clear out point to key map
            PointToKeyValueMap = null;

            mainWindow = mainWindow != null ? mainWindow : VisualAndLogicalTreeHelper.FindVisualParent<MainWindow>(this);

            //Clear any potential main window color overrides
            if (mainWindow != null)
            {
                keyFamily = mainWindow.KeyFamily;
                keyValueByGroup = mainWindow.KeyValueByGroup;
                overrideTimesByKey = mainWindow.OverrideTimesByKey;
                windowManipulationService = mainWindow.WindowManipulationService;
                mainWindow.BackgroundColourOverride = null;
                mainWindow.BorderBrushOverride = null;

                //Clear the dictionaries
                keyFamily?.Clear();
                keyValueByGroup?.Clear();
                overrideTimesByKey?.Clear();

                //https://github.com/OptiKey/OptiKey/pull/715
                //Fixing issue where navigating between dynamic and conversation keyboards causing sizing problems:
                //https://github.com/AdamRoden: "I think that because we use a dispatcher to apply the saved size and position,
                //we get in a situation where the main thread maximizes the window before it gets resized by the dispatcher thread.
                //My fix basically says, "don't try restoring the persisted state if we're navigating a maximized keyboard.""
                if (!(Keyboard is ViewModelKeyboards.DynamicKeyboard)
                    && !(Keyboard is ViewModelKeyboards.ConversationAlpha1)
                    && !(Keyboard is ViewModelKeyboards.ConversationAlpha2)
                    && !(Keyboard is ViewModelKeyboards.ConversationConfirm)
                    && !(Keyboard is ViewModelKeyboards.ConversationNumericAndSymbols))
                {
                    windowManipulationService.RestorePersistedState();
                }
            }

            object newContent = ErrorContent;

            if (Keyboard is ViewModelKeyboards.DynamicKeyboard)
            {
                var kb = Keyboard as ViewModelKeyboards.DynamicKeyboard;
                newContent = new DynamicKeyboard(mainWindow, kb.Link, keyFamily, keyValueByGroup, overrideTimesByKey, windowManipulationService) { DataContext = Keyboard };
            }
            else
            {
                newContent = Keyboard.GetContent();
            }

            Content = newContent;
        }

        #endregion

        #region Content Change Handler

        private static void ContentChangedHandler(object sender, EventArgs e)
        {
            var keyboardHost = sender as KeyboardHost;
            if (keyboardHost != null)
            {
                keyboardHost.BuildPointToKeyMap();
            }
        }

        private void OnMouseEnter(object sender, System.EventArgs e)
        {
            if (Settings.Default.PointsSource == PointsSources.MousePosition &&
                Settings.Default.PointsMousePositionHideCursor)
            {
                this.Cursor = System.Windows.Input.Cursors.None;
            }
            else
            {
                this.Cursor = System.Windows.Input.Cursors.Arrow;
            }
        }

        #endregion

        #region Build Point To Key Map

        private void BuildPointToKeyMap()
        {
            Log.Info("Building PointToKeyMap.");

            if (currentKeyboardKeyValueSubscriptions != null)
            {
                Log.Debug("Disposing of currentKeyboardKeyValueSubscriptions.");
                currentKeyboardKeyValueSubscriptions.Dispose();
            }
            currentKeyboardKeyValueSubscriptions = new CompositeDisposable();

            var contentAsFrameworkElement = Content as FrameworkElement;
            if (contentAsFrameworkElement != null)
            {
                if (contentAsFrameworkElement.IsLoaded)
                {
                    TraverseAllKeysAndBuildPointToKeyValueMap();
                }
                else
                {
                    RoutedEventHandler loaded = null;
                    loaded = (sender, args) =>
                    {
                        TraverseAllKeysAndBuildPointToKeyValueMap();
                        contentAsFrameworkElement.Loaded -= loaded;
                    };
                    contentAsFrameworkElement.Loaded += loaded;
                }
            }
        }

        private void TraverseAllKeysAndBuildPointToKeyValueMap()
        {
            var allKeys = VisualAndLogicalTreeHelper.FindVisualChildren<Key>(this).ToList();
            var pointToKeyValueMap = new Dictionary<Rect, KeyValue>();
            var topLeftPoint = new Point(0, 0);

            foreach (var key in allKeys)
            {
                if (key.IsVisible
                    && PresentationSource.FromVisual(key) != null
                    && key.Value != null
                    && key.Value.HasContent())
                {
                    var rect = new Rect
                    {
                        Location = key.PointToScreen(topLeftPoint),
                        Size = (Size)key.GetTransformToDevice().Transform((Vector)key.RenderSize)
                    };

                    if (key is KeyPopup)
                    {
                        rect = Graphics.DipsToPixels(new Rect()
                        {
                            Location = new Point(key.GazeRegion.Left * SystemParameters.VirtualScreenWidth,
                                key.GazeRegion.Top * SystemParameters.VirtualScreenHeight),
                            Size = new Size(key.GazeRegion.Width * SystemParameters.VirtualScreenWidth,
                                key.GazeRegion.Height * SystemParameters.VirtualScreenHeight)
                        });
                    }

                    if (rect.Size.Width != 0 && rect.Size.Height != 0)
                    {
                        if (pointToKeyValueMap.ContainsKey(rect))
                        {
                            // In Release, just log error
                            KeyValue existingKeyValue = pointToKeyValueMap[rect];
                            Log.ErrorFormat("Overlapping keys {0} and {1}, cannot add {1} to map", existingKeyValue, key.Value);

                            Debug.Assert(!pointToKeyValueMap.ContainsKey(rect));
                        }
                        else
                        {
                            pointToKeyValueMap.Add(rect, key.Value);
                        }
                    }

                    var keyValueChangedSubscription = key.OnPropertyChanges<KeyValue>(Key.ValueProperty).Subscribe(kv =>
                    {
                        KeyValue mapValue;
                        if (pointToKeyValueMap.TryGetValue(rect, out mapValue))
                        {
                            pointToKeyValueMap[rect] = kv;
                        }
                    });
                    currentKeyboardKeyValueSubscriptions.Add(keyValueChangedSubscription);
                }
            }

            Log.InfoFormat("PointToKeyValueMap rebuilt with {0} keys.", pointToKeyValueMap.Keys.Count);
            PointToKeyValueMap = pointToKeyValueMap;
        }

        #endregion

        #region Subscribe To Size Changes

        private void SubscribeToSizeChanges()
        {
            Observable.FromEventPattern<SizeChangedEventHandler, SizeChangedEventArgs>
                (h => SizeChanged += h,
                h => SizeChanged -= h)
                .Throttle(TimeSpan.FromSeconds(0.1))
                .ObserveOnDispatcher()
                .Subscribe(ep =>
                {
                    Log.Info($"KeyboardHost SizeChanged event detected from {ep.EventArgs.PreviousSize} to {ep.EventArgs.NewSize}.");
                    BuildPointToKeyMap();
                });
        }

        #endregion

        #region Subscribe To Parent Window Moves

        private void SubscribeToParentWindowMoves(Window parentWindow)
        {
            Observable.FromEventPattern<EventHandler, EventArgs>
                (h => parentWindow.LocationChanged += h,
                h => parentWindow.LocationChanged -= h)
                .Throttle(TimeSpan.FromSeconds(0.1))
                .ObserveOnDispatcher()
                .Subscribe(ep =>
                {
                    var window = ep.Sender as Window;
                    Log.Info($"Window's LocationChanged event detected. New window left:{window?.Left}, right:{(window?.Left ?? 0) + (window?.Width ?? 0)}, top:{window?.Top}, bottom:{(window?.Top ?? 0) + (window?.Height ?? 0)}.");
                    BuildPointToKeyMap();
                });
        }

        #endregion

        #region Subscribe To Parent Window State Changes

        private void SubscribeToParentWindowStateChanges(Window parentWindow)
        {
            Observable.FromEventPattern<EventHandler, EventArgs>
                (h => parentWindow.StateChanged += h,
                h => parentWindow.StateChanged -= h)
                .Throttle(TimeSpan.FromSeconds(0.1))
                .ObserveOnDispatcher()
                .Subscribe(_ =>
                {
                    Log.Info($"Window's StateChange event detected. New state: {parentWindow.WindowState}.");
                    BuildPointToKeyMap();
                });
        }

        #endregion
    }
}