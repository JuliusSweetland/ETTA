// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using JuliusSweetland.OptiKey.Enums;
using JuliusSweetland.OptiKey.Properties;
using JuliusSweetland.OptiKey.Services;
using log4net;
using MahApps.Metro.Controls;
using Prism.Mvvm;
using FontStretches = JuliusSweetland.OptiKey.Enums.FontStretches;
using FontWeights = JuliusSweetland.OptiKey.Enums.FontWeights;

namespace JuliusSweetland.OptiKey.UI.ViewModels.Management
{
    public class VisualsViewModel : BindableBase
    {
        #region Private Member Vars

        private const string B612Url = "/Resources/Fonts/#B612";
        private const string B612MonoUrl = "/Resources/Fonts/#B612 Mono";
        private const string CharisSILUrl = "/Resources/Fonts/#CharisSIL";
        public const string ElhamUrl = "/Resources/Fonts/#Elham"; //Persian (Iran) font
        public const string FajerNooriNastaliqueUrl = "/Resources/Fonts/#Fajer Noori Nastalique"; //Urdu (Pakistan) font
        public const string HomaUrl = "/Resources/Fonts/#Homa"; //Persian (Iran) font
        public const string KoodakUrl = "/Resources/Fonts/#Koodak"; //Persian (Iran) font
        private const string MandatoryUrl = "/Resources/Fonts/#Mandatory";
        public const string NafeesWebNaskhUrl = "/Resources/Fonts/#Nafees Web Naskh"; //Urdu (Pakistan) font
        public const string NazliUrl = "/Resources/Fonts/#Nazli"; //Persian (Iran) font
        public const string PakNastaleeqUrl = "/Resources/Fonts/#Pak Nastaleeq"; //Urdu (Pakistan) font
        private const string RobotoUrl = "/Resources/Fonts/#Roboto";
        public const string RoyaUrl = "/Resources/Fonts/#Roya"; //Persian (Iran) font
        public const string TerafikUrl = "/Resources/Fonts/#Terafik"; //Persian (Iran) font
        public const string TitrUrl = "/Resources/Fonts/#Titr"; //Persian (Iran) font

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Ctor

        private IWindowManipulationService windowManipulationService;

        public VisualsViewModel(IWindowManipulationService windowManipulationService)
        {
            this.windowManipulationService = windowManipulationService;
            Load();
        }
        
        #endregion
        
        #region Properties

        private Rect floatingSizeAndPosition;
        public Rect FloatingSizeAndPosition
        {
            get { return floatingSizeAndPosition; }
            set { SetProperty(ref floatingSizeAndPosition, value); }
        }

        public List<KeyValuePair<string, string>> Themes
        {
            get
            {
                return new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(Resources.ANDROID_DARK, "/Resources/Themes/Android_Dark.xaml"),
                    new KeyValuePair<string, string>(Resources.ANDROID_LIGHT, "/Resources/Themes/Android_Light.xaml"),
                    new KeyValuePair<string, string>(Resources.ANDROID_TWO_TONE, "/Resources/Themes/Android_Two_Tone.xaml")
                };
            }
        }

        private int opacity;
        public int Opacity
        {
            get { return opacity; }
            set { SetProperty(ref opacity, value); }
        }

        private bool persistedWindowState;
        public bool PersistedWindowState
        {
            get { return windowManipulationService.GetPersistedState(); }
        }

