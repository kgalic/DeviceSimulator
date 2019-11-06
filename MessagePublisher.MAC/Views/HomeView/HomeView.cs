using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using MvvmCross.Platforms.Mac.Binding.Views;

namespace MessagePublisher.MAC.Views.HomeView
{
    public partial class HomeView : MvxView
    {
        #region Constructors

        // Called when created from unmanaged code
        public HomeView(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public HomeView(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion
    }
}
