// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved

using JuliusSweetland.OptiKey.Enums;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace JuliusSweetland.OptiKey.Models
{
    public class XmlKeyCommand : INotifyPropertyChanged
    {
        [XmlIgnore] public KeyCommands Name;
        [XmlAttribute("Name")] public string NameAsString
        {
            get { return Name.ToString(); }
            set
            {
                if (System.Enum.TryParse(value, out KeyCommands key))
                    Name = key;

                OnPropertyChanged(null);
            }
        }

        [XmlIgnore] public bool HideFunctionList { get { return Name != KeyCommands.Action; } }
        [XmlAttribute] public string Value { get; set; }

        [XmlIgnore] public bool BackAction { get; set; }
        [XmlIgnore] public bool HideBack { get { return Name != KeyCommands.ChangeKeyboard; } }
        [XmlAttribute("BackAction")]
        public string XmlBackAction
        {
            get { return HideBack ? null : BackAction.ToString(); }
            set { BackAction = bool.Parse(value); }
        }

        [XmlIgnore] public bool HideMethod { get { return Name != KeyCommands.Plugin; } }
        [XmlAttribute] public string Method { get; set; }

        public List<PluginArgument> Argument { get; set; }

        public List<KeyCommand> LoopCommands { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}