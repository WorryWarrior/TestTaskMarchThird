using System;
using Content.UI.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.UI.MainScreen
{
    public class UserProfileWindowController : MonoBehaviour, IFavoriteSelector
    {
        [SerializeField] private Image userProfileImage = null;
        [SerializeField] private Toggle userFavoriteToggle = null;
        [SerializeField] private TextMeshProUGUI userProfileNameText = null;
        [SerializeField] private TextMeshProUGUI userProfileGenderText = null;
        [SerializeField] private TextMeshProUGUI userProfileEmailText = null;
        [SerializeField] private TextMeshProUGUI userProfileIpText = null;
        [SerializeField] private Button backButton = null;

        private int _referencedProfileID;

        public event Action OnBackButtonPressed;
        public event OnFavoriteTogglePressedEventHandler OnFavoriteTogglePressed;

        public void Initialize()
        {
            OnBackButtonPressed += Hide;
            backButton.onClick.AddListener(() => OnBackButtonPressed?.Invoke());
            userFavoriteToggle.onValueChanged.AddListener(it =>
                OnFavoriteTogglePressed?.Invoke(it, _referencedProfileID));
        }

        public void SetProfileWindowData(
            int referencedProfileID,
            Sprite userProfileImageSprite,
            string userProfileName,
            string userProfileGender,
            string userProfileEmail,
            string userProfileIp
        )
        {
            _referencedProfileID = referencedProfileID;
            userProfileImage.sprite = userProfileImageSprite;
            userProfileNameText.text = userProfileName;
            userProfileGenderText.text = userProfileGender;
            userProfileEmailText.text = userProfileEmail;
            userProfileIpText.text = userProfileIp;
        }

        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);

        public void ChangeFavoriteStatus(bool value)
        {
            userFavoriteToggle.isOn = value;
        }
    }
}