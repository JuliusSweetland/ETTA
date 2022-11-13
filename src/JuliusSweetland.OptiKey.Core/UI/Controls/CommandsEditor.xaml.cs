// Copyright(c) 2020 OPTIKEY LTD (UK company number11854839) - All Rights Reserved
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for CommandsEditor.xaml
    /// </summary>
    public partial class CommandsEditor : UserControl
    {
        private int commandIndex = -1;

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

        private ObservableCollection<KeyCommand> Commands { get { return Interactor.Commands; } }

        public static List<string> CommandKeyList = Enum.GetNames(typeof(Enums.KeyCommands)).Cast<string>().OrderBy(x => x).ToList();

        public static List<string> KeyboardList = new DynamicKeyboardFolder(null).AllKeyboards.Select(x => x.keyboardName).OrderBy(x => x).Concat(Enum.GetNames(typeof(Enums.Keyboards)).Cast<string>().OrderBy(x => x)).ToList();

        public static List<string> FunctionKeyList = Enum.GetNames(typeof(Enums.FunctionKeys)).Cast<string>().OrderBy(x => x).ToList();

        private void SelectCommand(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ListBoxItem item = sender as ListBoxItem;
            var listView = ItemsControl.ItemsControlFromItemContainer(item) as ListBox;
            int index = listView.ItemContainerGenerator.IndexFromContainer(item);
            commandIndex = index;
        }

        private void AddCommand_Click(object sender, RoutedEventArgs e)
        {
            Commands.Add(new TextCommand() { Value = Interactor.Label });
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (commandIndex < 1) return;

            Commands.Move(commandIndex, commandIndex - 1);
        }

        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (Commands.Count < 2) return;
            if (Commands.Count < commandIndex + 2) return;

            Commands.Move(commandIndex, commandIndex + 1);
        }

        private void SelectType(object sender, SelectionChangedEventArgs e)
        {
            if (commandIndex < 0)
                return;

            var newType = Enum.TryParse((string)((ComboBox)sender).SelectedItem, out KeyCommands kc) ? kc : KeyCommands.Text;
            Commands.RemoveAt(commandIndex);
            switch (newType)
            {
                case KeyCommands.Action:
                    Commands.Insert(commandIndex, new ActionCommand());
                    break;
                case KeyCommands.ChangeKeyboard:
                    Commands.Insert(commandIndex, new ChangeKeyboardCommand());
                    break;
                case KeyCommands.KeyDown:
                    Commands.Insert(commandIndex, new KeyDownCommand() { Value = Interactor.Label });
                    break;
                case KeyCommands.KeyToggle:
                    Commands.Insert(commandIndex, new KeyTogglCommand() { Value = Interactor.Label });
                    break;
                case KeyCommands.KeyUp:
                    Commands.Insert(commandIndex, new KeyUpCommand() { Value = Interactor.Label });
                    break;
                case KeyCommands.Loop:
                    Commands.Insert(commandIndex, new LoopCommand());
                    break;
                case KeyCommands.MoveWindow:
                    Commands.Insert(commandIndex, new MoveWindowCommand());
                    break;
                case KeyCommands.Plugin:
                    Commands.Insert(commandIndex, new PluginCommand());
                    break;
                case KeyCommands.Text:
                    Commands.Insert(commandIndex, new TextCommand() { Value = Interactor.Label });
                    break;
                case KeyCommands.Wait:
                    Commands.Insert(commandIndex, new WaitCommand());
                    break;
            }
        }

        private void DeleteCommand_Click(object sender, RoutedEventArgs e)
        {
            Commands.RemoveAt(commandIndex);
        }

        #endregion
    }
}
