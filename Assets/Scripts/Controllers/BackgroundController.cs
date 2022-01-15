using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class BackgroundController : MonoBehaviour
    {
        [SerializeField] private GameObject fallingSymbol;
        private readonly string[] suits = {"Spade", "Heart", "Club", "Diamond"};
        private int interval;

        void Start()
        {
            SetBackground();
            interval = RandomInterval();
        }

        private void FixedUpdate()
        {
            if (Time.frameCount % interval != 0) return;

            interval = RandomInterval();
            CreateRandomSymbol();
        }

        private void CreateRandomSymbol()
        {
            var obj = Instantiate(fallingSymbol, transform);
            int index = Random.Range(0, 4);
            obj.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Suits/{suits[index]}");
            obj.name = suits[index];
        }

        private int RandomInterval()
        {
            return Random.Range(200, 250);
        }

        private void SetBackground()
        {
            int index = Random.Range(0, 7);
            gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Backgrounds/BG{index}");
        }
    }
}