using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLauncher : MonoBehaviour
{
    public static GameLauncher Instance { get; private set; }

    private WorldGenerator worldGenerator;
    private PlayerController playerController;
    private InventorySystem inventorySystem;
    private Text statusText;
    private List<NPCController> npcs = new List<NPCController>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupScene();
    }

    private void SetupScene()
    {
        CreateDirectionalLight();
        CreateHUD();

        var worldRoot = new GameObject("WorldRoot");
        worldRoot.transform.SetParent(transform);
        worldGenerator = worldRoot.AddComponent<WorldGenerator>();
        worldGenerator.BuildStarterWorld();

        inventorySystem = new InventorySystem();
        inventorySystem.AddDefaultInventory();

        var playerGO = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        playerGO.name = "Player";
        playerGO.transform.position = new Vector3(0f, 2f, 0f);
        playerGO.tag = "Player";

        playerController = playerGO.AddComponent<PlayerController>();
        playerController.Initialize(worldGenerator, inventorySystem);

        CreateNPCs();
        UpdateStatus("Overpeaks Launcher ready. Move, place, use items and interact with NPCs.");
    }

    private void CreateDirectionalLight()
    {
        var lightGO = new GameObject("Directional Light");
        var light = lightGO.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.1f;
        light.color = new Color(1f, 0.95f, 0.85f);
        lightGO.transform.rotation = Quaternion.Euler(35f, 30f, 0f);
    }

    private void CreateHUD()
    {
        var canvasGO = new GameObject("HUDCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        canvasGO.AddComponent<GraphicRaycaster>();

        var statusGO = new GameObject("StatusText");
        statusGO.transform.SetParent(canvasGO.transform, false);
        statusText = statusGO.AddComponent<Text>();
        statusText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        statusText.fontSize = 32;
        statusText.alignment = TextAnchor.MiddleLeft;
        statusText.color = new Color(1f, 1f, 1f, 0.96f);
        statusText.rectTransform.anchorMin = new Vector2(0.02f, 0.88f);
        statusText.rectTransform.anchorMax = new Vector2(0.7f, 0.96f);
        statusText.rectTransform.offsetMin = Vector2.zero;
        statusText.rectTransform.offsetMax = Vector2.zero;
        statusText.text = "Loading world...";
    }

    private void CreateNPCs()
    {
        var npcData = new[]
        {
            new { name = "Ayla", position = new Vector3(6f, 1f, 3f) },
            new { name = "Bram", position = new Vector3(-8f, 1f, -2f) },
            new { name = "Tessa", position = new Vector3(3f, 1f, -7f) }
        };

        foreach (var npc in npcData)
        {
            var npcGO = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            npcGO.name = npc.name;
            npcGO.transform.position = npc.position;
            npcGO.transform.localScale = new Vector3(0.9f, 1.2f, 0.9f);
            npcGO.GetComponent<Collider>().isTrigger = true;

            var npcController = npcGO.AddComponent<NPCController>();
            npcController.Initialize(npc.name, playerController.transform);
            npcs.Add(npcController);
        }
    }

    public void UpdateStatus(string text)
    {
        if (statusText != null)
            statusText.text = text;
    }
}
