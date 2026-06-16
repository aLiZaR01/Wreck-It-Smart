using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Пороги звёзд (процент разрушенных блоков)")]
    [Range(0f, 1f)] public float oneStarThreshold = 0.50f;
    [Range(0f, 1f)] public float twoStarThreshold = 0.75f;
    [Range(0f, 1f)] public float threeStarThreshold = 1.00f;

    private List<Block> allBlocks = new List<Block>();
    private int totalBlockCount = 0;
    private int destroyedCount = 0;
    private bool levelEnded = false;

    private ToolPlacer toolPlacer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Block[] blocks = FindObjectsByType<Block>(FindObjectsSortMode.None);
        allBlocks.AddRange(blocks);
        totalBlockCount = allBlocks.Count;

        toolPlacer = FindFirstObjectByType<ToolPlacer>();

        Debug.Log($"Уровень загружен. Блоков: {totalBlockCount}");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            OnDetonatePressed();
    }

    public void OnBlockDestroyed(Block block)
    {
        allBlocks.Remove(block);
        destroyedCount++;

        if (!levelEnded)
            Invoke(nameof(EvaluateResult), 2f);
    }

    private void EvaluateResult()
    {
        if (levelEnded) return;
        levelEnded = true;

        if (totalBlockCount == 0)
        {
            Debug.LogWarning("Блоков не найдено на уровне.");
            return;
        }

        float destroyedPercent = (float)destroyedCount / totalBlockCount;

        int stars = 0;
        if (destroyedPercent >= threeStarThreshold) stars = 3;
        else if (destroyedPercent >= twoStarThreshold) stars = 2;
        else if (destroyedPercent >= oneStarThreshold) stars = 1;

        Debug.Log($"Готово. Разрушено: {destroyedPercent:P0} | Звёзд: {stars}");
    }

    public void OnDetonatePressed()
    {
        if (toolPlacer != null)
            toolPlacer.DetonateAll();
        else
            Debug.LogWarning("ToolPlacer не найден.");
    }
}