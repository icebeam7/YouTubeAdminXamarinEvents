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

namespace XamarinForms.ViewModels
{
    public class YoutubeViewModel : INotifyPropertyChanged
    {

        // use this link to get an api_key :   https://console.developers.google.com/apis/api/youtube/
        private const string ApiKey = "AIzaSyB0n0MjQMjjRnLGHa5w4kE-XJWddzA0hKU";

        // doc : https://developers.google.com/apis-explorer/#p/youtube/v3/youtube.videos.list 
        private string apiUrlForChannel = "https://www.googleapis.com/youtube/v3/search?part=snippet&maxResults=50&channelId="
            + "UCaswGLsFFj_ayngaVOTf7Hw"
            //+ "Your_ChannelId"
            + "&order=date&type=video&key="
            + ApiKey;

        // doc : https://developers.google.com/apis-explorer/#p/youtube/v3/youtube.playlistItems.list
        private string apiUrlForPlaylist = "https://www.googleapis.com/youtube/v3/playlistItems?part=contentDetails&maxResults=50&playlistId="
            + "PL8paXyQ9MxDcBLq1Kjo_CsyuxsFNunYwM"
            //+ "Your_PlaylistId"
            + "&key="
            + ApiKey;

        // doc : https://developers.google.com/apis-explorer/#p/youtube/v3/youtube.search.list
        private string apiUrlForVideosDetails = "https://www.googleapis.com/youtube/v3/videos?part=snippet,statistics&id="
            + "{0}"
            //+ "0ce22qhacyo,j8GU5hG-34I,_0aQJHoI1e8"
            //+ "Your_Videos_Id"
            + "&key="
            + ApiKey;

        private List<YoutubeItem> _youtubeItems;

        public List<YoutubeItem> YoutubeItems
        {
            get { return _youtubeItems; }
            set
            {
                _youtubeItems = value;
                OnPropertyChanged();
            }
        }

        public YoutubeViewModel()
        {
            InitDataAsync();
        }

        public async Task InitDataAsync()
        {

            var videoIds = await GetVideoIdsFromChannelAsync();

            //var videoIds = await GetVideoIdsFromPlaylistAsync();
        }

        private async Task<List<string>> GetVideoIdsFromChannelAsync()
        {
            var httpClient = new HttpClient();

            var json = await httpClient.GetStringAsync(apiUrlForChannel);

            var videoIds = new List<string>();

            try
            {
                JObject response = JsonConvert.DeserializeObject<dynamic>(json);

                var items = response.Value<JArray>("items");

                foreach (var item in items)
                {
                    videoIds.Add(item.Value<JObject>("id")?.Value<string>("videoId"));
                }

                YoutubeItems = await GetVideosDetailsAsync(videoIds);
            }
            catch (Exception exception)
            {
            }

            return videoIds;
        }

        private async Task<List<string>> GetVideoIdsFromPlaylistAsync()
        {

            var httpClient = new HttpClient();

            var json = await httpClient.GetStringAsync(apiUrlForPlaylist);

            var videoIds = new List<string>();

            try
            {
                JObject response = JsonConvert.DeserializeObject<dynamic>(json);

                var items = response.Value<JArray>("items");

                foreach (var item in items)
                {
                    videoIds.Add(item.Value<JObject>("contentDetails")?.Value<string>("videoId"));
                };

                YoutubeItems = await GetVideosDetailsAsync(videoIds);
            }
            catch (Exception exception)
            {
            }

            return videoIds;
        }

        private async Task<List<YoutubeItem>> GetVideosDetailsAsync(List<string> videoIds)
        {
            var videoIdsString = "";
            foreach (var s in videoIds)
            {
                videoIdsString += s + ",";
            }

            var httpClient = new HttpClient();

            var json = await httpClient.GetStringAsync(string.Format(apiUrlForVideosDetails, videoIdsString));

            var youtubeItems = new List<YoutubeItem>();

            try
            {
                JObject response = JsonConvert.DeserializeObject<dynamic>(json);

                var items = response.Value<JArray>("items");

                int count = 0;

                foreach (var item in items)
                {
                    count++;
                    var snippet = item.Value<JObject>("snippet");
                    var statistics = item.Value<JObject>("statistics");

                    var ytItem = new YoutubeItem();

                    ytItem.Title = snippet.Value<string>("title");
                    ytItem.Description = snippet.Value<string>("description");
                    ytItem.ChannelTitle = snippet.Value<string>("channelTitle");
                    ytItem.PublishedAt = snippet.Value<DateTime>("publishedAt");
                    ytItem.VideoId = item?.Value<string>("id");
                    ytItem.DefaultThumbnailUrl = snippet?.Value<JObject>("thumbnails")?.Value<JObject>("default")?.Value<string>("url");
                    ytItem.MediumThumbnailUrl = snippet?.Value<JObject>("thumbnails")?.Value<JObject>("medium")?.Value<string>("url");
                    ytItem.HighThumbnailUrl = snippet?.Value<JObject>("thumbnails")?.Value<JObject>("high")?.Value<string>("url");
                    ytItem.StandardThumbnailUrl = snippet?.Value<JObject>("thumbnails")?.Value<JObject>("standard")?.Value<string>("url");
                    ytItem.MaxResThumbnailUrl = snippet?.Value<JObject>("thumbnails")?.Value<JObject>("maxres")?.Value<string>("url");

                    ytItem.ViewCount = statistics?.Value<int>("viewCount");
                    ytItem.LikeCount = statistics?.Value<int>("likeCount");
                    ytItem.DislikeCount = statistics?.Value<int>("dislikeCount");
                    ytItem.FavoriteCount = statistics?.Value<int>("favoriteCount");
                    ytItem.CommentCount = statistics?.Value<int>("commentCount");

                    ytItem.Tags = (snippet?.Value<JArray>("tags")?.Count > 0) ? (from tag in snippet?.Value<JArray>("tags") select tag.ToString())?.ToList() : new List<string> { "" };

                    youtubeItems.Add(ytItem);
                }

                return youtubeItems.OrderByDescending(x => x.PublishedAt).ToList();
            }
            catch (Exception exception)
            {
                return youtubeItems;
            }
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