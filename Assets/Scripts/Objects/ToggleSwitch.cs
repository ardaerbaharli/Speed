using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Speed.Objects
{
    public class ToggleSwitch : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private bool isOff;
        public bool IsOff => isOff;

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
            background.color = IsOff ? offColor : onColor;
            Debug.Log(IsOff);
            Toggle(IsOff);
        }

        private void Toggle(bool value)
        {
            if (value != IsOff)
            {
                isOff = value;
                ToggleColor(IsOff);
                MoveIndicator(IsOff);

                if (valueChanged != null)
                    valueChanged(IsOff);
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
            Toggle(!IsOff);
        }
    }
}
