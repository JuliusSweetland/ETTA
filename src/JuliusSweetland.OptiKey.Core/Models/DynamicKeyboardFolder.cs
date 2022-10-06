// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using JuliusSweetland.OptiKey.Extensions;
using JuliusSweetland.OptiKey.Properties;
using JuliusSweetland.OptiKey.UI.Views.Keyboards.Common;

namespace JuliusSweetland.OptiKey.Models
{
    struct KeyboardInfo
    {
        public string fullPath;
        public string keyboardName;
        public string symbolString;
        public bool isHidden; // default false

        public override string ToString()
        {
            string str = fullPath +  ", ";
            str += keyboardName + ", ";
            str += symbolString + ", ";
            str += isHidden + ", ";
            return str;
        }
    }

    class DynamicKeyboardFolder
    {
        #region Private Members

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        public List<KeyboardInfo> AllKeyboards = new List<KeyboardInfo>();
        public List<KeyboardInfo> keyboards = new List<KeyboardInfo>();

        public DynamicKeyboardFolder()
        {
            // Find all possible xml files
            string filePath = Settings.Default.DynamicKeyboardsLocation;

            if (Directory.Exists(filePath))
            {
                string[] fileArray = Directory.GetFiles(filePath, "*.xml");

                Log.InfoFormat("Found {0} keyboard files", fileArray.Length);

                // Read in keyboard name, symbol, hidden state from each file
                // Note that ordering is currently undefined
                foreach (string fileName in fileArray)
                {
                    string keyboardPath = Path.Combine(filePath, fileName);
                    KeyboardInfo info = GetKeyboardInfo(keyboardPath);
                    if (null != info.fullPath)
                    {
                        AllKeyboards.Add(info);
                        if (!info.isHidden)
                        {
                            keyboards.Add(info);
                            Log.InfoFormat("Found keyboard file: {0}", info.fullPath);
                        }
                        else
                        {
                            Log.InfoFormat("Ignoring keyboard file: {0}", info.fullPath);
                        }
                    }
                }
            }
        }

        #region Private Methods

        // get name from XML if present
        private KeyboardInfo GetKeyboardInfo(string keyboardPath)
        {
            KeyboardInfo info = new KeyboardInfo();
            info.fullPath = keyboardPath;
            
            try
            {
                XmlKeyboard spec = XmlKeyboard.ReadFromFile(keyboardPath);
                
                info.keyboardName = DynamicKeyboard.StringWithValidNewlines(spec.Name);
                info.symbolString = spec.Symbol;
                info.isHidden = spec.Hidden;

                // default to filename if no name given
                if (info.keyboardName.Length == 0)
                {
                    info.keyboardName = Path.GetFileName(keyboardPath);
                }      
            }
            catch (Exception e)
            {
                // replace info with default (no contents)
                info = new KeyboardInfo();
                Log.Error(e.ToString());
            }

            return info;
        }

        #endregion

    }
}
