using System.Collections.Generic;
using JuliusSweetland.OptiKey.Properties;
using JuliusSweetland.OptiKey.Services;
using log4net;
using Prism.Commands;
using Prism.Mvvm;

namespace JuliusSweetland.OptiKey.UI.ViewModels.Management
{
    public class SoundsViewModel : BindableBase
    {
        #region Private Member Vars

        private readonly static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IAudioService audioService;

        #endregion
        
        #region Ctor

        public SoundsViewModel(IAudioService audioService)
        {
            this.audioService = audioService;

            InfoSoundPlayCommand = new DelegateCommand(() => audioService.PlaySound(InfoSoundFile, InfoSoundVolume));
            KeySelectionSoundPlayCommand = new DelegateCommand(() => audioService.PlaySound(KeySelectionSoundFile, KeySelectionSoundVolume));
            ErrorSoundPlayCommand = new DelegateCommand(() => audioService.PlaySound(ErrorSoundFile, ErrorSoundVolume));
            MultiKeySelectionCaptureStartSoundPlayCommand = new DelegateCommand(() => audioService.PlaySound(MultiKeySelectionCaptureStartSoundFile, MultiKeySelectionCaptureStartSoundVolume));
            MultiKeySelectionCaptureEndSoundPlayCommand = new DelegateCommand(() => audioService.PlaySound(MultiKeySelectionCaptureEndSoundFile, MultiKeySelectionCaptureEndSoundVolume));
            MouseClickSoundPlayCommand = new DelegateCommand(() => audioService.PlaySound(MouseClickSoundFile, MouseClickSoundVolume));
            MouseDownSoundPlayCommand = new DelegateCommand(() => audioService.PlaySound(MouseDownSoundFile, MouseDownSoundVolume));
            MouseUpSoundPlayCommand = new DelegateCommand(() => audioService.PlaySound(MouseUpSoundFile, MouseUpSoundVolume));
            MouseDoubleClickSoundPlayCommand = new DelegateCommand(() => audioService.PlaySound(MouseDoubleClickSoundFile, MouseDoubleClickSoundVolume));
            MouseScrollSoundPlayCommand = new DelegateCommand(() => audioService.PlaySound(MouseScrollSoundFile, MouseScrollSoundVolume));

            Load();
        }
        
        #endregion
        
        #region Properties

        public List<string> SpeechVoices
        {
            get { return audioService.GetAvailableVoices(); }
        }
        
        public List<KeyValuePair<string, string>> GeneralSoundFiles
        {
            get
            {
                return new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(Resources.NoSound, null),
                    new KeyValuePair<string, string>(Resources.Click1Sound, @"Resources\Sounds\Click1.wav"),
                    new KeyValuePair<string, string>(Resources.Click2Sound, @"Resources\Sounds\Click2.wav"),
                    new KeyValuePair<string, string>(Resources.Click3Sound, @"Resources\Sounds\Click3.wav"),
                    new KeyValuePair<string, string>(Resources.Click4Sound, @"Resources\Sounds\Click4.wav"),
                    new KeyValuePair<string, string>(Resources.Click5Sound, @"Resources\Sounds\Click5.wav"),
                    new KeyValuePair<string, string>(Resources.Click6Sound, @"Resources\Sounds\Click6.wav"),
                    new KeyValuePair<string, string>(Resources.Tone1Sound, @"Resources\Sounds\Tone1.wav"),
                    new KeyValuePair<string, string>(Resources.Tone2Sound, @"Resources\Sounds\Tone2.wav"),
                    new KeyValuePair<string, string>(Resources.Tone3Sound, @"Resources\Sounds\Tone3.wav"),
                    new KeyValuePair<string, string>(Resources.Tone4Sound, @"Resources\Sounds\Tone4.wav"),
                    new KeyValuePair<string, string>(Resources.Rising1Sound, @"Resources\Sounds\Rising1.wav"),
                    new KeyValuePair<string, string>(Resources.Rising2Sound, @"Resources\Sounds\Rising2.wav"),
                    new KeyValuePair<string, string>(Resources.Falling1Sound, @"Resources\Sounds\Falling1.wav"),
                    new KeyValuePair<string, string>(Resources.Falling2Sound, @"Resources\Sounds\Falling2.wav")
                };
            }
        }

