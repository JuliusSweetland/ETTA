// Copyright(c) 2020 OPTIKEY LTD (UK company number11854839) - All Rights Reserved
using JuliusSweetland.OptiKey.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

namespace JuliusSweetland.OptiKey.UI.Controls
{
    /// <summary>
    /// Interaction logic for InteractorEditor.xaml
    /// </summary>
    public partial class InteractorEditor : UserControl
    {
        public InteractorEditor()
        {
            InitializeComponent();
        }

        #region Properties

        public static readonly DependencyProperty SelectedInteractorTypeProperty = DependencyProperty
            .Register("SelectedInteractorType", typeof(string), typeof(InteractorEditor), new PropertyMetadata(default(string)));

        public string SelectedInteractorType
        {
            get { return (string)GetValue(SelectedInteractorTypeProperty); }
            set { SetValue(SelectedInteractorTypeProperty, value); }
        }
        public static List<string> InteractorTypeList = Enum.GetNames(typeof(InteractorTypes)).ToList(); 

        public static List<string> SymbolList = new List<string>() { "" }.Concat(new ResourceDictionary() { Source = new Uri("/OptiKey;component/Resources/Icons/KeySymbols.xaml", UriKind.RelativeOrAbsolute) }.Keys.Cast<string>()).OrderBy(x => x).ToList();

        public static List<string> CompatibilityList = new List<string>() { "", "Any Font", "Persian", "Unicode", "Urdu" };

        public static List<string> AutoScaleWidth = new List<string>() { "", "True", "False" };
        
        public static List<string> AutoScaleHeight = new List<string>() { "", "True", "False" };

        #endregion

        #region Methods

        private void SelectType(object sender, SelectionChangedEventArgs e)
        {
            SelectedInteractorType = (sender as ComboBox).SelectedItem as string;
        }

        #endregion

    }
}

