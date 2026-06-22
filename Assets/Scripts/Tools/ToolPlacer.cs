using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolPlacer : MonoBehaviour
{
    [Header("Настройки")]
    public int maxTools = 3;
    public GameObject explosivePrefab;
    public GameObject sawPrefab;
    public GameObject airCannonPrefab;
    public LayerMask blockLayer;
    public Camera mainCamera;

    [Header("Текущий инструмент")]
    public ToolType currentToolType = ToolType.Explosive;

    private List<MonoBehaviour> placedTools = new List<MonoBehaviour>();
    private bool detonated = false;

    private void Update()
    {
        if (detonated) return;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (!IsPointerOverUI(Input.GetTouch(0).fingerId))
                HandleInput(Input.GetTouch(0).position);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUI(-1))
                HandleInput(Input.mousePosition);
        }
    }

    private bool IsPointerOverUI(int fingerId)
    {
        if (EventSystem.current == null) return false;

        return fingerId == -1
            ? EventSystem.current.IsPointerOverGameObject()
            : EventSystem.current.IsPointerOverGameObject(fingerId);
    }

    private void HandleInput(Vector2 screenPos)
    {
        if (mainCamera == null) return;

        Vector2 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, blockLayer);

        if (hit.collider == null) return;

        for (int i = 0; i < placedTools.Count; i++)
        {
            if (placedTools[i] == null) continue;

            if (placedTools[i].transform.parent == hit.collider.transform)
            {
                Destroy(placedTools[i].gameObject);
                placedTools.RemoveAt(i);
                return;
            }
        }

        if (placedTools.Count < maxTools)
            PlaceTool(hit.point, hit.collider.transform);
    }

    private void PlaceTool(Vector2 position, Transform parent)
    {
        GameObject prefab = currentToolType switch
        {
            ToolType.Explosive => explosivePrefab,
            ToolType.Saw => sawPrefab,
            ToolType.AirCannon => airCannonPrefab,
            _ => null
        };

        if (prefab == null)
        {
            Debug.LogWarning($"Префаб для {currentToolType} не назначен в ToolPlacer.");
            return;
        }

        GameObject tool = Instantiate(prefab, position, Quaternion.identity);
        tool.transform.SetParent(parent);

        IPlaceableTool placeable = tool.GetComponent<IPlaceableTool>();
        if (placeable is MonoBehaviour toolBehaviour)
            placedTools.Add(toolBehaviour);
        else
            Debug.LogWarning($"Префаб {prefab.name} не реализует IPlaceableTool.");
    }

    public void SetToolType(ToolType type)
    {
        currentToolType = type;
    }

    public void DetonateAll()
    {
        if (detonated) return;
        detonated = true;

        foreach (MonoBehaviour tool in placedTools)
        {
            if (tool == null) continue;
            if (tool is IPlaceableTool placeable)
                placeable.Detonate();
        }

        placedTools.Clear();
    }

    public int GetPlacedCount() => placedTools.Count;
    public int GetMaxTools() => maxTools;
}