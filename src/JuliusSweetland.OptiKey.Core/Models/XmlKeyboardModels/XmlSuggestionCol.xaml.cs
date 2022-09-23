// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JuliusSweetland.OptiKey.Models
{
    /// <summary>
    /// Interaction logic for XmlSuggestions.xaml
    /// </summary>
    public partial class SuggestionCol : UserControl
    {
        public SuggestionCol()
        {
            InitializeComponent();
            Loaded += (sender, args) => NumberOfSuggestionsDisplayed = 4;
        }

        public static readonly DependencyProperty NumberOfSuggestionsDisplayedProperty =
            DependencyProperty.Register("NumberOfSuggestionsDisplayed", typeof(int), typeof(SuggestionCol), new PropertyMetadata(default(int)));

        public int NumberOfSuggestionsDisplayed
        {
            get { return (int)GetValue(NumberOfSuggestionsDisplayedProperty); }
            set { SetValue(NumberOfSuggestionsDisplayedProperty, value); }
        }

        public static readonly DependencyProperty BackgroundColourOverrideProperty =
            DependencyProperty.Register("BackgroundColourOverride", typeof(Brush), typeof(SuggestionCol), new PropertyMetadata(default(Brush)));

        public Brush BackgroundColourOverride
        {
            get { return (Brush)GetValue(BackgroundColourOverrideProperty); }
            set { SetValue(BackgroundColourOverrideProperty, value); }
        }

        public static readonly DependencyProperty DisabledBackgroundColourOverrideProperty =
            DependencyProperty.Register("DisabledBackgroundColourOverride", typeof(Brush), typeof(SuggestionCol), new PropertyMetadata(default(Brush)));

        public Brush DisabledBackgroundColourOverride
        {
            get { return (Brush)GetValue(DisabledBackgroundColourOverrideProperty); }
            set { SetValue(DisabledBackgroundColourOverrideProperty, value); }
        }

        public static readonly DependencyProperty OpacityOverrideProperty =
            DependencyProperty.Register("OpacityOverride", typeof(double), typeof(SuggestionCol), new PropertyMetadata(defaultValue: 1.0));

        public double OpacityOverride
        {
            get { return (double)GetValue(OpacityOverrideProperty); }
            set { SetValue(OpacityOverrideProperty, value); }
        }
    }
}