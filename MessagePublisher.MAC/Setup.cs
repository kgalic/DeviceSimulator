using System;
using AppKit;
using MessagePublisher.Core;
using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.Platforms.Mac.Core;
using MvvmCross.Platforms.Mac.Presenters;
using MvvmCross.ViewModels;

namespace MessagePublisher.MAC
{
    public class Setup : MvxMacSetup<Core.App>
    {
        public Setup()
        {

        }

        protected override IMvxMacViewPresenter CreateViewPresenter()
        {
            return new TouchViewPresenter(ApplicationDelegate);
        }

        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IFilePickerService, FilePickerService>();
        }
    }
}
