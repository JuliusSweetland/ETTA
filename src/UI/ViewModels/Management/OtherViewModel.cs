using JuliusSweetland.OptiKey.Properties;
using log4net;
using Prism.Mvvm;

namespace JuliusSweetland.OptiKey.UI.ViewModels.Management
{
    public class OtherViewModel : BindableBase
    {
        #region Private Member Vars

        private readonly static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion
        
        #region Ctor

        public OtherViewModel()
        {
            Load();
        }
        
        #endregion
        
        #region Properties

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

        private bool suppressModifierKeysWhenInMouseKeyboard;
        public bool SuppressModifierKeysWhenInMouseKeyboard
        {
            get { return suppressModifierKeysWhenInMouseKeyboard; }
            set { SetProperty(ref suppressModifierKeysWhenInMouseKeyboard, value); }
        }

        private bool debug;
        public bool Debug
        {
            get { return debug; }
            set { SetProperty(ref debug, value); }
        }

        public bool ChangesRequireRestart
        {
            get
            {
                return Settings.Default.Debug != Debug;
            }
        }
        
        #endregion
        
        #region Methods

        private void Load()
        {
            ShowSplashScreen = Settings.Default.ShowSplashScreen;
            CheckForUpdates = Settings.Default.CheckForUpdates;
            SuppressModifierKeysWhenInMouseKeyboard = Settings.Default.SuppressModifierKeysWhenInMouseKeyboard;
            Debug = Settings.Default.Debug;
        }

        public void ApplyChanges()
        {
            Settings.Default.ShowSplashScreen = ShowSplashScreen;
            Settings.Default.CheckForUpdates = CheckForUpdates;
            Settings.Default.SuppressModifierKeysWhenInMouseKeyboard = SuppressModifierKeysWhenInMouseKeyboard;
            Settings.Default.Debug = Debug;
            Settings.Default.Save();
        }

        #endregion
    }
}
