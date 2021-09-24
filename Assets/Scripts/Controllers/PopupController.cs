using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupController : MonoBehaviour
{
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject tinyBackground;

    private float fadeTime = 0.5f;

    private void Start()
    {
        int index = Random.Range(0, 7);
        tinyBackground.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Backgrounds/BG{index}");

        background.GetComponent<Image>().DOFade(0.4f, fadeTime);
    }

    public void ResumeButton()
    {
        Time.timeScale = 1;
        background.GetComponent<Image>().DOFade(0.1f, fadeTime);
        tinyBackground.SetActive(false);
        Destroy(gameObject, fadeTime);
    }

    public void RestartButton()
    {
        SceneController.LoadGameScreen();
    }
}