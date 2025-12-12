using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAlign : MonoBehaviour
{
    NavMeshAgent agent;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Call this after you decide final spawn point (pass navmesh position)
    public void AlignToNavMeshAt(Vector3 navPos)
    {
        // place root at navPos first (so renderer bounds are in world)
        transform.position = navPos;

        // compute visual bounds
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        if (rends == null || rends.Length == 0)
        {
            // no renderers: just warp the agent and return
            agent.Warp(navPos);
            return;
        }

        Bounds combined = rends[0].bounds;
        for (int i = 1; i < rends.Length; i++) combined.Encapsulate(rends[i].bounds);

        float visualBottom = combined.min.y;
        float desiredBottom = navPos.y;
        float deltaUp = desiredBottom - visualBottom;

        // shift visuals up
        transform.position += Vector3.up * deltaUp;

        // Now ensure agent is on navmesh. Use warp.
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
        else
        {
            // fallback: warp to provided navPos
            agent.Warp(navPos);
        }

        // Optionally set baseOffset to 0 (we aligned visuals), but if you want to adapt:
        agent.baseOffset = 0f;
    }
}
