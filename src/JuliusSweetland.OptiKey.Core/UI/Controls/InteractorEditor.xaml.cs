// Copyright(c) 2020 OPTIKEY LTD (UK company number11854839) - All Rights Reserved
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using JuliusSweetland.OptiKey.Enums;
using JuliusSweetland.OptiKey.Models;

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

        public static readonly DependencyProperty InteractorProperty = DependencyProperty
            .Register("Interactor", typeof(Interactor), typeof(InteractorEditor), new PropertyMetadata(default(Interactor)));

        public Interactor Interactor
        {
            get { return (Interactor)GetValue(InteractorProperty); }
            set { SetValue(InteractorProperty, value); }
        }

        public static List<string> SymbolList = new List<string>() { "" }.Concat(new ResourceDictionary() { Source = new Uri("/OptiKey;component/Resources/Icons/KeySymbols.xaml", UriKind.RelativeOrAbsolute) }.Keys.Cast<string>().ToList()).ToList();

        #endregion

        #region Methods

        #endregion

    }
}

