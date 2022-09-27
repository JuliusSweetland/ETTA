using JuliusSweetland.OptiKey.Models;
using JuliusSweetland.OptiKey.Properties;
using JuliusSweetland.OptiKey.UI.Controls;
using JuliusSweetland.OptiKey.UI.Utilities;
using JuliusSweetland.OptiKey.UI.ViewModels.Keyboards;
using log4net;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JuliusSweetland.OptiKey.UI.ViewModels.Management
{
    public class LayoutViewModel : INotifyPropertyChanged
    {
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public LayoutViewModel() 
        {
            OpenFileCommand = new DelegateCommand(OpenFile);
            SaveFileCommand = new DelegateCommand(SaveFile);
            AddLayoutCommand = new DelegateCommand(AddLayout);
            AddBuiltInCommand = new DelegateCommand(AddBuiltIn);
            Load();
            CreateViewbox();
        }

        #region Properties

        public static List<string> KeyboardList = new List<string>()
        {
            "Alpha1", "Alpha2", "Alpha3", "ConversationAlpha1", "ConversationAlpha2", "ConversationAlpha3",
            "ConversationNumericAndSymbols", "Currencies1", "Currencies2",
            "Diacritics1", "Diacritics2", "Diacritics3", "Language", "Menu", "Mouse",
            "NumericAndSymbols1", "NumericAndSymbols2", "NumericAndSymbols3", "PhysicalKeys", 
            "SimplifiedAlpha", "SimplifiedConversationAlpha", "SizeAndPosition", "WebBrowsing"
        };

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public DelegateCommand OpenFileCommand { get; private set; }
        public DelegateCommand SaveFileCommand { get; private set; }
        public DelegateCommand AddLayoutCommand { get; private set; }
        public DelegateCommand AddBuiltInCommand { get; private set; }

        public string KeyboardFile { get; set; }

        private Layout layout;
        public Layout Layout
        {
            get { return layout; }
            set { layout = value; OnPropertyChanged(); }
        }

        private string keyboard;
        public string Keyboard { get { return keyboard; } set { keyboard = value; OnPropertyChanged(); } }

        private Viewbox viewbox;
        public Viewbox Viewbox
        {
            get { return viewbox; }
            set { viewbox = value; OnPropertyChanged(); }
        }

        #endregion

        #region Methods

        public void ApplyChanges()
        {
            Settings.Default.KeyboardFile = KeyboardFile;
        }

        private void Load()
        {
            KeyboardFile = Settings.Default.KeyboardFile;
            Layout = new Layout();
            InitializeLayout();
        }

        private void InitializeLayout()
        {
            Layout.Load();
            CreateViewbox();
        }

        private void OpenFile()
        {
            var fileDialog = new System.Windows.Forms.OpenFileDialog() { FileName = Path.GetFileName(KeyboardFile) };
            var result = fileDialog.ShowDialog();
            string tempFilename;
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    tempFilename = fileDialog.FileName; // we will commit it if loading is okay
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    return;
            }
            try
            {
                Layout = Layout.ReadFromFile(tempFilename);
            }
            catch (Exception e)
            {
                Log.Error($"Error reading from Layout file: {tempFilename} :");
                Log.Info(e.ToString());
                return;
            }
            KeyboardFile = tempFilename;
            Layout.KeyboardFile = KeyboardFile;
            InitializeLayout();
        }

        private void SaveFile()
        {
            var fp = Settings.Default.DynamicKeyboardsLocation;
            var fn = Layout.Name + ".xml";
            try
            {
                fp = Path.GetDirectoryName(KeyboardFile);
                fn = Path.GetFileName(KeyboardFile);
            } catch { }
            var saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.InitialDirectory = fp;
            saveFileDialog.FileName = fn;
            saveFileDialog.Title = "Save File";
            saveFileDialog.CheckFileExists = false;
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.DefaultExt = "xml";
            saveFileDialog.Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;
            var result = saveFileDialog.ShowDialog();
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    KeyboardFile = saveFileDialog.FileName;
                    Layout.WriteToFile(KeyboardFile);
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    break;
            }
        }

        private void AddBuiltIn()
        {
            Layout = new Layout();
            InitializeLayout();

            Layout.Name = keyboard;
            DependencyObject content = (DependencyObject)new Alpha1().GetContent();

            switch (Keyboard)
            {
                case "Alpha2":
                    content = (DependencyObject)new Alpha2().GetContent();
                    break;
                case "Alpha3":
                    content = (DependencyObject)new Alpha3().GetContent();
                    break;
                case "ConversationAlpha1":
                    content = (DependencyObject)new ConversationAlpha1(null).GetContent();
                    break;
                case "ConversationAlpha2":
                    content = (DependencyObject)new ConversationAlpha1(null).GetContent();
                    break;
                case "ConversationAlpha3":
                    content = (DependencyObject)new ConversationAlpha3(null).GetContent();
                    break;
                case "ConversationNumericAndSymbols":
                    content = (DependencyObject)new ConversationNumericAndSymbols(null).GetContent();
                    break;
                case "Currencies1":
                    content = (DependencyObject)new Currencies1().GetContent();
                    break;
                case "Currencies2":
                    content = (DependencyObject)new Currencies2().GetContent();
                    break;
                case "Diacritics1":
                    content = (DependencyObject)new Diacritics1().GetContent();
                    break;
                case "Diacritics2":
                    content = (DependencyObject)new Diacritics2().GetContent();
                    break;
                case "Diacritics3":
                    content = (DependencyObject)new Diacritics3().GetContent();
                    break;
                case "Language":
                    content = (DependencyObject)new Language(null).GetContent();
                    break;
                case "Menu":
                    content = (DependencyObject)new Keyboards.Menu(null).GetContent();
                    break;
                case "Mouse":
                    content = (DependencyObject)new Mouse(null).GetContent();
                    break;
                case "NumericAndSymbols1":
                    content = (DependencyObject)new NumericAndSymbols1().GetContent();
                    break;
                case "NumericAndSymbols2":
                    content = (DependencyObject)new NumericAndSymbols2().GetContent();
                    break;
                case "NumericAndSymbols3":
                    content = (DependencyObject)new NumericAndSymbols3().GetContent();
                    break;
                case "PhysicalKeys":
                    content = (DependencyObject)new PhysicalKeys().GetContent();
                    break;
                case "SimplifiedAlpha":
                    content = (DependencyObject)new SimplifiedAlpha(null).GetContent();
                    break;
                case "SimplifiedConversationAlpha":
                    content = (DependencyObject)new SimplifiedConversationAlpha(null).GetContent();
                    break;
                case "SizeAndPosition":
                    content = (DependencyObject)new SizeAndPosition(null).GetContent();
                    break;
                case "WebBrowsing":
                    content = (DependencyObject)new WebBrowsing().GetContent();
                    break;
            }
            var symbols = new ResourceDictionary() { Source = new Uri("/OptiKey;component/Resources/Icons/KeySymbols.xaml", UriKind.RelativeOrAbsolute) };
            
            var symbolValues = symbols.Values.OfType<Geometry>().Select(x => x.ToString()).ToList();
            var symbolKeys = symbols.Keys.OfType<string>().ToList();
            var allKeys = VisualAndLogicalTreeHelper.FindLogicalChildren<Key>(content).ToList();
            var maxRow = 0d;
            var maxCol = 0d;
            foreach (Key key in allKeys.Where(x => x is Key && VisualAndLogicalTreeHelper.FindLogicalParent<Output>(x) == null))
            {
                var i = new DynamicKey() { Label = key.ShiftUpText, ShiftDownLabel = key.ShiftDownText, Col = Grid.GetColumn(key), Row = Grid.GetRow(key), Width = Grid.GetColumnSpan(key), Height = Grid.GetRowSpan(key), };
                if (key.Value != null)
                {
                    var kv = key.Value;
                    if (kv.FunctionKey.HasValue)
                        i.Commands.Add(new ActionCommand() { Value = kv.FunctionKey.Value.ToString() });
                    else
                        i.Commands.Add(new TextCommand() { Value = kv.String });
                }
                if (key.SymbolGeometry != null)
                    i.Symbol = symbolKeys[symbolValues.IndexOf(key.SymbolGeometry.ToString())];
                i.SharedSizeGroup = key.SharedSizeGroup;
                foreach (var p in Layout.Profiles)
                {
                    i.Profiles.Add(new InteractorProfileMap(p, p.Name == "All"));
                }
                Layout.Interactors.Add(i);
                maxCol = Math.Max(maxCol, i.Col + i.Width);
                maxRow = Math.Max(maxRow, i.Row + i.Height);
            }
            var output = VisualAndLogicalTreeHelper.FindLogicalChildren<Output>(content).ToList();
            if (output != null && output.Any())
            {
                var o = new DynamicOutputPanel();
                o.Col = 0;
                o.Row = 0;
                o.Width = (int)maxCol;
                o.Height = 2;
                layout.Interactors.Insert(0, o);
            }
            Layout.Rows = (int)maxRow;
            Layout.Columns = (int)maxCol;
        }

        private void AddLayout()
        {
            Layout = new Layout();
            InitializeLayout();
        }

        private void CreateViewbox()
        {
            Viewbox = null;
            if (Layout == null)
                return;

            Viewbox = Layout.View;
            Viewbox.Width = .7 * SystemParameters.VirtualScreenWidth;
            Viewbox.Height = .7 * SystemParameters.VirtualScreenHeight;
            Viewbox.Stretch = Stretch.Fill;
            Viewbox.StretchDirection = StretchDirection.Both;
        }

        #endregion

    }
}