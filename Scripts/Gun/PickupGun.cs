using UnityEngine;

public class GunPickupThrow : MonoBehaviour
{
    [Header("References")]
    public GunController gunController;
    public Rigidbody rb;
    public Collider gunCollider;

    [Header("Player References")]
    public Transform fpsCam;
    public Transform gunHolder;

    [Header("Input")]
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.Mouse1; // Right Mouse Button

    [Header("Pickup Settings")]
    public float pickupRange = 3f;

    [Header("Throw Settings")]
    public float throwForce = 7f;
    public float throwUpwardForce = 2f;
    public float settleDelay = 0.6f;

    private bool isEquipped;
    private bool isSettling;

    private static GunPickupThrow currentGun;

    void Start()
    {
        // Initial state: gun stays in world, no falling
        rb.isKinematic = true;
        rb.useGravity = false;
        gunController.enabled = false;
    }

    void Update()
    {
        if (!isEquipped)
            TryPickup();
        else
            TryDrop();
    }

    // -------------------- PICKUP --------------------
    void TryPickup()
    {
        if (currentGun != null) return;

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.position, fpsCam.forward, out hit, pickupRange))
        {
            if (hit.transform == transform && Input.GetKeyDown(pickupKey))
            {
                PickUp();
            }
        }
    }

    void PickUp()
    {
        isEquipped = true;
        currentGun = this;

        // Parent to gun holder
        transform.SetParent(gunHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // Disable physics
        rb.isKinematic = true;
        rb.useGravity = false;
        gunCollider.enabled = false;

        gunController.enabled = true;
    }

    // -------------------- DROP / THROW --------------------
    void TryDrop()
    {
        if (Input.GetKeyDown(dropKey))
        {
            ThrowGun();
        }
    }

    void ThrowGun()
    {
        isEquipped = false;
        currentGun = null;

        transform.SetParent(null);

        // Enable physics
        rb.isKinematic = false;
        rb.useGravity = true;
        gunCollider.enabled = true;

        // Clear old velocity
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 🔥 THROW TOWARD MOUSE (CAMERA FORWARD)
        Vector3 throwDirection = fpsCam.forward;

        rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        rb.AddForce(Vector3.up * throwUpwardForce, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * 6f, ForceMode.Impulse);

        gunController.enabled = false;

        if (!isSettling)
            StartCoroutine(SettleGun());
    }

    // -------------------- STOP PHYSICS AFTER LANDING --------------------
    System.Collections.IEnumerator SettleGun()
    {
        isSettling = true;

        yield return new WaitForSeconds(settleDelay);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.useGravity = false;

        isSettling = false;
    }
}



