using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace DeviceSimulator.Controls
{
    public sealed partial class NumericUpDownControl : UserControl
    {
        public NumericUpDownControl()
        {
            this.InitializeComponent();
            NumberTxtBox.IsEnabled = false;
        }

        public bool IsControlEnabled
        {
            get { return (bool)GetValue(IsControlEnabledProperty); }
            set { SetValue(IsControlEnabledProperty, value); }
        }

        public int NumericCounter
        {
            get { return (int)GetValue(NumericCounterProperty); }
            set { SetValue(NumericCounterProperty, value); }
        }

        public string ControlTitle
        {
            get { return (string)GetValue(ControlTitleProperty); }
            set { SetValue(ControlTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ControlTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControlTitleProperty =
            DependencyProperty.Register("ControlTitle", typeof(string), typeof(NumericUpDownControl), new PropertyMetadata(0));



        // Using a DependencyProperty as the backing store for NumericCounter.  This enables animation, styling, binding, etc..

        public static readonly DependencyProperty NumericCounterProperty =
            DependencyProperty.Register("NumericCounter", typeof(int), typeof(NumericUpDownControl), new PropertyMetadata(0));

        public static readonly DependencyProperty IsControlEnabledProperty =
            DependencyProperty.Register("IsControlEnabled", typeof(bool), typeof(NumericUpDownControl), new PropertyMetadata(0));

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            NumericCounter++;
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            if (NumericCounter > 0)
            {
                NumericCounter--;
            }
        }
    }
}
