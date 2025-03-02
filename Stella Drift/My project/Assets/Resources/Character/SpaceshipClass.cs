using UnityEngine;
using UnityEngine.UI; // For UI elements if you want to display health
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("UI References")]
    public Slider healthSlider; // Optional: for displaying health bar
    public Text healthText;     // Optional: for displaying health value
    
    [Header("Visual Feedback")]
    public bool flashOnDamage = true;
    public Color damageFlashColor = Color.red;
    public float flashDuration = 0.2f;
    
    private Renderer[] renderers;
    private Color[] originalColors;
    private bool isFlashing = false;
    public GameOverManager GameOverManager;

    void Start()
    {
        // Initialize health to max value
        currentHealth = maxHealth;
        
        // Set up UI elements if assigned
        UpdateHealthUI();
        
        // Cache renderers for visual feedback
        if (flashOnDamage)
        {
            renderers = GetComponentsInChildren<Renderer>();
            originalColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                originalColors[i] = renderers[i].material.color;
            }
        }
    }
    
    // Call this method when player takes damage
    public void TakeDamage(float damageAmount)
    {
        // Reduce health by damage amount
        currentHealth -= damageAmount;
        
        // Clamp health to prevent negative values
        currentHealth = Mathf.Max(currentHealth, 0);
        
        // Update UI
        UpdateHealthUI();
        
        // Visual feedback
        if (flashOnDamage && !isFlashing)
        {
            StartCoroutine(FlashDamage());
        }
        
        // Check if player is dead
        if (currentHealth <= 0)
        {
            Die();
        }
        
        //Debug.Log($"Player took {damageAmount} damage. Health: {currentHealth}/{maxHealth}");
    }
    
    // Call this method to heal the player
    public void Heal(float healAmount)
    {
        // Increase health by heal amount
        currentHealth += healAmount;
        
        // Clamp health to prevent exceeding max health
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        // Update UI
        UpdateHealthUI();
        
        Debug.Log($"Player healed for {healAmount}. Health: {currentHealth}/{maxHealth}");
    }
    
    void UpdateHealthUI()
    {
        // Update health slider if assigned
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        
        // Update health text if assigned
        if (healthText != null)
        {
            healthText.text = $"{Mathf.Ceil(currentHealth)}/{maxHealth}";
        }
    }
    
    IEnumerator FlashDamage()
    {
        isFlashing = true;
        
        // Change color to flash color
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = damageFlashColor;
        }
        
        // Wait for flash duration
        yield return new WaitForSeconds(flashDuration);
        
        // Restore original colors
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = originalColors[i];
        }
        
        isFlashing = false;
    }
    
    void Die()
    {
        Debug.Log("Player has died!");
        
        GameOverManager.Instance.ShowGameOver();

        gameObject.SetActive(false);
    }
}