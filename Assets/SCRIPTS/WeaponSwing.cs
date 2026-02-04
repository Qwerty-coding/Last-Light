using UnityEngine;

public class WeaponSwing : MonoBehaviour
{
    public float swingAngle = 50f;
    public float swingSpeed = 20f;
    public float chopRange = 2f;
    public float damageToZombie = 25f; // New variable for zombie damage

    private float currentAngle = 0f;
    private bool swingingForward = false;
    private bool swingingBack = false;

    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !swingingForward && !swingingBack)
        {
            swingingForward = true;
            currentAngle = 0f;
        }

        // Swing forward
        if (swingingForward)
        {
            float step = swingSpeed * Time.deltaTime;
            transform.Rotate(0,step,0);
            currentAngle += step;

            // Try chopping at middle of swing
            if (currentAngle >= swingAngle * 0.5f && currentAngle < swingAngle * 0.6f)
            {
                TryChop();
            }

            if (currentAngle >= swingAngle)
            {
                swingingForward = false;
                swingingBack = true;
            }
        }
        // Swing back
        else if (swingingBack)
        {
            float step = swingSpeed * Time.deltaTime;
            transform.Rotate(0,-step,0);
            currentAngle -= step;

            if (currentAngle <= 0f)
            {
                swingingBack = false;
                transform.localRotation = startRotation;
            }
        }
    }

    void TryChop()
    {
        RaycastHit hit;

        // Shoots ray from camera center
        if (Physics.Raycast(
            Camera.main.transform.position,
            Camera.main.transform.forward,
            out hit,
            chopRange))
        {
            // --- 1. EXISTING TREE LOGIC ---
            TreeInteractable tree = hit.collider.GetComponentInParent<TreeInteractable>();

            if (tree != null)
            {
                tree.Chop();
            }

            // --- 2. NEW ZOMBIE LOGIC ---
            // Look for the "Zombie1" component you provided
            Zombie1 zombie = hit.collider.GetComponent<Zombie1>();

            if (zombie != null)
            {
                zombie.TakeDamage(damageToZombie);
            }
            else 
            {
                BossZombie boss = hit.collider.GetComponent<BossZombie>();
                if (boss != null)
                {
                    boss.TakeDamage(damageToZombie);
                }
            }
        }
    }
}