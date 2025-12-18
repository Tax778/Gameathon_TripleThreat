using UnityEngine;

public class Prison : MonoBehaviour
{
    [SerializeField] float y = 10f;
    [SerializeField] float t;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > t)
        {
            transform.Translate(0, y * Time.deltaTime, 0);
            if (transform.position.y > 44f)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
