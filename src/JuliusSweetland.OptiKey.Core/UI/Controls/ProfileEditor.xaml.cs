// Copyright(c) 2020 OPTIKEY LTD (UK company number11854839) - All Rights Reserved
using System.Windows;
using System.Windows.Controls;
using JuliusSweetland.OptiKey.Models;

namespace JuliusSweetland.OptiKey.UI.Controls
{
    /// <summary>
    /// Interaction logic for ProfileEditor.xaml
    /// </summary>
    public partial class ProfileEditor : UserControl
    {
        public ProfileEditor()
        {
            InitializeComponent();
        }

        #region Properties

        public static readonly DependencyProperty ProfileProperty = DependencyProperty
            .Register("Profile", typeof(InteractorProfile), typeof(ProfileEditor), new PropertyMetadata(default(InteractorProfile)));

        public InteractorProfile Profile
        {
            get { return (InteractorProfile)GetValue(ProfileProperty); }
            set { SetValue(ProfileProperty, value); }
        }

        public static readonly DependencyProperty ViewProperty = DependencyProperty
            .Register("View", typeof(Canvas), typeof(ProfileEditor), new PropertyMetadata(default(Canvas)));

        public Canvas View
        {
            get { return (Canvas)GetValue(ViewProperty); }
            set { SetValue(ViewProperty, value); }
        }

        #endregion

        #region Methods


        private void UpdateView()
        {
            View = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }


        private void NumericUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            UpdateView();
        }

        #endregion

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateView();
        }
    }
}

