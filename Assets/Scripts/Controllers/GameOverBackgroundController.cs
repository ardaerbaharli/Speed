using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class GameOverBackgroundController : MonoBehaviour
    {
        [SerializeField] private GameObject background;
        
        void Start()
        {
            var gameObj = gameObject;
            gameObj.GetComponent<Image>().sprite = background.GetComponent<Image>().sprite;
            // StartCoroutine(FadeIn());
            iTween.FadeTo(gameObj, 0, 2f);
        }
    }
}