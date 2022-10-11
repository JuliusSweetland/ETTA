// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using JuliusSweetland.OptiKey.Enums;
using JuliusSweetland.OptiKey.Extensions;
using JuliusSweetland.OptiKey.Models;
using JuliusSweetland.OptiKey.Properties;
using JuliusSweetland.OptiKey.Services;
using JuliusSweetland.OptiKey.Services.Translation;
using JuliusSweetland.OptiKey.Static;
using JuliusSweetland.OptiKey.UI.ViewModels.Keyboards;
using JuliusSweetland.OptiKey.UI.ViewModels.Keyboards.Base;
using log4net;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System.Text;
using System.Net.Http;
using System.IO;

namespace JuliusSweetland.OptiKey.UI.ViewModels
{
    public partial class MainViewModel : BindableBase
    {
        #region Fields

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IAudioService audioService;
        private readonly ICalibrationService calibrationService;
        private readonly IDictionaryService dictionaryService;
        private readonly IKeyStateService keyStateService;
        private readonly ISuggestionStateService suggestionService;
        private readonly ICapturingStateManager capturingStateManager;
        private readonly ILastMouseActionStateManager lastMouseActionStateManager;
        private readonly IInputService inputService;
        private readonly IKeyboardOutputService keyboardOutputService;
        private readonly IMouseOutputService mouseOutputService;
        private readonly IWindowManipulationService mainWindowManipulationService;
        private readonly List<INotifyErrors> errorNotifyingServices; 
        private readonly InteractionRequest<NotificationWithCalibrationResult> calibrateRequest;
        private readonly StringBuilder pendingErrorToastNotificationContent = new StringBuilder();
        private readonly TranslationService translationService;

        private EventHandler<int> inputServicePointsPerSecondHandler;
        private EventHandler<Tuple<Point, KeyValue>> inputServiceCurrentPositionHandler;
        private EventHandler<Tuple<PointAndKeyValue, double>> inputServiceSelectionProgressHandler;
        private EventHandler<PointAndKeyValue> inputServiceSelectionHandler;
        private EventHandler<Tuple<List<Point>, KeyValue, List<string>>> inputServiceSelectionResultHandler;
        private SelectionModes selectionMode;
        private Point currentPositionPoint;
        private KeyValue currentPositionKey;
        private Tuple<Point, double> pointSelectionProgress;
        private Dictionary<Rect, KeyValue> pointToKeyValueMap;
        private bool showCursor;
        private bool showCrosshair;
        private bool showMonical;
        private bool showSuggestions;
        private bool suspendCommands;
        private bool manualModeEnabled;
        private Action<Point> nextPointSelectionAction;
        private Point? magnifyAtPoint;
        private Action<Point?> magnifiedPointSelectionAction;

        #endregion

        #region Ctor

        public MainViewModel(
            IAudioService audioService,
            ICalibrationService calibrationService,
            IDictionaryService dictionaryService,
            IKeyStateService keyStateService,
            ISuggestionStateService suggestionService,
            ICapturingStateManager capturingStateManager,
            ILastMouseActionStateManager lastMouseActionStateManager,
            IInputService inputService,
            IKeyboardOutputService keyboardOutputService,
            IMouseOutputService mouseOutputService,
            IWindowManipulationService mainWindowManipulationService,
            List<INotifyErrors> errorNotifyingServices,
            string startKeyboardOverride = null)
        { 
            this.audioService = audioService;
            this.calibrationService = calibrationService;
            this.dictionaryService = dictionaryService;
            this.keyStateService = keyStateService;
            this.suggestionService = suggestionService;
            this.capturingStateManager = capturingStateManager;
            this.lastMouseActionStateManager = lastMouseActionStateManager;
            this.inputService = inputService;
            this.keyboardOutputService = keyboardOutputService;
            this.mouseOutputService = mouseOutputService;
            this.mainWindowManipulationService = mainWindowManipulationService;
            this.errorNotifyingServices = errorNotifyingServices;

            calibrateRequest = new InteractionRequest<NotificationWithCalibrationResult>();
            SelectionMode = SelectionModes.Key;

            this.translationService = new TranslationService(new HttpClient());

            SetupInputServiceEventHandlers();
            InitialiseKeyboard(mainWindowManipulationService, startKeyboardOverride);
            AttachScratchpadEnabledListener();
            AttachKeyboardSupportsCollapsedDockListener(mainWindowManipulationService);
            AttachKeyboardSupportsSimulateKeyStrokesListener();
            AttachKeyboardSupportsMultiKeySelectionListener();
            ShowCrosshair = Settings.Default.GazeIndicatorStyle == GazeIndicatorStyles.Crosshair
                || Settings.Default.GazeIndicatorStyle == GazeIndicatorStyles.Scope;
            ShowMonical = Settings.Default.GazeIndicatorStyle == GazeIndicatorStyles.Monical
                || Settings.Default.GazeIndicatorStyle == GazeIndicatorStyles.Scope;
        }

