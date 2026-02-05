using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;

public class CreateGameUI : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Create Stylish UI")]
    static void Create()
    {
        // 1. Setup Canvas
        GameObject canvasObj = GameObject.Find("Canvas");
        if (canvasObj == null)
        {
            canvasObj = new GameObject("Canvas");
            canvasObj.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // 2. Setup Game UI Manager
        GameObject managerObj = GameObject.Find("GameManager");
        if (managerObj == null) managerObj = new GameObject("GameManager");
        GameUIManager uiManager = managerObj.GetComponent<GameUIManager>();
        if (uiManager == null) uiManager = managerObj.AddComponent<GameUIManager>();

        // 3. Create Health Bar Container (Top Left)
        GameObject healthPanel = CreateRect(canvasObj.transform, "HealthPanel", new Vector2(0, 1), new Vector2(0, 1), new Vector2(50, -50), new Vector2(400, 50));
        
        // Background
        Image bg = healthPanel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f); // Dark Grey

        // Fill Area
        GameObject healthFillObj = CreateRect(healthPanel.transform, "Fill", new Vector2(0, 0), new Vector2(1, 1), Vector2.zero, Vector2.zero);
        // Add padding
        RectTransform fillRect = healthFillObj.GetComponent<RectTransform>();
        fillRect.offsetMin = new Vector2(5, 5);
        fillRect.offsetMax = new Vector2(-5, -5);
        
        Image fillImg = healthFillObj.AddComponent<Image>();
        fillImg.color = new Color(1f, 0.2f, 0.2f, 1f); // Red
        fillImg.type = Image.Type.Simple;

        // Create Slider Component on Parent
        Slider healthSlider = healthPanel.AddComponent<Slider>();
        healthSlider.targetGraphic = bg;
        healthSlider.fillRect = fillRect;
        healthSlider.direction = Slider.Direction.LeftToRight;
        healthSlider.minValue = 0;
        healthSlider.maxValue = 1;
        healthSlider.value = 1;
        healthSlider.interactable = false; // Disable dragging

        // Create standard white sprite for UI rendering
        Sprite whiteSprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
        if (whiteSprite == null)
        {
            // Fallback: Generate one if builtin not found (common in some pipelines)
            Texture2D tex = new Texture2D(2, 2);
            tex.SetPixels(new Color[] {Color.white, Color.white, Color.white, Color.white});
            tex.Apply();
            whiteSprite = Sprite.Create(tex, new Rect(0,0,2,2), Vector2.one * 0.5f);
        }

        // 4. Create Dash Icon (Under Health Bar)
        GameObject dashPanel = CreateRect(canvasObj.transform, "DashPanel", new Vector2(0, 1), new Vector2(0, 1), new Vector2(80, -120), new Vector2(60, 60));
        Image dashBg = dashPanel.AddComponent<Image>();
        dashBg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        dashBg.sprite = whiteSprite; // Assign Sprite

        // Flash Icon (Text or Shape)
        GameObject dashIconObj = CreateRect(dashPanel.transform, "Icon", new Vector2(0, 0), new Vector2(1, 1), Vector2.zero, Vector2.zero);
        Text iconText = dashIconObj.AddComponent<Text>();
        iconText.text = "⚡";
        // iconText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); // Often null in newer Unity
        iconText.font = Resources.FindObjectsOfTypeAll<Font>()[0]; // Grab any font
        iconText.alignment = TextAnchor.MiddleCenter;
        iconText.resizeTextForBestFit = true;
        iconText.color = Color.yellow;

        // Cooldown Overlay (Darkens when on cooldown)
        GameObject cooldownOverlay = CreateRect(dashPanel.transform, "CooldownOverlay", new Vector2(0, 0), new Vector2(1, 1), Vector2.zero, Vector2.zero);
        Image cooldownImg = cooldownOverlay.AddComponent<Image>();
        cooldownImg.sprite = whiteSprite; // CRITICAL: Filled type needs a sprite!
        cooldownImg.color = new Color(0, 0, 0, 0.7f);
        cooldownImg.type = Image.Type.Filled;
        cooldownImg.fillMethod = Image.FillMethod.Radial360;
        cooldownImg.fillOrigin = 2; // Top
        cooldownImg.fillClockwise = false;

        // 5. Create Game Clear Panel
        GameObject gameClearPanel = CreateRect(canvasObj.transform, "GameClearPanel", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Image clearBg = gameClearPanel.AddComponent<Image>();
        clearBg.color = new Color(0, 0, 0, 0.8f);
        
        GameObject clearTextObj = CreateRect(gameClearPanel.transform, "ClearText", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 50), new Vector2(400, 100));
        Text clearText = clearTextObj.AddComponent<Text>();
        clearText.text = "GAME CLEAR";
        clearText.font = Resources.FindObjectsOfTypeAll<Font>()[0];
        clearText.fontSize = 60;
        clearText.color = Color.yellow;
        clearText.alignment = TextAnchor.MiddleCenter;

        GameObject mainMenuBtnObj = CreateRect(gameClearPanel.transform, "MainMenuButton", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -50), new Vector2(200, 50));
        Image btnImg = mainMenuBtnObj.AddComponent<Image>();
        btnImg.color = Color.white;
        Button btn = mainMenuBtnObj.AddComponent<Button>();
        
        GameObject btnTextObj = CreateRect(mainMenuBtnObj.transform, "Text", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Text btnText = btnTextObj.AddComponent<Text>();
        btnText.text = "Return to Main Menu";
        btnText.font = Resources.FindObjectsOfTypeAll<Font>()[0];
        btnText.color = Color.black;
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.resizeTextForBestFit = true;

        // Note: Connecting the button OnClick event in Editor script is tricky with persistent listeners.
        // It's safer to let the User or a Runtime script handle initialization, OR use `UnityEvent.AddListener` if running in Play mode.
        // For Editor time, we just setup the structure. The GameUIManager needs to assign the listener in Start or we do it manualy in Inspector.
        // HOWEVER, we can try to find the method via SerializedObject if we want to be fancy, but let's keep it simple:
        // User must assign the click event OR GameUIManager auto-wires it if possible.
        // Actually, let's just leave the panel inactive.
        gameClearPanel.SetActive(false);

        // 6. Link to Manager
        uiManager.healthSlider = healthSlider;
        uiManager.healthFillImage = fillImg;
        uiManager.dashCooldownImage = cooldownImg;
        uiManager.gameClearPanel = gameClearPanel; // Link new panel

        // Create Default Gradient (Red -> Yellow -> Green)
        Gradient g = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[3];
        colorKeys[0] = new GradientColorKey(Color.red, 0.0f);
        colorKeys[1] = new GradientColorKey(Color.yellow, 0.5f);
        colorKeys[2] = new GradientColorKey(Color.green, 1.0f);
        
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphaKeys[1] = new GradientAlphaKey(1.0f, 1.0f);
        
        g.SetKeys(colorKeys, alphaKeys);
        uiManager.healthColorGradient = g;
        
        // Force initial color update
        fillImg.color = Color.green; // Start Green

        // 7. Link Player
        PlayerController player = Object.FindAnyObjectByType<PlayerController>();
        if (player)
        {
            uiManager.playerController = player;
            PlayerHealth pHealth = player.GetComponent<PlayerHealth>();
            if (pHealth == null) pHealth = player.gameObject.AddComponent<PlayerHealth>();
        }

        // Add OnClick listener logic hint
        Debug.LogWarning("Please manually assign 'GameUIManager.ReturnToMainMenu' to the 'Return to Main Menu' Button OnClick event!");

        EditorUtility.SetDirty(uiManager); // Ensure changes are saved

        Selection.activeGameObject = canvasObj;
        Debug.Log("UI Created & Linked!");
    }

    static GameObject CreateRect(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = sizeDelta;
        return obj;
    }
#endif
}
