// Copyright(c) 2020 OPTIKEY LTD (UK company number11854839) - All Rights Reserved
using JuliusSweetland.OptiKey.Enums;
using JuliusSweetland.OptiKey.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace JuliusSweetland.OptiKey.UI.Controls
{
    /// Interaction logic for LayoutEditor.xaml
    /// </summary>
    public partial class LayoutEditor : UserControl
    {
        public LayoutEditor()
        {
            InitializeComponent();
        }

        #region Properties

        public static readonly DependencyProperty LayoutProperty = DependencyProperty
            .Register("Layout", typeof(Layout), typeof(LayoutEditor), new PropertyMetadata(default(Layout)));

        public Layout Layout
        {
            get { return (Layout)GetValue(LayoutProperty); }
            set { SetValue(LayoutProperty, value); }
        }
        
        public static List<string> InteractorTypeList = Enum.GetNames(typeof(InteractorTypes)).ToList();

        public static List<string> PositionList = Enum.GetNames(typeof(MoveToDirections)).ToList();

        #endregion

        #region Methods

        private void AddProfile(object sender, RoutedEventArgs e)
        {
            try { Layout.AddProfile(); } catch { }
        }

        private void DeleteProfile(object sender, RoutedEventArgs e)
        {
            try { Layout.DeleteProfile(); } catch { }
        }

        private void AddInteractor(object sender, RoutedEventArgs e)
        {
            try { Layout.AddInteractor(); } catch { }
        }

        private void DeleteInteractor(object sender, RoutedEventArgs e)
        {
            try { Layout.DeleteInteractor(); } catch { }
        }

        private void CloneInteractor(object sender, RoutedEventArgs e)
        {
            try { Layout.CloneInteractor(); } catch { }
        }

        #endregion

    }
}

