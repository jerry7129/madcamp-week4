using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [Header("Health UI")]
    public Slider healthSlider;
    public Image healthFillImage;
    public Gradient healthColorGradient; // Green -> Red

    [Header("Skill UI")]
    public Image dashCooldownImage; // Filled Image type
    public GameObject dashReadyEffect; // Optional flashing outline

    [Header("Game Over UI")]
    public GameObject gameOverPanel; 

    [Header("References")]
    public PlayerController playerController;

    void Start()
    {
        if (playerController == null)
        {
            // Prioritize object with "Player" tag
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj) 
            {
                playerController = playerObj.GetComponent<PlayerController>();
            }
            
            // Fallback
            if (playerController == null)
            {
                playerController = FindAnyObjectByType<PlayerController>();
            }
        }

        // Init Health Color
        if (healthFillImage && healthColorGradient != null)
            healthFillImage.color = healthColorGradient.Evaluate(1f);
            
        // Auto-Find Slider if missing
        if (healthSlider == null)
        {
            healthSlider = GetComponentInChildren<Slider>();
        }

        // Ensure Game Over UI is hidden on start (re-load)
        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    void Update()
    {
        UpdateDashUI();
    }

    public void UpdateHealthBar(float ratio)
    {
        if (healthSlider) 
        {
            healthSlider.value = ratio;
        }
        
        if (healthFillImage && healthColorGradient != null)
        {
            healthFillImage.color = healthColorGradient.Evaluate(ratio);
        }
    }

    private void UpdateDashUI()
    {
        if (playerController == null || dashCooldownImage == null) return;

        float ratio = playerController.GetDashCooldownRatio();
        dashCooldownImage.fillAmount = ratio;
        
        if (dashReadyEffect)
            dashReadyEffect.SetActive(ratio >= 1f);
    }

    public void ShowGameOver()
    {
        if (gameOverPanel) 
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void RestartGame()
    {
        // Don't reload scene, just respawn player
        if (playerController)
        {
            // Find Health component to trigger full respawn chain
            var health = playerController.GetComponent<PlayerHealth>();
            if (health)
            {
                health.Respawn();
            }
            else
            {
                // Fallback for controller only
                playerController.Respawn();
            }
        }
        else
        {
            // Fallback if controller missing: Reload Scene
             UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
             return;
        }

        // Hide UI
        if(gameOverPanel) gameOverPanel.SetActive(false);
    }
}
