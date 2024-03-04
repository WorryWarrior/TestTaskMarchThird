using System;
using Content.Data;
using Content.UI.Interfaces;
using PolyAndCode.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.UI.MainScreen
{
    public class UserInfoBoxController : MonoBehaviour, IFavoriteSelector, ICell
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

        private bool _callbacksInitialized = false;

        /*public void Initialize(
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
        }*/

        public void UpdateData(int index,
            Sprite previewImageSprite,
            string userName,
            string userEmail,
            string userIp,
            bool useBackground)
        {
            Index = index;
            previewImage.sprite = previewImageSprite;
            nameText.text = userName;
            emailText.text = userEmail;
            ipText.text = userIp;
            parentImage.enabled = useBackground;
        }

        public void InitializeCallbacks()
        {
            if (_callbacksInitialized)
                return;

            openProfileButton.onClick.AddListener(() => OnOpenProfileButtonPressed?.Invoke());
            favoriteToggle.onValueChanged.AddListener(it => OnFavoriteTogglePressed?.Invoke(it));
            _callbacksInitialized = true;
        }

        public void ChangeFavoriteStatus(bool value)
        {
            favoriteToggle.isOn = value;
        }
    }
}