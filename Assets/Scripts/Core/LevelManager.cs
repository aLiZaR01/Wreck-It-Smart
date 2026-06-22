using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Пороги звёзд (процент разрушенных блоков)")]
    [Range(0f, 1f)] public float oneStarThreshold = 0.50f;
    [Range(0f, 1f)] public float twoStarThreshold = 0.75f;
    [Range(0f, 1f)] public float threeStarThreshold = 1.00f;

    [Header("Награда")]
    public int coinsPerStar = 20;

    [Header("Тайминг результата")]
    public float resultDelayAfterLastHit = 3.5f;

    private List<Block> allBlocks = new List<Block>();
    private int totalBlockCount = 0;
    private int destroyedCount = 0;
    private bool levelEnded = false;
    private bool detonated = false;

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
        toolPlacer = FindAnyObjectByType<ToolPlacer>();

        LevelSpawner spawner = FindAnyObjectByType<LevelSpawner>();
        if (spawner == null)
        {
            Block[] blocks = FindObjectsByType<Block>(FindObjectsInactive.Exclude);
            RegisterBlocks(new List<Block>(blocks));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            OnDetonatePressed();
    }

    public void RegisterBlocks(List<Block> blocks)
    {
        allBlocks = blocks;
        totalBlockCount = blocks.Count;
        destroyedCount = 0;
        levelEnded = false;
        detonated = false;

        Debug.Log($"Уровень загружен. Блоков: {totalBlockCount}");
    }

    public void OnBlockDestroyed(Block block)
    {
        allBlocks.Remove(block);
        destroyedCount++;

        if (!levelEnded && detonated)
        {
            CancelInvoke(nameof(EvaluateResult));
            Invoke(nameof(EvaluateResult), resultDelayAfterLastHit);
        }
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

        int coins = stars * coinsPerStar;

        Debug.Log($"Готово. Разрушено: {destroyedPercent:P0} | Звёзд: {stars} | Монет: {coins}");

        if (UIManager.Instance != null)
            UIManager.Instance.ShowResult(stars, destroyedPercent, coins);
    }

    public void OnDetonatePressed()
    {
        detonated = true;

        if (toolPlacer != null)
            toolPlacer.DetonateAll();
        else
            Debug.LogWarning("ToolPlacer не найден.");

        CancelInvoke(nameof(EvaluateResult));
        Invoke(nameof(EvaluateResult), resultDelayAfterLastHit);
    }
}