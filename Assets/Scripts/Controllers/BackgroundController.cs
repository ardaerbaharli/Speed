using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] private   GameObject fallingSymbol;
    private readonly string[] Suits = new string[] { "Spade", "Heart", "Club", "Diamond" };
    private int interval;
    void Start()
    {
        SetBackground();
        interval = GetInterval();
    }

    void FixedUpdate()
    {
        if (Time.frameCount % interval == 0)
        {
            interval = GetInterval();
            var obj = Instantiate(fallingSymbol, transform);
            int index = Random.Range(0, 4);
            obj.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Suits/{Suits[index]}");
            obj.name = Suits[index];
        }
    }

    private int GetInterval()
    {
        return Random.Range(200, 250);
    }

    private void SetBackground()
    {
        int index = Random.Range(0, 7);
        gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Backgrounds/BG{index}");
    }
}
