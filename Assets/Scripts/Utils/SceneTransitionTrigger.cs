using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; 

public class SceneTransitionTrigger : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Name of the scene to load (e.g. 'Final Stage')")]
    public string targetSceneName = "Final Stage";
    
    [Header("UI Interaction")]
    public GameObject promptUI; // Drag a UI Text object here

    private bool isPlayerInZone = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.gameObject.name == "Zenitsu")
        {
            isPlayerInZone = true;
            if (promptUI) promptUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.gameObject.name == "Zenitsu")
        {
            isPlayerInZone = false;
            if (promptUI) promptUI.SetActive(false);
        }
    }

    void Update()
    {
        // New Input System check
        if (isPlayerInZone && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log($"Transition Triggered by User. Loading {targetSceneName}...");
            LoadTargetScene();
        }
    }

    void LoadTargetScene()
    {
        SceneManager.LoadScene(targetSceneName);
    }
}
