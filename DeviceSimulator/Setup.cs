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
using MvvmCross.Platforms.Uap.Presenters;
using MvvmCross.Platforms.Uap.Views;
using MessagePublisher.Core;

namespace MessagePublisher
{
    public class Setup : MvxWindowsSetup<Core.App>
    {
        public Setup()
        {
        }

        protected override IMvxWindowsViewPresenter CreateViewPresenter(IMvxWindowsFrame rootFrame)
        {
            return new MvxWindowsViewPresenter(rootFrame);
        }

        protected override IMvxApplication CreateApp()
        {
            return new MessagePublisher.Core.App();
        }

        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();

            Mvx.IoCProvider.RegisterSingleton<IMvxMessenger>(new MvxMessengerHub());
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IFilePickerService, FilePickerService>();
        }
    }
}
