using System.Collections.Generic;
using UnityEngine;

public class ProceduralLevelGenerator : MonoBehaviour
{
    [Header("Префаб блока")]
    public GameObject blockPrefab;

    [Header("Размер одного блока")]
    public Vector2 blockSize = new Vector2(1f, 1f);

    [Header("Параметры структуры")]
    public Vector2Int heightRange = new Vector2Int(8, 16);
    public Vector2Int widthRange = new Vector2Int(1, 4);
    [Range(0f, 0.4f)] public float jaggedTopChance = 0.25f;
    public float startY = -3f;

    [Header("Разрешённые типы структур")]
    public bool allowTower = true;
    public bool allowWideWall = true;
    public bool allowPyramid = true;
    public bool allowArch = true;

    // Цвета материалов — временные пока нет спрайтов
    private static readonly Color WoodColor = new Color(0.72f, 0.48f, 0.25f);
    private static readonly Color MetalColor = new Color(0.65f, 0.70f, 0.75f);
    private static readonly Color StoneColor = new Color(0.45f, 0.45f, 0.50f);
    private static readonly Color GlassColor = new Color(0.75f, 0.90f, 1.00f, 0.85f);

    private enum StructureType { Tower, WideWall, Pyramid, Arch }

    private struct BlockConfig
    {
        public BlockType type;
        public float maxHealth;
        public float mass;
        public Color color;
    }

    private List<Block> spawnedBlocks = new List<Block>();
    private Transform levelRoot;

    // Тип материала для всей структуры выбирается один раз за уровень
    private BlockConfig currentMaterial;

    public List<Block> Generate()
    {
        Clear();
        levelRoot = new GameObject("GeneratedLevel").transform;

        currentMaterial = PickMaterial();

        StructureType type = PickStructureType();
        switch (type)
        {
            case StructureType.Tower: GenerateTower(); break;
            case StructureType.WideWall: GenerateWideWall(); break;
            case StructureType.Pyramid: GeneratePyramid(); break;
            case StructureType.Arch: GenerateArch(); break;
        }

        return spawnedBlocks;
    }

    // -------------------------------------------------------------------
    // Выбор материала
    // -------------------------------------------------------------------

    private BlockConfig PickMaterial()
    {
        // Случайный материал, дерево встречается чаще
        int roll = Random.Range(0, 10);

        if (roll < 5)
            return new BlockConfig
            {
                type = BlockType.Wood,
                maxHealth = 80f,
                mass = 1f,
                color = WoodColor
            };

        if (roll < 8)
            return new BlockConfig
            {
                type = BlockType.Stone,
                maxHealth = 180f,
                mass = 2.5f,
                color = StoneColor
            };

        if (roll == 8)
            return new BlockConfig
            {
                type = BlockType.Metal,
                maxHealth = 300f,
                mass = 4f,
                color = MetalColor
            };

        // roll == 9
        return new BlockConfig
        {
            type = BlockType.Glass,
            maxHealth = 40f,
            mass = 0.5f,
            color = GlassColor
        };
    }

    // -------------------------------------------------------------------
    // Типы структур
    // -------------------------------------------------------------------

    private StructureType PickStructureType()
    {
        List<StructureType> options = new List<StructureType>();
        if (allowTower) options.Add(StructureType.Tower);
        if (allowWideWall) options.Add(StructureType.WideWall);
        if (allowPyramid) options.Add(StructureType.Pyramid);
        if (allowArch) options.Add(StructureType.Arch);
        if (options.Count == 0) return StructureType.Tower;
        return options[Random.Range(0, options.Count)];
    }

    // Вертикальная башня, слегка рваный верх
    private void GenerateTower()
    {
        int height = Random.Range(heightRange.x, heightRange.y + 1);

        for (int row = 0; row < height; row++)
        {
            float skipChance = row > height * 0.65f ? jaggedTopChance : 0f;
            if (row > 0 && Random.value < skipChance) continue;

            float offsetX = row > height * 0.5f ? Random.Range(-0.1f, 0.1f) : 0f;
            SpawnBlock(new Vector3(offsetX, startY + row * blockSize.y, 0f));
        }
    }

