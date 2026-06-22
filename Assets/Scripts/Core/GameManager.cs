using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Прогресс игрока")]
    public int totalCoins = 0;
    public int currentLevelIndex = 0;

    private const string CoinsKey = "WIS_TotalCoins";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        totalCoins = PlayerPrefs.GetInt(CoinsKey, 0);
    }

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        PlayerPrefs.SetInt(CoinsKey, totalCoins);
        PlayerPrefs.Save();
        Debug.Log($"Монет всего: {totalCoins}");
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;
        SceneManager.LoadScene("Game");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ReloadCurrentLevel()
    {
        SceneManager.LoadScene("Game");
    }
}