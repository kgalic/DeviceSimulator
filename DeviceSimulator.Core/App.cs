using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.ViewModels;
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

            RegisterAppStart<MainViewModel>();
        }

        public override Task Startup()
        {
            Mvx.IoCProvider.Resolve<ITranslationsService>().LoadTranslations();
            return base.Startup();
        }

        private void RegisterServices()
        {
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IDeviceService, DeviceService>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IEventGridService, EventGridService>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IMessageExpressionService, MessageExpressionService>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IConsoleLoggerService, ConsoleLoggerService>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<ITranslationsService, TranslationsService>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IDeviceSettingDataService, DeviceSettingDataService>();

            Mvx.IoCProvider.RegisterType<ITimerService, TimerService>();
        }
    }
}
