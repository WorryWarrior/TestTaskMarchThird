using System.Threading.Tasks;
using Content.Infrastructure.AssetManagement;
using Content.Infrastructure.Factories.Interfaces;
using Content.Infrastructure.Services.PersistentData;
using Content.Infrastructure.Services.SaveLoad;
using Content.UI.MainScreen;
using UnityEngine;

namespace Content.Infrastructure.Factories
{
    public class UIFactory : IUIFactory
    {
        private const string UIRootPrefabId = "PFB_UIRoot";
        private const string HudPrefabId = "PFB_HUD";
        private const string UserInfoBoxPrefabId = "PFB_UserInfoBox";

        private readonly IAssetProvider _assetProvider;

        private Canvas _uiRoot;

        public UIFactory(
            IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public async Task WarmUp()
        {
            await _assetProvider.Load<GameObject>(UIRootPrefabId);
            await _assetProvider.Load<GameObject>(HudPrefabId);
            await _assetProvider.Load<GameObject>(UserInfoBoxPrefabId);
        }

        public void CleanUp()
        {
            _assetProvider.Release(UIRootPrefabId);
            _assetProvider.Release(HudPrefabId);
            _assetProvider.Release(UserInfoBoxPrefabId);
        }

        public async Task CreateUIRoot()
        {
            GameObject prefab = await _assetProvider.Load<GameObject>(UIRootPrefabId);
            _uiRoot = Object.Instantiate(prefab).GetComponent<Canvas>();
        }

        public async Task<HudController> CreateHud()
        {
            GameObject prefab = await _assetProvider.Load<GameObject>(HudPrefabId);
            HudController hud = Object.Instantiate(prefab, _uiRoot.transform).GetComponent<HudController>();
            hud.Construct(DIContainer.Container.GetService<IPersistentDataService>());

            return hud;
        }

        public async Task<UserInfoBoxController> CreateUserInfoBox(HudController hudController, bool asFavorite)
        {
            GameObject prefab = await _assetProvider.Load<GameObject>(UserInfoBoxPrefabId);
            RectTransform hudControllerParent =
                asFavorite ? hudController.userInfoBoxContainerFavorite : hudController.userInfoBoxContainer;

            UserInfoBoxController hud = Object.Instantiate(prefab, hudControllerParent)
                .GetComponent<UserInfoBoxController>();

            return hud;
        }
    }
}