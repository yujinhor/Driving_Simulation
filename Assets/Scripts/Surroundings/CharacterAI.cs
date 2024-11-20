using UnityEngine;

public class CharacterAI : MonoBehaviour
{
    public Transform[] waypoints; // 이동할 Waypoint 배열
    private int currentWaypointIndex = 0;
    public float speed = 3.0f;

    private bool isMoving = false;

    public void StartMoving()
    {
        isMoving = true;
        currentWaypointIndex = 0;
    }

    void Update()
    {
        if (!isMoving) return;

        // 현재 Waypoint로 이동
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        // Waypoint에 도달 시 다음 Waypoint로
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 반경 밖으로 나가면 풀로 반환
        if (other.CompareTag("MainCarRadius"))
        {
            FindObjectOfType<ObjectPool>().ReturnObject(gameObject);
            isMoving = false;
        }
    }
}
