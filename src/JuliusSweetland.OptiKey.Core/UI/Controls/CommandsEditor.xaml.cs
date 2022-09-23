// Copyright(c) 2020 OPTIKEY LTD (UK company number11854839) - All Rights Reserved
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using JuliusSweetland.OptiKey.Models;

namespace JuliusSweetland.OptiKey.UI.Controls
{
    /// <summary>
    /// Interaction logic for CommandsEditor.xaml
    /// </summary>
    public partial class CommandsEditor : UserControl
    {
        private int commandIndex;

        public CommandsEditor()
        {
            InitializeComponent();
        }

        #region Properties

        public static readonly DependencyProperty InteractorProperty = DependencyProperty
            .Register("Interactor", typeof(Interactor), typeof(CommandsEditor), new PropertyMetadata(default(Interactor)));

        public Interactor Interactor
        {
            get { return (Interactor)GetValue(InteractorProperty); }
            set { SetValue(InteractorProperty, value); }
        }

        public static List<string> CommandKeyList = Enum.GetNames(typeof(Enums.KeyCommands)).Cast<string>().OrderBy(mb => mb.ToString()).ToList();

        public static List<string> KeyboardList = Enum.GetNames(typeof(Enums.Keyboards)).Cast<string>().OrderBy(mb => mb.ToString()).ToList();

        public static List<string> FunctionKeyList = Enum.GetNames(typeof(Enums.FunctionKeys)).Cast<string>().OrderBy(mb => mb.ToString()).ToList();

        #endregion

        #region Methods

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Interactor.Commands.Move(commandIndex - 1, commandIndex);
            }
            catch { }
        }

        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Interactor.Commands.Move(commandIndex + 1, commandIndex);
            }
            catch { }
        }

        private void SelectCommand(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ListBoxItem item = sender as ListBoxItem;
            var listView = ItemsControl.ItemsControlFromItemContainer(item) as ListBox;
            int index = listView.ItemContainerGenerator.IndexFromContainer(item);
            commandIndex = index;
        }

        private void AddCommand_Click(object sender, RoutedEventArgs e)
        {
            Interactor.AddCommand();
        }

        private void DeleteCommand_Click(object sender, RoutedEventArgs e)
        {
            Interactor.Commands.RemoveAt(commandIndex);
        }

        #endregion

        private void Command_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
