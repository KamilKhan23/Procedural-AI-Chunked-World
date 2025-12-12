using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public class ChunkManager : MonoBehaviour
{
    public LevelGenerator levelGenerator;
    public Transform chunksRoot;
    public Transform player;
    public int loadRadius = 1;

    [Header("NavMesh")]
    public NavMeshSurface navSurface;
    public float bakeCooldown = 0.5f;

    [Header("Enemy Spawning")]
    public GameObject enemyPrefab;          // assign prefab
    public int minEnemiesPerChunk = 1;
    public int maxEnemiesPerChunk = 3;
    public LayerMask floorLayerMask;        // optional: layer for floor if you raycast

    private float lastBakeTime = -999f;
    private Dictionary<Vector2Int, GameObject> activeChunks = new Dictionary<Vector2Int, GameObject>();
    private List<Vector2Int> chunksPendingSpawn = new List<Vector2Int>();

    void Start()
    {
        if (chunksRoot == null)
        {
            GameObject cr = new GameObject("ChunksRoot");
            chunksRoot = cr.transform;
        }
        UpdateChunks();
    }

    void Update()
    {
        UpdateChunks();
        // Bake & spawn if there are pending chunks and cooldown passed
        if (navSurface != null && chunksPendingSpawn.Count > 0 && Time.time - lastBakeTime > bakeCooldown)
        {
            navSurface.BuildNavMesh();
            lastBakeTime = Time.time;
            // After bake, spawn enemies for all pending chunks
            foreach (var coord in chunksPendingSpawn.ToArray())
            {
                if (activeChunks.ContainsKey(coord))
                    SpawnEnemiesInChunk(activeChunks[coord]);
            }
            chunksPendingSpawn.Clear();
        }
    }

    void UpdateChunks()
    {
        if (player == null || levelGenerator == null) return;

        int cx = Mathf.FloorToInt(player.position.x / levelGenerator.ChunkWorldWidth);
        int cy = Mathf.FloorToInt(player.position.z / levelGenerator.ChunkWorldHeight);

        HashSet<Vector2Int> desired = new HashSet<Vector2Int>();
        for (int dx = -loadRadius; dx <= loadRadius; dx++)
            for (int dy = -loadRadius; dy <= loadRadius; dy++)
                desired.Add(new Vector2Int(cx + dx, cy + dy));

        // create missing
        foreach (var coord in desired)
        {
            if (!activeChunks.ContainsKey(coord))
            {
                GameObject chunk = levelGenerator.GenerateChunk(coord.x, coord.y, chunksRoot);
                activeChunks.Add(coord, chunk);
                // schedule for spawning after navmesh build
                chunksPendingSpawn.Add(coord);
            }
        }

        // remove no longer needed
        var removeList = activeChunks.Keys.Where(k => !desired.Contains(k)).ToList();
        foreach (var r in removeList)
        {
            levelGenerator.DestroyChunk(activeChunks[r]);
            activeChunks.Remove(r);
        }
    }

    // Spawn enemies inside a chunk GameObject

    void SpawnEnemiesInChunk(GameObject chunk)
    {
        if (enemyPrefab == null) return;

        int count = Random.Range(minEnemiesPerChunk, maxEnemiesPerChunk + 1);

        // Collect candidate NavMesh positions inside the chunk
        List<Vector3> spawnCandidates = new List<Vector3>();

        foreach (Transform child in chunk.transform)
        {
            if (child.name.ToLower().Contains("floor"))
            {
                Vector3 center = child.position + Vector3.up * 0.3f;

                for (int i = 0; i < 4; i++)
                {
                    Vector3 samplePos = center +
                        new Vector3(Random.Range(-0.6f, 0.6f), 0, Random.Range(-0.6f, 0.6f));

                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(samplePos, out hit, 1.5f, NavMesh.AllAreas))
                        spawnCandidates.Add(hit.position);
                }
            }
        }

        // Fallback if no floor samples
        if (spawnCandidates.Count == 0)
        {
            Vector3 center = chunk.transform.position +
                new Vector3(levelGenerator.ChunkWorldWidth * 0.5f, 0,
                            levelGenerator.ChunkWorldHeight * 0.5f);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(center, out hit, 5f, NavMesh.AllAreas))
                spawnCandidates.Add(hit.position);
        }

        // Spawn enemies at sampled navmesh points
        for (int i = 0; i < count && spawnCandidates.Count > 0; i++)
        {
            int idx = Random.Range(0, spawnCandidates.Count);
            Vector3 navPos = spawnCandidates[idx];
            spawnCandidates.RemoveAt(idx);

            GameObject e = Instantiate(enemyPrefab, navPos, Quaternion.identity, chunk.transform);

            // ALIGN using EnemyAlign.cs
            EnemyAlign align = e.GetComponent<EnemyAlign>();
            if (align != null)
                align.AlignToNavMeshAt(navPos);

            // optional random agent speed
            NavMeshAgent agent = e.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.speed = Random.Range(2f, 3.5f);
                agent.angularSpeed = 540f;
                agent.acceleration = 22f;
            }
        }
    }
}
