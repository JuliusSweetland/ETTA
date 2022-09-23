// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using JuliusSweetland.OptiKey.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace JuliusSweetland.OptiKey.Models
{
    public class KeyCommand : INotifyPropertyChanged
    {
        public KeyCommand() { }
        [XmlIgnore]
        public string Type
        {
            get
            {
                return this is ActionCommand ? KeyCommands.Action.ToString() :
                    this is ChangeKeyboardCommand ? KeyCommands.ChangeKeyboard.ToString() :
                    this is KeyDownCommand ? KeyCommands.KeyDown.ToString() :
                    this is KeyTogglCommand ? KeyCommands.KeyToggle.ToString() :
                    this is KeyUpCommand ? KeyCommands.KeyUp.ToString() :
                    this is LoopCommand ? KeyCommands.Loop.ToString() :
                    this is MoveWindowCommand ? KeyCommands.MoveWindow.ToString() :
                    this is PluginCommand ? KeyCommands.Plugin.ToString() :
                    this is TextCommand ? KeyCommands.Text.ToString() :
                    KeyCommands.Wait.ToString();
            }
        }
        [XmlIgnore] public bool HideBack { get { return !(this is ChangeKeyboardCommand); } }
        [XmlIgnore] public bool HideFunctionList { get { return !(this is ActionCommand); } }
        [XmlIgnore] public bool HideMethod { get { return !(this is PluginCommand); } }

        private string value;
        [XmlIgnore] public string Value { get { return value; } set { this.value = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
