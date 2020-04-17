using API;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Mercury
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 		

    public sealed partial class SavedSessionPage : Page
    {
        public List<SavedSession> SavedSessions { get; }

        public SavedSessionPage()
        {
            this.InitializeComponent();
            SavedSessions = Session.RetrieveSavedSessions(Windows.Storage.ApplicationData.Current.LocalFolder.Path);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //SessionList.SelectedItems;
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
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
