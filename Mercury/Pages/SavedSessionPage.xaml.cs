using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using API;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Mercury.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 		

    public sealed partial class SavedSessionPage : Page
    {
	    public List<SavedSession> SavedSessions;

        public SavedSessionPage()
        {
            this.InitializeComponent();
            SavedSessions = Session.RetrieveSavedSessions(Windows.Storage.ApplicationData.Current.LocalFolder.Path);
        }

        private void Button_Back(object sender, RoutedEventArgs e)
        {
	        Frame.GoBack();
        }

        private void View_Stats(object sender, RoutedEventArgs e)
        {
            var items = SessionList.SelectedItems;
            var selected = new List<SavedSession>();
            foreach(var s in items)
            {
                selected.Add(s as SavedSession);
            }

            //Navigate to stats page and pass selected sessions
            Frame.Navigate(typeof(StatisticsPage), selected);
        }
    }
}
