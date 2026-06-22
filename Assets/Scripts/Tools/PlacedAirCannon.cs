using UnityEngine;

public class PlacedAirCannon : MonoBehaviour, IPlaceableTool
{
    [Header("Параметры толчка")]
    public float pushForce = 35f;
    public float damagePerForce = 1.8f;
    public Vector2 boxSize = new Vector2(8f, 3f);
    public LayerMask blockLayer;

    [Header("Направление (0,0 = авто по позиции)")]
    public Vector2 overrideDirection = Vector2.zero;

    [Header("Эффекты")]
    public GameObject pushEffect;

    private Vector2 resolvedDirection;

    private void Start()
    {
        resolvedDirection = ResolveDirection();
    }

    private Vector2 ResolveDirection()
    {
        if (overrideDirection != Vector2.zero)
            return overrideDirection.normalized;

        Vector2 constructionCenter = Vector2.zero;
        Vector2 fromCenter = (Vector2)transform.position - constructionCenter;

        if (fromCenter.magnitude < 0.5f)
            return Vector2.right;

        return fromCenter.normalized;
    }

    public void Detonate()
    {
        Vector2 pos = transform.position;
        Vector2 dir = resolvedDirection;

        if (pushEffect != null)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            GameObject fx = Instantiate(pushEffect, pos, Quaternion.Euler(0f, 0f, angle));
            Destroy(fx, 2f);
        }

        Vector2 boxCenter = pos + dir * (boxSize.x * 0.5f);
        float boxAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, boxSize, boxAngle, blockLayer);

        foreach (Collider2D hit in hits)
        {
            Rigidbody2D rb = hit.attachedRigidbody;
            Block block = hit.GetComponent<Block>();

            if (rb == null) continue;

            float distance = Vector2.Distance(pos, hit.transform.position);
            float maxDistance = Mathf.Max(boxSize.x, 0.1f);
            float falloff = Mathf.Clamp01(1f - (distance / maxDistance));

            // Минимальный falloff 0.4 чтобы даже дальние блоки ощутили удар
            float effectiveFalloff = Mathf.Max(falloff, 0.4f);
            float actualForce = pushForce * effectiveFalloff;

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(dir * actualForce, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-25f, 25f));

            if (block != null)
            {
                float damage = actualForce * damagePerForce;
                block.ApplyDirectDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Vector2 dir = overrideDirection != Vector2.zero
            ? overrideDirection.normalized
            : Vector2.right;

        Vector2 pos = transform.position;
        Vector2 boxCenter = pos + dir * (boxSize.x * 0.5f);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Gizmos.color = new Color(0.4f, 0.8f, 1f, 0.3f);
        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, Quaternion.Euler(0f, 0f, angle), Vector3.one);
        Gizmos.DrawCube(Vector3.zero, new Vector3(boxSize.x, boxSize.y, 0.1f));
        Gizmos.matrix = old;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pos, pos + dir * 2f);
    }
}