using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.UI.MainScreen
{
    public class FooterToggleController : MonoBehaviour
    {
        [SerializeField] private string toggleName = string.Empty;
        [SerializeField] private Image toggleImage = null;
        [SerializeField] private TextMeshProUGUI toggleText = null;
        [SerializeField] private Sprite inactiveSprite = null;

        public Toggle toggle;

        public string ToggleName => toggleName;

        public void Initialize()
        {
            toggleText.text = toggleName;
        }

        public void ForceToggleInactive()
        {
            toggleImage.sprite = inactiveSprite;
        }
    }
}