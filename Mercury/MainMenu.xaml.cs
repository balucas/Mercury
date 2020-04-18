using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using API;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Mercury
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
