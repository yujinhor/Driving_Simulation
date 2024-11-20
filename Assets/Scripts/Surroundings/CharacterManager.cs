using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public ObjectPool objectPool; // Object Pool 참조
    public Transform mainCar; // 메인 카 Transform
    public float radius = 50f; // 메인 카 반경
    public Transform waypointParent; // Waypoint가 들어 있는 부모 오브젝트
    private Transform[] waypoints; // 모든 Waypoint 배열

    void Start()
    {
        // WaypointParent 아래 자식 오브젝트를 자동으로 배열에 추가
        waypoints = new Transform[waypointParent.childCount];
        for (int i = 0; i < waypointParent.childCount; i++)
        {
            waypoints[i] = waypointParent.GetChild(i); // 자식 Waypoint 가져오기
        }
    }

    void Update()
    {
        foreach (Transform waypoint in waypoints)
        {
            float distance = Vector3.Distance(mainCar.position, waypoint.position);
            if (distance <= radius)
            {
                // 반경 내에 들어온 Waypoint라면 캐릭터 활성화
                ActivateCharacterAtWaypoint(waypoint);
            }
        }
    }

    void ActivateCharacterAtWaypoint(Transform waypoint)
    {
        GameObject character = objectPool.GetObject(); // 풀에서 캐릭터 가져오기
        character.transform.position = waypoint.position; // Waypoint 위치에 배치
        character.GetComponent<CharacterAI>().StartMoving(); // 이동 시작
    }
}
