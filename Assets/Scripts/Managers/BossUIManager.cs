using UnityEngine;
using UnityEngine.UI;

public class BossUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject bossPanel; // The entire UI group (to show/hide)
    public Slider healthSlider;
    public Text nameText;
    public Text descriptionText;

    [Header("Settings")]
    public string bossName = "Upper Rank 6 - Kaigaku";
    [TextArea] public string bossDescription = "Thunder Breathing User";
    public bool showOnStart = true;

    [Header("Target Boss")]
    public EnemyHealth targetBoss;

    void Start()
    {
        // Setup UI Text
        if (nameText) nameText.text = bossName;
        if (descriptionText) descriptionText.text = bossDescription;

        // Link to Boss Health
        if (targetBoss != null)
        {
            // Subscribe to event
            targetBoss.OnHealthChanged += UpdateHealthUI;
            
            // Initialize
            UpdateHealthUI((float)targetBoss.currentHealth / targetBoss.maxHealth);
        }
        else
        {
            // Auto-find if missing (Try to find 'DoppelgangerAI' which usually has EnemyHealth)
             DoppelgangerAI ai = FindAnyObjectByType<DoppelgangerAI>();
             if (ai != null)
             {
                 targetBoss = ai.GetComponent<EnemyHealth>();
                 if(targetBoss)
                 {
                     targetBoss.OnHealthChanged += UpdateHealthUI;
                     UpdateHealthUI(1.0f);
                 }
             }
        }

        if (bossPanel) bossPanel.SetActive(showOnStart);
    }

    void OnDestroy()
    {
        // Clean up event subscription to avoid memory leaks
        if (targetBoss != null)
        {
            targetBoss.OnHealthChanged -= UpdateHealthUI;
        }
    }

    void UpdateHealthUI(float ratio)
    {
        if (healthSlider)
        {
            healthSlider.value = ratio;

            // Fix for "remaining red bit" issue:
            // Default sliders have padding. We forcibly hide the fill if 0.
            if (healthSlider.fillRect)
            {
                healthSlider.fillRect.gameObject.SetActive(ratio > 0);
            }
        }

        if (ratio <= 0)
        {
            // Optional: Hide UI on death or show "Defeated"
            // StartCoroutine(HideDelay());
        }
    }
}
