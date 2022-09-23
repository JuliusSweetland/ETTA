// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved

using System.Linq;
using JuliusSweetland.OptiKey.Properties;
using JuliusSweetland.OptiKey.Services;
using JuliusSweetland.OptiKey.UI.ViewModels.Management;
using log4net;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System.Windows;
using JuliusSweetland.OptiKey.Enums;

namespace JuliusSweetland.OptiKey.UI.ViewModels
{
    public class ManagementViewModel : BindableBase
    {
        #region Private Member Vars

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion
        
        #region Ctor

        public ManagementViewModel(
            IAudioService audioService,
            IDictionaryService dictionaryService,
            IWindowManipulationService windowManipulationService)
        {
            //Instantiate child VMs
            DictionaryViewModel = new DictionaryViewModel(dictionaryService);
            GesturesViewModel = new GesturesViewModel();
            LayoutViewModel = new LayoutViewModel();
            PointingAndSelectingViewModel = new PointingAndSelectingViewModel();
            SoundsViewModel = new SoundsViewModel(audioService);
            VisualsViewModel = new VisualsViewModel(windowManipulationService);
            FeaturesViewModel = new FeaturesViewModel();
            WordsViewModel = new WordsViewModel(dictionaryService);
            
            //Instantiate interaction requests and commands
            ConfirmationRequest = new InteractionRequest<Confirmation>();
            OkCommand = new DelegateCommand<Window>(Ok); //Can always click Ok
            CancelCommand = new DelegateCommand<Window>(Cancel); //Can always click Cancel
        }
        
        #endregion
        
        #region Properties
        
        public bool ChangesRequireRestart
        {
            get
            {
                return DictionaryViewModel.ChangesRequireRestart
                    || PointingAndSelectingViewModel.ChangesRequireRestart
                    || SoundsViewModel.ChangesRequireRestart
                    || VisualsViewModel.ChangesRequireRestart
                    || FeaturesViewModel.ChangesRequireRestart
                    || WordsViewModel.ChangesRequireRestart;
            }
        }

        public DictionaryViewModel DictionaryViewModel { get; private set; }
        public GesturesViewModel GesturesViewModel { get; private set; }
        public LayoutViewModel LayoutViewModel { get; private set; }
        public PointingAndSelectingViewModel PointingAndSelectingViewModel { get; private set; }
        public SoundsViewModel SoundsViewModel { get; private set; }
        public VisualsViewModel VisualsViewModel { get; private set; }
        public FeaturesViewModel FeaturesViewModel { get; private set; }
        public WordsViewModel WordsViewModel { get; private set; }
        
        public InteractionRequest<Confirmation> ConfirmationRequest { get; private set; }
        public DelegateCommand<Window> OkCommand { get; private set; }
        public DelegateCommand<Window> CancelCommand { get; private set; }
        
        #endregion
        
        #region Methods

        private void CoerceValues()
        {
            CoercePersianSettings();
            CoerceUrduSettings();
        }

        private void CoercePersianSettings()
        {
            if (WordsViewModel.KeyboardAndDictionaryLanguage == Languages.PersianIran
                && WordsViewModel.UiLanguage != Languages.PersianIran)
            {
                ConfirmationRequest.Raise(
                    new Confirmation
                    {
                        Title = Resources.UILANGUAGE_AND_KEYBOARDANDDICTIONARYLANGUAGE_DIFFER_TITLE,
                        Content = Resources.DEFAULT_UILANGUAGE_TO_PERSIAN
                    }, confirmation =>
                    {
                        if (confirmation.Confirmed)
                        {
                            Log.Info("Prompting user to change the UiLanguage to Persian as the KeyboardAndDictionaryLanguage is Persian. The UiLanguage controls whether the scratchpad has text flow RightToLeft, which Persian requires.");
                            WordsViewModel.UiLanguage = Languages.PersianIran;
                        }
                    });
            }

            if ((WordsViewModel.KeyboardAndDictionaryLanguage == Languages.PersianIran
                 || WordsViewModel.UiLanguage == Languages.PersianIran)
                && !new[]
                {
                    VisualsViewModel.ElhamUrl,
                    VisualsViewModel.HomaUrl,
                    VisualsViewModel.KoodakUrl,
                    VisualsViewModel.NazliUrl,
                    VisualsViewModel.RoyaUrl,
                    VisualsViewModel.TerafikUrl,
                    VisualsViewModel.TitrUrl
                }.Contains(VisualsViewModel.FontFamily))
            {
                ConfirmationRequest.Raise(
                    new Confirmation
                    {
                        Title = Resources.LANGUAGE_SPECIFIC_FONT_RECOMMENDED,
                        Content = Resources.FONTFAMILY_IS_NOT_COMPATIBLE_WITH_PERSIAN_LANGUAGE
                    }, confirmation =>
                    {
                        if (confirmation.Confirmed)
                        {
                            Log.Info("Prompting user to change the font to an Persian compatible font. If another font is used then text may be displayed incorrectly.");
                            VisualsViewModel.FontFamily = VisualsViewModel.NazliUrl;
                            VisualsViewModel.FontStretch = Enums.FontStretches.Normal;
                            VisualsViewModel.FontWeight = Enums.FontWeights.Regular;
                        }
                    });
            }
        }

