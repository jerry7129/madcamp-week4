using UnityEngine;
using System.Collections.Generic;

public class BackgroundTrigger : MonoBehaviour
{
    [Header("Target Group")]
    [Tooltip("The group to FADE IN when entering this zone")]
    public BackgroundGroup targetGroup;

    [Header("Groups to Hide")]
    [Tooltip("All OTHER groups to FADE OUT when entering this zone")]
    public List<BackgroundGroup> otherGroups;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if it's the player
        if (collision.CompareTag("Player") || collision.gameObject.name == "Zenitsu")
        {
            SwitchBackground();
        }
    }

    private bool isTargetActive = false;

    public void SwitchBackground()
    {
        if (!isTargetActive)
        {
            // Switch TO Target
            if (targetGroup != null)
            {
                targetGroup.FadeIn();
                Debug.Log($"Background Switched TO: {targetGroup.name}");
            }

            foreach (var group in otherGroups)
            {
                if (group != null && group != targetGroup)
                {
                    group.FadeOut();
                }
            }
            isTargetActive = true;
        }
        else
        {
            // Switch BACK from Target
            if (targetGroup != null)
            {
                targetGroup.FadeOut();
                Debug.Log($"Background Switched BACK from: {targetGroup.name}");
            }

            foreach (var group in otherGroups)
            {
                if (group != null && group != targetGroup)
                {
                    group.FadeIn();
                }
            }
            isTargetActive = false;
        }
    }
}
