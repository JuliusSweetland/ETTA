// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using System.Collections.Generic;
using JuliusSweetland.OptiKey.Enums;
using JuliusSweetland.OptiKey.Models;
using JuliusSweetland.OptiKey.Properties;
using JuliusSweetland.OptiKey.Services.PluginEngine;
using log4net;
using Prism.Mvvm;
using JuliusSweetland.OptiKey.Services.Translation.Languages;
using JuliusSweetland.OptiKey.Services.Translation;
using System.Windows.Media;
using System.Linq;

namespace JuliusSweetland.OptiKey.UI.ViewModels.Management
{
    public class FeaturesViewModel : BindableBase
    {
        #region Private Member Vars

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Ctor

        public FeaturesViewModel()
        {
            Load();
        }

        #endregion

        #region Properties

        private bool enableQuitKeys;
        public bool EnableQuitKeys
        {
            get { return enableQuitKeys; }
            set { SetProperty(ref enableQuitKeys, value); }
        }

        private bool enableAttentionKey;
        public bool EnableAttentionKey
        {
            get { return enableAttentionKey; }
            set { SetProperty(ref enableAttentionKey, value); }
        }

        private bool enableCopyAllScratchpadKey;
        public bool EnableCopyAllScratchpadKey
        {
            get { return enableCopyAllScratchpadKey; }
            set { SetProperty(ref enableCopyAllScratchpadKey, value); }
        }

        private bool enableTranslationKey;

        public bool EnableTranslationKey
        {
            get { return enableTranslationKey; }
            set { SetProperty(ref enableTranslationKey, value); }
        }

        private string translationTargetLanguage;
        public string TranslationTargetLanguage
        {
            get { return translationTargetLanguage; }
            set
            {
                SetProperty(ref translationTargetLanguage, value);
            }
        }

