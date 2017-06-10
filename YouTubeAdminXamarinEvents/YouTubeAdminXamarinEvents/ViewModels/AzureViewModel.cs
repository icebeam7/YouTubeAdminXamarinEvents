using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XamarinForms.Models;
using YouTubeAdminXamarinEvents.Models;
using YouTubeAdminXamarinEvents.Services;

namespace YouTubeAdminXamarinEvents.ViewModels
{
    public class AzureViewModel : INotifyPropertyChanged
    {
        private List<Video> _youtubeItems;

        public List<Video> YoutubeItems
        {
            get { return _youtubeItems; }
            set
            {
                _youtubeItems = value;
                OnPropertyChanged();
            }
        }

        public AzureViewModel()
        {
            InitDataAsync();
        }

        public async Task InitDataAsync()
        {
            var service = App.AzureService;
            YoutubeItems = (await service.GetVideos()).ToList();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}