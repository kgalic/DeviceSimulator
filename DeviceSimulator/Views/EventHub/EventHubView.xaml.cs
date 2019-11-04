using MessagePublisher.Core;
using MvvmCross.Platforms.Uap.Views;
using MvvmCross.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DeviceSimulator.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [MvxViewFor(typeof(EventHubViewModel))]
    public sealed partial class EventHubView : MvxWindowsPage
    {
        public EventHubView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // if cache mode is 'Required' then View Model 
            // should be retrieved instead of creating a new one
            var cachedViewModel = ViewModel;
            if (cachedViewModel == null)
            {
                base.OnNavigatedTo(e);
            }
        }

        private void StatusTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var scrollableHeight = StatusScrollView.ScrollableHeight;
            if (scrollableHeight > 0)
            {
                StatusScrollView.ChangeView(StatusScrollView.HorizontalOffset, scrollableHeight, null);
            }
        }
    }
}