        public static List<KeyValuePair<string, string>> AvailableTranslationTargetLanguages
        {
            get
            {
                return new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(TranslationResources.AZERBAIJANI, TranslationTargetLanguages.AZERBAIJANI),
                    new KeyValuePair<string, string>(TranslationResources.ALBANIAN, TranslationTargetLanguages.ALBANIAN),
                    new KeyValuePair<string, string>(TranslationResources.ARABIC, TranslationTargetLanguages.ARABIC),
                    new KeyValuePair<string, string>(TranslationResources.ARMENIAN, TranslationTargetLanguages.ARMENIAN),
                    new KeyValuePair<string, string>(TranslationResources.BASHKIR, TranslationTargetLanguages.BASHKIR),
                    new KeyValuePair<string, string>(TranslationResources.BELARUSIAN, TranslationTargetLanguages.BELARUSIAN),
                    new KeyValuePair<string, string>(TranslationResources.BULGARIAN, TranslationTargetLanguages.BULGARIAN),
                    new KeyValuePair<string, string>(TranslationResources.CATALAN, TranslationTargetLanguages.CATALAN),
                    new KeyValuePair<string, string>(TranslationResources.CROATIAN, TranslationTargetLanguages.CROATIAN),
                    new KeyValuePair<string, string>(TranslationResources.CZECH, TranslationTargetLanguages.CZECH),
                    new KeyValuePair<string, string>(TranslationResources.DANISH, TranslationTargetLanguages.DANISH),
                    new KeyValuePair<string, string>(TranslationResources.DUTCH, TranslationTargetLanguages.DUTCH),
                    new KeyValuePair<string, string>(TranslationResources.ENGLISH, TranslationTargetLanguages.ENGLISH),
                    new KeyValuePair<string, string>(TranslationResources.ESTONIAN, TranslationTargetLanguages.ESTONIAN),
                    new KeyValuePair<string, string>(TranslationResources.FINNISH, TranslationTargetLanguages.FINNISH),
                    new KeyValuePair<string, string>(TranslationResources.FRENCH, TranslationTargetLanguages.FRENCH),
                    new KeyValuePair<string, string>(TranslationResources.GEORGIAN, TranslationTargetLanguages.GEORGIAN),
                    new KeyValuePair<string, string>(TranslationResources.GERMAN, TranslationTargetLanguages.GERMAN),
                    new KeyValuePair<string, string>(TranslationResources.GREEK, TranslationTargetLanguages.GREEK),
                    new KeyValuePair<string, string>(TranslationResources.HEBREW, TranslationTargetLanguages.HEBREW),
                    new KeyValuePair<string, string>(TranslationResources.HUNGARIAN, TranslationTargetLanguages.HUNGARIAN),
                    new KeyValuePair<string, string>(TranslationResources.ITALIAN, TranslationTargetLanguages.ITALIAN),
                    new KeyValuePair<string, string>(TranslationResources.KAZAKH, TranslationTargetLanguages.KAZAKH),
                    new KeyValuePair<string, string>(TranslationResources.LATVIAN, TranslationTargetLanguages.LATVIAN),
                    new KeyValuePair<string, string>(TranslationResources.LITHUANIAN, TranslationTargetLanguages.LITHUANIAN),
                    new KeyValuePair<string, string>(TranslationResources.MACEDONIAN, TranslationTargetLanguages.MACEDONIAN),
                    new KeyValuePair<string, string>(TranslationResources.NORWEGIAN, TranslationTargetLanguages.NORWEGIAN),
                    new KeyValuePair<string, string>(TranslationResources.PERSIAN, TranslationTargetLanguages.PERSIAN),
                    new KeyValuePair<string, string>(TranslationResources.POLISH, TranslationTargetLanguages.POLISH),
                    new KeyValuePair<string, string>(TranslationResources.PORTUGUESE, TranslationTargetLanguages.PORTUGUESE),
                    new KeyValuePair<string, string>(TranslationResources.ROMANIAN, TranslationTargetLanguages.ROMANIAN),
                    new KeyValuePair<string, string>(TranslationResources.RUSSIAN, TranslationTargetLanguages.RUSSIAN),
                    new KeyValuePair<string, string>(TranslationResources.SERBIAN, TranslationTargetLanguages.SERBIAN),
                    new KeyValuePair<string, string>(TranslationResources.SLOVAK, TranslationTargetLanguages.SLOVAK),
                    new KeyValuePair<string, string>(TranslationResources.SLOVENIAN, TranslationTargetLanguages.SLOVENIAN),
                    new KeyValuePair<string, string>(TranslationResources.SPANISH, TranslationTargetLanguages.SPANISH),
                    new KeyValuePair<string, string>(TranslationResources.SWEDISH, TranslationTargetLanguages.SWEDISH),
                    new KeyValuePair<string, string>(TranslationResources.TATAR, TranslationTargetLanguages.TATAR),
                    new KeyValuePair<string, string>(TranslationResources.TURKISH, TranslationTargetLanguages.TURKISH),
                    new KeyValuePair<string, string>(TranslationResources.UKRAINIAN, TranslationTargetLanguages.UKRAINIAN)
                };
            }
        }

        private bool enableOverrideTranslationApiKey;

        public bool EnableOverrideTranslationApiKey
        {
            get { return enableOverrideTranslationApiKey; }
            set { SetProperty(ref enableOverrideTranslationApiKey, value); }
        }

        private string overriddenTranslationApiKey;
        public string OverriddenTranslationApiKey
        {
            get { return overriddenTranslationApiKey; }
            set { SetProperty(ref overriddenTranslationApiKey, value); }
        }

        private bool enableCommuniKateKeyboardLayout;
        public bool EnableCommuniKateKeyboardLayout
        {
            get { return enableCommuniKateKeyboardLayout; }
            set
            {
                SetProperty(ref enableCommuniKateKeyboardLayout, value);
            }
        }

        private bool allowMultipleInstances;
        public bool AllowMultipleInstances
        {
            get { return allowMultipleInstances; }
            set
            {
                SetProperty(ref allowMultipleInstances, value);
            }
        }

        public List<string> ColourNames
        {
            get
            {
                // Based on: https://stackoverflow.com/a/26287682/9091159
                return typeof(Brushes)
                    .GetProperties()
                    .Where(pi => pi.PropertyType == typeof(SolidColorBrush))
                    .Select(pi => pi.Name)
                    .ToList();
            }
        }

        private bool showSplashScreen;
        public bool ShowSplashScreen
        {
            get { return showSplashScreen; }
            set { SetProperty(ref showSplashScreen, value); }
        }

        private bool checkForUpdates;
        public bool CheckForUpdates
        {
            get { return checkForUpdates; }
            set { SetProperty(ref checkForUpdates, value); }
        }

        private bool publishVirtualKeyCodesForCharacters;
        public bool PublishVirtualKeyCodesForCharacters
        {
            get { return publishVirtualKeyCodesForCharacters; }
            set { SetProperty(ref publishVirtualKeyCodesForCharacters, value); }
        }

