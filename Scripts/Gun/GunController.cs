using UnityEngine;
using TMPro;
using System.Collections;

public class GunController : MonoBehaviour
{
    [Header("Gun Stats")]
    public int magazineSize = 30;
    public float fireRate = 0.1f;
    public float reloadTime = 1.5f;

    [Header("Projectile")]
    public GameObject bulletPrefab;
    public Transform attackPoint;
    public float bulletSpeed = 50f;

    [Header("Muzzle")]
    public GameObject muzzlePrefab;
    public Transform muzzlePosition;

    [Header("Hit Effect")]
    public GameObject hitEffectPrefab;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip gunShotClip;
    public Vector2 pitchRange = new Vector2(0.9f, 1.1f);

    [Header("Recoil")]
    public float recoilBack = 0.08f;
    public float recoilReturnSpeed = 12f;

    [Header("Reload Spin")]
    public float reloadSpinAngle = 360f;

    [Header("UI")]
    public TextMeshProUGUI ammoText;
    public Camera fpsCam;

    private int bulletsLeft;
    private bool readyToShoot = true;
    private bool reloading;

    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;

    void Start()
    {
        bulletsLeft = magazineSize;
        originalLocalPos = transform.localPosition;
        originalLocalRot = transform.localRotation;
        UpdateAmmoUI();
    }

    void Update()
    {
        HandleInput();
        RecoilReturn();
        UpdateAmmoUI();
    }

    void HandleInput()
    {
        if (Input.GetMouseButton(0) && readyToShoot && !reloading)
        {
            if (bulletsLeft > 0)
                Shoot();
            else
                StartCoroutine(Reload());
        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
        readyToShoot = false;
        bulletsLeft--;

        // 🔥 MUZZLE FLASH
        if (muzzlePrefab && muzzlePosition)
        {
            GameObject flash = Instantiate(
                muzzlePrefab,
                muzzlePosition.position,
                muzzlePosition.rotation,
                muzzlePosition
            );
            Destroy(flash, 0.5f);
        }

        // 🔫 BULLET (ROTATED 90 DEGREE FIX)
        Vector3 shootDirection = fpsCam.transform.forward;

        GameObject bullet = Instantiate(
            bulletPrefab,
            attackPoint.position,
            Quaternion.LookRotation(shootDirection)
        );

        // ✅ FORCE VERTICAL ALIGNMENT
        bullet.transform.Rotate(90f, 0f, 0f);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = shootDirection * bulletSpeed;

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript)
            bulletScript.hitEffect = hitEffectPrefab;

        // 🔊 AUDIO
        if (audioSource && gunShotClip)
        {
            audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
            audioSource.PlayOneShot(gunShotClip);
        }

        // 🔁 RECOIL
        transform.localPosition -= Vector3.forward * recoilBack;

        Invoke(nameof(ResetShot), fireRate);
    }

    void ResetShot()
    {
        readyToShoot = true;
    }

    IEnumerator Reload()
    {
        reloading = true;

        float elapsed = 0f;
        Quaternion startRot = transform.localRotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, 0, reloadSpinAngle);

        while (elapsed < reloadTime)
        {
            transform.localRotation = Quaternion.Slerp(
                startRot,
                endRot,
                elapsed / reloadTime
            );
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = originalLocalRot;
        bulletsLeft = magazineSize;
        reloading = false;
    }

    void RecoilReturn()
    {
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            originalLocalPos,
            Time.deltaTime * recoilReturnSpeed
        );
    }

    void UpdateAmmoUI()
    {
        ammoText.text = bulletsLeft + " / " + magazineSize;
    }
}
