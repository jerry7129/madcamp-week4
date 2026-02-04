using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEditor.Events; // For linking buttons

public class MainMenuBuilder : MonoBehaviour
{
    [MenuItem("Tools/Build Main Menu")]
    public static void BuildMenu()
    {
        // 1. Open Scene
        string scenePath = "Assets/Scenes/MainMenu.unity";
        EditorSceneManager.OpenScene(scenePath);

        // 2. Setup Camera
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            cam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
            camObj.AddComponent<AudioListener>();
        }
        cam.backgroundColor = Color.black; 
        cam.clearFlags = CameraClearFlags.SolidColor;

        // 3. Create Canvas
        GameObject canvasObj = new GameObject("UI_Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Create EventSystem if missing
        if (!FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>())
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // 4. Main Panel
        GameObject mainPanel = new GameObject("Main_Panel");
        mainPanel.transform.SetParent(canvasObj.transform, false);
        RectTransform panelRect = mainPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one; 
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Title Text
        GameObject titleObj = new GameObject("Title_Text");
        titleObj.transform.SetParent(mainPanel.transform, false);
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "DEMON SLAYER";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 80;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.7f);
        titleRect.anchorMax = new Vector2(0.5f, 0.7f);
        titleRect.sizeDelta = new Vector2(800, 200);

        // Start Button
        GameObject btnObj = new GameObject("Start_Button");
        btnObj.transform.SetParent(mainPanel.transform, false);
        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = new Color(0.9f, 0.9f, 0.9f);
        Button btn = btnObj.AddComponent<Button>();
        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.4f);
        btnRect.anchorMax = new Vector2(0.5f, 0.4f);
        btnRect.sizeDelta = new Vector2(300, 80);

        // Button Text
        GameObject btnTextObj = new GameObject("Text");
        btnTextObj.transform.SetParent(btnObj.transform, false);
        Text btnText = btnTextObj.AddComponent<Text>();
        btnText.text = "GAME START";
        btnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        btnText.fontSize = 32;
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.color = Color.black;
        RectTransform btnTextRect = btnTextObj.GetComponent<RectTransform>();
        btnTextRect.anchorMin = Vector2.zero;
        btnTextRect.anchorMax = Vector2.one;
        btnTextRect.offsetMin = Vector2.zero;
        btnTextRect.offsetMax = Vector2.zero;

        // 5. Video Manager
        GameObject mgrObj = new GameObject("GameManager");
        OpeningVideoManager videoMgr = mgrObj.AddComponent<OpeningVideoManager>();
        
        // Add VideoPlayer
        VideoPlayer vp = mgrObj.AddComponent<VideoPlayer>();
        vp.playOnAwake = false;
        vp.renderMode = VideoRenderMode.CameraNearPlane; // Play directly on camera
        vp.targetCamera = cam;
        
        // Configure Manager
        videoMgr.videoPlayer = vp;
        videoMgr.menuUIRoot = mainPanel;
        videoMgr.videoDisplayUI = null; // Not needed for CameraNearPlane
        videoMgr.nextSceneName = "Map"; // Game Scene

        // 6. Link Button
        UnityEventTools.AddPersistentListener(btn.onClick, videoMgr.PlayOpening);

        // Save
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        
        // 7. Auto-Scrolling Backgrounds
        GameObject bgGroup = GameObject.Find("Background_Group");
        if (bgGroup) DestroyImmediate(bgGroup); 
        bgGroup = new GameObject("Background_Group");

        string[] bgFiles = { "background-layer1.png", "background-layer2.png", "background-layer3.png" };
        float[] speeds = { 2f, 4f, 8f }; // Slow to fast
        int[] orders = { -20, -10, -5 };

        for (int i = 0; i < 3; i++)
        {
            string path = "Assets/Sprites/Environment/" + bgFiles[i];
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null) 
            {
                Debug.LogWarning("Background sprite not found at: " + path);
                continue;
            }

            GameObject layer = new GameObject("Layer_" + (i+1));
            layer.transform.SetParent(bgGroup.transform);
            layer.transform.localScale = Vector3.one * 2f; // Scale up like in Map
            
            SpriteRenderer sr = layer.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = orders[i];

            ParallaxLayer pl = layer.AddComponent<ParallaxLayer>();
            pl.infiniteHorizontal = true;
            pl.autoScrollSpeed = new Vector2(speeds[i], 0); 
            pl.parallaxEffect = Vector2.zero; // Static camera, so no move parallax
        }

        Debug.Log("Main Menu Built Successfully!");
    }
}
