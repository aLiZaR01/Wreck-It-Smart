using UnityEngine;

public class PlacedSaw : MonoBehaviour, IPlaceableTool
{
    [Header("Параметры реза")]
    public float cutWidth = 0.4f;

    // Высота реза намеренно маленькая — только сам блок на котором стоит пила
    // и его ближайшие соседи по вертикали (зазор для точного контакта)
    public float cutHeight = 1.2f;

    public LayerMask blockLayer;

    [Header("Эффекты")]
    public GameObject cutEffect;

    public void Detonate()
    {
        Vector2 pos = transform.position;

        if (cutEffect != null)
        {
            GameObject fx = Instantiate(cutEffect, pos, Quaternion.identity);
            Destroy(fx, 1.5f);
        }

        // Маленький прямоугольник — только в месте размещения
        Vector2 boxSize = new Vector2(cutWidth, cutHeight);
        Collider2D[] hits = Physics2D.OverlapBoxAll(pos, boxSize, 0f, blockLayer);

        foreach (Collider2D hit in hits)
        {
            Block block = hit.GetComponent<Block>();
            if (block != null)
                block.CutThrough();
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        // Теперь в Scene View видно реальный маленький прямоугольник
        Gizmos.color = new Color(0.8f, 0.8f, 1f, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(cutWidth, cutHeight, 0.1f));
        Gizmos.color = new Color(0.8f, 0.8f, 1f, 1f);
        Gizmos.DrawWireCube(transform.position, new Vector3(cutWidth, cutHeight, 0.1f));
    }
}