    // Широкая стена — несколько колонн
    private void GenerateWideWall()
    {
        int width = Random.Range(widthRange.x, widthRange.y + 1);
        int height = Random.Range(heightRange.x, heightRange.y + 1);

        float totalWidth = width * blockSize.x;
        float startX = -totalWidth * 0.5f + blockSize.x * 0.5f;

        for (int row = 0; row < height; row++)
            for (int col = 0; col < width; col++)
            {
                bool nearTop = row >= height - 2;
                if (nearTop && Random.value < jaggedTopChance * 1.5f) continue;

                Vector3 pos = new Vector3(
                    startX + col * blockSize.x,
                    startY + row * blockSize.y,
                    0f
                );
                SpawnBlock(pos);
            }
    }

    // Пирамида — каждый следующий ряд уже предыдущего
    private void GeneratePyramid()
    {
        int baseWidth = Random.Range(widthRange.x + 1, widthRange.y + 2);
        int height = baseWidth; // высота = ширина основания

        for (int row = 0; row < height; row++)
        {
            int rowWidth = baseWidth - row;
            if (rowWidth <= 0) break;

            float totalWidth = rowWidth * blockSize.x;
            float startX = -totalWidth * 0.5f + blockSize.x * 0.5f;

            for (int col = 0; col < rowWidth; col++)
            {
                Vector3 pos = new Vector3(
                    startX + col * blockSize.x,
                    startY + row * blockSize.y,
                    0f
                );
                SpawnBlock(pos);
            }
        }
    }

    // Арка — две колонны и перекладина сверху
    private void GenerateArch()
    {
        int legHeight = Random.Range(4, 9);
        int spanWidth = Random.Range(2, 5); // расстояние между ногами в блоках

        float halfSpan = (spanWidth * blockSize.x) * 0.5f + blockSize.x * 0.5f;

        // Левая нога
        for (int row = 0; row < legHeight; row++)
            SpawnBlock(new Vector3(-halfSpan, startY + row * blockSize.y, 0f));

        // Правая нога
        for (int row = 0; row < legHeight; row++)
            SpawnBlock(new Vector3(halfSpan, startY + row * blockSize.y, 0f));

        // Перекладина
        int totalSpanBlocks = spanWidth + 2;
        float spanStartX = -halfSpan;

        for (int col = 0; col < totalSpanBlocks; col++)
        {
            Vector3 pos = new Vector3(
                spanStartX + col * blockSize.x,
                startY + legHeight * blockSize.y,
                0f
            );
            SpawnBlock(pos);
        }

        // Дополнительный ряд поверх перекладины
        if (Random.value > 0.4f)
        {
            int topWidth = Random.Range(1, totalSpanBlocks);
            float topStartX = -(topWidth * blockSize.x * 0.5f) + blockSize.x * 0.5f;

            for (int col = 0; col < topWidth; col++)
            {
                Vector3 pos = new Vector3(
                    topStartX + col * blockSize.x,
                    startY + (legHeight + 1) * blockSize.y,
                    0f
                );
                SpawnBlock(pos);
            }
        }
    }

    // -------------------------------------------------------------------
    // Спавн блока
    // -------------------------------------------------------------------

    private void SpawnBlock(Vector3 worldPosition)
    {
        if (blockPrefab == null)
        {
            Debug.LogError("ProceduralLevelGenerator: blockPrefab не назначен.");
            return;
        }

        GameObject obj = Instantiate(blockPrefab, levelRoot);
        obj.transform.position = worldPosition;

        Block block = obj.GetComponent<Block>();
        if (block != null)
        {
            block.blockType = currentMaterial.type;
            block.maxHealth = currentMaterial.maxHealth;
            block.mass = currentMaterial.mass;
            spawnedBlocks.Add(block);
        }

        // Применяем цвет материала через SpriteRenderer
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = currentMaterial.color;

        // Применяем массу к Rigidbody2D
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.mass = currentMaterial.mass;
    }

    public void Clear()
    {
        if (levelRoot != null)
            Destroy(levelRoot.gameObject);

        spawnedBlocks.Clear();
    }
}