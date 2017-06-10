using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YouTubeAdminXamarinEvents.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Menu : ContentPage
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void YouTube_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new XamarinForms.Views.YoutubeViewPage());
        }

        private void Azure_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new YouTubeAdminXamarinEvents.Views.AzureViewPage());
        }
    }
}
