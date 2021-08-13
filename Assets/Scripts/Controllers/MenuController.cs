using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject bottomScore;
    [SerializeField] GameObject topScore;
    void Start()
    {
        bottomScore.transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetInt("BottomPlayerScore", 0).ToString();
        topScore.transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetInt("TopPlayerScore", 0).ToString();
    }


    public void StartGame()
    {
        SceneController.LoadGameScreen();
    }

    public void ResetScores()
    {
        PlayerPrefs.SetInt("TopPlayerScore", 0);
        PlayerPrefs.SetInt("BottomPlayerScore", 0);
        bottomScore.transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetInt("BottomPlayerScore", 0).ToString();
        topScore.transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetInt("TopPlayerScore", 0).ToString();
    }
}
