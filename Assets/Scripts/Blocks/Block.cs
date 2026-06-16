using UnityEngine;

public class Block : MonoBehaviour
{
    [Header("Настройки блока")]
    public BlockType blockType = BlockType.Wood;
    public float maxHealth = 100f;
    public float mass = 1f;

    [Header("Спрайты состояний")]
    public Sprite spriteNormal;
    public Sprite spriteDamaged;   // показывается при здоровье < 50%

    private float currentHealth;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool isBroken = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        rb.mass = mass;
        rb.linearDamping = 2f;
        rb.angularDamping = 2f;

        currentHealth = maxHealth;

        if (spriteNormal != null)
            sr.sprite = spriteNormal;
    }

    // Вызывается только взрывом — НЕ от столкновения с полом
    public void ApplyExplosionForce(Vector2 origin, float force, float radius)
    {
        if (isBroken) return;

        Vector2 direction = (Vector2)transform.position - origin;
        float distance = Mathf.Max(direction.magnitude, 0.1f);
        float falloff = Mathf.Clamp01(1f - (distance / radius));

        // Урон пропорционален близости к взрыву
        float damage = maxHealth * falloff;
        TakeDamage(damage);

        // Физический импульс — отдельно и небольшой
        float physicsForce = force * falloff;
        rb.AddForce(direction.normalized * physicsForce, ForceMode2D.Impulse);
        rb.AddTorque(Random.Range(-20f, 20f));
    }

    private void TakeDamage(float damage)
    {
        if (isBroken) return;

        currentHealth -= damage;
        UpdateVisual();

        if (currentHealth <= 0f)
            Break();
    }

    private void UpdateVisual()
    {
        float percent = currentHealth / maxHealth;

        if (percent <= 0.5f && spriteDamaged != null)
            sr.sprite = spriteDamaged;
    }

    public void Break()
    {
        if (isBroken) return;
        isBroken = true;

        SpawnFragments();

        if (LevelManager.Instance != null)
            LevelManager.Instance.OnBlockDestroyed(this);

        Destroy(gameObject);
    }

    private void SpawnFragments()
    {
        // Если нет готовых фрагментов — создаём автоматически из блока
        Vector3 size = transform.localScale;
        Color col = sr != null ? sr.color : Color.white;
        Sprite spr = sr != null ? sr.sprite : null;

        int count = 4;
        for (int i = 0; i < count; i++)
        {
            GameObject frag = new GameObject("Fragment");
            frag.layer = gameObject.layer;

            // Позиция — случайно внутри родительского блока
            frag.transform.position = transform.position + new Vector3(
                Random.Range(-size.x * 0.25f, size.x * 0.25f),
                Random.Range(-size.y * 0.25f, size.y * 0.25f),
                0f
            );
            frag.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            frag.transform.localScale = size * 0.48f;

            // Визуал
            SpriteRenderer fragSr = frag.AddComponent<SpriteRenderer>();
            fragSr.sprite = spr;
            fragSr.color = new Color(col.r * 0.75f, col.g * 0.75f, col.b * 0.75f, 1f);
            fragSr.sortingOrder = 2;

            // Физика
            Rigidbody2D fragRb = frag.AddComponent<Rigidbody2D>();
            fragRb.mass = 0.3f;
            fragRb.linearDamping = 1.5f;
            fragRb.angularDamping = 1.5f;
            fragRb.AddForce(Random.insideUnitCircle.normalized * 1.5f, ForceMode2D.Impulse);
            fragRb.AddTorque(Random.Range(-30f, 30f));

            // Коллайдер чтобы не проваливались в пол
            frag.AddComponent<BoxCollider2D>();

            // Исчезают через 6 секунд
            Destroy(frag, 6f);
        }
    }
}