        private bool suppressTriggerWarning;
        public bool SuppressTriggerWarning
        {
            get { return suppressTriggerWarning; }
            set { SetProperty(ref suppressTriggerWarning, value); }
        }

        private bool suppressModifierKeysForAllMouseActions;
        public bool SuppressModifierKeysForAllMouseActions
        {
            get { return suppressModifierKeysForAllMouseActions; }
            set { SetProperty(ref suppressModifierKeysForAllMouseActions, value); }
        }

        private bool suppressModifierKeysWhenInMouseKeyboard;
        public bool SuppressModifierKeysWhenInMouseKeyboard
        {
            get { return suppressModifierKeysWhenInMouseKeyboard; }
            set { SetProperty(ref suppressModifierKeysWhenInMouseKeyboard, value); }
        }

        private bool magnifySuppressedForScrollingActions;
        public bool MagnifySuppressedForScrollingActions
        {
            get { return magnifySuppressedForScrollingActions; }
            set { SetProperty(ref magnifySuppressedForScrollingActions, value); }
        }

        private bool debug;
        public bool Debug
        {
            get { return debug; }
            set { SetProperty(ref debug, value); }
        }

        private bool lookToScrollIsDefault;
        public bool LookToScrollIsDefault
        {
            get { return lookToScrollIsDefault; }
            set { SetProperty(ref lookToScrollIsDefault, value); }
        }

        public List<KeyValuePair<string, LookToScrollBounds>> LookToScrollBoundsList
        {
            get
            {
                return new List<KeyValuePair<string, LookToScrollBounds>>
                {
                    new KeyValuePair<string, LookToScrollBounds>(LookToScrollBounds.ScreenCentred.ToString(), LookToScrollBounds.ScreenCentred),
                    new KeyValuePair<string, LookToScrollBounds>(LookToScrollBounds.ScreenPoint.ToString(), LookToScrollBounds.ScreenPoint),
                    new KeyValuePair<string, LookToScrollBounds>(LookToScrollBounds.Window.ToString(), LookToScrollBounds.Window),
                    new KeyValuePair<string, LookToScrollBounds>(LookToScrollBounds.Subwindow.ToString(), LookToScrollBounds.Subwindow),
                    new KeyValuePair<string, LookToScrollBounds>(LookToScrollBounds.Custom.ToString(), LookToScrollBounds.Custom)
                };
            }
        }

        private LookToScrollBounds lookToScrollBounds;
        public LookToScrollBounds LookToScrollBounds
        {
            get { return lookToScrollBounds; }
            set { SetProperty(ref lookToScrollBounds, value); }
        }

        public List<KeyValuePair<string, LookToScrollModes>> LookToScrollModesList
        {
            get
            {
                return new List<KeyValuePair<string, LookToScrollModes>>
                {
                    new KeyValuePair<string, LookToScrollModes>(LookToScrollModes.Free.ToString(), LookToScrollModes.Free),
                    new KeyValuePair<string, LookToScrollModes>(LookToScrollModes.Cross.ToString(), LookToScrollModes.Cross),
                    new KeyValuePair<string, LookToScrollModes>(LookToScrollModes.Horizontal.ToString(), LookToScrollModes.Horizontal),
                    new KeyValuePair<string, LookToScrollModes>(LookToScrollModes.Vertical.ToString(), LookToScrollModes.Vertical),
                };
            }
        }

        private LookToScrollModes lookToScrollMode;
        public LookToScrollModes LookToScrollMode
        {
            get { return lookToScrollMode; }
            set { SetProperty(ref lookToScrollMode, value); }
        }

        private bool lookToScrollCentreMouseWhenActivated;
        public bool LookToScrollCentreMouseWhenActivated
        {
            get { return lookToScrollCentreMouseWhenActivated; }
            set { SetProperty(ref lookToScrollCentreMouseWhenActivated, value); }
        }

        private bool lookToScrollBringWindowToFrontWhenActivated;
        public bool LookToScrollBringWindowToFrontWhenActivated
        {
            get { return lookToScrollBringWindowToFrontWhenActivated; }
            set { SetProperty(ref lookToScrollBringWindowToFrontWhenActivated, value); }
        }

