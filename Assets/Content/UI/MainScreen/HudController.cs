using System;
using System.Collections.Generic;
using Content.Data;
using Content.Infrastructure.Services.PersistentData;
using PolyAndCode.UI;
using TMPro;
using UnityEngine;

namespace Content.UI.MainScreen
{
    public delegate void OnProfileWindowOpenRequestedEventHandler(UserProfileWindowController windowInstance,
        int profileID);

    public delegate void OnProfileWindowInitializeRequestedEventHandler(UserProfileWindowController windowInstance);

    public delegate void OnFavoriteTogglePressedEventHandler(bool toggleState, int referencedProfileID);

    public class HudController : MonoBehaviour, IRecyclableScrollRectDataSource
    {
        [SerializeField] private FooterToggleController leftFooterToggle = null;
        [SerializeField] private FooterToggleController rightFooterToggle = null;
        [SerializeField] private UserProfileWindowController userProfileWindowController = null;
        [SerializeField] private TextMeshProUGUI headerText = null;
        [SerializeField] private CanvasGroup fullScrollViewCanvasGroup = null;
        [SerializeField] private CanvasGroup favoriteScrollViewCanvasGroup = null;

        [SerializeField] private RecyclableScrollRect userInfoBoxScrollRect = null;

        public RectTransform userInfoBoxContainer;
        public RectTransform userInfoBoxContainerFavorite;

        private IPersistentDataService _persistentDataService;

        public event Action OnFavoriteContainerToggleSelected;
        public event OnProfileWindowInitializeRequestedEventHandler OnProfileWindowInitializeRequested;
        public event OnProfileWindowOpenRequestedEventHandler OnProfileWindowOpenRequested;
        public event Action<UserInfoBoxController> OnUserInfoBoxCreated;

        public void Construct(
            IPersistentDataService persistentDataService
        )
        {
            _persistentDataService = persistentDataService;
        }

        public void Initialize()
        {
            userInfoBoxScrollRect.Initialize(this);

            InitializeFooterToggles();
            Construct(DIContainer.Container.GetService<IPersistentDataService>());

            userProfileWindowController.OnBackButtonPressed += ToggleScrollViews;
            OnProfileWindowInitializeRequested?.Invoke(userProfileWindowController);
            userProfileWindowController.Initialize();

            ToggleFavoriteScrollView(false);
            userProfileWindowController.gameObject.SetActive(false);
        }

        public void OpenUserProfile(int profileID)
        {
            DisableScrollViews();
            OnProfileWindowOpenRequested?.Invoke(userProfileWindowController, profileID);
            userProfileWindowController.Show();
        }

        private void OnFooterToggleStateChanged(bool value)
        {
            headerText.text = $"| {GetHeaderTextValue()} |";
            ToggleScrollViews();
        }

        private string GetHeaderTextValue() => leftFooterToggle.toggle.isOn
            ? leftFooterToggle.ToggleName
            : rightFooterToggle.ToggleName;

        private void InitializeFooterToggles()
        {
            leftFooterToggle.Initialize();
            rightFooterToggle.Initialize();

            leftFooterToggle.toggle.onValueChanged.AddListener(OnFooterToggleStateChanged);
            rightFooterToggle.toggle.onValueChanged.AddListener(OnFooterToggleStateChanged);

            leftFooterToggle.toggle.isOn = true;
            leftFooterToggle.toggle.onValueChanged?.Invoke(true);
        }

        private void ToggleScrollViews()
        {
            if (leftFooterToggle.toggle.isOn)
            {
                ForceRepaintFullListFavoriteStatus();
                ToggleFullScrollView(true);
                ToggleFavoriteScrollView(false);
            }
            else
            {
                RefreshFavoriteTab();
                ToggleFullScrollView(false);
                ToggleFavoriteScrollView(true);
            }
        }

        private void DisableScrollViews()
        {
            ToggleFullScrollView(false);
            ToggleFavoriteScrollView(false);
        }

        private void ToggleFullScrollView(bool value)
        {
            fullScrollViewCanvasGroup.alpha = value ? 1f : 0f;
            fullScrollViewCanvasGroup.interactable = value;
            fullScrollViewCanvasGroup.blocksRaycasts = value;
        }

        private void ToggleFavoriteScrollView(bool value)
        {
            favoriteScrollViewCanvasGroup.alpha = value ? 1f : 0f;
            favoriteScrollViewCanvasGroup.interactable = value;
            favoriteScrollViewCanvasGroup.blocksRaycasts = value;
        }

        private void RefreshFavoriteTab()
        {
            for (int i = 0; i < userInfoBoxContainerFavorite.childCount; i++)
            {
                Destroy(userInfoBoxContainerFavorite.GetChild(i).gameObject);
            }

            OnFavoriteContainerToggleSelected?.Invoke();
        }

        private void ForceRepaintFullListFavoriteStatus()
        {
            UserInfoBoxController[] userInfoBoxes =
                userInfoBoxScrollRect.GetComponentsInChildren<UserInfoBoxController>();

            for (int i = 0; i < userInfoBoxes.Length; i++)
            {
                userInfoBoxes[i]
                    .ChangeFavoriteStatus(
                        _persistentDataService.UserFavorite.FavoriteIDs.Contains(userInfoBoxes[i].Index));
            }
        }

        public int GetItemCount() => _persistentDataService.UserConfig.Data.Count;

        public void SetCell(ICell cell, int index)
        {
            UserInfoBoxController item = (UserInfoBoxController)cell;

            UserData userData = _persistentDataService.UserConfig.Data[index];
            Sprite userDataPicture = _persistentDataService.UserPictures.UserPictures[
                index % _persistentDataService.UserPictures.UserPictures.Count];

            item.UpdateData(index,
                userDataPicture,
                string.Join(" ", userData.First_Name, userData.Last_Name),
                userData.Email,
                userData.Ip_Address,
                index % 2 == 0);

            OnUserInfoBoxCreated?.Invoke(item);
        }
    }
}