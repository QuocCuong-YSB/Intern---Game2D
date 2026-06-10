using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SetupEndlessRunnerScene
{
    private const string LEVEL1_PATH = "Assets/Levels/Level1.unity";
    private const string GROUND_TILE_PREFAB = "Assets/Prefabs/GroundTile.prefab";
    private const string PLAYER_PREFAB = "Assets/Prefabs/Player/Player.prefab";
    private const string FIREBALL_PREFAB = "Assets/Prefabs/Projectiles/Fireball.prefab";
    private const string OBSTACLE_PREFAB = "Assets/Prefabs/Obstacle/Idle.prefab";

    private static GameObject groundManagerObj;
    private static GameObject obtascleObj;

    [MenuItem("Tools/Setup Endless Runner Scene")]
    public static void SetupScene()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SetActiveScene(scene);

        CreateGameManager();
        CreateSoundManager();
        CreateLoadingManager();
        CreateGroundManager();
        CreatePlayer();
        CreateMainCamera();
        CreateObstacleSpawner();
        CreateUICanvas();
        CreateBackground();
        CreateEventSystem();

        EditorSceneManager.SaveScene(scene, LEVEL1_PATH);
        Debug.Log("Endless Runner scene created at " + LEVEL1_PATH);
    }

    static GameObject CreateEmpty(string name, Transform parent = null)
    {
        GameObject go = new GameObject(name);
        if (parent != null) go.transform.SetParent(parent);
        go.transform.localPosition = Vector3.zero;
        return go;
    }

    static T AddComponent<T>(GameObject go) where T : Component
    {
        return go.AddComponent<T>();
    }

    static void SetLayerRecursive(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
            SetLayerRecursive(child.gameObject, layer);
    }

    static void CreateGameManager()
    {
        GameObject core = CreateEmpty("Core");
        GameObject go = CreateEmpty("GameManager", core.transform);
        DinoGameManager dgm = go.AddComponent<DinoGameManager>();
        SerializedObject so = new SerializedObject(dgm);
        so.FindProperty("baseSpeed").floatValue = 5f;
        so.FindProperty("speedIncreasePerCheckpoint").floatValue = 1.2f;
        so.FindProperty("maxSpeedMultiplier").floatValue = 3f;
        so.ApplyModifiedProperties();
    }

    static void CreateSoundManager()
    {
        GameObject go = new GameObject("SoundManager");
        go.AddComponent<AudioSource>();
        GameObject musicChild = new GameObject("MusicSource");
        musicChild.transform.SetParent(go.transform);
        musicChild.AddComponent<AudioSource>();
        go.AddComponent<SoundManager>();
    }

    static void CreateLoadingManager()
    {
        GameObject go = CreateEmpty("LoadingManager");
        go.AddComponent<LoadingManager>();
    }

    static void CreateGroundManager()
    {
        groundManagerObj = CreateEmpty("GroundManager");
        GroundScroller gs = groundManagerObj.AddComponent<GroundScroller>();

        GameObject groundTilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(GROUND_TILE_PREFAB);
        if (groundTilePrefab == null)
        {
            Debug.LogError("GroundTile prefab not found at " + GROUND_TILE_PREFAB);
            return;
        }

        int tileCount = 9;
        Transform[] tiles = new Transform[tileCount];
        float tileSpacing = 2.39f;

        for (int i = 0; i < tileCount; i++)
        {
            GameObject tile = (GameObject)PrefabUtility.InstantiatePrefab(groundTilePrefab, groundManagerObj.transform);
            tile.name = i == 0 ? "GroundTile" : $"GroundTile ({i})";
            tile.transform.localPosition = new Vector3(i * tileSpacing, -1.415f, 0);
            tile.transform.localScale = new Vector3(5.433684f, 4.9693246f, 1);
            tiles[i] = tile.transform;
        }

        SerializedObject so = new SerializedObject(gs);
        so.FindProperty("tileWidth").floatValue = 2.39f;
        so.FindProperty("despawnX").floatValue = -10f;
        SerializedProperty tileArray = so.FindProperty("groundTiles");
        tileArray.ClearArray();
        tileArray.arraySize = tileCount;
        for (int i = 0; i < tileCount; i++)
            tileArray.GetArrayElementAtIndex(i).objectReferenceValue = tiles[i];
        so.ApplyModifiedProperties();
    }

    static void CreatePlayer()
    {
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PLAYER_PREFAB);
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab not found at " + PLAYER_PREFAB);
            return;
        }

        GameObject player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
        player.name = "Player";
        player.transform.position = new Vector3(-6.75f, -1.617f, 0);

        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            SerializedObject so = new SerializedObject(pm);
            so.FindProperty("jumpPower").floatValue = 20f;
            so.FindProperty("duckColliderHeight").floatValue = 0.5f;
            so.FindProperty("groundY").floatValue = -1.68f;
            so.ApplyModifiedProperties();
        }

        PlayerAttack pa = player.GetComponent<PlayerAttack>();
        if (pa != null)
        {
            SerializedObject so = new SerializedObject(pa);
            so.FindProperty("attackCooldown").floatValue = 0.25f;

            GameObject fireballPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(FIREBALL_PREFAB);
            if (fireballPrefab != null)
            {
                int fireballCount = 10;
                GameObject[] fireballs = new GameObject[fireballCount];
                for (int i = 0; i < fireballCount; i++)
                {
                    GameObject fb = (GameObject)PrefabUtility.InstantiatePrefab(fireballPrefab, player.transform);
                    fb.name = i == 0 ? "Fireball" : $"Fireball ({i})";
                    fb.SetActive(false);
                    fireballs[i] = fb;
                }

                SerializedProperty fbArray = so.FindProperty("fireballs");
                fbArray.ClearArray();
                fbArray.arraySize = fireballCount;
                for (int i = 0; i < fireballCount; i++)
                    fbArray.GetArrayElementAtIndex(i).objectReferenceValue = fireballs[i];

                so.ApplyModifiedProperties();
            }
        }

        player.tag = "Player";
        SetLayerRecursive(player, 10);
    }

    static void CreateMainCamera()
    {
        GameObject cameraObj = new GameObject("Main Camera");
        Camera cam = cameraObj.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        cameraObj.tag = "MainCamera";
        cameraObj.transform.position = new Vector3(0, 0, -10);

        CameraController cc = cameraObj.AddComponent<CameraController>();
        SerializedObject so = new SerializedObject(cc);
        so.FindProperty("fixedX").floatValue = 0f;
        so.FindProperty("fixedY").floatValue = 0f;
        so.ApplyModifiedProperties();
    }

    static void CreateObstacleSpawner()
    {
        obtascleObj = CreateEmpty("Obtascle");
        ObstacleSpawner os = obtascleObj.AddComponent<ObstacleSpawner>();

        GameObject obstaclePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(OBSTACLE_PREFAB);
        if (obstaclePrefab != null)
        {
            SerializedObject so = new SerializedObject(os);

            SerializedProperty groundArray = so.FindProperty("groundObstacles");
            groundArray.ClearArray();
            groundArray.arraySize = 1;
            groundArray.GetArrayElementAtIndex(0).objectReferenceValue = obstaclePrefab;

            so.FindProperty("minSpawnInterval").floatValue = 1f;
            so.FindProperty("maxSpawnInterval").floatValue = 2.5f;
            so.FindProperty("spawnXPosition").floatValue = 10f;
            so.FindProperty("groundYPosition").floatValue = -1.5f;
            so.FindProperty("airYPosition").floatValue = 1.5f;
            so.FindProperty("minIntervalDecrease").floatValue = 0.8f;
            so.FindProperty("obstacleSpeed").floatValue = 5f;

            so.ApplyModifiedProperties();
        }
    }

    static void CreateUICanvas()
    {
        GameObject canvasObj = new GameObject("UICanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();
        SetLayerRecursive(canvasObj, 5);

        Font arialFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        TMP_FontAsset tmpFont = Resources.Load<TMP_FontAsset>("LiberationSans SDF");

        GameObject scoreObj = CreateText(canvasObj.transform, "ScoreText", "0", new Vector2(950, 500), new Vector2(200, 50), 36, TextAnchor.MiddleRight);
        GameObject highScoreObj = CreateText(canvasObj.transform, "HighScoreText", "HI 0", new Vector2(950, 450), new Vector2(200, 50), 24, TextAnchor.MiddleRight);

        GameObject gameOverUI = CreateEmpty("GameOverUI", canvasObj.transform);
        RectTransform gameOverRect = gameOverUI.AddComponent<RectTransform>();
        gameOverRect.anchorMin = Vector2.zero;
        gameOverRect.anchorMax = Vector2.one;
        gameOverRect.sizeDelta = Vector2.zero;
        gameOverUI.AddComponent<CanvasGroup>();
        gameOverUI.SetActive(false);

        GameObject dedText = CreateText(gameOverUI.transform, "DED", "GAME OVER", new Vector2(0, 50), new Vector2(400, 80), 64, TextAnchor.MiddleCenter);
        GameObject restartBtn = CreateButton(gameOverUI.transform, "Restart", "RESTART", new Vector2(0, -50), new Vector2(200, 50));

        GameObject pauseUI = CreateEmpty("PauseUI", canvasObj.transform);
        RectTransform pauseRect = pauseUI.AddComponent<RectTransform>();
        pauseRect.anchorMin = Vector2.zero;
        pauseRect.anchorMax = Vector2.one;
        pauseRect.sizeDelta = Vector2.zero;
        pauseUI.AddComponent<CanvasGroup>();
        pauseUI.SetActive(false);

        GameObject resumeBtn = CreateButton(pauseUI.transform, "Resume", "RESUME", new Vector2(0, 50), new Vector2(200, 50));
        GameObject quitBtn = CreateButton(pauseUI.transform, "Quit", "QUIT", new Vector2(0, -50), new Vector2(200, 50));

        UIManager uim = canvasObj.AddComponent<UIManager>();
        SerializedObject so = new SerializedObject(uim);
        so.FindProperty("gameOverScreen").objectReferenceValue = gameOverUI;
        so.FindProperty("pauseScreen").objectReferenceValue = pauseUI;
        so.FindProperty("scoreText").objectReferenceValue = scoreObj.GetComponent<TextMeshProUGUI>();
        so.FindProperty("highScoreText").objectReferenceValue = highScoreObj.GetComponent<TextMeshProUGUI>();
        so.ApplyModifiedProperties();
    }

    static GameObject CreateText(Transform parent, string name, string text, Vector2 anchoredPos, Vector2 size, int fontSize, TextAnchor alignment)
    {
        GameObject go = CreateEmpty(name, parent);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Right;
        tmp.color = Color.white;

        return go;
    }

    static GameObject CreateButton(Transform parent, string name, string label, Vector2 anchoredPos, Vector2 size)
    {
        GameObject go = CreateEmpty(name, parent);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;

        Image image = go.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        Button button = go.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(0.4f, 0.4f, 0.4f);
        button.colors = colors;

        GameObject textObj = CreateText(go.transform, "Label", label, Vector2.zero, size, 24, TextAnchor.MiddleCenter);

        return go;
    }

    static void CreateBackground()
    {
        GameObject bgObj = CreateEmpty("Background");

        SpriteRenderer[] bgLayers = new SpriteRenderer[3];
        Color[] colors = { new Color(0.1f, 0.1f, 0.1f), new Color(0.15f, 0.15f, 0.15f), new Color(0.05f, 0.05f, 0.05f) };

        for (int i = 0; i < 3; i++)
        {
            GameObject layer = CreateEmpty($"Layer{i}", bgObj.transform);
            SpriteRenderer sr = layer.AddComponent<SpriteRenderer>();
            sr.color = colors[i];
            sr.sortingOrder = -10 - i;
            bgLayers[i] = sr;
        }

        GroundScroller gs = groundManagerObj?.GetComponent<GroundScroller>();
        if (gs != null)
        {
            SerializedObject so = new SerializedObject(gs);
            SerializedProperty bgArray = so.FindProperty("backgroundLayers");
            bgArray.ClearArray();
            bgArray.arraySize = bgLayers.Length;
            for (int i = 0; i < bgLayers.Length; i++)
                bgArray.GetArrayElementAtIndex(i).objectReferenceValue = bgLayers[i].transform;
            so.ApplyModifiedProperties();
        }
    }

    static void CreateEventSystem()
    {
        GameObject es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
    }
}
