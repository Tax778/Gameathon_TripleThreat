using UnityEngine;

public class WeaponSlot : MonoBehaviour
{
    [Header("Weapons")]
    public GameObject gun1;
    public GameObject gun2;

    [Header("Input")]
    public KeyCode gun1Key = KeyCode.Alpha1;
    public KeyCode gun2Key = KeyCode.Alpha2;

    void Start()
    {
        // Ensure both guns start inactive
        if (gun1) gun1.SetActive(false);
        if (gun2) gun2.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(gun1Key))
        {
            ToggleGun(gun1, gun2);
        }

        if (Input.GetKeyDown(gun2Key))
        {
            ToggleGun(gun2, gun1);
        }
    }

    void ToggleGun(GameObject targetGun, GameObject otherGun)
    {
        if (targetGun == null) return;

        bool isActive = targetGun.activeSelf;

        // If target gun is active → deactivate it
        if (isActive)
        {
            targetGun.SetActive(false);
        }
        else
        {
            // Activate target gun and deactivate the other
            targetGun.SetActive(true);
            if (otherGun) otherGun.SetActive(false);
        }
    }
}
