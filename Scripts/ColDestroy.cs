using UnityEngine;

public class ColDestroy : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Destroy(this.gameObject, 3f);
        }
    }
}
