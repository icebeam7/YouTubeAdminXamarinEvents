using Xamarin.Forms;
using YouTubeAdminXamarinEvents.Services;

namespace YouTubeAdminXamarinEvents
{
    public partial class App : Application
    {
        public static AzureDataService AzureService = new AzureDataService();

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
