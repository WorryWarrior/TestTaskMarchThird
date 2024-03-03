using System;
using System.Collections.Generic;
using Content.Infrastructure.Services.PersistentData;
using TMPro;
using UnityEngine;

namespace Content.UI.MainScreen
{
    public delegate void OnProfileWindowOpenRequestedEventHandler(UserProfileWindowController windowInstance, int profileID);
    public delegate void OnProfileWindowInitializeRequestedEventHandler(UserProfileWindowController windowInstance);
    public delegate void OnFavoriteTogglePressedEventHandler(bool toggleState, int referencedProfileID);

    public class HudController : MonoBehaviour
    {
        [SerializeField] private FooterToggleController leftFooterToggle = null;
        [SerializeField] private FooterToggleController rightFooterToggle = null;
        [SerializeField] private UserProfileWindowController userProfileWindowController = null;
        [SerializeField] private TextMeshProUGUI headerText = null;
        [SerializeField] private GameObject fullScrollView = null;
        [SerializeField] private GameObject favoriteScrollView = null;

        public RectTransform userInfoBoxContainer;
        public RectTransform userInfoBoxContainerFavorite;

        private IPersistentDataService _persistentDataService;

        private readonly List<UserInfoBoxController> _userInfoBoxes = new();

        public event Action OnFavoriteContainerToggleSelected;
        public event OnProfileWindowInitializeRequestedEventHandler OnProfileWindowInitializeRequested;
        public event OnProfileWindowOpenRequestedEventHandler OnProfileWindowOpenRequested;

        public void Construct(
            IPersistentDataService persistentDataService
        )
        {
            _persistentDataService = persistentDataService;
        }

        public void Initialize()
        {
            InitializeFooterToggles();

            userProfileWindowController.OnBackButtonPressed += ToggleScrollViews;
            OnProfileWindowInitializeRequested?.Invoke(userProfileWindowController);
            userProfileWindowController.Initialize();

            favoriteScrollView.gameObject.SetActive(false);
            userProfileWindowController.gameObject.SetActive(false);
        }

        public void RegisterUserInfoBox(UserInfoBoxController userInfoBoxController) =>
            _userInfoBoxes.Add(userInfoBoxController);

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
                RefreshFullTab();
                fullScrollView.SetActive(true);
                favoriteScrollView.SetActive(false);
            }
            else
            {
                RefreshFavoriteTab();
                fullScrollView.SetActive(false);
                favoriteScrollView.SetActive(true);
            }
        }

        private void DisableScrollViews()
        {
            fullScrollView.SetActive(false);
            favoriteScrollView.SetActive(false);
        }

        private void RefreshFullTab()
        {
            for (int i = 0; i < _userInfoBoxes.Count; i++)
            {
                _userInfoBoxes[i]
                    .ChangeFavoriteStatus(
                        _persistentDataService.UserFavorite.FavoriteIDs.Contains(_userInfoBoxes[i].Index));
            }
        }

        private void RefreshFavoriteTab()
        {
            for (int i = 0; i < userInfoBoxContainerFavorite.childCount; i++)
            {
                Destroy(userInfoBoxContainerFavorite.GetChild(i).gameObject);
            }

            OnFavoriteContainerToggleSelected?.Invoke();
        }
    }
}