using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] GameObject fallingSymbol;
    private readonly string[] Suits = new string[] { "Spade", "Heart", "Club", "Diamond" };
    private int interval;
    void Start()
    {
        interval = GetInterval();
    }

    void Update()
    {
        if (Time.frameCount % interval == 0)
        {
            interval = GetInterval();
            var obj = Instantiate(fallingSymbol, transform);
            int index = Random.Range(0, 4);
            obj.GetComponent<Image>().sprite = Resources.Load<Sprite>(Suits[index]);
            obj.name = Suits[index];
        }
    }

    private int GetInterval()
    {
        return Random.Range(200, 250);
    }
}
