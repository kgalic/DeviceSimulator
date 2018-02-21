using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            base.Initialize();

            RegisterServices();

            RegisterNavigationServiceAppStart<MainViewModel>();
        }

        private void RegisterServices()
        {
            Mvx.LazyConstructAndRegisterSingleton<IDeviceService, DeviceService>();
            Mvx.LazyConstructAndRegisterSingleton<IConstantsService, ConstantsService>();
        }
    }
}
