using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Mercury.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainMenu : Page
    {
	    public MainMenu()
        {
            this.InitializeComponent();
        }

        // Go to Recording page
        private void Begin_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RecordingPage));
        }

        // Go to Session List
        private void ViewPrev_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SavedSessionPage));
        }

        // Exit application
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
	        CoreApplication.Exit();
        }
    }
}
