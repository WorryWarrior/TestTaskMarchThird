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
            await _uiFactory.WarmUp();

            OnGameSceneLoaded(SceneName.Core);
            //await _sceneLoader.LoadScene(SceneName.Core, OnGameSceneLoaded);
        }

        public void Exit()
        {
        }

        private async void OnGameSceneLoaded(SceneName sceneName)
        {
            //await ConstructUIRoot();
            await ConstructHUD();
        }

        private async Task ConstructUIRoot() => await _uiFactory.CreateUIRoot();

        private Task ConstructHUD()
        {
            HudController
                hudController = UnityEngine.Object.FindObjectOfType<HudController>(); //await _uiFactory.CreateHud();

            hudController.OnFavoriteContainerToggleSelected +=
                async () => await ConstructFavoriteUserInfoBoxes(hudController);

            hudController.OnUserInfoBoxCreated += userInfoBox =>
            {
                userInfoBox.OnFavoriteTogglePressed += status =>
                {
                    if (status)
                        _persistentDataService.UserFavorite.FavoriteIDs.Add(userInfoBox.Index);
                    else
                        _persistentDataService.UserFavorite.FavoriteIDs.Remove(userInfoBox.Index);

                    _saveLoadService.SaveUserFavorites();
                };

                userInfoBox.OnOpenProfileButtonPressed += () => hudController.OpenUserProfile(userInfoBox.Index);
                userInfoBox.ChangeFavoriteStatus(
                    _persistentDataService.UserFavorite.FavoriteIDs.Contains(userInfoBox.Index));
                userInfoBox.InitializeCallbacks();
            };

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
                Sprite userDataPicture = _persistentDataService.UserPictures.UserPictures[
                    id % _persistentDataService.UserPictures.UserPictures.Count];

                windowInstance.SetProfileWindowData(
                    id,
                    userDataPicture,
                    string.Join(" ", userData.First_Name, userData.Last_Name),
                    userData.Gender,
                    userData.Email,
                    userData.Ip_Address
                );

                windowInstance.ChangeFavoriteStatus(_persistentDataService.UserFavorite.FavoriteIDs.Contains(id));
            };

            hudController.Initialize();
            return Task.CompletedTask;
        }

        private async Task ConstructFavoriteUserInfoBoxes(HudController hudController)
        {
            IOrderedEnumerable<int> favoriteIDsAscending =
                _persistentDataService.UserFavorite.FavoriteIDs.AsEnumerable().ToList().OrderBy(it => it);

            int listIndex = 0;
            foreach (int favoriteID in favoriteIDsAscending)
            {
                UserInfoBoxController infoBoxController =
                    await ConstructInfoBox(hudController, favoriteID, listIndex, true);
                infoBoxController.ChangeFavoriteStatus(true);
                infoBoxController.InitializeCallbacks();
                listIndex++;
            }
        }

        private async Task<UserInfoBoxController> ConstructInfoBox(HudController hudController,
            int id, int orderInList, bool asFavorite)
        {
            UserData userData = _persistentDataService.UserConfig.Data[id];
            Sprite userDataPicture = _persistentDataService.UserPictures.UserPictures[
                id % _persistentDataService.UserPictures.UserPictures.Count];

            UserInfoBoxController userInfoBoxController = await _uiFactory.CreateUserInfoBox(hudController, asFavorite);
            userInfoBoxController.UpdateData(id,
                userDataPicture,
                string.Join(" ", userData.First_Name, userData.Last_Name),
                userData.Email,
                userData.Ip_Address,
                orderInList % 2 == 0);
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