        private bool lookToScrollResumeAfterChoosingPointForMouse;
        public bool LookToScrollResumeAfterChoosingPointForMouse
        {
            get { return lookToScrollResumeAfterChoosingPointForMouse; }
            set { SetProperty(ref lookToScrollResumeAfterChoosingPointForMouse, value); }
        }

        private bool lookToScrollDeactivateUponSwitchingKeyboards;
        public bool LookToScrollDeactivateUponSwitchingKeyboards
        {
            get { return lookToScrollDeactivateUponSwitchingKeyboards; }
            set { SetProperty(ref lookToScrollDeactivateUponSwitchingKeyboards, value); }
        }

        private string lookToScrollOverlayBoundsColour;
        public string LookToScrollOverlayBoundsColour
        {
            get { return lookToScrollOverlayBoundsColour; }
            set { SetProperty(ref lookToScrollOverlayBoundsColour, value); }
        }

        private string lookToScrollOverlayDeadzoneColour;
        public string LookToScrollOverlayDeadzoneColour
        {
            get { return lookToScrollOverlayDeadzoneColour; }
            set { SetProperty(ref lookToScrollOverlayDeadzoneColour, value); }
        }

        private int lookToScrollOverlayBoundsThickness;
        public int LookToScrollOverlayBoundsThickness
        {
            get { return lookToScrollOverlayBoundsThickness; }
            set { SetProperty(ref lookToScrollOverlayBoundsThickness, value); }
        }

        private int lookToScrollOverlayDeadzoneThickness;
        public int LookToScrollOverlayDeadzoneThickness
        {
            get { return lookToScrollOverlayDeadzoneThickness; }
            set { SetProperty(ref lookToScrollOverlayDeadzoneThickness, value); }
        }

        private int lookToScrollHorizontalDeadzone;
        public int LookToScrollHorizontalDeadzone
        {
            get { return lookToScrollHorizontalDeadzone; }
            set { SetProperty(ref lookToScrollHorizontalDeadzone, value); }
        }

        private int lookToScrollVerticalDeadzone;
        public int LookToScrollVerticalDeadzone
        {
            get { return lookToScrollVerticalDeadzone; }
            set { SetProperty(ref lookToScrollVerticalDeadzone, value); }
        }

        public List<KeyValuePair<string, Enums.LookToScrollSpeeds>> LookToScrollSpeeds
        {
            get
            {
                return new List<KeyValuePair<string, Enums.LookToScrollSpeeds>>
                {
                    new KeyValuePair<string, LookToScrollSpeeds>(Enums.LookToScrollSpeeds.Slow.ToString(), Enums.LookToScrollSpeeds.Slow),
                    new KeyValuePair<string, LookToScrollSpeeds>(Enums.LookToScrollSpeeds.Medium.ToString(), Enums.LookToScrollSpeeds.Medium),
                    new KeyValuePair<string, LookToScrollSpeeds>(Enums.LookToScrollSpeeds.Fast.ToString(), Enums.LookToScrollSpeeds.Fast),
                };
            }
        }

        private LookToScrollSpeeds lookToScrollSpeed;
        public LookToScrollSpeeds LookToScrollSpeed
        {
            get { return lookToScrollSpeed; }
            set { SetProperty(ref lookToScrollSpeed, value); }
        }

        private string pluginsLocation;
        public string PluginsLocation
        {
            get { return pluginsLocation; }
            set { SetProperty(ref pluginsLocation, value); }
        }

        public List<Plugin> AvailablePlugins
        {
            get
            {
                return PluginEngine.GetAllAvailablePlugins();
            }
        }

        public bool ChangesRequireRestart
        {
            get
            {

                return Settings.Default.Debug != Debug
                    || Settings.Default.CommuniKatePagesetLocation != CommuniKatePagesetLocation
                    || Settings.Default.LookToScrollIsDefault != LookToScrollIsDefault;
            }
        }

        private bool enablePlugins;
        public bool EnablePlugins
        {
            get { return enablePlugins; }
            set { SetProperty(ref enablePlugins, value); }
        }

        #endregion

        #region Communikate properties

        private string communiKatePagesetLocation;
        public string CommuniKatePagesetLocation
        {
            get { return communiKatePagesetLocation; }
            set { SetProperty(ref communiKatePagesetLocation, value); }
        }

        private bool communiKateStagedForDeletion;
        public bool CommuniKateStagedForDeletion
        {
            get { return communiKateStagedForDeletion; }
            set { SetProperty(ref communiKateStagedForDeletion, value); }
        }