        private void CoerceUrduSettings()
        {
            if (WordsViewModel.KeyboardAndDictionaryLanguage == Languages.UrduPakistan
                && WordsViewModel.UiLanguage != Languages.UrduPakistan)
            {
                ConfirmationRequest.Raise(
                    new Confirmation
                    {
                        Title = Resources.UILANGUAGE_AND_KEYBOARDANDDICTIONARYLANGUAGE_DIFFER_TITLE,
                        Content = Resources.DEFAULT_UILANGUAGE_TO_URDU
                    }, confirmation =>
                    {
                        if (confirmation.Confirmed)
                        {
                            Log.Info("Prompting user to change the UiLanguage to Urdu as the KeyboardAndDictionaryLanguage is Urdu. The UiLanguage controls whether the scratchpad has text flow RightToLeft, which Urdu requires.");
                            WordsViewModel.UiLanguage = Languages.UrduPakistan;
                        }
                    });
            }

            if ((WordsViewModel.KeyboardAndDictionaryLanguage == Languages.UrduPakistan
                 || WordsViewModel.UiLanguage == Languages.UrduPakistan)
                && !new[]
                {
                    VisualsViewModel.FajerNooriNastaliqueUrl,
                    VisualsViewModel.NafeesWebNaskhUrl,
                    VisualsViewModel.PakNastaleeqUrl
                }.Contains(VisualsViewModel.FontFamily))
            {
                ConfirmationRequest.Raise(
                    new Confirmation
                    {
                        Title = Resources.LANGUAGE_SPECIFIC_FONT_RECOMMENDED,
                        Content = Resources.FONTFAMILY_IS_NOT_COMPATIBLE_WITH_URDU_LANGUAGE
                    }, confirmation =>
                    {
                        if (confirmation.Confirmed)
                        {
                            Log.Info("Prompting user to change the font to an Urdu compatible font. If another font is used then text (especially numbers which are only displayed correctly in Urdu if an Urdu font is used) may be displayed incorrectly.");
                            VisualsViewModel.FontFamily = VisualsViewModel.NafeesWebNaskhUrl;
                            VisualsViewModel.FontStretch = Enums.FontStretches.Normal;
                            VisualsViewModel.FontWeight = Enums.FontWeights.Regular;
                        }
                    });
            }
        }

        private void ApplyChanges()
        {
            DictionaryViewModel.ApplyChanges();
            GesturesViewModel.ApplyChanges();
            LayoutViewModel.ApplyChanges();
            PointingAndSelectingViewModel.ApplyChanges();
            SoundsViewModel.ApplyChanges();
            VisualsViewModel.ApplyChanges();
            FeaturesViewModel.ApplyChanges();
            WordsViewModel.ApplyChanges();
        }

        private void Ok(Window window)
        {
            CoerceValues();

            if (ChangesRequireRestart)
            {
                //Warn if restart required and prompt for Confirmation before restarting
                ConfirmationRequest.Raise(
                    new Confirmation
                    {
                        Title = Resources.VERIFY_RESTART,
                        Content = Resources.RESTART_MESSAGE
                    }, confirmation =>
                    {
                        if (confirmation.Confirmed)
                        {
                            Log.Info("Applying management changes and attempting to restart OptiKey");
                            ApplyChanges();
                            Settings.Default.Save();
                            try
                            {
                                Settings.Default.CleanShutdown = true;
                                Settings.Default.Save();
                                OptiKeyApp.RestartApp();
                            }
                            catch { } //Swallow any exceptions (e.g. DispatcherExceptions) - we're shutting down so it doesn't matter.
                            Application.Current.Shutdown();
                        }
                    });
            }
            else
            {
                Log.Info("Applying management changes");
                ApplyChanges();
                window.Close();
            }
        }

        private static void Cancel(Window window)
        {
            window.Close();
        }

        #endregion
    }
}
