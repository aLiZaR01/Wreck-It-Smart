using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Result Screen")]
    public GameObject resultPanel;
    public Image[] starIcons;
    public Color starFilledColor = new Color(1f, 0.85f, 0.2f);
    public Color starEmptyColor = new Color(0.3f, 0.3f, 0.3f);
    public TMP_Text percentText;
    public TMP_Text coinsText;

    [Header("Buttons")]
    public Button retryButton;
    public Button nextButton;
    public Button menuButton;

    [Header("Игровой HUD")]
    public TMP_Text toolCounterText;
    public ToolPlacer toolPlacer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (resultPanel != null)
            resultPanel.SetActive(false);

        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryPressed);

        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextPressed);

        if (menuButton != null)
            menuButton.onClick.AddListener(OnMenuPressed);
    }

    private void Update()
    {
        UpdateToolCounter();
    }

    private void UpdateToolCounter()
    {
        if (toolCounterText == null || toolPlacer == null) return;
        toolCounterText.text = $"{toolPlacer.GetPlacedCount()} / {toolPlacer.GetMaxTools()}";
    }

    public void ShowResult(int stars, float destroyedPercent, int coins)
    {
        if (resultPanel == null) return;

        resultPanel.SetActive(true);

        for (int i = 0; i < starIcons.Length; i++)
        {
            if (starIcons[i] == null) continue;
            starIcons[i].color = (i < stars) ? starFilledColor : starEmptyColor;
        }

        if (percentText != null)
            percentText.text = $"Разрушено: {Mathf.RoundToInt(destroyedPercent * 100f)}%";

        if (coinsText != null)
            coinsText.text = $"+{coins}";

        if (GameManager.Instance != null)
            GameManager.Instance.AddCoins(coins);
    }

    public void HideResult()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);
    }

    private void OnRetryPressed()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ReloadCurrentLevel();
    }

    private void OnNextPressed()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.LoadNextLevel();
    }

    private void OnMenuPressed()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.LoadMainMenu();
    }
}