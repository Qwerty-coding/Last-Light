using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class weapon : MonoBehaviour
{
    // public Camera playerCamera;
    public bool isShooting;
    public bool readyToShoot = true;
    private bool allowReset = true;

    public float shootingDelay = 0.2f;

    public int bulletsPerBurst = 3;
    private int burstBulletsLeft;

    public float spreadIntensity = 0.1f;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30f;
    public float bulletLifeTime = 3f;
    public GameObject muzzleEffect;
    private Animator animator;

    // Reload & Ammo
    public float reloadTime = 1.5f;
    public int magazineSize = 30;
    public int bulletsLeft;
    public bool isReloading;

    // --- Total Reserve Ammo ---
    public int totalReserveAmmo = 30; 

    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    public ShootingMode currentShootingMode;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        bulletsLeft = magazineSize;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. CRITICAL: Stop everything if the game is paused
        if (GameManagerBehaviour.isPaused) return;

        // 2. Play empty magazine sound
        if (bulletsLeft == 0 && isShooting)
        {
             // SoundManager.Instance.emptyManagizeSound1911.Play();
        }

        // 3. INPUT HANDLING
        if (EventSystem.current.IsPointerOverGameObject())
        {
            isShooting = false;
            return;
        }

        bool mouseClicked = currentShootingMode == ShootingMode.Auto ?
            Input.GetKey(KeyCode.Mouse0) :
            Input.GetKeyDown(KeyCode.Mouse0);

        isShooting = mouseClicked;

        // 4. RELOAD LOGIC
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !isReloading && totalReserveAmmo > 0)
        {
            Reload();
        }

        // AUTO RELOAD
        if (readyToShoot && isShooting && !isReloading && bulletsLeft <= 0 && totalReserveAmmo > 0)
        {
            Reload();
            return;
        }

        // 5. SHOOT LOGIC
        if (readyToShoot && isShooting && !isReloading && bulletsLeft > 0)
        {
            burstBulletsLeft = bulletsPerBurst;
            FireWeapon();
        }

        // 6. UI UPDATE
        if (Ammomanager.Instance.ammoDisplay != null)
        {
            if (bulletsLeft == 0 && totalReserveAmmo == 0)
            {
                // --- CHANGED: "No Ammo" with smaller font size ---
                Ammomanager.Instance.ammoDisplay.text = "<size=80%>No Ammo</size>"; 
                Ammomanager.Instance.ammoDisplay.color = Color.red; 
            }
            else
            {
                Ammomanager.Instance.ammoDisplay.text = $"{bulletsLeft} / {totalReserveAmmo}";
                Ammomanager.Instance.ammoDisplay.color = Color.white; 
            }
        }
    }

    private void FireWeapon()
    {
        muzzleEffect.GetComponent<ParticleSystem>().Play();

        animator.SetTrigger("RECOIL");

        SoundManager.Instance.shootingSound1911.Play();
        bulletsLeft--;
        readyToShoot = false;

        Vector3 direction = CalculateDirectionAndSpread().normalized;

        GameObject bullet = Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            Quaternion.LookRotation(direction)
        );

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(direction * bulletVelocity, ForceMode.Impulse);

        StartCoroutine(DestroyBulletAfterTime(bullet, bulletLifeTime));

        if (allowReset)
        {
            Invoke(nameof(ResetShot), shootingDelay);
            allowReset = false;
        }

        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke(nameof(FireWeapon), shootingDelay);
        }
    }

    private void Reload()
    {
        SoundManager.Instance.reloadingSound1911.Play();

        isReloading = true;
        Invoke(nameof(ReloadCompleted), reloadTime);
    }

    private void ReloadCompleted()
    {
        int ammoNeeded = magazineSize - bulletsLeft;
        int ammoToReload = Mathf.Min(ammoNeeded, totalReserveAmmo);

        bulletsLeft += ammoToReload;
        totalReserveAmmo -= ammoToReload;

        isReloading = false;
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    private Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint = Physics.Raycast(ray, out hit)
            ? hit.point
            : ray.GetPoint(100f);

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = Random.Range(-spreadIntensity, spreadIntensity);
        float y = Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x, y, 0);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}