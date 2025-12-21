using UnityEngine;
using UnityEngine.UI; // This line is REQUIRED to talk to the UI

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI Reference")]
    // Drag your 'HealthBarFill' image here in the Inspector
    public Image healthBarFill; 

[Header("Game Manager Reference")]
    public GameManagerScript gameManager; // <--- ADD THIS LINE

    void Start()
    {
        // Start with full health
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    void Update()
    {
        // --- TEMPORARY TESTING CODE ---
        // Press SPACEBAR to hurt yourself. Delete this later!
        if (Input.GetKeyDown(KeyCode.N))
        {
            TakeDamage(10);
        }
        // -----------------------------
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount; // Subtract damage

        // Prevent health from going below 0
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        // Update the bar
        UpdateHealthUI();

        // Check if dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        // This math converts health to a number between 0 and 1
        // Example: 80 health / 100 max = 0.8 fill amount
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
        // Later we will add: SceneManager.LoadScene("GameOver");
    }
}