using UnityEngine;
using UnityEngine.AI;

public class SurroundCarAI : MonoBehaviour
{
    public NavMeshAgent agent;
    private Transform[] waypoints; // Waypoints 배열
    private int currentWaypointIndex = 0;

    public float minDistance = 1.0f; // Waypoint 도달 판정 거리

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent가 할당되지 않았습니다!");
        }
    }

    public void SetWaypointsParent(GameObject waypointParent)
    {
        if (waypointParent == null)
        {
            Debug.LogError("WaypointParent가 null입니다. Waypoints 초기화 실패!");
            return;
        }

        Transform parentTransform = waypointParent.transform;
        int childCount = parentTransform.childCount;

        if (childCount == 0)
        {
            Debug.LogError("WaypointParent에 자식 Waypoints가 없습니다!");
            waypoints = null; // Waypoints 초기화 실패 처리
            return;
        }

        waypoints = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            waypoints[i] = parentTransform.GetChild(i);
        }

        Debug.Log($"Waypoints 초기화 성공: {childCount}개의 Waypoints가 설정되었습니다.");
    }

    public void StartMoving()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("Waypoints가 설정되지 않았습니다. 이동 불가!");
            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError($"NavMeshAgent가 NavMesh 위에 배치되지 않았습니다! {gameObject.name}");
            return;
        }

        currentWaypointIndex = Random.Range(0, waypoints.Length); // 랜덤 시작 지점
        agent.isStopped = false;
        agent.SetDestination(waypoints[currentWaypointIndex].position); // 첫 목적지 설정
    }

    void Update()
    {
        if (agent == null || waypoints == null || waypoints.Length == 0 || agent.isStopped)
        {
            return;
        }

        // Waypoint에 도달했는지 확인
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < minDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }
}
