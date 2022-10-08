// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - AllRights Reserved
using System.Xml.Serialization;

namespace JuliusSweetland.OptiKey.Models
{
    public class XmlElementValue
    {
        [XmlText] public string Value { get; set; }
    }
}
