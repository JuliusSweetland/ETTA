// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using JuliusSweetland.OptiKey.UI.Controls;
using JuliusSweetland.OptiKey.Models;
using System.Windows.Controls;
using System;
using System.Linq;
using System.Windows.Media;
using System.Reflection;
using log4net;
using JuliusSweetland.OptiKey.Properties;
using JuliusSweetland.OptiKey.Extensions;

namespace JuliusSweetland.OptiKey.UI.Views.Keyboards.Common
{

    /// <summary>
    /// Interaction logic for DynamicKeyboardSelector.xaml
    /// </summary>
    public partial class DynamicKeyboardSelector : KeyboardView
    {

        #region Private Members
    
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private DynamicKeyboardFolder folder;
        private int pageIndex = 0;

        // TODO: Could be user configurable at some point?
        private int mRows = 3;
        private int mCols = 4;

        #endregion

        public DynamicKeyboardSelector(int pageIndex, string keyboardsPath = null)
        {
            InitializeComponent();
            this.pageIndex = pageIndex;

            // Populate model
            folder = new DynamicKeyboardFolder(keyboardsPath);

            // Setup grid
            for (int i = 0; i < this.mRows; i++)
            {
                MainGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < this.mCols; i++)
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Add back/menu key, bottom right
            if (keyboardsPath == Settings.Default.DynamicKeyboardsLocation)
            {
                Key newKey = new Key();
                newKey.SharedSizeGroup = "SingleKey";
                newKey.SymbolGeometry = (Geometry)this.Resources["BackIcon"];
                newKey.Text = JuliusSweetland.OptiKey.Properties.Resources.BACK;
                newKey.Value = KeyValues.BackFromKeyboardKey;
                this.AddKey(newKey, this.mRows - 1, this.mCols - 1);
            }
            else
            {
                Key newKey = new Key();
                newKey.SharedSizeGroup = "SingleKey";
                newKey.SymbolGeometry = (Geometry)this.Resources["MenuIcon"];
                newKey.Text = JuliusSweetland.OptiKey.Properties.Resources.MENU;
                newKey.Value = KeyValues.MenuKeyboardKey;
                this.AddKey(newKey, this.mRows - 1, this.mCols - 1);
            }            

            // Sleep key for bottom left
            {
                Key newKey = new Key();
                newKey.SymbolGeometry = (Geometry)this.Resources["SleepIcon"];                
                newKey.Text = JuliusSweetland.OptiKey.Properties.Resources.SLEEP;
                newKey.Value = KeyValues.SleepKey;
                this.AddKey(newKey, this.mRows - 1, 0);
            }

            // Add keyboard keys, or blanks
            int maxKeyboardsPerPage = (this.mCols - 2) * this.mRows;
            int totalNumKeyboards = folder.keyboards.Count;
            int remainingKeyboards = totalNumKeyboards - maxKeyboardsPerPage * pageIndex;
            int nKBs = Math.Min(remainingKeyboards, maxKeyboardsPerPage);
            int firstKB = maxKeyboardsPerPage * pageIndex;

            if (totalNumKeyboards > 0)
            {
                for (int i = 0; i < maxKeyboardsPerPage; i++)
                {
                    var r = i / (this.mCols - 2); // integer division
                    var c = 1 + (i % (this.mCols - 2));

                    if (i < nKBs)
                    {
                        KeyboardInfo kbInfo = folder.keyboards.ElementAt(firstKB + i);
                        // Add key to link to keyboard
                        this.AddKeyboardKey(kbInfo, r, c);
                    }
                    else
                    {
                        // Add empty/inactive key for consistent aesthetic
                        this.AddKey(new Key(), r, c);
                    }
                }
            }
            else
            {
                // Error layout for when there are no keyboards
                {
                    Key newKey = new Key();
                    newKey.SharedSizeGroup = "ErrorText1";
                    newKey.Text = JuliusSweetland.OptiKey.Properties.Resources.NO_KEYBOARDS_FOUND;
                    this.AddKey(newKey, 0, 1, 1, 2);
                }
                {
                    Key newKey = new Key();
                    newKey.SharedSizeGroup = "ErrorText1";
                    newKey.Text = Properties.Resources.KEYBOARD_FOLDER_CAN_BE_CHANGED_SPLIT_LINE.ToStringWithValidNewlines();
                    this.AddKey(newKey, 1, 1, 1, 2);
                }
                {
                    Key newKey = new Key();
                    newKey.SharedSizeGroup = "ErrorText2";
                    newKey.Text = JuliusSweetland.OptiKey.Properties.Resources.DYNAMIC_KEYBOARDS_LOCATION_LABEL;
                    newKey.Text += "\n";
                    newKey.Text += Settings.Default.DynamicKeyboardsLocation;
                    this.AddKey(newKey, 2, 1, 1, 2);
                }
            }

            // Add prev/next keys (may be disabled)
            {
                Key newKey = new Key();
                newKey.SharedSizeGroup = "SingleKey";
                newKey.SymbolGeometry = (Geometry)this.Resources["ArrowPointingToLeftIcon"];
                newKey.Text = JuliusSweetland.OptiKey.Properties.Resources.PREV;
                if (this.pageIndex > 0)
                {
                    newKey.Value = KeyValues.DynamicKeyboardPrev;
                }
                this.AddKey(newKey, 0, 0, 2, 1);
            }
            {
                Key newKey = new Key();
                newKey.SharedSizeGroup = "SingleKey";
                newKey.SymbolGeometry = (Geometry)this.Resources["ArrowPointingToRightIcon"];
                newKey.Text = JuliusSweetland.OptiKey.Properties.Resources.NEXT;
                if (nKBs < remainingKeyboards)
                {
                    newKey.Value = KeyValues.DynamicKeyboardNext;
                }
                this.AddKey(newKey, 0, this.mCols - 1, 2, 1);
            }
        }

        private void AddKey(Key key, int row, int col, int rowspan = 1, int colspan = 1)
        {
            MainGrid.Children.Add(key);
            Grid.SetColumn(key, col);
            Grid.SetRow(key, row);
            Grid.SetColumnSpan(key, colspan);
            Grid.SetRowSpan(key, rowspan);

        }

        private void AddKeyboardKey(KeyboardInfo info, int row, int col)
        {
            Key lKey = new Key();
            lKey.Value = new ChangeKeyboardKeyValue(info.fullPath);
            lKey.SharedSizeGroup = "KeyboardKey";
            lKey.Text = info.keyboardName;
            if (info.symbolString != null)
            {
                Geometry geom = (Geometry)this.Resources[info.symbolString];
                if (null != geom)
                {
                    lKey.SymbolGeometry = geom;
                }
            }
            this.AddKey(lKey, row, col);
        }

        private int RowFromIndex(int index)
        {
            return index / this.mCols; // integer division
        }

        private int ColFromIndex(int index)
        {
            return index % this.mCols;
        }

    }
}
