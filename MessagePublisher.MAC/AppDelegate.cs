using AppKit;
using Foundation;
using MvvmCross.Platforms.Mac.Core;

namespace MessagePublisher.MAC
{
    [Register("AppDelegate")]
    public class AppDelegate : MvxApplicationDelegate<Setup, Core.App>
    {
        public AppDelegate() 
        {
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            MvxMacSetupSingleton.EnsureSingletonAvailable(this, MainWindow).EnsureInitialized();
            RunAppStart();
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }
    }
}
