using UnityEngine;

namespace Objects
{
    public class FallingSymbol : MonoBehaviour
    {
        public float speed;
        void Start()
        {
            transform.position = new Vector3(Random.Range(0, Screen.width), Screen.height + Screen.height / 20, 0);
            speed = Random.Range(0.5f, 1.2f);
        }

        void Update()
        {
            var pos = transform.position;
            var newPos = new Vector3(pos.x, pos.y - speed, 0);
            transform.position = newPos;
        }
    }
}
