using UnityEngine;
using UnityEngine.UI;

public class ToolSelectionUI : MonoBehaviour
{
    [Header("Ссылки")]
    public ToolPlacer toolPlacer;
    public Button explosiveButton;
    public Button sawButton;
    public Button airCannonButton;

    [Header("Визуальное выделение")]
    public Image explosiveHighlight;
    public Image sawHighlight;
    public Image airCannonHighlight;
    public Color selectedColor = new Color(1f, 0.8f, 0.2f);
    public Color unselectedColor = new Color(1f, 1f, 1f, 0.5f);

    private void Start()
    {
        if (explosiveButton != null)
            explosiveButton.onClick.AddListener(() => SelectTool(ToolType.Explosive));

        if (sawButton != null)
            sawButton.onClick.AddListener(() => SelectTool(ToolType.Saw));

        if (airCannonButton != null)
            airCannonButton.onClick.AddListener(() => SelectTool(ToolType.AirCannon));

        SelectTool(ToolType.Explosive);
    }

    private void SelectTool(ToolType type)
    {
        if (toolPlacer != null)
            toolPlacer.SetToolType(type);

        if (explosiveHighlight != null)
            explosiveHighlight.color = (type == ToolType.Explosive) ? selectedColor : unselectedColor;

        if (sawHighlight != null)
            sawHighlight.color = (type == ToolType.Saw) ? selectedColor : unselectedColor;

        if (airCannonHighlight != null)
            airCannonHighlight.color = (type == ToolType.AirCannon) ? selectedColor : unselectedColor;
    }
}