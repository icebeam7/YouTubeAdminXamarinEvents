using Plugin.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinForms.Models;
using YouTubeAdminXamarinEvents.Models;
using YouTubeAdminXamarinEvents.Services;

namespace YouTubeAdminXamarinEvents.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VideoDetailPage : ContentPage
    {
        YoutubeItem youtubeItem;
        Video videoItem;
        AzureDataService service;
        string id;

        public VideoDetailPage(YoutubeItem yt)
        {
            InitializeComponent();
            youtubeItem = yt;
            this.BindingContext = youtubeItem;
            id = youtubeItem.VideoId;
        }

        public VideoDetailPage(Video yt)
        {
            InitializeComponent();
            videoItem = yt;
            this.BindingContext = videoItem;
            id = videoItem.VideoId;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            service = App.AzureService;
            var azureVideo = await service.GetVideo(id);

            if (azureVideo != null)
                this.ToolbarItems.RemoveAt(0);
        }

        private async void Add_Clicked(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sbTags = new StringBuilder();

                foreach (var item in youtubeItem.Tags)
                {
                    sbTags.Append($"{item}, ");
                }

                Video video = new Video()
                {
                    ChannelTitle = youtubeItem.ChannelTitle,
                    CommentCount = youtubeItem.CommentCount.HasValue ? youtubeItem.CommentCount.Value : 0,
                    DefaultThumbnailUrl = youtubeItem.DefaultThumbnailUrl,
                    Description = youtubeItem.Description,
                    DislikeCount = youtubeItem.DislikeCount.HasValue ? youtubeItem.DislikeCount.Value : 0,
                    FavoriteCount = youtubeItem.FavoriteCount.HasValue ? youtubeItem.FavoriteCount.Value : 0,
                    HighThumbnailUrl = youtubeItem.HighThumbnailUrl,
                    LikeCount = youtubeItem.LikeCount.HasValue ? youtubeItem.LikeCount.Value : 0,
                    MaxResThumbnailUrl = youtubeItem.MaxResThumbnailUrl,
                    MediumThumbnailUrl = youtubeItem.MediumThumbnailUrl,
                    PublishedAt = youtubeItem.PublishedAt,
                    StandardThumbnailUrl = youtubeItem.StandardThumbnailUrl,
                    Tags = sbTags.ToString(),
                    Title = youtubeItem.Title,
                    VideoId = youtubeItem.VideoId,
                    ViewCount = youtubeItem.ViewCount.HasValue ? youtubeItem.ViewCount.Value : 0
                };

                await service.AddVideo(video);

                await DisplayAlert("Success", "Your video details have been saved in Azure", "OK");
            }
            catch(Exception ex)
            {
                await DisplayAlert("Error", "Video details not saved. Error: " + ex.Message, "OK");
            }
        }

        private async void Watch_Clicked(object sender, EventArgs e)
        {
            await CrossShare.Current.OpenBrowser("https://www.youtube.com/watch?v=" + id);
        }
    }
}