        #endregion

        #region Events

        public event EventHandler<NotificationEventArgs> ToastNotification;
        public event EventHandler<KeyValue> KeySelection;
        public event EventHandler<Point> PointSelection;

        #endregion

        #region Properties

        public IInputService InputService { get { return inputService; } }
        public ICapturingStateManager CapturingStateManager { get { return capturingStateManager; } }
        public IKeyboardOutputService KeyboardOutputService { get { return keyboardOutputService; } }
        public IKeyStateService KeyStateService { get { return keyStateService; } }
        public ISuggestionStateService SuggestionService { get { return suggestionService; } }
        public ICalibrationService CalibrationService { get { return calibrationService; } }

        public IWindowManipulationService MainWindowManipulationService { get { return mainWindowManipulationService; } }


        private IKeyboard keyboard;
        public IKeyboard Keyboard
        {
            get { return keyboard; }
            set
            {
                DeactivateLookToScrollUponSwitchingKeyboards();
                keyboard?.OnExit(); // previous keyboard
                SetProperty(ref keyboard, value);
                keyboard?.OnEnter(); // new keyboard
                Log.InfoFormat("Keyboard changed to {0}", value);
            }
        }

        public void BackFromKeyboard()
        {
            Log.Info("Navigating back from keyboard via contextmenu.");
            var navigableKeyboard = Keyboard as IBackAction;
            if (navigableKeyboard != null && navigableKeyboard.BackAction != null)
            {
                navigableKeyboard.BackAction();
            }
            else
            {
                Log.Error("Keyboard doesn't have backaction, going back to initial keyboard instead");
                InitialiseKeyboard(this.mainWindowManipulationService);
            }
        }

        private bool keyboardSupportsCollapsedDock = true;
        public bool KeyboardSupportsCollapsedDock
        {
            get { return keyboardSupportsCollapsedDock; }
            set
            {
                Log.InfoFormat("KeyboardSupportsCollapsedDock changed to {0}", value);
                SetProperty(ref keyboardSupportsCollapsedDock, value);
            }
        }

        public Dictionary<Rect, KeyValue> PointToKeyValueMap
        {
            set
            {
                if (pointToKeyValueMap != value)
                {
                    pointToKeyValueMap = value;

                    inputService.PointToKeyValueMap = value;
                    SelectionResultPoints = null; //The last selection result points cannot be valid if this has changed (window has moved or resized)
                }
            }
        }

        public SelectionModes SelectionMode
        {
            get { return selectionMode; }
            set
            {
                if (SetProperty(ref selectionMode, value))
                {
                    Log.InfoFormat("SelectionMode changed to {0}", value);

                    ResetSelectionProgress();

                    if (inputService != null)
                    {
                        inputService.SelectionMode = value;
                    }
                }
            }
        }

        public bool ManualModeEnabled
        {
            get { return manualModeEnabled; }
            set { SetProperty(ref manualModeEnabled, value); }
        }

        public bool ShowCursor
        {
            get { return showCursor; }
            set { SetProperty(ref showCursor, value); }
        }

        public bool ShowCrosshair
        {
            get { return showCrosshair; }
            set { SetProperty(ref showCrosshair, value); }
        }

        public bool ShowMonical
        {
            get { return showMonical; }
            set { SetProperty(ref showMonical, value); }
        }

