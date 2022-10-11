// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using System;
using JuliusSweetland.OptiKey.Properties;
using JuliusSweetland.OptiKey.UI.ViewModels.Keyboards.Base;

namespace JuliusSweetland.OptiKey.UI.ViewModels.Keyboards
{
    public class DynamicKeyboardSelector : BackActionKeyboard
    {
        private int pageIndex;
        private string directory;

        public DynamicKeyboardSelector(Action backAction, int pageIndex, string directory = null)
            : base(backAction)
        {
            this.pageIndex = pageIndex;
            if (String.IsNullOrEmpty(directory))
            {
                this.directory = Settings.Default.DynamicKeyboardsLocation;
            }
            else
            {
                this.directory = directory;
            }
        }

        public int PageIndex
        {
            get { return pageIndex; }
        }

        public string Directory
        {
            get { return directory; }
        }
    }
}
