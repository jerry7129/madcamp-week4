using UnityEngine;

[DefaultExecutionOrder(10)] // Run AFTER CameraFollow logic
public class ParallaxLayer : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("0 = Static (Normal), 1 = Moves with Camera (Infinite Distance), >1 = Foreground")]
    public Vector2 parallaxEffect = new Vector2(0.5f, 0.5f);

    [Tooltip("If true, the background will repeat infinitely on X axis")]
    public bool infiniteHorizontal = false;
    [Tooltip("If true, the background will repeat infinitely on Y axis")]
    public bool infiniteVertical = false;

    [Tooltip("Horizontal clone layers (1 = 1 left + 1 right)")]
    [Range(0, 10)]
    public int cloneGridRadiusX = 1;

    [Tooltip("Vertical clone layers (1 = 1 up + 1 down)")]
    [Range(0, 10)]
    public int cloneGridRadiusY = 1;

    [Tooltip("Extra spacing between clones (Negative = Overlap, Positive = Gap)")]
    public Vector2 cloneGap = new Vector2(-0.05f, -0.05f);

    [Tooltip("Automatic scrolling speed (e.g. for Main Menu)")]
    public Vector2 autoScrollSpeed = Vector2.zero;

    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private float textureUnitSizeX;
    private float textureUnitSizeY;

    void Awake()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) mainCam = FindFirstObjectByType<Camera>(); 
        
        if (mainCam != null)
        {
            cameraTransform = mainCam.transform;
            lastCameraPosition = cameraTransform.position;
        }

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            Texture2D texture = sprite.sprite.texture;
            
            // Calculate Unit Size based on Rect (for Atlases) + User defined Gap
            textureUnitSizeX = (sprite.sprite.rect.width / sprite.sprite.pixelsPerUnit) + cloneGap.x; 
            textureUnitSizeY = (sprite.sprite.rect.height / sprite.sprite.pixelsPerUnit) + cloneGap.y;

            // NEW: Create dynamic Grid of Sidekicks based on split radius
            if (infiniteHorizontal || infiniteVertical) 
            {
                for (int x = -cloneGridRadiusX; x <= cloneGridRadiusX; x++)
                {
                    for (int y = -cloneGridRadiusY; y <= cloneGridRadiusY; y++)
                    {
                        if (x == 0 && y == 0) continue; // Skip self

                        CreateSidekick(sprite, x * textureUnitSizeX, y * textureUnitSizeY);
                    }
                }
            }
        }
    }

    void CreateSidekick(SpriteRenderer parentSprite, float offsetX, float offsetY)
    {
        GameObject sidekick = new GameObject($"{name}_Sidekick_{offsetX}_{offsetY}");
        sidekick.transform.SetParent(transform);
        sidekick.transform.localPosition = new Vector3(offsetX, offsetY, 0);
        sidekick.transform.localScale = Vector3.one;
        
        SpriteRenderer sr = sidekick.AddComponent<SpriteRenderer>();
        sr.sprite = parentSprite.sprite;
        sr.sortingOrder = parentSprite.sortingOrder;
        sr.color = parentSprite.color;
        
        // Ensure sidekicks don't run Update scripts if any
        // (Since they are just visual children, this is fine)
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        
        // Apply Parallax
        transform.position += new Vector3(deltaMovement.x * parallaxEffect.x, deltaMovement.y * parallaxEffect.y, 0);
        
        // Apply Auto Scroll
        transform.position += (Vector3)autoScrollSpeed * Time.deltaTime;

        lastCameraPosition = cameraTransform.position;

        // Infinite Scrolling Logic
        if (infiniteHorizontal)
        {
            float worldWidth = textureUnitSizeX * transform.localScale.x;
            if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= worldWidth)
            {
                float offsetPositionX = (cameraTransform.position.x - transform.position.x) % worldWidth;
                transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y, transform.position.z);
            }
        }
        
        if (infiniteVertical)
        {
             float worldHeight = textureUnitSizeY * transform.localScale.y;
             if (Mathf.Abs(cameraTransform.position.y - transform.position.y) >= worldHeight)
            {
                float offsetPositionY = (cameraTransform.position.y - transform.position.y) % worldHeight;
                transform.position = new Vector3(transform.position.x, cameraTransform.position.y + offsetPositionY, transform.position.z);
            }
        }
    }
}
