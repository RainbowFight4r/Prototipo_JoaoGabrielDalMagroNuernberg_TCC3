using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectionMinigame : MonoBehaviour
{
    [Header("Camera Animation")]
    [SerializeField] Camera mainCamera;
    [SerializeField] Transform cameraOverviewPoint;
    [SerializeField] float overviewDuration = 3f;
    [SerializeField] float transitionDuration = 1.5f;

    [Header("Spawn Settings")]
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] GameObject blockPrefab;

    [Header("Block Colors")]
    [SerializeField]
    Color[] blockColors = new Color[]
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        new Color(1f, 0f, 1f)
    };
    [SerializeField]
    string[] colorNames = new string[]
    {
        "Vermelho",
        "Azul",
        "Verde",
        "Amarelo",
        "Roxo"
    };

    [Header("Game Settings")]
    [SerializeField] float gameDuration = 120f;
    [SerializeField] TMP_Text timerText;

    [Header("Results UI")]
    [SerializeField] GameObject resultsPanel;
    [SerializeField] TMP_Text resultsText;

    Dictionary<string, int> collectedBlocks = new Dictionary<string, int>();
    Dictionary<string, Color> blockColorMap = new Dictionary<string, Color>();
    List<GameObject> spawnedBlocks = new List<GameObject>();
    float timeRemaining;
    bool gameActive = false;
    Vector3 originalCameraPos;

    void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (resultsPanel)
            resultsPanel.SetActive(false);

        // Esconde o timer no início
        if (timerText != null)
            timerText.gameObject.SetActive(false);

        for (int i = 0; i < colorNames.Length; i++)
        {
            collectedBlocks[colorNames[i]] = 0;
            if (i < blockColors.Length)
                blockColorMap[colorNames[i]] = blockColors[i];
        }
    }

    public void StartGame()
    {
        StartCoroutine(GameSequence());
    }

    IEnumerator GameSequence()
    {
        originalCameraPos = mainCamera.transform.position;

        SpawnBlocks();

        yield return new WaitForSeconds(0.3f);

        yield return AnimateCameraOverview();

        // Ativa o timer somente agora
        if (timerText != null)
            timerText.gameObject.SetActive(true);

        timeRemaining = gameDuration;
        gameActive = true;

        while (timeRemaining > 0 && gameActive)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();
            yield return null;
        }

        EndGame();
    }

    IEnumerator AnimateCameraOverview()
    {
        yield return MoveCamera(mainCamera.transform, cameraOverviewPoint.position, transitionDuration);
        yield return new WaitForSeconds(overviewDuration);
        yield return MoveCamera(mainCamera.transform, originalCameraPos, transitionDuration);
    }

    IEnumerator MoveCamera(Transform cam, Vector3 targetPos, float duration)
    {
        Vector3 startPos = cam.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            cam.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        cam.position = targetPos;
    }

    void SpawnBlocks()
    {
        if (spawnPoints.Length == 0 || blockPrefab == null)
        {
            Debug.LogError("SpawnPoints ou BlockPrefab não configurados!");
            return;
        }

        foreach (Transform spawnPoint in spawnPoints)
        {
            int colorIndex = Random.Range(0, colorNames.Length);
            string colorName = colorNames[colorIndex];
            Color color = blockColors[colorIndex];

            GameObject block = Instantiate(blockPrefab, spawnPoint.position, Quaternion.identity);
            block.name = $"Block_{colorName}";

            SpriteRenderer sr = block.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = color;
                sr.sortingLayerName = "Player";
                sr.sortingOrder = 1;
            }

            Collider2D col = block.GetComponent<Collider2D>();
            if (col != null)
                col.isTrigger = true;

            CollectibleBlock cb = block.GetComponent<CollectibleBlock>();
            if (cb == null)
                cb = block.AddComponent<CollectibleBlock>();

            cb.Initialize(this, colorName);

            spawnedBlocks.Add(block);
        }
    }

    public void CollectBlock(string blockType)
    {
        if (!gameActive) return;
        if (collectedBlocks.ContainsKey(blockType))
            collectedBlocks[blockType]++;
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
            if (timeRemaining < 10f)
                timerText.color = Color.red;
        }
    }

    void EndGame()
    {
        gameActive = false;

        foreach (GameObject block in spawnedBlocks)
        {
            if (block != null)
                Destroy(block);
        }

        StartCoroutine(ShowResults());
    }

    IEnumerator ShowResults()
    {
        yield return new WaitForSeconds(0.5f);

        if (resultsPanel)
            resultsPanel.SetActive(true);

        int totalCollected = 0;
        foreach (var kvp in collectedBlocks)
            totalCollected += kvp.Value;

        string results = "=== ANÁLISE ESTATÍSTICA ===\n\n";
        results += $"<b>Total Coletado:</b> {totalCollected} blocos\n";
        results += $"<b>Total Disponível:</b> {spawnPoints.Length} blocos\n\n";
        results += "<b>DISTRIBUIÇÃO DE FREQUÊNCIAS:</b>\n";

        string modaType = "";
        int modaValue = 0;

        foreach (var kvp in collectedBlocks)
        {
            int count = kvp.Value;
            float percentage = totalCollected > 0 ? (count / (float)totalCollected) * 100f : 0;

            string colorHex = "";
            if (blockColorMap.ContainsKey(kvp.Key))
                colorHex = ColorUtility.ToHtmlStringRGB(blockColorMap[kvp.Key]);

            results += $"• <color=#{colorHex}>{kvp.Key}</color>: {count} ({percentage:F1}%)\n";

            if (count > modaValue)
            {
                modaValue = count;
                modaType = kvp.Key;
            }
        }

        float mean = totalCollected / (float)colorNames.Length;
        float variance = 0;
        foreach (var kvp in collectedBlocks)
        {
            variance += Mathf.Pow(kvp.Value - mean, 2);
        }
        variance /= colorNames.Length;
        float stdDev = Mathf.Sqrt(variance);
        float cv = mean > 0 ? (stdDev / mean) * 100f : 0;


        if (resultsText)
            resultsText.text = results;

        GameManager.Instance?.AddChallengeCompleted();
    }
}