        public bool ShowSuggestions
        {
            get { return showSuggestions; }
            set { SetProperty(ref showSuggestions, value); }
        }

        public Point? MagnifyAtPoint
        {
            get { return magnifyAtPoint; }
            set { SetProperty(ref magnifyAtPoint, value); }
        }

        public Action<Point?> MagnifiedPointSelectionAction
        {
            get { return magnifiedPointSelectionAction; }
            set { SetProperty(ref magnifiedPointSelectionAction, value); }
        }

        public Point CurrentPositionPoint
        {
            get { return currentPositionPoint; }
            set { SetProperty(ref currentPositionPoint, value); }
        }

        public KeyValue CurrentPositionKey
        {
            get { return currentPositionKey; }
            set { SetProperty(ref currentPositionKey, value); }
        }

        public Tuple<Point, double> PointSelectionProgress
        {
            get { return pointSelectionProgress; }
            private set { SetProperty(ref pointSelectionProgress, value); }
        }

        private List<Point> selectionResultPoints;
        public List<Point> SelectionResultPoints
        {
            get { return selectionResultPoints; }
            set { SetProperty(ref selectionResultPoints, value); }
        }

        private int pointsPerSecond;
        public int PointsPerSecond
        {
            get { return pointsPerSecond; }
            set { SetProperty(ref pointsPerSecond, value); }
        }

        public string ApplicationAndSystemInfo
        {
            get
            {
                return string.Format("v:{0} | P:{1} {2} | OS:{3} {4} ({5})",
                    DiagnosticInfo.AssemblyVersion,
                    DiagnosticInfo.ProcessBitness,
                    DiagnosticInfo.IsProcessElevated ? "(Elevated)" : "(Not Elevated)",
                    DiagnosticInfo.OperatingSystemBitness,
                    DiagnosticInfo.OperatingSystemVersion,
                    DiagnosticInfo.OperatingSystemServicePack);
            }
        }

        private bool scratchpadIsDisabled;
        public bool ScratchpadIsDisabled
        {
            get { return scratchpadIsDisabled; }
            set { SetProperty(ref scratchpadIsDisabled, value); }
        }

        public InteractionRequest<NotificationWithCalibrationResult> CalibrateRequest { get { return calibrateRequest; } }

        #endregion

        #region Methods

        public void FireKeySelectionEvent(KeyValue kv)
        {
            if (KeySelection != null)
            {
                KeySelection(this, kv);
            }
        }

        private void InitialiseKeyboard(IWindowManipulationService windowManipulationService, string keyboardOverride = null)
        {
            if (keyboardOverride != null)
            {
                // Keyboard specified via command line args can be single XML file or a directory
                if (Directory.Exists(keyboardOverride))
                {
                    Log.Info($"Loading keyboards from requested directory: {keyboardOverride}");
                    Keyboard = new DynamicKeyboardSelector(() => { }, 0, keyboardOverride);
                    return;
                }
                else if (File.Exists(keyboardOverride))
                {
                    Log.Info($"Loading keyboard from requested file: {keyboardOverride}");
                    Keyboard = new DynamicKeyboard(() =>
                    {
                        mainWindowManipulationService.Restore();
                        Keyboard = new Menu(() => Keyboard = new Alpha1());
                    }, keyStateService, keyboardOverride);
                    return;
                }
            }

            Action backaction = null;
            if (Settings.Default.ConversationOnlyMode)
            {
                Settings.Default.StartupKeyboard = Enums.Keyboards.ConversationAlpha;
            }
            else if (Settings.Default.ConversationConfirmOnlyMode)
            {
                Settings.Default.StartupKeyboard = Enums.Keyboards.ConversationConfirm;
            }
            else 
            {
                switch (Settings.Default.StartupKeyboard)
                {
                    case Enums.Keyboards.ConversationAlpha:
                    case Enums.Keyboards.ConversationConfirm:
                    case Enums.Keyboards.ConversationNumericAndSymbols:
                    case Enums.Keyboards.Minimised:
                        backaction = () =>
                            {
                                windowManipulationService.Restore();
                                windowManipulationService.ResizeDockToFull();
                                Keyboard = new Menu(() => Keyboard = new Alpha1());
                            };
                        break;

                    case Enums.Keyboards.CustomKeyboardFile:
                        backaction = () =>
                            {
                                Keyboard = new Menu(() => Keyboard = new Alpha1());
                            };
                        break;

                    default:
                        backaction = () =>
                            {
                                Keyboard = new Alpha1();
                            };
                        break;
                }
            }
            SetKeyboardFromEnum(Settings.Default.StartupKeyboard, windowManipulationService, backaction);
        }

