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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Mercury
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 		

    public sealed partial class SavedSessionPage : Page
    {
        public ObservableCollection<AudienceFrame> FaceData { get; }

        public SavedSessionPage()
        {
            this.InitializeComponent();
            FaceData = new ObservableCollection<AudienceFrame>();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //this.DataContext = this;

            //Random rand = new Random();

            //for (int i = 0; i <= 50; i++)
            //{
            //    FaceData.Add(new AudienceFrame("")
            //    {
            //        Anger = rand.NextDouble(),
            //        Contempt = rand.NextDouble(),
            //        Disgust = rand.NextDouble(),
            //        Fear = rand.NextDouble(),
            //        Happiness = rand.NextDouble(),
            //        Neutral = rand.NextDouble(),
            //        Sadness = rand.NextDouble(),
            //        Surprise = rand.NextDouble(),
            //        Time = ""
            //    });
            //}

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
