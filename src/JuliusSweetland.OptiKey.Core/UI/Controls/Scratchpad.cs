// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JuliusSweetland.OptiKey.UI.Controls
{
    public class Scratchpad : UserControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(Scratchpad), new PropertyMetadata(default(string)));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty BackgroundColourOverrideProperty =
            DependencyProperty.Register("BackgroundColourOverride", typeof(Brush), typeof(Scratchpad), new PropertyMetadata(default(Brush)));

        public Brush BackgroundColourOverride
        {
            get { return (Brush)GetValue(BackgroundColourOverrideProperty); }
            set { SetValue(BackgroundColourOverrideProperty, value); }
        }

        public static readonly DependencyProperty BorderColourOverrideProperty =
            DependencyProperty.Register("BorderColourOverrideProperty", typeof(Brush), typeof(Scratchpad), new PropertyMetadata(default(Brush)));

        public Brush BorderColourOverride
        {
            get { return (Brush)GetValue(BorderColourOverrideProperty); }
            set { SetValue(BorderColourOverrideProperty, value); }
        }

        public static readonly DependencyProperty BorderThicknessOverrideProperty =
            DependencyProperty.Register("BorderThicknessOverrideProperty", typeof(int), typeof(Scratchpad), new PropertyMetadata(defaultValue: 1));

        public int BorderThicknessOverride
        {
            get { return (int)GetValue(BorderThicknessOverrideProperty); }
            set { SetValue(BorderThicknessOverrideProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusOverrideProperty =
            DependencyProperty.Register("CornerRadiusOverrideProperty", typeof(int), typeof(Scratchpad), new PropertyMetadata(defaultValue: 0));

        public int CornerRadiusOverride
        {
            get { return (int)GetValue(CornerRadiusOverrideProperty); }
            set { SetValue(CornerRadiusOverrideProperty, value); }
        }

        public static readonly DependencyProperty ForegroundColourOverrideProperty =
            DependencyProperty.Register("ForegroundColourOverride", typeof(Brush), typeof(Scratchpad), new PropertyMetadata(default(Brush)));

        public Brush ForegroundColourOverride
        {
            get { return (Brush)GetValue(ForegroundColourOverrideProperty); }
            set { SetValue(ForegroundColourOverrideProperty, value); }
        }

        public static readonly DependencyProperty OpacityOverrideProperty =
            DependencyProperty.Register("OpacityOverride", typeof(double), typeof(Scratchpad), new PropertyMetadata(defaultValue: 1.0));

        public double OpacityOverride
        {
            get { return (double)GetValue(OpacityOverrideProperty); }
            set { SetValue(OpacityOverrideProperty, value); }
        }

        public static readonly DependencyProperty DisabledBackgroundColourOverrideProperty =
            DependencyProperty.Register("DisabledBackgroundColourOverride", typeof(Brush), typeof(Scratchpad), new PropertyMetadata(default(Brush)));

        public Brush DisabledBackgroundColourOverride
        {
            get { return (Brush)GetValue(DisabledBackgroundColourOverrideProperty); }
            set { SetValue(DisabledForegroundColourOverrideProperty, value); }
        }

        public static readonly DependencyProperty DisabledForegroundColourOverrideProperty =
            DependencyProperty.Register("DisabledForegroundColourOverride", typeof(Brush), typeof(Scratchpad), new PropertyMetadata(default(Brush)));

        public Brush DisabledForegroundColourOverride
        {
            get { return (Brush)GetValue(DisabledForegroundColourOverrideProperty); }
            set { SetValue(DisabledForegroundColourOverrideProperty, value); }
        }
    }
}
