using UnityEngine;

namespace Objects
{
    public class FlyingSymbolKiller : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D col)
        {
            Destroy(col.gameObject);
        }
    }
}