        private void SetKeyboardFromEnum(Enums.Keyboards keyboardEnum,
                                         IWindowManipulationService windowManipulationService,
                                         Action backAction)
        {
            if (Keyboard is Minimised && keyboardEnum == Enums.Keyboards.Minimised)
                return;
            // Set up the keyboard
            switch (keyboardEnum)
            {
                case Enums.Keyboards.Alpha:
                    Keyboard = new Alpha1();
                    break;

                case Enums.Keyboards.ConversationAlpha:
                    Keyboard = new ConversationAlpha1(backAction);
                    break;

                case Enums.Keyboards.ConversationNumericAndSymbols:
                    Keyboard = new ConversationNumericAndSymbols(backAction);
                    break;

                case Enums.Keyboards.ConversationConfirm:
                    Keyboard = new ConversationConfirm(backAction);
                    break;

                case Enums.Keyboards.Currencies1:
                    Keyboard = new Currencies1();
                    break;

                case Enums.Keyboards.Currencies2:
                    Keyboard = new Currencies2();
                    break;

                case Enums.Keyboards.CustomKeyboardFile:
                    Keyboard = new DynamicKeyboard(backAction, keyStateService, Settings.Default.StartupKeyboardFile);
                    break;

                case Enums.Keyboards.Diacritics1:
                    Keyboard = new Diacritics1();
                    break;

                case Enums.Keyboards.Diacritics2:
                    Keyboard = new Diacritics2();
                    break;

                case Enums.Keyboards.Diacritics3:
                    Keyboard = new Diacritics3();
                    break;

                case Enums.Keyboards.DynamicKeyboard:
                    Keyboard = new DynamicKeyboardSelector(() => { }, 0);
                    break;

                case Enums.Keyboards.Menu:
                    Keyboard = new Menu(backAction);
                    break;

                case Enums.Keyboards.Minimised:
                    mainWindowManipulationService.Minimise();
                    var currentKeyboard = Keyboard; 
                    Keyboard = new Minimised(() =>
                    {
                        mainWindowManipulationService.Restore();
                        Keyboard = currentKeyboard != null ? currentKeyboard : new Menu(() => Keyboard = new Alpha1());
                    });
                    break;

                case Enums.Keyboards.Mouse:
                    Keyboard = new Mouse(backAction);
                    break;

                case Enums.Keyboards.NumericAndSymbols1:
                    Keyboard = new NumericAndSymbols1();
                    break;

                case Enums.Keyboards.NumericAndSymbols2:
                    Keyboard = new NumericAndSymbols2();
                    break;

                case Enums.Keyboards.NumericAndSymbols3:
                    Keyboard = new NumericAndSymbols3();
                    break;

                case Enums.Keyboards.PhysicalKeys:
                    Keyboard = new PhysicalKeys();
                    break;

                case Enums.Keyboards.SizeAndPosition:
                    Keyboard = new SizeAndPosition(backAction);
                    break;

                case Enums.Keyboards.WebBrowsing:
                    Keyboard = new WebBrowsing();
                    break;

                default:
                    Log.ErrorFormat("Cannot load keyboard: {0}, this is not a valid StartupKeyboard",
                        Settings.Default.StartupKeyboard);
                    break;
            }

            // Set the window appropriately according to keyboard
            switch (keyboardEnum)
            {
                case Enums.Keyboards.ConversationAlpha:
                case Enums.Keyboards.ConversationConfirm:
                case Enums.Keyboards.ConversationNumericAndSymbols:
                    windowManipulationService.Maximise();
                    break;

                case Enums.Keyboards.Minimised:
                    break;

                case Enums.Keyboards.Mouse:
                    windowManipulationService.Restore();
                    if (Settings.Default.MouseKeyboardDockSize == DockSizes.Full)
                    {
                        windowManipulationService.ResizeDockToFull();
                    }
                    else
                    {
                        windowManipulationService.ResizeDockToCollapsed();
                    }
                    break;

                default:
                    windowManipulationService.Restore();
                    windowManipulationService.ResizeDockToFull();
                    break;
            }
        }