        public List<KeyValuePair<string, string>> FontFamilies
        {
            get
            {
                return new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("B612", B612Url),
                    new KeyValuePair<string, string>("B612 Mono", B612MonoUrl),
                    new KeyValuePair<string, string>("Charis SIL", CharisSILUrl),
                    new KeyValuePair<string, string>("Elham (for Persian)", ElhamUrl),
                    new KeyValuePair<string, string>("Fajer Noori Nastalique (for Urdu)", FajerNooriNastaliqueUrl),
                    new KeyValuePair<string, string>("Homa (for Persian)", HomaUrl),
                    new KeyValuePair<string, string>("Koodak (for Persian)", KoodakUrl),
                    new KeyValuePair<string, string>("Mandatory", MandatoryUrl),
                    new KeyValuePair<string, string>("Nafees Web Naskh (for Urdu)", NafeesWebNaskhUrl),
                    new KeyValuePair<string, string>("Nazli (for Persian)", NazliUrl),
                    new KeyValuePair<string, string>("Pak Nastaleeq (for Urdu)", PakNastaleeqUrl),
                    new KeyValuePair<string, string>("Roboto", RobotoUrl),
                    new KeyValuePair<string, string>("Roya (for Persian)", RoyaUrl),
                    new KeyValuePair<string, string>("Terafik (for Persian)", TerafikUrl),
                    new KeyValuePair<string, string>("Titr (for Persian)", TitrUrl)
                };
            }
        }

        public List<FontStretches> FontStretches
        {
            get
            {
                switch (FontFamily)
                {
                    case B612Url:
                        return new List<FontStretches>
                        {
                            Enums.FontStretches.Normal
                        };

                    case B612MonoUrl:
                        return new List<FontStretches>
                        {
                            Enums.FontStretches.Normal
                        };

                    case CharisSILUrl:
                        return new List<FontStretches>
                        {
                            Enums.FontStretches.Normal
                        };

                    case ElhamUrl:
                        return new List<FontStretches>
                        {
                            Enums.FontStretches.Normal
                        };

                    case FajerNooriNastaliqueUrl:
                        return new List<FontStretches>
                        {
                            Enums.FontStretches.Normal
                        };

                    case HomaUrl:
                        return new List<FontStretches>
                        {
                            Enums.FontStretches.Normal
                        };

                    case KoodakUrl:
                        return new List<FontStretches>
                        {
                            Enums.FontStretches.Normal
                        };

                    case MandatoryUrl:
                        return new List<FontStretches>
                        {
                            Enums.FontStretches.Normal
                        };

                    case NafeesWebNaskhUrl:
                        return new List<FontStretches>
                        {
                            Enums.FontStretches.Normal
                        };

                    case NazliUrl:
                        return new List<FontStretches>
                        {
                            Enums.FontStretches.Normal
                        };

                    case PakNastaleeqUrl:
                        return new List<FontStretches>
                        {
                            Enums.FontStretches.Normal
                        };

                    case RobotoUrl:
                        return new List<FontStretches>
                        {
                            Enums.FontStretches.Normal, 
                            Enums.FontStretches.Condensed
                        };

                    case RoyaUrl:
                        return new List<FontStretches>
                        {
                            Enums.FontStretches.Normal
                        };

                    case TerafikUrl:
                        return new List<FontStretches>
                        {
                            Enums.FontStretches.Normal
                        };

                    case TitrUrl:
                        return new List<FontStretches>
                        {
                            Enums.FontStretches.Normal
                        };
                }

                return null;
            }
        }

        public List<FontWeights> FontWeights
        {
            get
            {
                switch (FontFamily)
                {
                    case B612Url:
                        return new List<FontWeights> { Enums.FontWeights.Regular };

                    case B612MonoUrl:
                        return new List<FontWeights> { Enums.FontWeights.Regular };

                    case CharisSILUrl:
                        return new List<FontWeights> { Enums.FontWeights.Regular };

                    case ElhamUrl:
                        return new List<FontWeights> { Enums.FontWeights.Regular };

                    case HomaUrl:
                        return new List<FontWeights> { Enums.FontWeights.Regular };

                    case FajerNooriNastaliqueUrl:
                        return new List<FontWeights> { Enums.FontWeights.Regular };

                    case KoodakUrl:
                        return new List<FontWeights> { Enums.FontWeights.Regular };

                    case MandatoryUrl:
                        return new List<FontWeights> { Enums.FontWeights.Regular };

                    case NafeesWebNaskhUrl:
                        return new List<FontWeights> { Enums.FontWeights.Regular };

                    case NazliUrl:
                        return new List<FontWeights> { Enums.FontWeights.Regular, Enums.FontWeights.Bold };

                    case PakNastaleeqUrl:
                        return new List<FontWeights> { Enums.FontWeights.Regular };

                    case RobotoUrl:
                        switch (FontStretch)
                        {
                            case Enums.FontStretches.Normal:
                                return new List<FontWeights> 
                                            { 
                                                Enums.FontWeights.Thin, 
                                                Enums.FontWeights.Light, 
                                                Enums.FontWeights.Regular, 
                                                Enums.FontWeights.Medium, 
                                                Enums.FontWeights.Bold, 
                                                Enums.FontWeights.Black 
                                            };

                            case Enums.FontStretches.Condensed:
                                return new List<FontWeights> 
                                            { 
                                                Enums.FontWeights.Light, 
                                                Enums.FontWeights.Regular, 
                                                Enums.FontWeights.Bold
                                            };
                        }
                        break;

                    case RoyaUrl:
                        return new List<FontWeights> { Enums.FontWeights.Regular, Enums.FontWeights.Bold };

                    case TerafikUrl:
                        return new List<FontWeights> { Enums.FontWeights.Regular, Enums.FontWeights.Bold };

                    case TitrUrl:
                        return new List<FontWeights> { Enums.FontWeights.Regular };
                }

                return null;
            }
        }
        
        public List<KeyValuePair<string, Enums.Keyboards>> StartupKeyboards
        {
            get
            {
                return new List<KeyValuePair<string, Enums.Keyboards>>
                {
                    new KeyValuePair<string, Enums.Keyboards>(Resources.ALPHA_LAYOUT, Enums.Keyboards.Alpha),
                    new KeyValuePair<string, Enums.Keyboards>(Resources.CONVERSATION_LAYOUT, Enums.Keyboards.ConversationAlpha),
                    new KeyValuePair<string, Enums.Keyboards>(Resources.CONVERSATION_NUM_SYM_LAYOUT, Enums.Keyboards.ConversationNumericAndSymbols),
                    new KeyValuePair<string, Enums.Keyboards>(Resources.CURRENCIES_LAYOUT_1, Enums.Keyboards.Currencies1),
                    new KeyValuePair<string, Enums.Keyboards>(Resources.CURRENCIES_LAYOUT_2, Enums.Keyboards.Currencies2),
                    new KeyValuePair<string, Enums.Keyboards>(Resources.CUSTOM_LAYOUT, Enums.Keyboards.CustomKeyboardFile),
                    new KeyValuePair<string, Enums.Keyboards>(Resources.DYNAMIC_KEYBOARD_LAYOUT, Enums.Keyboards.DynamicKeyboard),
                    new KeyValuePair<string, Enums.Keyboards>(Resources.DIACRITICS_LAYOUT_1, Enums.Keyboards.Diacritics1),
                    new KeyValuePair<string, Enums.Keyboards>(Resources.DIACRITICS_LAYOUT_2, Enums.Keyboards.Diacritics2),
                    new KeyValuePair<string, Enums.Keyboards>(Resources.DIACRITICS_LAYOUT_3, Enums.Keyboards.Diacritics3),
                    new KeyValuePair<string, Enums.Keyboards>(Resources.MENU_LAYOUT, Enums.Keyboards.Menu),
                    new KeyValuePair<string, Enums.Keyboards>(Resources.MINIMISED_LAYOUT, Enums.Keyboards.Minimised),
                    new KeyValuePair<string, Enums.Keyboards>(Resources.MOUSE_LAYOUT, Enums.Keyboards.Mouse),
                    new KeyValuePair<string, Enums.Keyboards>(Resources.NUMBERS_SYMBOLS_LAYOUT_1, Enums.Keyboards.NumericAndSymbols1),
                    new KeyValuePair<string, Enums.Keyboards>(Resources.NUMBERS_SYMBOLS_LAYOUT_2, Enums.Keyboards.NumericAndSymbols2),
                    new KeyValuePair<string, Enums.Keyboards>(Resources.NUMBERS_SYMBOLS_LAYOUT_3, Enums.Keyboards.NumericAndSymbols3),
                    new KeyValuePair<string, Enums.Keyboards>(Resources.PHYSICAL_KEYS_LAYOUT, Enums.Keyboards.PhysicalKeys),
                    new KeyValuePair<string, Enums.Keyboards>(Resources.SIZE_POSITION_LAYOUT, Enums.Keyboards.SizeAndPosition),
                    new KeyValuePair<string, Enums.Keyboards>(Resources.WEB_BROWSING_LAYOUT, Enums.Keyboards.WebBrowsing)
                };
            }
        }

        public List<KeyValuePair<string, Enums.DockEdges>> DockPositions
        {
            get
            {
                return new List<KeyValuePair<string, Enums.DockEdges>>
                {
                    new KeyValuePair<string, Enums.DockEdges>(Resources.TOP, Enums.DockEdges.Top),
                    new KeyValuePair<string, Enums.DockEdges>(Resources.BOTTOM, Enums.DockEdges.Bottom),
                    new KeyValuePair<string, Enums.DockEdges>(Resources.LEFT, Enums.DockEdges.Left),
                    new KeyValuePair<string, Enums.DockEdges>(Resources.RIGHT, Enums.DockEdges.Right),
                };
            }
        }

        public List<KeyValuePair<string, Enums.WindowStates>> MainWindowStates
        {
            get
            {
                return new List<KeyValuePair<string, Enums.WindowStates>>
                {
                    new KeyValuePair<string, Enums.WindowStates>("Floating", Enums.WindowStates.Floating),
                    new KeyValuePair<string, Enums.WindowStates>("Docked", Enums.WindowStates.Docked),
                };
            }
        }
        public List<KeyValuePair<string, Enums.MinimisedEdges>> MinimisedPositions
        {
            get
            {
                return new List<KeyValuePair<string, Enums.MinimisedEdges>>
                {
                    new KeyValuePair<string, Enums.MinimisedEdges>(Resources.SAME_AS_DOCK_POSITION, Enums.MinimisedEdges.SameAsDockedPosition),
                    new KeyValuePair<string, Enums.MinimisedEdges>(Resources.UP, Enums.MinimisedEdges.Top),
                    new KeyValuePair<string, Enums.MinimisedEdges>(Resources.RIGHT, Enums.MinimisedEdges.Right),
                    new KeyValuePair<string, Enums.MinimisedEdges>(Resources.BOTTOM, Enums.MinimisedEdges.Bottom),
                    new KeyValuePair<string, Enums.MinimisedEdges>(Resources.LEFT, Enums.MinimisedEdges.Left),
                };
            }
        }

        public List<KeyValuePair<string, Enums.Case>> KeyCases
        {
            get
            {
                return new List<KeyValuePair<string, Enums.Case>>
                {
                    new KeyValuePair<string, Enums.Case>(Resources.UPPER_CASE, Enums.Case.Upper),
                    new KeyValuePair<string, Enums.Case>(Resources.LOWER_CASE, Enums.Case.Lower),
                    new KeyValuePair<string, Enums.Case>(Resources.TITLE_CASE, Enums.Case.Title)
                };
            }
        }
        
        private string theme;
        public string Theme
        {
            get { return theme; }
            set { SetProperty(ref theme, value); }
        }

        private string fontFamily;
        public string FontFamily
        {
            get { return fontFamily; }
            set
            {
                SetProperty(ref fontFamily, value);
                
                RaisePropertyChanged(nameof(FontStretches));
                if (FontStretches != null)
                {
                    //Retain current stretch if available; otherwise pick the first one
                    FontStretch = FontStretches.Contains(FontStretch) ? FontStretch : FontStretches[0];
                }
                
                RaisePropertyChanged(nameof(FontWeights));
            }
        }

        private FontStretches fontStretch;
        public FontStretches FontStretch
        {
            get { return fontStretch; }
            set
            {
                SetProperty(ref fontStretch, value);

                RaisePropertyChanged(nameof(FontWeights));
                if (FontWeights != null)
                {
                    //Retain current weight if available; otherwise pick the first one
                    FontWeight = FontWeights.Contains(FontWeight) ? FontWeight : FontWeights[0];
                }
            }
        }

        private FontWeights fontWeight;
        public FontWeights FontWeight
        {
            get { return fontWeight; }
            set { SetProperty(ref fontWeight, value); }
        }

        private Case keyCase;
        public Case KeyCase
        {
            get { return keyCase; }
            set { SetProperty(ref keyCase, value); }
        }

        private Enums.DockEdges dockPosition;
        public Enums.DockEdges DockPosition
        {
            get { return dockPosition; }
            set { SetProperty(ref dockPosition, value); }
        }

        private int scratchpadNumberOfLines;
        public int ScratchpadNumberOfLines
        {
            get { return scratchpadNumberOfLines; }
            set { SetProperty(ref scratchpadNumberOfLines, value); }
        }

        private int toastNotificationHorizontalFillPercentage;
        public int ToastNotificationHorizontalFillPercentage
        {
            get { return toastNotificationHorizontalFillPercentage; }
            set { SetProperty(ref toastNotificationHorizontalFillPercentage, value); }
        }
        
        private int toastNotificationVerticalFillPercentage;
        public int ToastNotificationVerticalFillPercentage
        {
            get { return toastNotificationVerticalFillPercentage; }
            set { SetProperty(ref toastNotificationVerticalFillPercentage, value); }
        }

        private decimal toastNotificationSecondsPerCharacter;
        public decimal ToastNotificationSecondsPerCharacter
        {
            get { return toastNotificationSecondsPerCharacter; }
            set { SetProperty(ref toastNotificationSecondsPerCharacter, value); }
        }

        private int cursorWidthInPixels;
        public int CursorWidthInPixels
        {
            get { return cursorWidthInPixels; }
            set { SetProperty(ref cursorWidthInPixels, value); }
        }

        private int cursorHeightInPixels;
        public int CursorHeightInPixels
        {
            get { return cursorHeightInPixels; }
            set { SetProperty(ref cursorHeightInPixels, value); }
        }

        public List<string> GazeIndicatorStyleList
        {
            get { return Enum.GetNames(typeof(GazeIndicatorStyles)).Cast<string>().ToList(); }
        }

        private string gazeIndicatorStyle;
        public string GazeIndicatorStyle
        {
            get { return gazeIndicatorStyle; }
            set { SetProperty(ref gazeIndicatorStyle, value); }
        }

        private int gazeIndicatorSize;
        public int GazeIndicatorSize
        {
            get { return gazeIndicatorSize; }
            set { Settings.Default.GazeIndicatorSize = value; SetProperty(ref gazeIndicatorSize, value); }
        }

        private bool magnifierCenterOnScreen;
        public bool MagnifierCenterOnScreen
        {
            get { return magnifierCenterOnScreen; }
            set { SetProperty(ref magnifierCenterOnScreen, value); }
        }

        private double magnifySourcePercentageOfScreen;
        public double MagnifySourcePercentageOfScreen
        {
            get { return magnifySourcePercentageOfScreen; }
            set { SetProperty(ref magnifySourcePercentageOfScreen, value); }
        }

        private double magnifyDestinationPercentageOfScreen;
        public double MagnifyDestinationPercentageOfScreen
        {
            get { return magnifyDestinationPercentageOfScreen; }
            set { SetProperty(ref magnifyDestinationPercentageOfScreen, value); }
        }

        private bool conversationOnlyMode;
        public bool ConversationOnlyMode
        {
            get { return conversationOnlyMode; }
            set { SetProperty(ref conversationOnlyMode, value); }
        }

        private bool conversationconfirmEnable;
        public bool ConversationConfirmEnable
        {
            get { return conversationconfirmEnable; }
            set { SetProperty(ref conversationconfirmEnable, value); }
        }

        private bool conversationconfirmOnlyMode;
        public bool ConversationConfirmOnlyMode
        {
            get { return conversationconfirmOnlyMode; }
            set { SetProperty(ref conversationconfirmOnlyMode, value); }
        }

        private Enums.Keyboards startupKeyboard;
        public Enums.Keyboards StartupKeyboard
        {
            get { return startupKeyboard; }
            set { SetProperty(ref startupKeyboard, value); }
        }

        private Enums.MinimisedEdges minimsedPosition;
        public Enums.MinimisedEdges MinimisedPosition
        {
            get { return minimsedPosition; }
            set { SetProperty(ref minimsedPosition, value); }
        }

        private double mainWindowFullDockThicknessAsPercentageOfScreen;
        public double MainWindowFullDockThicknessAsPercentageOfScreen
        {
            get { return mainWindowFullDockThicknessAsPercentageOfScreen; }
            set { SetProperty(ref mainWindowFullDockThicknessAsPercentageOfScreen, value); }
        }

        private double mainWindowCollapsedDockThicknessAsPercentageOfFullDockThickness;
        public double MainWindowCollapsedDockThicknessAsPercentageOfFullDockThickness
        {
            get { return mainWindowCollapsedDockThicknessAsPercentageOfFullDockThickness; }
            set { SetProperty(ref mainWindowCollapsedDockThicknessAsPercentageOfFullDockThickness, value); }
        }

        private bool enableResizeWithMouse;
        public bool EnableResizeWithMouse
        {
            get { return enableResizeWithMouse; }
            set { SetProperty(ref enableResizeWithMouse, value); }
        }

        private Thickness conversationBorderThickness;
        public Thickness ConversationBorderThickness
        {
            get { return conversationBorderThickness; }
            set { SetProperty(ref conversationBorderThickness, value); }
        }

        public bool ChangesRequireRestart
        {
            get
            {
                return Settings.Default.ConversationOnlyMode != ConversationOnlyMode
                    || Settings.Default.ConversationConfirmEnable != ConversationConfirmEnable
                    || Settings.Default.ConversationConfirmOnlyMode != ConversationConfirmOnlyMode;
            }
        }

        private string dynamicKeyboardsLocation;
        public string DynamicKeyboardsLocation
        {
            get { return dynamicKeyboardsLocation; }
            set { SetProperty(ref dynamicKeyboardsLocation, value); }
        }

        private string startupKeyboardFile;
        public string StartupKeyboardFile
        {
            get { return startupKeyboardFile; }
            set { SetProperty(ref startupKeyboardFile, value); }
        }

        private WindowStates mainWindowState;
        public WindowStates MainWindowState
        {
            get { return mainWindowState; }
            set { SetProperty(ref mainWindowState, value); }
        }

        public bool MaximisedApp
        {
            get
            {
                // Is this an always-maximised app?
                AppType appType = Settings.Default.GetApp();
                return (appType == AppType.Chat || appType == AppType.Symbol);
            }
        }


        #endregion

        #region Methods

        private void Load()
        {
            FloatingSizeAndPosition = Settings.Default.MainWindowFloatingSizeAndPosition;
            Theme = Settings.Default.Theme;
            Opacity = (int)(100.0f*Settings.Default.MainWindowOpacity);
            FontFamily = Settings.Default.FontFamily;
            FontStretch = (FontStretches)Enum.Parse(typeof(FontStretches), Settings.Default.FontStretch);
            FontWeight = (FontWeights)Enum.Parse(typeof(FontWeights), Settings.Default.FontWeight);
            ScratchpadNumberOfLines = Settings.Default.ScratchpadNumberOfLines;
            ToastNotificationVerticalFillPercentage = Settings.Default.ToastNotificationVerticalFillPercentage;
            ToastNotificationHorizontalFillPercentage = Settings.Default.ToastNotificationHorizontalFillPercentage;
            ToastNotificationSecondsPerCharacter = Settings.Default.ToastNotificationSecondsPerCharacter;
            CursorWidthInPixels = Settings.Default.CursorWidthInPixels;
            CursorHeightInPixels = Settings.Default.CursorHeightInPixels;
            GazeIndicatorStyle = Settings.Default.GazeIndicatorStyle.ToString();
            GazeIndicatorSize = Settings.Default.GazeIndicatorSize;
            MagnifierCenterOnScreen = Settings.Default.MagnifierCenterOnScreen;
            MagnifySourcePercentageOfScreen = Settings.Default.MagnifySourcePercentageOfScreen;
            MagnifyDestinationPercentageOfScreen = Settings.Default.MagnifyDestinationPercentageOfScreen;
            ConversationOnlyMode = Settings.Default.ConversationOnlyMode;
            ConversationConfirmEnable = Settings.Default.ConversationConfirmEnable;
            ConversationConfirmOnlyMode = Settings.Default.ConversationConfirmOnlyMode;
            StartupKeyboard = Settings.Default.StartupKeyboard;
            MinimisedPosition = Settings.Default.MainWindowMinimisedPosition;
            KeyCase = Settings.Default.KeyCase;
            MainWindowFullDockThicknessAsPercentageOfScreen = Settings.Default.MainWindowFullDockThicknessAsPercentageOfScreen;
            MainWindowCollapsedDockThicknessAsPercentageOfFullDockThickness = Settings.Default.MainWindowCollapsedDockThicknessAsPercentageOfFullDockThickness;
            ConversationBorderThickness = Settings.Default.ConversationBorderThickness;
            DynamicKeyboardsLocation = Settings.Default.DynamicKeyboardsLocation;
            StartupKeyboardFile = Settings.Default.StartupKeyboardFile;
            DockPosition = Settings.Default.MainWindowDockPosition;
            MainWindowState = Settings.Default.MainWindowState;
            EnableResizeWithMouse = Settings.Default.EnableResizeWithMouse;
        }

        public void ApplyChanges()
        {
            Settings.Default.Theme = Theme;
            Settings.Default.MainWindowOpacity = (double) Opacity / 100.0f;
            Settings.Default.FontFamily = FontFamily;
            Settings.Default.FontStretch = FontStretch.ToString();
            Settings.Default.FontWeight = FontWeight.ToString();
            Settings.Default.ScratchpadNumberOfLines = ScratchpadNumberOfLines;
            Settings.Default.ToastNotificationVerticalFillPercentage = ToastNotificationVerticalFillPercentage;
            Settings.Default.ToastNotificationHorizontalFillPercentage = ToastNotificationHorizontalFillPercentage;
            Settings.Default.ToastNotificationSecondsPerCharacter = ToastNotificationSecondsPerCharacter;
            Settings.Default.CursorWidthInPixels = CursorWidthInPixels;
            Settings.Default.CursorHeightInPixels = CursorHeightInPixels;
            Settings.Default.GazeIndicatorStyle = (GazeIndicatorStyles)Enum.Parse(typeof(GazeIndicatorStyles), GazeIndicatorStyle);
            Settings.Default.GazeIndicatorSize = GazeIndicatorSize;
            Settings.Default.MagnifierCenterOnScreen = MagnifierCenterOnScreen;
            Settings.Default.MagnifySourcePercentageOfScreen = MagnifySourcePercentageOfScreen;
            Settings.Default.MagnifyDestinationPercentageOfScreen = MagnifyDestinationPercentageOfScreen;
            Settings.Default.ConversationOnlyMode = ConversationOnlyMode;
            Settings.Default.ConversationConfirmEnable = ConversationConfirmEnable;
            Settings.Default.ConversationConfirmOnlyMode = ConversationConfirmOnlyMode;
            Settings.Default.StartupKeyboard = StartupKeyboard;
            Settings.Default.MainWindowMinimisedPosition = MinimisedPosition;
            Settings.Default.KeyCase = KeyCase;
            Settings.Default.MainWindowCollapsedDockThicknessAsPercentageOfFullDockThickness = MainWindowCollapsedDockThicknessAsPercentageOfFullDockThickness;
            Settings.Default.ConversationBorderThickness = ConversationBorderThickness;
            Settings.Default.DynamicKeyboardsLocation = DynamicKeyboardsLocation;
            Settings.Default.StartupKeyboardFile = StartupKeyboardFile;

            // We don't apply changes to window/size position if Optikey's state has changed to one in which re-positioning isn't supported
            bool allowReposition = windowManipulationService.GetPersistedState() &&
                                   Settings.Default.MainWindowState != WindowStates.Maximised &&
                                   Settings.Default.MainWindowState != WindowStates.Minimised &&
                                   Settings.Default.MainWindowState != WindowStates.Hidden &&
                                   Settings.Default.MainWindowDockSize != DockSizes.Collapsed;

            if (allowReposition) 
            {
                // Changes to window state, these methods will save the new values also
                if (Settings.Default.MainWindowState != MainWindowState ||
                    Settings.Default.MainWindowDockPosition != DockPosition ||
                    !Settings.Default.MainWindowFullDockThicknessAsPercentageOfScreen.IsCloseTo(
                        MainWindowFullDockThicknessAsPercentageOfScreen) ||
                    !Settings.Default.MainWindowFloatingSizeAndPosition.IsCloseTo(FloatingSizeAndPosition))
                {
                    Settings.Default.MainWindowFullDockThicknessAsPercentageOfScreen = MainWindowFullDockThicknessAsPercentageOfScreen;
                    Settings.Default.MainWindowFloatingSizeAndPosition = FloatingSizeAndPosition;

                    // this also saves the changes
                    windowManipulationService.ChangeState(MainWindowState, DockPosition);
                }
            }

            windowManipulationService.SetOpacity(Settings.Default.MainWindowOpacity);
            Settings.Default.EnableResizeWithMouse = EnableResizeWithMouse;
        }

        #endregion
    }
}
