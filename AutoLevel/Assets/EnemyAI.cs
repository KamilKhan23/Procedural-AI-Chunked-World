using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    public enum Difficulty { Easy, Medium, Hard }

    [Header("Agent")]
    public NavMeshAgent agent;
    public Difficulty difficulty = Difficulty.Medium;

    [Header("Perception")]
    public float viewRadius = 8f;
    [Range(0f, 360f)] public float viewAngle = 90f;
    public LayerMask obstructionMask;    // e.g. Default
    public LayerMask playerMask;         // assign Player layer or use tag detection

    [Header("Patrol")]
    public int patrolPointCount = 4;
    public float patrolRadius = 4f;
    public float idleTimeMin = 1f;
    public float idleTimeMax = 3f;

    [Header("Chase / Search")]
    public float chaseSpeed = 4f;
    public float patrolSpeed = 2f;
    public float loseSightTime = 3f;     // seconds to switch to search when player lost
    public float searchDuration = 6f;    // how long to search before returning to patrol
    public float attackDistance = 1.2f;

    [Header("Debug")]
    public bool drawGizmos = true;

    // internal state
    enum State { Patrol, Idle, Chase, Search }
    State state = State.Patrol;

    Transform player;
    Vector3 lastKnownPlayerPos;
    float lastTimeSeen = -999f;
    float idleTimer = 0f;
    float searchTimer = 0f;

    List<Vector3> patrolPoints = new List<Vector3>();
    int currentPatrolIndex = 0;

    // difficulty multiplier (tweakable)
    float difficultyMultiplier => (difficulty == Difficulty.Easy) ? 0.8f : (difficulty == Difficulty.Medium) ? 1f : 1.25f;

    void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = attackDistance;
        agent.autoBraking = true;

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
    }

    void Start()
    {
        // apply difficulty-based params
        patrolSpeed *= difficultyMultiplier;
        chaseSpeed *= difficultyMultiplier;
        agent.speed = patrolSpeed;

        BuildPatrolPoints();
        GoToPatrolPoint();
    }

    void Update()
    {
        switch (state)
        {
            case State.Patrol:
                UpdatePatrol();
                break;
            case State.Idle:
                UpdateIdle();
                break;
            case State.Chase:
                UpdateChase();
                break;
            case State.Search:
                UpdateSearch();
                break;
        }

        // Perception check every frame (cheap)
        if (CanSeePlayer())
        {
            lastKnownPlayerPos = player.position;
            lastTimeSeen = Time.time;
            if (state != State.Chase)
            {
                EnterChase();
            }
        }
    }

    #region States
    void UpdatePatrol()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            // reached patrol point -> idle
            state = State.Idle;
            idleTimer = Random.Range(idleTimeMin, idleTimeMax);
            agent.isStopped = true;
        }
    }

    void UpdateIdle()
    {
        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0f)
        {
            // next patrol
            state = State.Patrol;
            agent.isStopped = false;
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
            GoToPatrolPoint();
        }

        // If player detected during idle, chase
        if (Time.time - lastTimeSeen < 0.5f && CanSeePlayer())
        {
            EnterChase();
        }
    }

    void UpdateChase()
    {
        if (player == null)
        {
            ReturnToPatrol();
            return;
        }

        agent.SetDestination(player.position);

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= attackDistance)
        {
            // reached attack range - stop and could perform attack
            agent.isStopped = true;
            // (optional) call Attack() here
        }
        else
        {
            agent.isStopped = false;
        }

        // If we haven't seen the player recently, start Search
        if (Time.time - lastTimeSeen > loseSightTime)
        {
            EnterSearch();
        }
    }

    void UpdateSearch()
    {
        if (agent.remainingDistance <= agent.stoppingDistance + 0.2f && !agent.pathPending)
        {
            // look around by rotating in place for a moment
            searchTimer -= Time.deltaTime;
            transform.Rotate(Vector3.up, 60f * Time.deltaTime); // look-around rotation

            if (searchTimer <= 0f)
            {
                ReturnToPatrol();
            }
        }

        // If player reappears while searching, resume chase
        if (CanSeePlayer())
        {
            EnterChase();
        }
    }
    #endregion

    #region Transitions
    void EnterChase()
    {
        state = State.Chase;
        agent.speed = chaseSpeed;
        agent.isStopped = false;
    }

    void EnterSearch()
    {
        state = State.Search;
        searchTimer = searchDuration;
        agent.isStopped = false;
        // go to last known player pos
        agent.SetDestination(lastKnownPlayerPos);
    }

    void GoToPatrolPoint()
    {
        if (patrolPoints.Count == 0)
        {
            BuildPatrolPoints();
            if (patrolPoints.Count == 0) return;
        }
        agent.speed = patrolSpeed;
        agent.SetDestination(patrolPoints[currentPatrolIndex]);
    }

    void ReturnToPatrol()
    {
        state = State.Patrol;
        agent.speed = patrolSpeed;
        GoToPatrolPoint();
    }
    #endregion

    #region Patrol generation
    void BuildPatrolPoints()
    {
        patrolPoints.Clear();

        // sample points around the enemy's spawn position within patrolRadius
        Vector3 origin = transform.position;

        int attempts = patrolPointCount * 6;
        int found = 0;
        for (int i = 0; i < attempts && found < patrolPointCount; i++)
        {
            Vector3 rnd = origin + Random.insideUnitSphere * patrolRadius;
            rnd.y = origin.y;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(rnd, out hit, 2.5f, NavMesh.AllAreas))
            {
                patrolPoints.Add(hit.position);
                found++;
            }
        }

        if (patrolPoints.Count == 0)
        {
            // fallback: stay near spawn
            patrolPoints.Add(origin);
        }
    }
    #endregion

    #region Perception
    bool CanSeePlayer()
    {
        if (player == null) return false;
        Vector3 dir = (player.position - transform.position);
        float dist = dir.magnitude;
        if (dist > viewRadius) return false;

        float angleToPlayer = Vector3.Angle(transform.forward, dir);
        if (angleToPlayer > viewAngle * 0.5f) return false;

        // line-of-sight
        RaycastHit hit;
        Vector3 eye = transform.position + Vector3.up * 0.9f; // slightly above ground
        if (Physics.Raycast(eye, dir.normalized, out hit, viewRadius, ~0, QueryTriggerInteraction.Ignore))
        {
            // if first hit is the player or a child
            if (hit.transform == player || hit.transform.IsChildOf(player))
            {
                return true;
            }
            else
            {
                // there's an obstruction
                return false;
            }
        }

        return true;
    }
    #endregion

    #region Gizmos & debug
    void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 leftDir = DirFromAngle(-viewAngle / 2f, true);
        Vector3 rightDir = DirFromAngle(viewAngle / 2f, true);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.9f, leftDir * viewRadius);
        Gizmos.DrawRay(transform.position + Vector3.up * 0.9f, rightDir * viewRadius);

        Gizmos.color = Color.green;
        foreach (var p in patrolPoints)
            Gizmos.DrawSphere(p, 0.15f);
    }

    Vector3 DirFromAngle(float angleInDegrees, bool global)
    {
        if (!global) angleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    #endregion
}
