using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Content.Data;
using Content.Infrastructure.AssetManagement;
using Content.Infrastructure.Factories.Interfaces;
using Content.Infrastructure.Services.Logging;
using Content.Infrastructure.Services.PersistentData;
using Content.Infrastructure.Services.SaveLoad;
using Content.Infrastructure.States.Interfaces;
using UnityEngine;
using UnityEngine.Networking;

namespace Content.Infrastructure.States
{
    public class LoadProgressState : IState
    {
        private const int UserProfilePictureCount = 5;
        private const string PlaceholderUserImageKey = "AST_PlaceholderUserImage";
        private const string UserProfilePictureSource = "https://picsum.photos/200";

        private readonly IStateMachine _stateMachine;
        private readonly ISaveLoadService _saveLoadService;
        private readonly IPersistentDataService _persistentDataService;
        private readonly IAssetProvider _assetProvider;
        private readonly ILoggingService _loggingService;

        public LoadProgressState(
            IStateMachine stateMachine,
            ISaveLoadService saveLoadService,
            IPersistentDataService persistentDataService,
            IAssetProvider assetProvider,
            ILoggingService loggingService)
        {
            _stateMachine = stateMachine;
            _saveLoadService = saveLoadService;
            _persistentDataService = persistentDataService;
            _assetProvider = assetProvider;
            _loggingService = loggingService;
        }

        public async void Enter()
        {
            await LoadOrDownloadUserPictures();
            await LoadUserConfig();
            LoadOrCreateFavoriteIndices();

            _stateMachine.Enter<LoadGameState>();
        }

        public void Exit()
        {
        }

        private async Task LoadUserConfig()
        {
            UserConfigData userConfigData = await _saveLoadService.LoadUserConfig();

            List<UserData> clampedUserData = userConfigData.Data.Take(150).ToList();
            _persistentDataService.UserConfig = new UserConfigData
            {
                Data = clampedUserData
            };

            _persistentDataService.UserConfig = userConfigData;
        }

        private async void LoadOrCreateFavoriteIndices()
        {
            _persistentDataService.UserFavorite =
                await _saveLoadService.LoadUserFavourite() ?? CreateNewFavoriteIndices();
        }

        private UserFavoriteData CreateNewFavoriteIndices() => new()
        {
            FavoriteIDs = new HashSet<int>()
        };

        private async Task LoadOrDownloadUserPictures()
        {
            _persistentDataService.UserPictures = await _saveLoadService.LoadUserPictures();

            if (_persistentDataService.UserPictures == null)
            {
                _persistentDataService.UserPictures = await DownloadUserPictures();
                _saveLoadService.SaveUserPictures();
            }
        }

        private async Task<UserPictureData> DownloadUserPictures()
        {
            List<Sprite> tempUserPictures = new();
            Sprite placeholderPicture = await _assetProvider.Load<Sprite>(PlaceholderUserImageKey);

            for (int i = 0; i < UserProfilePictureCount; i++)
            {
                Texture2D texture = await DownloadImageAsync(UserProfilePictureSource);

                if (texture != null)
                {
                    tempUserPictures.Add(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f)));
                }
                else
                {
                    tempUserPictures.Add(placeholderPicture);
                }
            }

            return new UserPictureData
            {
                UserPictures = tempUserPictures
            };
        }

        private async Task<Texture2D> DownloadImageAsync(string url)
        {
            TaskCompletionSource<Texture2D> tcs = new TaskCompletionSource<Texture2D>();

            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                UnityWebRequestAsyncOperation asyncOp = www.SendWebRequest();

                asyncOp.completed += _ =>
                {
                    if (www.result == UnityWebRequest.Result.ConnectionError ||
                        www.result == UnityWebRequest.Result.ProtocolError)
                    {
                        _loggingService.LogWarning("Error loading image: " + www.error);
                        tcs.SetResult(null);
                    }
                    else
                    {
                        Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                        tcs.SetResult(texture);
                    }
                };

                await tcs.Task;
            }

            return tcs.Task.Result;
        }
    }
}