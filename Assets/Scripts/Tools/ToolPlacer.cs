using System.Collections.Generic;
using UnityEngine;

public class ToolPlacer : MonoBehaviour
{
    [Header("Настройки")]
    public int maxTools = 3;
    public GameObject explosivePrefab;
    public LayerMask blockLayer;
    public Camera mainCamera;

    private List<PlacedExplosive> placedTools = new List<PlacedExplosive>();
    private bool detonated = false;

    private void Update()
    {
        if (detonated) return;

        // Тап на мобиле
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            HandleInput(Input.GetTouch(0).position);

        // Мышь в редакторе
        if (Input.GetMouseButtonDown(0))
            HandleInput(Input.mousePosition);
    }

    private void HandleInput(Vector2 screenPos)
    {
        if (mainCamera == null) return;

        Vector2 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, blockLayer);

        if (hit.collider != null)
        {
            // Если на блоке уже стоит инструмент — снять
            PlacedExplosive existing =
                hit.collider.GetComponentInParent<PlacedExplosive>();

            if (existing != null)
            {
                placedTools.Remove(existing);
                Destroy(existing.gameObject);
                return;
            }

            // Поставить новый если не превышен лимит
            if (placedTools.Count < maxTools)
                PlaceTool(hit.point, hit.collider.transform);
        }
    }

    private void PlaceTool(Vector2 position, Transform parent)
    {
        if (explosivePrefab == null) return;

        GameObject tool = Instantiate(explosivePrefab, position, Quaternion.identity);
        tool.transform.SetParent(parent);

        PlacedExplosive explosive = tool.GetComponent<PlacedExplosive>();
        if (explosive != null)
            placedTools.Add(explosive);
    }

    public void DetonateAll()
    {
        if (detonated) return;
        detonated = true;

        foreach (PlacedExplosive tool in placedTools)
        {
            if (tool != null)
                tool.Detonate();
        }

        placedTools.Clear();
    }

    public int GetPlacedCount() => placedTools.Count;
    public int GetMaxTools() => maxTools;
}