        private void AddTextToDictionary()
        {
            Log.Info("AddTextToDictionary called.");

            var possibleEntries = keyboardOutputService.Text.ExtractWordsAndLines();

            if (possibleEntries != null)
            {
                var candidates = possibleEntries.Where(pe => !dictionaryService.ExistsInDictionary(pe)).ToList();

                if (candidates.Any())
                {
                    PromptToAddCandidatesToDictionary(candidates, Keyboard);
                }
                else
                {
                    Log.InfoFormat("No new words or phrases found in output service's Text: '{0}'.", keyboardOutputService.Text);

                    inputService.RequestSuspend();
                    audioService.PlaySound(Settings.Default.InfoSoundFile, Settings.Default.InfoSoundVolume);
                    RaiseToastNotification(Resources.NOTHING_NEW, Resources.NO_NEW_ENTRIES_IN_SCRATCHPAD,
                        NotificationTypes.Normal, () => { inputService.RequestResume(); });
                }
            }
            else
            {
                Log.InfoFormat("No possible words or phrases found in output service's Text: '{0}'.", keyboardOutputService.Text);
                audioService.PlaySound(Settings.Default.InfoSoundFile, Settings.Default.InfoSoundVolume);
            }
        }

        private void PromptToAddCandidatesToDictionary(List<string> candidates, IKeyboard originalKeyboard)
        {
            if (candidates.Any())
            {
                var candidate = candidates.First();

                var prompt = candidate.Contains(' ')
                    ? string.Format(Resources.ADD_PHRASE_TO_DICTIONARY_CONFIRMATION_MESSAGE,
                        candidate, candidate.NormaliseAndRemoveRepeatingCharactersAndHandlePhrases(log: true))
                    : string.Format(Resources.ADD_WORD_TO_DICTIONARY_CONFIRMATION_MESSAGE, candidate);

                if (candidate.Any(char.IsUpper))
                {
                    prompt = string.Concat(prompt, Resources.NEW_DICTIONARY_ENTRY_WILL_CONTAIN_CAPITALS);
                }

                var similarEntries = dictionaryService.GetAllEntries()
                    .Where(de => string.Equals(de.Entry, candidate, StringComparison.InvariantCultureIgnoreCase))
                    .Select(de => de.Entry)
                    .ToList();

                if (similarEntries.Any())
                {
                    string similarEntriesPrompt = string.Format(Resources.SIMILAR_DICTIONARY_ENTRIES_EXIST,
                        string.Join(", ", similarEntries.Select(se => string.Format("'{0}'", se))));

                    prompt = string.Concat(prompt, "\n\n", similarEntriesPrompt);
                }

                Action nextAction = candidates.Count > 1
                        ? (Action)(() => PromptToAddCandidatesToDictionary(candidates.Skip(1).ToList(), originalKeyboard))
                        : (Action)(() => Keyboard = originalKeyboard);

                Keyboard = new YesNoQuestion(
                    prompt,
                    () =>
                    {
                        dictionaryService.AddNewEntryToDictionary(candidate);
                        inputService.RequestSuspend();

                        RaiseToastNotification(Resources.ADDED, string.Format(Resources.ENTRY_ADDED_TO_DICTIONARY, candidate),
                            NotificationTypes.Normal, () => { inputService.RequestResume(); });

                        nextAction();
                    },
                    () => nextAction());
            }
        }

        private void HandleYesNoQuestionResult(bool yesResult)
        {
            Log.InfoFormat("YesNoQuestion result of '{0}' received.", yesResult ? "YES" : "NO");

            var yesNoQuestion = Keyboard as YesNoQuestion;
            if (yesNoQuestion != null)
            {
                if (yesResult)
                {
                    yesNoQuestion.YesAction();
                }
                else
                {
                    yesNoQuestion.NoAction();
                }
            }
        }