        private int communiKateSoundVolume;
        public int CommuniKateSoundVolume
        {
            get { return communiKateSoundVolume; }
            set { SetProperty(ref communiKateSoundVolume, value); }
        }

        private bool communiKateSpeakSelected;
        public bool CommuniKateSpeakSelected
        {
            get { return communiKateSpeakSelected; }
            set { SetProperty(ref communiKateSpeakSelected, value); }
        }

        private int communiKateSpeakSelectedVolume;
        public int CommuniKateSpeakSelectedVolume
        {
            get { return communiKateSpeakSelectedVolume; }
            set { SetProperty(ref communiKateSpeakSelectedVolume, value); }
        }

        private int communiKateSpeakSelectedRate;
        public int CommuniKateSpeakSelectedRate
        {
            get { return communiKateSpeakSelectedRate; }
            set { SetProperty(ref communiKateSpeakSelectedRate, value); }
        }

        #endregion

        #region Methods

        private void Load()
        {
            EnableQuitKeys = Settings.Default.EnableQuitKeys;
            EnableAttentionKey = Settings.Default.EnableAttentionKey;
            EnableCopyAllScratchpadKey = Settings.Default.EnableCopyAllScratchpadKey;
            EnableTranslationKey = Settings.Default.EnableTranslationKey;
            TranslationTargetLanguage = Settings.Default.TranslationTargetLanguage;
            EnableOverrideTranslationApiKey = Settings.Default.EnableOverrideTranslationApiKey;
            OverriddenTranslationApiKey = Settings.Default.OverriddenTranslationApiKey;

            ShowSplashScreen = Settings.Default.ShowSplashScreen;
            CheckForUpdates = Settings.Default.CheckForUpdates;
            PublishVirtualKeyCodesForCharacters = Settings.Default.PublishVirtualKeyCodesForCharacters;
            SuppressModifierKeysForAllMouseActions = Settings.Default.SuppressModifierKeysForAllMouseActions;
            SuppressModifierKeysWhenInMouseKeyboard = Settings.Default.SuppressModifierKeysWhenInMouseKeyboard;
            SuppressTriggerWarning = Settings.Default.SuppressTriggerWithoutPositionError;
            MagnifySuppressedForScrollingActions = Settings.Default.MagnifySuppressedForScrollingActions;
            Debug = Settings.Default.Debug;

            LookToScrollIsDefault = Settings.Default.LookToScrollIsDefault;
            LookToScrollBounds = Settings.Default.LookToScrollBounds;
            LookToScrollMode = Settings.Default.LookToScrollMode;
            LookToScrollCentreMouseWhenActivated = Settings.Default.LookToScrollCentreMouseWhenActivated;
            LookToScrollBringWindowToFrontWhenActivated = Settings.Default.LookToScrollBringWindowToFrontWhenActivated;
            LookToScrollResumeAfterChoosingPointForMouse = Settings.Default.LookToScrollResumeAfterChoosingPointForMouse;
            LookToScrollDeactivateUponSwitchingKeyboards = Settings.Default.LookToScrollDeactivateUponSwitchingKeyboards;
            LookToScrollOverlayBoundsColour = Settings.Default.LookToScrollOverlayBoundsColour;
            LookToScrollOverlayDeadzoneColour = Settings.Default.LookToScrollOverlayDeadzoneColour;
            LookToScrollOverlayBoundsThickness = Settings.Default.LookToScrollOverlayBoundsThickness;
            LookToScrollOverlayDeadzoneThickness = Settings.Default.LookToScrollOverlayDeadzoneThickness;
            LookToScrollHorizontalDeadzone = Settings.Default.LookToScrollHorizontalDeadzone;
            LookToScrollVerticalDeadzone = Settings.Default.LookToScrollVerticalDeadzone;
            LookToScrollSpeed = Settings.Default.LookToScrollSpeed;

            EnableCommuniKateKeyboardLayout = Settings.Default.EnableCommuniKateKeyboardLayout;
            CommuniKatePagesetLocation = Settings.Default.CommuniKatePagesetLocation;
            CommuniKateStagedForDeletion = Settings.Default.CommuniKateStagedForDeletion;
            CommuniKateSoundVolume = Settings.Default.CommuniKateSoundVolume;
            CommuniKateSpeakSelected = Settings.Default.CommuniKateSpeakSelected;
            CommuniKateSpeakSelectedVolume = Settings.Default.CommuniKateSpeakSelectedVolume;
            CommuniKateSpeakSelectedRate = Settings.Default.CommuniKateSpeakSelectedRate;

            PluginsLocation = Settings.Default.PluginsLocation;
            EnablePlugins = Settings.Default.EnablePlugins;
            AllowMultipleInstances = Settings.Default.AllowMultipleInstances;
        }