        public List<KeyValuePair<string, string>> MouseClickSoundFiles
        {
            get
            {
                return new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(Resources.NoSound, null),
                    new KeyValuePair<string, string>(Resources.MouseClick1Sound, @"Resources\Sounds\MouseClick1.wav"),
                    new KeyValuePair<string, string>(Resources.MouseClick2Sound, @"Resources\Sounds\MouseClick2.wav"),
                    new KeyValuePair<string, string>(Resources.MouseClick3Sound, @"Resources\Sounds\MouseClick3.wav"),
                    new KeyValuePair<string, string>(Resources.MouseClick4Sound, @"Resources\Sounds\MouseClick4.wav")
                };
            }
        }

        public List<KeyValuePair<string, string>> MouseDownSoundFiles
        {
            get
            {
                return new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(Resources.NoSound, null),
                    new KeyValuePair<string, string>(Resources.MouseDown1Sound, @"Resources\Sounds\MouseDown1.wav"),
                    new KeyValuePair<string, string>(Resources.MouseDown2Sound, @"Resources\Sounds\MouseDown2.wav"),
                    new KeyValuePair<string, string>(Resources.MouseDown3Sound, @"Resources\Sounds\MouseDown3.wav"),
                    new KeyValuePair<string, string>(Resources.MouseDown4Sound, @"Resources\Sounds\MouseDown4.wav")
                };
            }
        }

        public List<KeyValuePair<string, string>> MouseUpSoundFiles
        {
            get
            {
                return new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(Resources.NoSound, null),
                    new KeyValuePair<string, string>(Resources.MouseUp1Sound, @"Resources\Sounds\MouseUp1.wav"),
                    new KeyValuePair<string, string>(Resources.MouseUp2Sound, @"Resources\Sounds\MouseUp2.wav"),
                    new KeyValuePair<string, string>(Resources.MouseUp3Sound, @"Resources\Sounds\MouseUp3.wav"),
                    new KeyValuePair<string, string>(Resources.MouseUp4Sound, @"Resources\Sounds\MouseUp4.wav")
                };
            }
        }

        public List<KeyValuePair<string, string>> MouseDoubleClickSoundFiles
        {
            get
            {
                return new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(Resources.NoSound, null),
                    new KeyValuePair<string, string>(Resources.MouseDoubleClick1Sound, @"Resources\Sounds\MouseDoubleClick1.wav"),
                    new KeyValuePair<string, string>(Resources.MouseDoubleClick2Sound, @"Resources\Sounds\MouseDoubleClick2.wav"),
                    new KeyValuePair<string, string>(Resources.MouseDoubleClick3Sound, @"Resources\Sounds\MouseDoubleClick3.wav")
                };
            }
        }

        public List<KeyValuePair<string, string>> MouseScrollSoundFiles
        {
            get
            {
                return new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(Resources.NoSound, null),
                    new KeyValuePair<string, string>(Resources.MouseScroll1Sound, @"Resources\Sounds\MouseScroll1.wav"),
                    new KeyValuePair<string, string>(Resources.MouseScroll2Sound, @"Resources\Sounds\MouseScroll2.wav"),
                    new KeyValuePair<string, string>(Resources.MouseScroll3Sound, @"Resources\Sounds\MouseScroll3.wav"),
                    new KeyValuePair<string, string>(Resources.MouseScroll4Sound, @"Resources\Sounds\MouseScroll4.wav"),
                    new KeyValuePair<string, string>(Resources.MouseScroll5Sound, @"Resources\Sounds\MouseScroll5.wav"),
                    new KeyValuePair<string, string>(Resources.MouseScroll6Sound, @"Resources\Sounds\MouseScroll6.wav")
                };
            }
        }
        
        private string speechVoice;
        public string SpeechVoice
        {
            get { return speechVoice; }
            set { SetProperty(ref speechVoice, value); }
        }
        
        private int speechVolume;
        public int SpeechVolume
        {
            get { return speechVolume; }
            set { SetProperty(ref speechVolume, value); }
        }

        private int speechRate;
        public int SpeechRate
        {
            get { return speechRate; }
            set { SetProperty(ref speechRate, value); }
        }
        
        private string infoSoundFile;
        public string InfoSoundFile
        {
            get { return infoSoundFile; }
            set { SetProperty(ref infoSoundFile, value); }
        }

        private int infoSoundVolume;
        public int InfoSoundVolume
        {
            get { return infoSoundVolume; }
            set { SetProperty(ref infoSoundVolume, value); }
        }

        private string keySelectionSoundFile;
        public string KeySelectionSoundFile
        {
            get { return keySelectionSoundFile; }
            set { SetProperty(ref keySelectionSoundFile, value); }
        }

        private int keySelectionSoundVolume;
        public int KeySelectionSoundVolume
        {
            get { return keySelectionSoundVolume; }
            set { SetProperty(ref keySelectionSoundVolume, value); }
        }

        private string errorSoundFile;
        public string ErrorSoundFile
        {
            get { return errorSoundFile; }
            set { SetProperty(ref errorSoundFile, value); }
        }

        private int errorSoundVolume;
        public int ErrorSoundVolume
        {
            get { return errorSoundVolume; }
            set { SetProperty(ref errorSoundVolume, value); }
        }

        private string multiKeySelectionCaptureStartSoundFile;
        public string MultiKeySelectionCaptureStartSoundFile
        {
            get { return multiKeySelectionCaptureStartSoundFile; }
            set { SetProperty(ref multiKeySelectionCaptureStartSoundFile, value); }
        }

        private int multiKeySelectionCaptureStartSoundVolume;
        public int MultiKeySelectionCaptureStartSoundVolume
        {
            get { return multiKeySelectionCaptureStartSoundVolume; }
            set { SetProperty(ref multiKeySelectionCaptureStartSoundVolume, value); }
        }

        private string multiKeySelectionCaptureEndSoundFile;
        public string MultiKeySelectionCaptureEndSoundFile
        {
            get { return multiKeySelectionCaptureEndSoundFile; }
            set { SetProperty(ref multiKeySelectionCaptureEndSoundFile, value); }
        }

        private int multiKeySelectionCaptureEndSoundVolume;
        public int MultiKeySelectionCaptureEndSoundVolume
        {
            get { return multiKeySelectionCaptureEndSoundVolume; }
            set { SetProperty(ref multiKeySelectionCaptureEndSoundVolume, value); }
        }

        private string mouseClickSoundFile;
        public string MouseClickSoundFile
        {
            get { return mouseClickSoundFile; }
            set { SetProperty(ref mouseClickSoundFile, value); }
        }

        private int mouseClickSoundVolume;
        public int MouseClickSoundVolume
        {
            get { return mouseClickSoundVolume; }
            set { SetProperty(ref mouseClickSoundVolume, value); }
        }

        private string mouseDownSoundFile;
        public string MouseDownSoundFile
        {
            get { return mouseDownSoundFile; }
            set { SetProperty(ref mouseDownSoundFile, value); }
        }

        private int mouseDownSoundVolume;
        public int MouseDownSoundVolume
        {
            get { return mouseDownSoundVolume; }
            set { SetProperty(ref mouseDownSoundVolume, value); }
        }

        private string mouseUpSoundFile;
        public string MouseUpSoundFile
        {
            get { return mouseUpSoundFile; }
            set { SetProperty(ref mouseUpSoundFile, value); }
        }

        private int mouseUpSoundVolume;
        public int MouseUpSoundVolume
        {
            get { return mouseUpSoundVolume; }
            set { SetProperty(ref mouseUpSoundVolume, value); }
        }

        private string mouseDoubleClickSoundFile;
        public string MouseDoubleClickSoundFile
        {
            get { return mouseDoubleClickSoundFile; }
            set { SetProperty(ref mouseDoubleClickSoundFile, value); }
        }

        private int mouseDoubleClickSoundVolume;
        public int MouseDoubleClickSoundVolume
        {
            get { return mouseDoubleClickSoundVolume; }
            set { SetProperty(ref mouseDoubleClickSoundVolume, value); }
        }

        private string mouseScrollSoundFile;
        public string MouseScrollSoundFile
        {
            get { return mouseScrollSoundFile; }
            set { SetProperty(ref mouseScrollSoundFile, value); }
        }

        private int mouseScrollSoundVolume;
        public int MouseScrollSoundVolume
        {
            get { return mouseScrollSoundVolume; }
            set { SetProperty(ref mouseScrollSoundVolume, value); }
        }

        public bool ChangesRequireRestart
        {
            get { return false; }
        }

        public DelegateCommand InfoSoundPlayCommand { get; private set; }
        public DelegateCommand KeySelectionSoundPlayCommand { get; private set; }
        public DelegateCommand ErrorSoundPlayCommand { get; private set; }
        public DelegateCommand MultiKeySelectionCaptureStartSoundPlayCommand { get; private set; }
        public DelegateCommand MultiKeySelectionCaptureEndSoundPlayCommand { get; private set; }
        public DelegateCommand MouseClickSoundPlayCommand { get; private set; }
        public DelegateCommand MouseDownSoundPlayCommand { get; private set; }
        public DelegateCommand MouseUpSoundPlayCommand { get; private set; }
        public DelegateCommand MouseDoubleClickSoundPlayCommand { get; private set; }
        public DelegateCommand MouseScrollSoundPlayCommand { get; private set; }
        
        #endregion
        
        #region Methods

        private void Load()
        {
            SpeechVoice = Settings.Default.SpeechVoice;
            SpeechVolume = Settings.Default.SpeechVolume;
            SpeechRate = Settings.Default.SpeechRate;
            InfoSoundFile = Settings.Default.InfoSoundFile;
            InfoSoundVolume = Settings.Default.InfoSoundVolume;
            KeySelectionSoundFile = Settings.Default.KeySelectionSoundFile;
            KeySelectionSoundVolume = Settings.Default.KeySelectionSoundVolume;
            ErrorSoundFile = Settings.Default.ErrorSoundFile;
            ErrorSoundVolume = Settings.Default.ErrorSoundVolume;
            MultiKeySelectionCaptureStartSoundFile = Settings.Default.MultiKeySelectionCaptureStartSoundFile;
            MultiKeySelectionCaptureStartSoundVolume = Settings.Default.MultiKeySelectionCaptureStartSoundVolume;
            MultiKeySelectionCaptureEndSoundFile = Settings.Default.MultiKeySelectionCaptureEndSoundFile;
            MultiKeySelectionCaptureEndSoundVolume = Settings.Default.MultiKeySelectionCaptureEndSoundVolume;
            MouseClickSoundFile = Settings.Default.MouseClickSoundFile;
            MouseClickSoundVolume = Settings.Default.MouseClickSoundVolume;
            MouseDownSoundFile = Settings.Default.MouseDownSoundFile;
            MouseDownSoundVolume = Settings.Default.MouseDownSoundVolume;
            MouseUpSoundFile = Settings.Default.MouseUpSoundFile;
            MouseUpSoundVolume = Settings.Default.MouseUpSoundVolume;
            MouseDoubleClickSoundFile = Settings.Default.MouseDoubleClickSoundFile;
            MouseDoubleClickSoundVolume = Settings.Default.MouseDoubleClickSoundVolume;
            MouseScrollSoundFile = Settings.Default.MouseScrollSoundFile;
            MouseScrollSoundVolume = Settings.Default.MouseScrollSoundVolume;
        }

        public void ApplyChanges()
        {
            Settings.Default.SpeechVoice = SpeechVoice;
            Settings.Default.SpeechVolume = SpeechVolume;
            Settings.Default.SpeechRate = SpeechRate;
            Settings.Default.InfoSoundFile = InfoSoundFile;
            Settings.Default.InfoSoundVolume = InfoSoundVolume;
            Settings.Default.KeySelectionSoundFile = KeySelectionSoundFile;
            Settings.Default.KeySelectionSoundVolume = KeySelectionSoundVolume;
            Settings.Default.ErrorSoundFile = ErrorSoundFile;
            Settings.Default.ErrorSoundVolume = ErrorSoundVolume;
            Settings.Default.MultiKeySelectionCaptureStartSoundFile = MultiKeySelectionCaptureStartSoundFile;
            Settings.Default.MultiKeySelectionCaptureStartSoundVolume = MultiKeySelectionCaptureStartSoundVolume;
            Settings.Default.MultiKeySelectionCaptureEndSoundFile = MultiKeySelectionCaptureEndSoundFile;
            Settings.Default.MultiKeySelectionCaptureEndSoundVolume = MultiKeySelectionCaptureEndSoundVolume;
            Settings.Default.MouseClickSoundFile = MouseClickSoundFile;
            Settings.Default.MouseClickSoundVolume = MouseClickSoundVolume;
            Settings.Default.MouseDownSoundFile = MouseDownSoundFile;
            Settings.Default.MouseDownSoundVolume = MouseDownSoundVolume;
            Settings.Default.MouseUpSoundFile = MouseUpSoundFile;
            Settings.Default.MouseUpSoundVolume = MouseUpSoundVolume;
            Settings.Default.MouseDoubleClickSoundFile = MouseDoubleClickSoundFile;
            Settings.Default.MouseDoubleClickSoundVolume = MouseDoubleClickSoundVolume;
            Settings.Default.MouseScrollSoundFile = MouseScrollSoundFile;
            Settings.Default.MouseScrollSoundVolume = MouseScrollSoundVolume;
        }

        #endregion
    }
}
