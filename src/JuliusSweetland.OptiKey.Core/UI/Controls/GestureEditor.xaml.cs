// Copyright(c) 2020 OPTIKEY LTD (UK company number11854839) - All Rights Reserved
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using JuliusSweetland.OptiKey.Models;

namespace JuliusSweetland.OptiKey.UI.Controls
{
    /// <summary>
    /// Interaction logic for GestureEditor.xaml
    /// </summary>
    public partial class GestureEditor : UserControl
    {
        private int stepIndex;
        private int eventIndex;

        public GestureEditor()
        {
            InitializeComponent();
        }

        #region Properties

        public static readonly DependencyProperty EyeGestureProperty = DependencyProperty
            .Register("EyeGesture", typeof(EyeGesture), typeof(GestureEditor), new PropertyMetadata(default(EyeGesture)));

        public EyeGesture EyeGesture
        {
            get { return (EyeGesture)GetValue(EyeGestureProperty); }
            set { SetValue(EyeGestureProperty, value); }
        }

        public static readonly DependencyProperty PreviewProperty = DependencyProperty
            .Register("Preview", typeof(Canvas), typeof(GestureEditor), new PropertyMetadata(default(Canvas)));

        public Canvas Preview
        {
            get { return (Canvas)GetValue(PreviewProperty); }
            set { SetValue(PreviewProperty, value); }
        }

        public static List<string> CommandKeyList = Enum.GetNames(typeof(Enums.KeyCommands)).Cast<string>().OrderBy(mb => mb.ToString()).ToList();

        public static List<string> KeyboardList = Enum.GetNames(typeof(Enums.Keyboards)).Cast<string>().OrderBy(mb => mb.ToString()).ToList();

        public static List<string> FunctionKeyList = Enum.GetNames(typeof(Enums.FunctionKeys)).Cast<string>().OrderBy(mb => mb.ToString()).ToList();
        
        public static List<string> StepTypeList = Enum.GetNames(typeof(Enums.EyeGestureStepTypes)).Cast<string>().OrderBy(mb => mb.ToString()).ToList();

        #endregion

        #region Methods

        private void Redraw()
        {
            Preview = null;
        }

        private void TypeChanged(object sender, SelectionChangedEventArgs e)
        {
            Redraw();
        }

        private void SelectStep(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ListBoxItem item = sender as ListBoxItem;
            var listView = ItemsControl.ItemsControlFromItemContainer(item) as ListBox;
            int index = listView.ItemContainerGenerator.IndexFromContainer(item);
            stepIndex = index;
        }

        private void CopyStep_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var step = XmlEyeGestures.ReadFromString(new XmlEyeGestures() { GestureList = new ObservableCollection<EyeGesture>() { EyeGesture } }.WriteToString())
                    .GestureList[0].Steps[stepIndex];
                EyeGesture.Steps.Insert(stepIndex + 1, step);
                
            }
            catch { try { EyeGesture.Steps.Insert(0, new EyeGestureStep()); } catch { } }
            Redraw();
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EyeGesture.Steps.Move(stepIndex - 1, stepIndex);
                Redraw();
            }
            catch { }
        }

        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EyeGesture.Steps.Move(stepIndex + 1, stepIndex);
                Redraw();
            }
            catch { }
        }

        private void DeleteStep_Click(object sender, RoutedEventArgs e)
        {
            EyeGesture.Steps.RemoveAt(stepIndex);
            if (!EyeGesture.Steps.Any())
                EyeGesture.Steps.Add(new EyeGestureStep());

            Redraw();
        }

        private void SelectEvent(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ListBoxItem item = sender as ListBoxItem;
            var listView = ItemsControl.ItemsControlFromItemContainer(item) as ListBox;
            int index = listView.ItemContainerGenerator.IndexFromContainer(item);
            eventIndex = index;
        }

        private void AddEvent_Click(object sender, RoutedEventArgs e)
        {
            if (EyeGesture.Steps[stepIndex].Commands == null)
                EyeGesture.Steps[stepIndex].Commands = new ObservableCollection<XmlKeyCommand>();

            EyeGesture.Steps[stepIndex].Commands.Add(new XmlKeyCommand());
            Redraw();
        }

        private void DeleteEvent_Click(object sender, RoutedEventArgs e)
        {
            EyeGesture.Steps[stepIndex].Commands.RemoveAt(eventIndex);
            Redraw();
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
             Redraw();
        }

        private void NumericUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            Redraw();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            var keyCommand = EyeGesture.Steps[stepIndex].Commands[eventIndex].Name;
            if (keyCommand == Enums.KeyCommands.ChangeKeyboard)
            {
                comboBox.ItemsSource = KeyboardList;
            }
            else if (keyCommand == Enums.KeyCommands.Action)
            {
                comboBox.ItemsSource = FunctionKeyList;
            }
            else if (comboBox.ItemsSource != null)
            {
                comboBox.ItemsSource = null;
            }
        }

        #endregion

    }
}

