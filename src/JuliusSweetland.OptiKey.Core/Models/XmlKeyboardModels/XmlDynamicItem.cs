// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using System.Collections.Generic;
using System.Windows;
using System.Xml.Serialization;

namespace JuliusSweetland.OptiKey.Models
{
    public class XmlDynamicItem
    {
        public XmlDynamicItem() { }

        [XmlElement("Popup", typeof(XmlDynamicPopup))]
        [XmlElement("DynamicKey", typeof(XmlDynamicKey))]
        [XmlElement("OutputPanel", typeof(XmlDynamicOutputPanel))]
        [XmlElement("Scratchpad", typeof(XmlDynamicScratchpad))]
        [XmlElement("SuggestionRow", typeof(XmlDynamicSuggestionRow))]
        [XmlElement("SuggestionCol", typeof(XmlDynamicSuggestionCol))]
        public List<XmlDynamicItem> Items { get; } = new List<XmlDynamicItem>();
        
        [XmlElement("KeyGroup")] public List<KeyGroup> KeyGroups { get; set; }

        [XmlAttribute] public int Row { get; set; } = -1;
        [XmlAttribute] public int Col { get; set; } = -1;
        [XmlAttribute] public int Width { get; set; } = 1;
        [XmlAttribute] public int Height { get; set; } = 1;
        [XmlAttribute] public string ForegroundColor { get; set; }
        [XmlAttribute] public string KeyDisabledForeground { get; set; }
        [XmlAttribute] public string KeyDownForeground { get; set; }
        [XmlAttribute] public string BackgroundColor { get; set; }
        [XmlAttribute] public string KeyDisabledBackground { get; set; }
        [XmlAttribute] public string KeyDownBackground { get; set; }
        [XmlAttribute] public string BorderColor { get; set; }
        [XmlAttribute] public string BorderThickness { get; set; }
        [XmlAttribute] public string CornerRadius { get; set; }
        [XmlAttribute] public string Margin { get; set; }
        [XmlAttribute] public string Opacity { get; set; }
        [XmlAttribute] public string KeyDisabledOpacity { get; set; }
        [XmlAttribute] public string KeyDownOpacity { get; set; }
        [XmlAttribute] public int LockOnTime { get; set; } = -1;
        //This is a comma separated list of times required to trigger keystrokes
        //When used the time to trigger the first keystroke is overridden and the time to trigger repetitive keystrokes can be shortened
        [XmlAttribute] public string CompletionTimes { get; set; }
        //The amount of time a continuous gaze is required to lock down a key
        //When used the key will be pressed down when triggerred and remain down as long as gaze is fixed on key
        [XmlAttribute] public int TimeRequiredToLockDown { get; set; } = -1;
        //The purpose of this is to allow the key to be quickly released if TimeRequiredToLockDown is used
        //It is the amount of time focus can be lost before resetting the key
        //When used this will override the keyFixationTimeout
        [XmlAttribute] public int LockDownAttemptTimeout { get; set; } = -1;

        [XmlIgnore] public Rect GazeRegion { get; set; }
        [XmlAttribute("GazeRegion")] public string GazeRegionString
        {
            get { return GazeRegion.ToString(); }
            set { try { GazeRegion = Rect.Parse(value); } catch { } }
        }
    }

    public class XmlDynamicPopup : XmlDynamicKey { }
    public class XmlDynamicOutputPanel: XmlDynamicItem { }
    public class XmlDynamicScratchpad : XmlDynamicItem { }
    public class XmlDynamicSuggestionRow : XmlDynamicItem { }
    public class XmlDynamicSuggestionCol : XmlDynamicItem { }
    public class KeyGroup
    {
        [XmlText] public string Value { get; set; }
    }
}