        private void AttachScratchpadEnabledListener()
        {
            Action calculateScratchpadIsDisabled = () =>
                ScratchpadIsDisabled = KeyValues.KeysWhichPreventTextCaptureIfDownOrLocked.Any(kv =>
                        keyStateService.KeyDownStates[kv].Value.IsDownOrLockedDown());

            KeyValues.KeysWhichPreventTextCaptureIfDownOrLocked.ForEach(kv =>
                keyStateService.KeyDownStates[kv].OnPropertyChanges(s => s.Value)
                    .Subscribe(value => calculateScratchpadIsDisabled()));

            calculateScratchpadIsDisabled();
        }

        private void AttachKeyboardSupportsSimulateKeyStrokesListener()
        {
            Action<IKeyboard> setSimulateKeyStrokes = kb =>
            {
                keyStateService.SimulateKeyStrokes = kb.SimulateKeyStrokes;
            };
            this.OnPropertyChanges(mvm => mvm.Keyboard).Subscribe(setSimulateKeyStrokes);
            setSimulateKeyStrokes(Keyboard);
        }

        private void AttachKeyboardSupportsMultiKeySelectionListener()
        {
            Action<IKeyboard> setMultiKeySelectionSupported = kb =>
            {
                InputService.MultiKeySelectionSupported = kb.MultiKeySelectionSupported;
            };

            this.OnPropertyChanges(mvm => mvm.Keyboard).Subscribe(setMultiKeySelectionSupported);
            setMultiKeySelectionSupported(Keyboard);
        }

        private void AttachKeyboardSupportsCollapsedDockListener(IWindowManipulationService mainWindowManipulationService)
        {
            Action<bool> resizeDockIfCollapsedDockingNotSupported = collapsedDockingSupported =>
            {
                if (!collapsedDockingSupported
                    && Settings.Default.MainWindowState == WindowStates.Docked
                    && Settings.Default.MainWindowDockSize == DockSizes.Collapsed)
                {
                    Log.Info("Keyboard does not support collapsed dock and main window is docked and collapsed - resizing to full dock");
                    mainWindowManipulationService.ResizeDockToFull();
                }
            };

            this.OnPropertyChanges(mvm => mvm.KeyboardSupportsCollapsedDock).Subscribe(resizeDockIfCollapsedDockingNotSupported);
            resizeDockIfCollapsedDockingNotSupported(KeyboardSupportsCollapsedDock);
        }

        private void ResetSelectionProgress()
        {
            PointSelectionProgress = null;

            if (keyStateService != null)
            {
                keyStateService.KeySelectionProgress.Clear();
            }
        }

        public bool RaiseToastNotification(string title, string content, NotificationTypes notificationType, Action callback)
        {
            bool notificationRaised = false;
            
            if (ToastNotification != null)
            {
                ToastNotification(this, new NotificationEventArgs(title, content, notificationType, callback));
                notificationRaised = true;
            }
            else
            {
                if (notificationType == NotificationTypes.Error)
                {
                    pendingErrorToastNotificationContent.AppendLine(content);
                }

                //Error raised before the ToastNotification is initialised. Call callback delegate to ensure everything continues.
                callback();
            }

            return notificationRaised;
        }

        public async Task<bool> RaiseAnyPendingErrorToastNotifications()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            if (ToastNotification != null && pendingErrorToastNotificationContent.Length > 0)
            {
                Log.ErrorFormat("Toast notification popup will be shown to display startup errors:{0}", pendingErrorToastNotificationContent);
                audioService.PlaySound(Settings.Default.ErrorSoundFile, Settings.Default.ErrorSoundVolume);
                inputService.RequestSuspend();
                ToastNotification(this, new NotificationEventArgs(
                    Resources.STARTUP_CRASH_TITLE,
                    pendingErrorToastNotificationContent.ToString(), NotificationTypes.Error, () =>
                    {
                        pendingErrorToastNotificationContent.Clear();
                        inputService.RequestResume();
                        taskCompletionSource.SetResult(true);
                    }));
            }
            else
            {
                taskCompletionSource.SetResult(false);
            }

            return await taskCompletionSource.Task;
        }

        #endregion
    }
}
