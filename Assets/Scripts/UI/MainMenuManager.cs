using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Refs")]
    public Button playButton;
    public TMP_Text coinsText;

    private void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayPressed);

        UpdateCoinsDisplay();
    }

    private void UpdateCoinsDisplay()
    {
        if (coinsText != null && GameManager.Instance != null)
            coinsText.text = GameManager.Instance.totalCoins.ToString();
    }

    private void OnPlayPressed()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.LoadGame();
        else
            Debug.LogWarning("GameManager не найден — создай его в стартовой сцене.");
    }
}