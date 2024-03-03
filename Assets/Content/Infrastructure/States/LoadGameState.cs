using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Content.Data;
using Content.Infrastructure.Factories.Interfaces;
using Content.Infrastructure.SceneManagement;
using Content.Infrastructure.Services.PersistentData;
using Content.Infrastructure.Services.SaveLoad;
using Content.Infrastructure.States.Interfaces;
using Content.UI.MainScreen;
using UnityEngine;

namespace Content.Infrastructure.States
{
    public class LoadGameState : IState
    {
        private readonly IStateMachine _stateMachine;

        private readonly IUIFactory _uiFactory;
        private readonly ISceneLoader _sceneLoader;
        private readonly IPersistentDataService _persistentDataService;
        private readonly ISaveLoadService _saveLoadService;

        public LoadGameState(
            IStateMachine stateMachine,
            IUIFactory uiFactory,
            ISceneLoader sceneLoader,
            IPersistentDataService persistentDataService,
            ISaveLoadService saveLoadService)
        {
            _stateMachine = stateMachine;
            _uiFactory = uiFactory;
            _sceneLoader = sceneLoader;
            _persistentDataService = persistentDataService;
            _saveLoadService = saveLoadService;
        }

        public async void Enter()
        {
            await _sceneLoader.LoadScene(SceneName.Core, OnGameSceneLoaded);
        }

        public void Exit()
        {
        }

        private async void OnGameSceneLoaded(SceneName sceneName)
        {
            await ConstructUIRoot();
            await ConstructHUD();
        }

        private async Task ConstructUIRoot() => await _uiFactory.CreateUIRoot();

        private async Task ConstructHUD()
        {
            HudController hudController = await _uiFactory.CreateHud();

            hudController.OnFavoriteContainerToggleSelected +=
                async () => await ConstructFavoriteUserInfoBoxes(hudController);

            hudController.OnProfileWindowInitializeRequested += windowInstance =>
            {
                windowInstance.OnFavoriteTogglePressed += (state, id) =>
                {
                    if (state)
                        _persistentDataService.UserFavorite.FavoriteIDs.Add(id);
                    else
                        _persistentDataService.UserFavorite.FavoriteIDs.Remove(id);
                };
            };

            hudController.OnProfileWindowOpenRequested += (windowInstance, id) =>
            {
                UserData userData = _persistentDataService.UserConfig.Data[id];

                windowInstance.SetProfileWindowData(
                    id,
                    null,
                    string.Join(" ", userData.First_Name, userData.Last_Name),
                    userData.Gender,
                    userData.Email,
                    userData.Ip_Address
                );

                windowInstance.ChangeFavoriteStatus(_persistentDataService.UserFavorite.FavoriteIDs.Contains(id));
            };

            hudController.Initialize();
            await ConstructAllUserInfoBoxes(hudController);
        }

        private async Task ConstructAllUserInfoBoxes(HudController hudController)
        {
            for (int i = 0; i < _persistentDataService.UserConfig.Data.Count; i++)
            {
                UserInfoBoxController infoBoxController = await ConstructInfoBox(hudController, i, false);

                infoBoxController.ChangeFavoriteStatus(_persistentDataService.UserFavorite.FavoriteIDs.Contains(i));
                hudController.RegisterUserInfoBox(infoBoxController);
            }
        }

        private async Task ConstructFavoriteUserInfoBoxes(HudController hudController)
        {
            IOrderedEnumerable<int> favoriteIDsAscending =
                _persistentDataService.UserFavorite.FavoriteIDs.AsEnumerable().ToList().OrderBy(it => it);

            foreach (int favoriteID in favoriteIDsAscending)
            {
                UserInfoBoxController infoBoxController = await ConstructInfoBox(hudController, favoriteID, true);
                infoBoxController.ChangeFavoriteStatus(true);
            }
        }

        private async Task<UserInfoBoxController> ConstructInfoBox(HudController hudController, int id, bool asFavorite)
        {
            UserData userData = _persistentDataService.UserConfig.Data[id];
            Sprite userDataPicture = _persistentDataService.UserPictures.UserPictures[
                id % _persistentDataService.UserPictures.UserPictures.Count];

            UserInfoBoxController userInfoBoxController = await _uiFactory.CreateUserInfoBox(hudController, asFavorite);
            userInfoBoxController.Initialize(id,
                userDataPicture,
                string.Join(" ", userData.First_Name, userData.Last_Name),
                userData.Email,
                userData.Ip_Address);
            userInfoBoxController.ChangeFavoriteStatus(true);

            userInfoBoxController.OnFavoriteTogglePressed += status =>
            {
                if (status)
                    _persistentDataService.UserFavorite.FavoriteIDs.Add(userInfoBoxController.Index);
                else
                    _persistentDataService.UserFavorite.FavoriteIDs.Remove(userInfoBoxController.Index);

                _saveLoadService.SaveUserFavorites();
            };

            userInfoBoxController.OnOpenProfileButtonPressed +=
                () => hudController.OpenUserProfile(userInfoBoxController.Index);

            return userInfoBoxController;
        }
    }
}