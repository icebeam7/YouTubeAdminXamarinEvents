using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using YouTubeAdminXamarinEvents.Models;

namespace YouTubeAdminXamarinEvents.Services
{
    public class AzureDataService
    {
        public MobileServiceClient MobileService { get; set; }

        private IMobileServiceSyncTable<Video> _table;
        const string dbPath = "YouTubeXamarinDBv5";
        private const string serviceUri = "http://youtubexamarinevents.azurewebsites.net";

        public AzureDataService()
        {
            createAzureClient();
        }

        void createAzureClient()
        {
            if (string.IsNullOrWhiteSpace(serviceUri))
                throw new Exception("Debes introducir la url de tu servicio Azure");
            MobileService = new MobileServiceClient(serviceUri);
            var store = new MobileServiceSQLiteStore(dbPath);
            store.DefineTable<Video>();
            MobileService.SyncContext.InitializeAsync(store);
            _table = MobileService.GetSyncTable<Video>();
        }

        public async Task<IEnumerable<Video>> GetVideos()
        {
            var empty = new Video[0];
            try
            {
                if (Plugin.Connectivity.CrossConnectivity.Current.IsConnected)
                    await SyncAsync();

                return await _table.ToEnumerableAsync();
            }
            catch (Exception ex)
            {
                return empty;
            }
        }

        public async Task<Video> GetVideo(string videoId)
        {
            var video = _table.Where(x => x.VideoId == videoId);

            if (video != null)
            {
                var list = await video.ToListAsync();
                return list.Count > 0 ? list[0] : null;;
            }
            else
                return null;
        }

        public async Task AddVideo(Video video)
        {
            await _table.InsertAsync(video);
            await SyncAsync();
        }

        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;
            try
            {
                await MobileService.SyncContext.PushAsync();

                await _table.PullAsync("allVideos", _table.CreateQuery());
            }
            catch (MobileServicePushFailedException pushEx)
            {
                if (pushEx.PushResult != null)
                    syncErrors = pushEx.PushResult.Errors;
            }
        }

        public async Task CleanData()
        {
            await _table.PurgeAsync("allVideos", _table.CreateQuery(), new System.Threading.CancellationToken());
        }
    }
}
