using DeviceSimulator.Core.ViewModels;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator.Core
{
    public class MainViewModel : MvxViewModel
    {
        #region Fields

        private readonly List<(string Tag, Type viewModel)> _pages = new List<(string Tag, Type Page)>
        {
            ("home", typeof(HomeViewModel)),
            ("direct_method", typeof(DirectMethodCommunicationViewModel)),
            ("c2d_messages", typeof(CloudToDeviceCommunicationViewModel))
        };

        #endregion

        #region Constructors & Lifecycle

        public MainViewModel()
        {
            
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<HomeViewModel>();
        }

        #endregion

        #region Commands

        public IMvxCommand ItemInvokedCommand
        {
            get => new MvxCommand<string>((item) =>
            {
                var viewModel = _pages.FirstOrDefault(i => i.Tag == item);
                var type = viewModel.viewModel;
                if (type == typeof(HomeViewModel))
                {
                    Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<HomeViewModel> ();
                }
                else if (type == typeof(DirectMethodCommunicationViewModel))
                {
                    Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<DirectMethodCommunicationViewModel>();
                }
                else if (type == typeof(CloudToDeviceCommunicationViewModel))
                {
                    Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<CloudToDeviceCommunicationViewModel>();
                }
            });
        }

        #endregion
    }
}
