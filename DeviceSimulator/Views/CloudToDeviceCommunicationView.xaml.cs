﻿using MvvmCross.Platforms.Uap.Views;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DeviceSimulator.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CloudToDeviceCommunicationView : MvxWindowsPage
    {
        public CloudToDeviceCommunicationView()
        {
            this.InitializeComponent();
        }

        private void MessagesTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var scrollableHeight = MessagesScrollView.ScrollableHeight;
            if (scrollableHeight > 0)
            {
                MessagesScrollView.ChangeView(MessagesScrollView.HorizontalOffset, scrollableHeight, null);
            }
        }
    }
}
