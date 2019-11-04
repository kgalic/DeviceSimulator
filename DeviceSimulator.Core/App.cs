using MessagePublisher.Core.ViewModels;
using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagePublisher.Core
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
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IEventHubService, EventHubService>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IMessageExpressionService, MessageExpressionService>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<ITranslationsService, TranslationsService>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IDeviceSettingDataService, DeviceSettingDataService>();
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IServiceBusPublisherService, ServiceBusPublisherService>();

            Mvx.IoCProvider.RegisterType<ITimerService<EventGridViewModel>, TimerService<EventGridViewModel>>();
            Mvx.IoCProvider.RegisterType<ITimerService<HomeViewModel>, TimerService<HomeViewModel>>();
            Mvx.IoCProvider.RegisterType<ITimerService<ServiceBusViewModel>, TimerService<ServiceBusViewModel>>();
            Mvx.IoCProvider.RegisterType<ITimerService<EventHubViewModel>, TimerService<EventHubViewModel>>();
            Mvx.IoCProvider.RegisterType<IConsoleLoggerService, ConsoleLoggerService>();
        }
    }
}
