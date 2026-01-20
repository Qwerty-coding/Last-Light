using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI Reference")]
    public Image healthBarFill;

    [Header("Damage Effect")]
    public DamageEffect damageEffect;

    [Header("Game Manager Reference")]
    public GameManagerScript gameManager;

    // ---------------- REGENERATION ----------------
    [Header("Health Regeneration")]
    public bool enableRegen = true;
    [Tooltip("Health restored per second")]
    public float regenRate = 5f;
    [Tooltip("Seconds to wait after taking damage before regen starts")]
    public float regenDelay = 3f;
    [Tooltip("Minimum health required to allow regen")]
    public float minHealthForRegen = 1f;

    private float lastDamageTime;
    // ------------------------------------------------

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    void Update()
    {
        // TEMP TEST
        if (Input.GetKeyDown(KeyCode.N))
        {
            TakeDamage(10);
        }

        HandleRegeneration();
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        lastDamageTime = Time.time; // reset regen timer

        if (damageEffect != null)
        {
            damageEffect.ShowDamage();
        }

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void HandleRegeneration()
    {
        if (!enableRegen)
            return;

        if (currentHealth <= minHealthForRegen)
            return;

        if (currentHealth >= maxHealth)
            return;

        // Wait before regen starts
        if (Time.time < lastDamageTime + regenDelay)
            return;

        currentHealth += regenRate * Time.deltaTime;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        Debug.Log("Player has died!");
        if (gameManager != null)
        {
            gameManager.TriggerGameOver();
        }
    }
}