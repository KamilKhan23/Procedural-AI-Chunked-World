using UnityEngine;

public class ObstaclePlacer : MonoBehaviour
{
    public LayerMask spawnAvoidMask;
    public float avoidRadius = 1.5f;

    public bool CanPlace(Vector3 pos)
    {
        Collider[] cols = Physics.OverlapSphere(pos, avoidRadius, spawnAvoidMask);
        return cols.Length == 0;
    }
}
