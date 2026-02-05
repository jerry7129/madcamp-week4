using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class BossUISetup
{
    [MenuItem("Tools/Setup Boss UI")]
    public static void SetupBossUI()
    {
        GameObject bossHUD = GameObject.Find("BossHUD");
        if (bossHUD == null)
        {
            // Try to find Canvas to parent it to
            Canvas canvas = GameObject.FindAnyObjectByType<Canvas>();
            if (canvas != null)
            {
                bossHUD = new GameObject("BossHUD");
                bossHUD.transform.SetParent(canvas.transform, false);
                // Anchor to bottom center
                RectTransform rt = bossHUD.AddComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0);
                rt.anchorMax = new Vector2(0.5f, 0);
                rt.pivot = new Vector2(0.5f, 0);
                rt.anchoredPosition = new Vector2(0, 50);
                rt.sizeDelta = new Vector2(600, 150);
            }
            else
            {
                Debug.LogError("No BossHUD found and no Canvas found to create one!");
                return;
            }
        }

        Undo.RegisterCompleteObjectUndo(bossHUD, "Setup Boss UI");

        // 1. Add Manager
        BossUIManager manager = bossHUD.GetComponent<BossUIManager>();
        if (manager == null) manager = bossHUD.AddComponent<BossUIManager>();

        // 4. Create Desc Text (TOP)
        Text descText = FindChildText(bossHUD, "DescText");
        if (descText == null)
        {
            GameObject tObj = new GameObject("DescText");
            tObj.transform.SetParent(bossHUD.transform, false);
            descText = tObj.AddComponent<Text>();
            descText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            descText.fontSize = 14;
            descText.alignment = TextAnchor.MiddleCenter;
            descText.color = Color.yellow;

            RectTransform tRect = tObj.GetComponent<RectTransform>();
            // Top area
            tRect.anchorMin = new Vector2(0, 0.8f); 
            tRect.anchorMax = new Vector2(1, 1.0f);
            tRect.offsetMin = Vector2.zero;
            tRect.offsetMax = Vector2.zero;
        }

        // 3. Create Name Text (MIDDLE)
        Text nameText = FindChildText(bossHUD, "NameText");
        if (nameText == null)
        {
            GameObject tObj = new GameObject("NameText");
            tObj.transform.SetParent(bossHUD.transform, false);
            nameText = tObj.AddComponent<Text>();
            nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            nameText.fontSize = 24;
            nameText.alignment = TextAnchor.MiddleCenter;
            nameText.color = Color.white;
            
            RectTransform tRect = tObj.GetComponent<RectTransform>();
            // Middle area
            tRect.anchorMin = new Vector2(0, 0.4f);
            tRect.anchorMax = new Vector2(1, 0.75f);
            tRect.offsetMin = Vector2.zero;
            tRect.offsetMax = Vector2.zero;
        }

        // 2. Create Slider (Health Bar) (BOTTOM)
        Slider slider = bossHUD.GetComponentInChildren<Slider>();
        if (slider == null)
        {
            GameObject sliderObj = DefaultControls.CreateSlider(new DefaultControls.Resources());
            sliderObj.name = "HealthSlider";
            sliderObj.transform.SetParent(bossHUD.transform, false);
            slider = sliderObj.GetComponent<Slider>();
            
            // Positioning - Bottom Area
            RectTransform sRect = sliderObj.GetComponent<RectTransform>();
            sRect.anchorMin = new Vector2(0.1f, 0.1f);
            sRect.anchorMax = new Vector2(0.9f, 0.1f);
            sRect.anchoredPosition = new Vector2(0, 10); // Slight padding from bottom
            sRect.sizeDelta = new Vector2(0, 20); // Height

            // Color (Red)
            Image fill = slider.fillRect.GetComponent<Image>();
            fill.color = Color.red;
        }

        // 5. Assign References
        manager.healthSlider = slider;
        manager.nameText = nameText;
        manager.descriptionText = descText;
        manager.bossPanel = bossHUD;

        // 6. Find Boss
        if (manager.targetBoss == null)
        {
            // Try to find object with GravityBoundEnemyAI or DoppelgangerAI
            DoppelgangerAI bossAI = GameObject.FindAnyObjectByType<DoppelgangerAI>();
            if (bossAI != null)
            {
                manager.targetBoss = bossAI.GetComponent<EnemyHealth>();
            }
        }

        Debug.Log("Boss UI Setup Complete!");
    }

    private static Text FindChildText(GameObject parent, string name)
    {
        foreach(Transform t in parent.transform)
        {
            if (t.name == name) return t.GetComponent<Text>();
        }
        return null;
    }
}
