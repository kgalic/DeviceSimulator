using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using MvvmCross.Platforms.Mac.Views;

namespace MessagePublisher.MAC.Views.HomeView
{
    public partial class HomeViewController : MvxViewController
    {
        #region Constructors

        // Called when created from unmanaged code
        public HomeViewController(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public HomeViewController(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public HomeViewController() : base("HomeView", NSBundle.MainBundle)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion

        //strongly typed view accessor
        public new HomeView View
        {
            get
            {
                return (HomeView)base.View;
            }
        }
    }
}
