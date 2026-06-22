using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    [Header("Ссылки")]
    public ProceduralLevelGenerator generator;
    public LandscapeManager landscapeManager;

    [Header("Ручные уровни (пока 0 — позже подключим LevelData)")]
    public int designedLevelCount = 0;

    private void Start()
    {
        int levelIndex = GameManager.Instance != null ? GameManager.Instance.currentLevelIndex : 0;

        if (levelIndex < designedLevelCount)
        {
            Debug.LogWarning("Ручные уровни ещё не подключены — используется генератор.");
        }

        if (landscapeManager != null)
            landscapeManager.ApplyRandomLandscape();

        if (generator == null)
        {
            Debug.LogError("LevelSpawner: generator не назначен.");
            return;
        }

        var blocks = generator.Generate();

        if (LevelManager.Instance != null)
            LevelManager.Instance.RegisterBlocks(blocks);
    }
}