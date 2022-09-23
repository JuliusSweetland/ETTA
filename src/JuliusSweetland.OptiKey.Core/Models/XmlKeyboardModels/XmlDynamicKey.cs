// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - AllRights Reserved
using JuliusSweetland.OptiKey.Enums;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace JuliusSweetland.OptiKey.Models
{
    public class XmlDynamicKey : XmlDynamicItem , IXmlKey
    {
        public XmlDynamicKey() { }

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
        
        public string Label { get; set; } //Either set this, the Symbol, or both. This value become the Text value on the created Key.
        public string ShiftDownLabel { get; set; } //Optional - only required to display an alternate Text value when the shift key is down.
        public string Symbol { get; set; }
        [XmlIgnore] public bool? AutoScaleToOneKeyWidth;
        [XmlIgnore] public bool? AutoScaleToOneKeyHeight;
        [XmlAttribute("AutoScaleToOneKeyWidth")]
        public string AutoScaleToOneKeyWidthString
        {
            get { return AutoScaleToOneKeyWidth.HasValue ? AutoScaleToOneKeyWidth.Value.ToString() : null; }
            set
            {
                if (bool.TryParse(value, out bool v))
                    AutoScaleToOneKeyWidth = v;
            }
        }
        [XmlAttribute("AutoScaleToOneKeyHeight")]
        public string AutoScaleToOneKeyHeightString
        {
            get { return AutoScaleToOneKeyHeight.HasValue ? AutoScaleToOneKeyHeight.Value.ToString() : null; }
            set
            {
                if (bool.TryParse(value, out bool v))
                    AutoScaleToOneKeyHeight = v;
            }
        }
        [XmlAttribute] public string SharedSizeGroup { get; set; } //Optional - only required to break out a key, or set of keys, to size separately, otherwise size grouping is determined automatically
        [XmlAttribute] public bool UsePersianCompatibilityFont { get; set; }
        [XmlAttribute] public bool UseUnicodeCompatibilityFont { get; set; }
        [XmlAttribute] public bool UseUrduCompatibilityFont { get; set; }
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
            set { BackAction = XmlUtils.ConvertToBoolean(value); }
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

        public string Name { get; set; }
        public string Method { get; set; }
        [XmlElement("Argument")] public List<DynamicArgument> Arguments { get; set; }
    }

    public class DynamicArgument
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
