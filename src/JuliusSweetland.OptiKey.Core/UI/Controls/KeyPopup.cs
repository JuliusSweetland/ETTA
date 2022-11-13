// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using System.ComponentModel;

namespace JuliusSweetland.OptiKey.UI.Controls
{
    public class KeyPopup : Key, INotifyPropertyChanged
    {
        #region Ctor

        public KeyPopup()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        #endregion

    }
}