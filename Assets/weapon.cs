using System.Collections;
using UnityEngine;

public class weapon : MonoBehaviour
{
    public Camera playerCamera;

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

    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    public ShootingMode currentShootingMode;

    private void Awake()
    {
        burstBulletsLeft = bulletsPerBurst;
    }

    private void Update()
    {
        // Input handling
        if (currentShootingMode == ShootingMode.Auto)
        {
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else
        {
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        // Shooting logic
        if (readyToShoot && isShooting)
        {
            burstBulletsLeft = bulletsPerBurst;
            FireWeapon();
        }
    }

    private void FireWeapon()
    {
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

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    private Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
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
