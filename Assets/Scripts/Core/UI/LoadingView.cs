using UnityEngine;
using UnityEngine.UI;

namespace SwordHero.Core.UI
{
    public class LoadingView : MonoBehaviour
    {
        [SerializeField] private Image _loadingBar;

        public void Show()
        {
            gameObject.SetActive(true);
            _loadingBar.fillAmount = 0f;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdateProgress(float progress)
        {
            _loadingBar.fillAmount = Mathf.Clamp01(progress);
        }
    }
}