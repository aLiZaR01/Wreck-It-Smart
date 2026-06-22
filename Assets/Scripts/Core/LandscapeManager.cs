using UnityEngine;

public class LandscapeManager : MonoBehaviour
{
    [Header("Камера")]
    public Camera targetCamera;

    [Header("Варианты фона (пока цвет — арт фонов ещё не готов)")]
    public Color[] backgroundColors;

    [Header("Опционально — спрайты фона, когда появится арт")]
    public SpriteRenderer backgroundSpriteRenderer;
    public Sprite[] backgroundSprites;

    public void ApplyRandomLandscape()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (backgroundSprites != null && backgroundSprites.Length > 0 && backgroundSpriteRenderer != null)
        {
            backgroundSpriteRenderer.sprite = backgroundSprites[Random.Range(0, backgroundSprites.Length)];
            return;
        }

        if (backgroundColors != null && backgroundColors.Length > 0 && targetCamera != null)
            targetCamera.backgroundColor = backgroundColors[Random.Range(0, backgroundColors.Length)];
    }
}