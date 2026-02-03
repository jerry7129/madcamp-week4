using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10f);
    
    [Header("Settings")]
    public float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
            else 
            {
                // Fallback: Find by name if tag is missing
                GameObject zenitsu = GameObject.Find("Zenitsu");
                if(zenitsu) target = zenitsu.transform;
            }
        }

        // Snap rotation immediately on start
        UpdateCameraRotation(true);
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Position Follow
            Vector3 targetPosition = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

            // Rotation Follow (Gravity)
            UpdateCameraRotation(false);
        }
    }

    void UpdateCameraRotation(bool snap)
    {
        Vector2 g = Physics2D.gravity;
        if (g == Vector2.zero) return; // No gravity means no specific up

        Vector2 targetUp = -g.normalized;
        float targetAngle = Mathf.Atan2(targetUp.y, targetUp.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        if (snap)
        {
            transform.rotation = targetRotation;
        }
        else
        {
            // Rotate camera smoothly to match gravity/player orientation
            // Use slightly slower speed than player for smoother feel
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        }
    }
}
