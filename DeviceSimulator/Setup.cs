using MvvmCross;
using MvvmCross.Logging;
using MvvmCross.Platforms.Uap.Core;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using MvvmCross.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace DeviceSimulator
{
    public class Setup : MvxWindowsSetup<Core.App>
    {
        public Setup()
        {
        }

        protected override IMvxApplication CreateApp()
        {
            return new DeviceSimulator.Core.App();
        }

        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();

            Mvx.IoCProvider.RegisterSingleton<IMvxMessenger>(new MvxMessengerHub());
        }
    }
}
