using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 50f;
    public float lifeTime = 3f;

    [Header("Spin (Visual)")]
    public float spinSpeed = 1500f; // degrees per second

    [Header("Impact")]
    public GameObject hitEffect;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // FPS-safe rigidbody settings
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.freezeRotation = true; // we rotate manually
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// Call this right after instantiating the bullet
    /// </summary>
    public void Fire(Vector3 direction)
    {
        direction.Normalize();

        // Face the direction of travel
        transform.rotation = Quaternion.LookRotation(direction);

        // Move forward
        rb.linearVelocity = direction * speed;
    }

    void Update()
    {
        // Visual bullet spin around forward axis (Z)
        transform.Rotate(Vector3.forward * spinSpeed * Time.deltaTime, Space.Self);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hitEffect)
        {
            ContactPoint contact = collision.contacts[0];
            Instantiate(
                hitEffect,
                contact.point,
                Quaternion.LookRotation(contact.normal)
            );
        }

        Destroy(gameObject);
    }
}
