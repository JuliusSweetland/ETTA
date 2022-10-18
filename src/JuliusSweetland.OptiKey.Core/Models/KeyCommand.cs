// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using JuliusSweetland.OptiKey.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace JuliusSweetland.OptiKey.Models
{
    public class KeyCommand : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public KeyCommand() { }

        [XmlIgnore] public string Type
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
        [XmlIgnore] public bool HideTextBox { get { return (this is ActionCommand || this is ChangeKeyboardCommand || this is PluginCommand); } }
        [XmlIgnore] public bool HideBack { get { return !(this is ChangeKeyboardCommand); } }
        [XmlIgnore] public bool HideFunction { get { return !(this is ActionCommand); } }
        [XmlIgnore] public bool HidePlugin { get { return !(this is PluginCommand); } }

        private string value;
        [XmlIgnore] public string Value { get { return value; } set { this.value = value; OnPropertyChanged(); } }
    }

    public class ActionCommand : KeyCommand
    {
        [XmlText] public string XmlText { get { return Value; } set { Value = value; } }

        [XmlIgnore] public FunctionKeys FunctionKey { get; set; }
    }

    public class ChangeKeyboardCommand : KeyCommand
    {
        [XmlText] public string XmlText { get { return Value; } set { Value = value; } }

        [XmlIgnore] public bool BackAction { get; set; } = true;

        [XmlAttribute("BackReturnsHere")]
        public string BackReturnsHereAsString
        {
            get { return BackAction ? "True" : "False"; }
            set { BackAction = bool.TryParse(value, out bool result) && result; }
        }
    }

    public class KeyDownCommand : KeyCommand
    {
        [XmlText] public string XmlText { get { return Value; } set { Value = value; } }
    }

    public class KeyUpCommand : KeyCommand
    {
        [XmlText] public string XmlText { get { return Value; } set { Value = value; } }
    }

    public class KeyTogglCommand : KeyCommand
    {
        [XmlText] public string XmlText { get { return Value; } set { Value = value; } }
    }

    public class LoopCommand : KeyCommand
    {
        [XmlElement("Action", typeof(ActionCommand))]
        [XmlElement("ChangeKeyboard", typeof(ChangeKeyboardCommand))]
        [XmlElement("KeyDown", typeof(KeyDownCommand))]
        [XmlElement("KeyUp", typeof(KeyUpCommand))]
        [XmlElement("KeyToggle", typeof(KeyTogglCommand))]
        [XmlElement("Loop", typeof(LoopCommand))]
        [XmlElement("Plugin", typeof(PluginCommand))]
        [XmlElement("MoveWindow", typeof(MoveWindowCommand))]
        [XmlElement("Text", typeof(TextCommand))]
        [XmlElement("Wait", typeof(WaitCommand))]
        public List<KeyCommand> Commands { get; set; } = new List<KeyCommand>();

        [XmlAttribute] public int Count { get; set; } = 1; //The number of loop repetitions
    }

    public class MoveWindowCommand : KeyCommand
    {
        [XmlText] public string XmlText { get { return Value; } set { Value = value; } }
    }

    public class SwitchCommand : KeyCommand
    {
        [XmlElement("Time", typeof(SwitchTime))]
        public List<SwitchTime> SwitchTimes = new List<SwitchTime>();
    }

    public class SwitchTime
    {
        [XmlIgnore] public TimeSpan? TimeSpan;
        [XmlAttribute] public string Value
        {
            get { return TimeSpan.HasValue ? TimeSpan.Value.TotalMilliseconds.ToString() : null; }
            set { TimeSpan = double.TryParse(value, out double result) 
                    ? (TimeSpan?)System.TimeSpan.FromMilliseconds(result) : null; }
        }
        public KeyCommand KeyCommand { get; set; }
    }

    public class TextCommand : KeyCommand
    {
        [XmlText] public string XmlText { get { return Value; } set { Value = value; } }
    }

    public class WaitCommand : KeyCommand
    {
        [XmlText] public string XmlText { get { return Value; } set { Value = value; } }
    }

    public class PluginCommand : KeyCommand
    {
        [XmlText] public string XmlText { get { return Value; } set { Value = value; } }

        [XmlAttribute] public string Name { get; set; }
        [XmlAttribute] public string Method { get; set; }
        [XmlElement("Argument")] public List<PluginArgument> Arguments { get; set; }

        //Legacy
        [XmlElement("Name")] public string LegacyName { get; set; }
        [XmlElement("Method")] public string LegacyMethod { get; set; }
    }

    public class PluginArgument
    {
        [XmlAttribute] public string Name { get; set; }
        [XmlAttribute] public string Value { get; set; }

        //Legacy
        [XmlElement("Argument")] public List<PluginArgument> LegacyArgumentList { get; set; }
        [XmlElement("Name")] public string LegacyName { get; set; }
        [XmlElement("Value")] public string LegacyValue { get; set; }
    }
}
