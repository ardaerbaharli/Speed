using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Objects
{
    public class ToggleSwitch : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private bool _isOff = false;
        public bool isOff { get { return _isOff; } }

        [SerializeField] private RectTransform toggleIndicator;
        [SerializeField] private Image background;
        [SerializeField] private Color onColor;
        [SerializeField] private Color offColor;

        private float offX;
        private float onX;

        [SerializeField] private float tweenTime = 0.25f;

        public delegate void ValueChanged(bool value);
        public event ValueChanged valueChanged;

        private void Start()
        {
            onX = toggleIndicator.anchoredPosition.x;
            offX = onX - toggleIndicator.rect.width;
            background.color = isOff ? offColor : onColor;
            Debug.Log(isOff);
            Toggle(isOff);
        }

        private void Toggle(bool value)
        {
            if (value != isOff)
            {
                _isOff = value;
                ToggleColor(isOff);
                MoveIndicator(isOff);

                if (valueChanged != null)
                    valueChanged(isOff);
            }
        }

        private void ToggleColor(bool value)
        {
            if (value)
                background.DOColor(offColor, tweenTime);
            else
                background.DOColor(onColor, tweenTime);
        }

        private void MoveIndicator(bool value)
        {
            if (value)
                toggleIndicator.DOAnchorPosX(offX, tweenTime);
            else
                toggleIndicator.DOAnchorPosX(onX, tweenTime);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Toggle(!isOff);
        }
    }
}
