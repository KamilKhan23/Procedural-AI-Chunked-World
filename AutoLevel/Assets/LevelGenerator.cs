using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Chunk / Grid Settings")]
    public int chunkWidth = 20;
    public int chunkHeight = 12;
    public float cellSize = 2f;

    [Header("Prefabs")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject obstaclePrefab;

    [Header("Generation Settings")]
    [Range(0f, 1f)] public float obstacleDensity = 0.08f;
    [Range(0f, 1f)] public float wallDensity = 0.04f; // NEW: interior wall chance
    public bool randomSeed = true;
    public int seed = 0;

    public enum Difficulty { Easy, Medium, Hard }
    public Difficulty difficulty = Difficulty.Easy;

    public float ChunkWorldWidth => chunkWidth * cellSize;
    public float ChunkWorldHeight => chunkHeight * cellSize;

    public GameObject GenerateChunk(int cx, int cy, Transform parent)
    {
        if (!floorPrefab || !wallPrefab || !obstaclePrefab)
        {
            Debug.LogError("Assign all prefabs!");
            return null;
        }

        // Seed
        int chunkSeed = (randomSeed ? Random.Range(int.MinValue, int.MaxValue) : seed)
                        ^ (cx * 73856093) ^ (cy * 19349663);
        System.Random rnd = new System.Random(chunkSeed);

        // Create Chunk GO
        GameObject chunkGO = new GameObject($"chunk_{cx}_{cy}");
        chunkGO.transform.parent = parent;
        chunkGO.transform.position = new Vector3(cx * ChunkWorldWidth, 0, cy * ChunkWorldHeight);

        List<Vector2Int> floorCells = new List<Vector2Int>();

        // -----------------------------
        // Generate Floor Tiles
        // -----------------------------
        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                Vector3 localPos = new Vector3(x * cellSize, 0, y * cellSize);
                GameObject f = Instantiate(floorPrefab, chunkGO.transform);
                f.transform.localPosition = localPos;
                f.transform.localScale = new Vector3(cellSize, 1, cellSize);

                floorCells.Add(new Vector2Int(x, y));
            }
        }

        // -----------------------------
        // Generate Obstacles
        // -----------------------------
        float density = GetDensityForDifficulty();

        foreach (Vector2Int cell in floorCells)
        {
            if (rnd.NextDouble() < density)
            {
                Vector3 pos = new Vector3(cell.x * cellSize, 0.5f, cell.y * cellSize);
                GameObject ob = Instantiate(obstaclePrefab, chunkGO.transform);
                ob.transform.localPosition = pos;
                ob.transform.localRotation = Quaternion.Euler(0, (float)rnd.NextDouble() * 360f, 0);
            }
        }

        // -----------------------------
        // Random Interior Walls (No Borders)
        // -----------------------------
        for (int i = 0; i < floorCells.Count; i++)
        {
            if (rnd.NextDouble() < wallDensity)
            {
                Vector2Int c = floorCells[i];

                // Avoid borders so chunks remain open
                if (c.x == 0 || c.y == 0 || c.x == chunkWidth - 1 || c.y == chunkHeight - 1)
                    continue;

                Vector3 pos = new Vector3(c.x * cellSize, 1f, c.y * cellSize);
                GameObject w = Instantiate(wallPrefab, chunkGO.transform);
                w.transform.localPosition = pos;

                // Random rotation: vertical or horizontal
                bool vertical = rnd.NextDouble() > 0.5;
                float angle = vertical ? 0f : 90f;
                w.transform.localRotation = Quaternion.Euler(0, angle, 0);

                // Scale wall to span across cell
                Vector3 scale = w.transform.localScale;
                w.transform.localScale = new Vector3(scale.x, scale.y, cellSize * 2f);
            }
        }

        return chunkGO;
    }

    public void DestroyChunk(GameObject chunk)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) DestroyImmediate(chunk);
        else Destroy(chunk);
#else
        Destroy(chunk);
#endif
    }

    float GetDensityForDifficulty()
    {
        switch (difficulty)
        {
            case Difficulty.Easy: return obstacleDensity * 0.6f;
            case Difficulty.Medium: return obstacleDensity * 1.0f;
            case Difficulty.Hard: return obstacleDensity * 1.6f;
            default: return obstacleDensity;
        }
    }
}
