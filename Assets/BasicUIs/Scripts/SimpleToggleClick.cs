using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GoldSprite.BasicUIs
{
    public class SimpleToggleClick : MonoBehaviour
    {
        public GameObject DropView;
        public bool isDroping;
        public bool scrollToTop = true;

        public UnityAction CGDroping;

        private void Start()
        {
            DropView.SetActive(isDroping);

            var btn = GetComponent<Button>();
            CGDroping = () =>
            {
                isDroping = !isDroping;
                DropView.SetActive(isDroping);
                if (isDroping && scrollToTop && DropView.TryGetComponent<ScrollRect>(out ScrollRect scroll))
                    ScrollToTop(scroll);
            };
            btn.onClick.AddListener(CGDroping);
        }

        private void Update()
        {
        }

        public void ScrollToTop(ScrollRect scrollRect)
        {
            if (scrollRect != null) scrollRect.normalizedPosition = new Vector2(0, 1);
        }
    }
}
