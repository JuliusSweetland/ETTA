// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using System.Collections.Generic;
using System.Xml.Serialization;

namespace JuliusSweetland.OptiKey.Models
{
    public class XmlInteractors
    {
        [XmlElement("ActionKey", typeof(ActionKey))]
        [XmlElement("ChangeKeyboardKey", typeof(ChangeKeyboardKey))]
        [XmlElement("DynamicKey", typeof(DynamicKey))]
        [XmlElement("PluginKey", typeof(PluginKey))]
        [XmlElement("TextKey", typeof(TextKey))]
        [XmlElement("OutputPanel", typeof(DynamicOutputPanel))]
        [XmlElement("Popup", typeof(DynamicPopup))]
        [XmlElement("Scratchpad", typeof(DynamicScratchpad))]
        [XmlElement("SuggestionRow", typeof(DynamicSuggestionRow))]
        [XmlElement("SuggestionCol", typeof(DynamicSuggestionCol))]
        public List<Interactor> Interactors { get; set; } = new List<Interactor>();
    }
}
