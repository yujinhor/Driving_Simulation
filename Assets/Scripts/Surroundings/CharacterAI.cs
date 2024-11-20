using UnityEngine;
using UnityEngine.AI;

public class CharacterAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator animator;
    private Transform[] waypoints; // 자식 Waypoints 배열
    private int currentWaypointIndex = 0;

    public float minDistance = 1.0f; // Waypoint 도달 판정 거리
    public float speed = 3.0f;

    private bool isMoving = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.speed = speed;
    }

    public void SetWaypointsParent(GameObject waypointParent)
    {
        // Waypoint Parent에서 모든 자식 Transform 가져오기
        Transform parentTransform = waypointParent.transform;
        waypoints = new Transform[parentTransform.childCount];
        for (int i = 0; i < parentTransform.childCount; i++)
        {
            waypoints[i] = parentTransform.GetChild(i);
        }
    }

    public void StartMoving()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        isMoving = true;
        currentWaypointIndex = Random.Range(0, waypoints.Length); // 랜덤 시작 지점
        agent.isStopped = false; // NavMeshAgent 이동 허용
        agent.SetDestination(waypoints[currentWaypointIndex].position); // 첫 목적지 설정
    }

    void Update()
    {
        if (isMoving && waypoints != null && waypoints.Length > 0)
        {
            MoveToWaypoint();
            UpdateAnimation();
        }
    }

    private void MoveToWaypoint()
    {
        // 현재 Waypoint에 도달했는지 확인
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < minDistance)
        {
            // 다음 Waypoint로 이동
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    private void UpdateAnimation()
    {
        // NavMeshAgent의 속도를 정규화하여 "vertical" 파라미터 설정
        float velocity = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("vertical", velocity); // 애니메이션 블렌드 트리의 vertical 업데이트
    }

    public void StopMoving()
    {
        isMoving = false;
        agent.isStopped = true;
        animator.SetFloat("vertical", 0f); // 정지 애니메이션
    }
}
