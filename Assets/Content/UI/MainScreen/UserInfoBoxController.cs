using System;
using Content.UI.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.UI.MainScreen
{
    public class UserInfoBoxController : MonoBehaviour, IFavoriteSelector
    {
        [SerializeField] private Image parentImage = null;
        [SerializeField] private Toggle favoriteToggle = null;
        [SerializeField] private Image previewImage = null;
        [SerializeField] private TextMeshProUGUI nameText = null;
        [SerializeField] private TextMeshProUGUI emailText = null;
        [SerializeField] private TextMeshProUGUI ipText = null;
        [SerializeField] private Button openProfileButton = null;

        public int Index { get; private set; }

        public event Action<bool> OnFavoriteTogglePressed;
        public event Action OnOpenProfileButtonPressed;

        public void Initialize(
            int index,
            Sprite previewImageSprite,
            string userName,
            string userEmail,
            string userIp)
        {
            Index = index;
            parentImage.enabled = index % 2 == 0;
            previewImage.sprite = previewImageSprite;
            nameText.text = userName;
            emailText.text = userEmail;
            ipText.text = userIp;

            openProfileButton.onClick.AddListener(() => OnOpenProfileButtonPressed?.Invoke());
            favoriteToggle.onValueChanged.AddListener(it => OnFavoriteTogglePressed?.Invoke(it));
        }

        public void ChangeFavoriteStatus(bool value)
        {
            favoriteToggle.isOn = value;
        }
    }
}