        public void ApplyChanges()
        {
            Settings.Default.EnableQuitKeys = EnableQuitKeys;
            Settings.Default.EnableAttentionKey = EnableAttentionKey;
            Settings.Default.EnableCopyAllScratchpadKey = EnableCopyAllScratchpadKey;
            Settings.Default.EnableTranslationKey = EnableTranslationKey;
            Settings.Default.TranslationTargetLanguage = TranslationTargetLanguage;
            Settings.Default.EnableOverrideTranslationApiKey = EnableOverrideTranslationApiKey;
            if (EnableOverrideTranslationApiKey)
            {
                Settings.Default.OverriddenTranslationApiKey = OverriddenTranslationApiKey;
            }

            Settings.Default.ShowSplashScreen = ShowSplashScreen;
            Settings.Default.CheckForUpdates = CheckForUpdates;
            Settings.Default.PublishVirtualKeyCodesForCharacters = PublishVirtualKeyCodesForCharacters;
            Settings.Default.SuppressModifierKeysForAllMouseActions = SuppressModifierKeysForAllMouseActions;
            Settings.Default.SuppressModifierKeysWhenInMouseKeyboard = SuppressModifierKeysWhenInMouseKeyboard;
            Settings.Default.SuppressTriggerWithoutPositionError = SuppressTriggerWarning;
            Settings.Default.MagnifySuppressedForScrollingActions = MagnifySuppressedForScrollingActions;
            Settings.Default.Debug = Debug;

            Settings.Default.LookToScrollIsDefault = LookToScrollIsDefault;
            Settings.Default.LookToScrollBounds = LookToScrollBounds;
            Settings.Default.LookToScrollMode = LookToScrollMode;
            Settings.Default.LookToScrollCentreMouseWhenActivated = LookToScrollCentreMouseWhenActivated;
            Settings.Default.LookToScrollBringWindowToFrontWhenActivated = LookToScrollBringWindowToFrontWhenActivated;
            Settings.Default.LookToScrollResumeAfterChoosingPointForMouse = LookToScrollResumeAfterChoosingPointForMouse;
            Settings.Default.LookToScrollDeactivateUponSwitchingKeyboards = LookToScrollDeactivateUponSwitchingKeyboards;
            Settings.Default.LookToScrollOverlayBoundsColour = LookToScrollOverlayBoundsColour;
            Settings.Default.LookToScrollOverlayDeadzoneColour = LookToScrollOverlayDeadzoneColour;
            Settings.Default.LookToScrollOverlayBoundsThickness = LookToScrollOverlayBoundsThickness;
            Settings.Default.LookToScrollOverlayDeadzoneThickness = LookToScrollOverlayDeadzoneThickness;
            Settings.Default.LookToScrollHorizontalDeadzone = LookToScrollHorizontalDeadzone;
            Settings.Default.LookToScrollVerticalDeadzone = LookToScrollVerticalDeadzone;
            Settings.Default.LookToScrollSpeed = LookToScrollSpeed;

            Settings.Default.EnableCommuniKateKeyboardLayout = EnableCommuniKateKeyboardLayout;
            Settings.Default.CommuniKatePagesetLocation = CommuniKatePagesetLocation;
            Settings.Default.CommuniKateStagedForDeletion = CommuniKateStagedForDeletion;
            Settings.Default.CommuniKateSoundVolume = CommuniKateSoundVolume;
            Settings.Default.CommuniKateSpeakSelected = CommuniKateSpeakSelected;
            Settings.Default.CommuniKateSpeakSelectedVolume = CommuniKateSpeakSelectedVolume;
            Settings.Default.CommuniKateSpeakSelectedRate = CommuniKateSpeakSelectedRate;

            Settings.Default.PluginsLocation = PluginsLocation;
            Settings.Default.EnablePlugins = EnablePlugins;
            Settings.Default.AllowMultipleInstances = AllowMultipleInstances;
        }

        #endregion
    }
}
