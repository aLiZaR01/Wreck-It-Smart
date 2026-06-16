using UnityEngine;

public class PlacedExplosive : MonoBehaviour
{
    [Header("Параметры взрыва")]
    public float radius = 3f;
    public float force = 4f;      // только физический импульс, урон считается в Block
    public LayerMask blockLayer;

    [Header("Эффекты")]
    public GameObject explosionEffect;

    public void Detonate()
    {
        Vector2 pos = transform.position;

        if (explosionEffect != null)
        {
            GameObject fx = Instantiate(explosionEffect, pos, Quaternion.identity);
            Destroy(fx, 2f);
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, radius, blockLayer);

        foreach (Collider2D hit in hits)
        {
            Block block = hit.GetComponent<Block>();
            if (block != null)
                block.ApplyExplosionForce(pos, force, radius);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
        Gizmos.DrawSphere(transform.position, radius);
        Gizmos.color = new Color(1f, 0.5f, 0f, 1f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}