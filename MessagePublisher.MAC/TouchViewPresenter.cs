using System;
using System.Threading.Tasks;
using AppKit;
using MvvmCross.Platforms.Mac.Presenters;
using MvvmCross.ViewModels;

namespace MessagePublisher.MAC
{
    public class TouchViewPresenter : MvxMacViewPresenter
    {
        public TouchViewPresenter(INSApplicationDelegate applicationDelegate) : base(applicationDelegate)
        {
        }

        public override Task<bool> Show(MvxViewModelRequest request)
        {
            return base.Show(request);
        }
    }
}
