using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SurroundCarManager : MonoBehaviour
{
    public ObjectPool objectPool; // Object Pool 참조
    public Transform mainCar; // MainCar Transform
    public float radius = 50f; // MainCar 반경
    public GameObject[] waypointParents; // 모든 WaypointParent 그룹
    public int maxCarsPerParent = 10; // 각 Parent 그룹당 최대 캐릭터 수

    private Dictionary<GameObject, List<GameObject>> activeCars = new Dictionary<GameObject, List<GameObject>>();

    void Start()
    {
        // 각 WaypointParent 그룹 초기화
        foreach (var parent in waypointParents)
        {
            activeCars[parent] = new List<GameObject>();
        }
    }

    void Update()
    {
        foreach (var parent in waypointParents)
        {
            List<Transform> activeWaypoints = GetActiveWaypoints(parent);

            // activeWaypoints가 비어 있으면 넘어감
            if (activeWaypoints.Count == 0)
            {
                Debug.LogWarning($"WaypointParent {parent.name}의 activeWaypoints가 비어 있습니다.");
                continue;
            }

            // 배경 자동차 생성 관리
            while (activeCars[parent].Count < maxCarsPerParent && activeWaypoints.Count > 0)
            {
                Transform waypoint = activeWaypoints[Random.Range(0, activeWaypoints.Count)];
                CreateCarAtWaypoint(parent, waypoint);
                activeWaypoints.Remove(waypoint); // 중복 생성 방지
            }

            // 반경에서 벗어난 자동차 비활성화
            DisableCarsOutOfRange(parent, activeWaypoints);
        }
    }

    private List<Transform> GetActiveWaypoints(GameObject waypointParent)
    {
        List<Transform> activeWaypoints = new List<Transform>();
        Transform parentTransform = waypointParent.transform;

        foreach (Transform waypoint in parentTransform)
        {
            float distance = Vector3.Distance(mainCar.position, waypoint.position);

            // NavMesh 위에 있는 Waypoint만 추가
            if (distance <= radius && NavMesh.SamplePosition(waypoint.position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                activeWaypoints.Add(waypoint);
            }
        }

        return activeWaypoints;
    }

    private void CreateCarAtWaypoint(GameObject waypointParent, Transform waypoint)
    {
        GameObject car = objectPool.GetObject();
        car.transform.position = waypoint.position;

        if (!NavMesh.SamplePosition(car.transform.position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            Debug.LogError($"자동차 {car.name}를 NavMesh에 배치하지 못했습니다. Waypoint: {waypoint.name}, 위치: {waypoint.position}");
            return;
        }

        // NavMesh 위로 자동차 위치 조정
        car.transform.position = hit.position;
        Debug.Log($"자동차 {car.name}가 NavMesh로 이동: {hit.position}");

        SurroundCarAI ai = car.GetComponent<SurroundCarAI>();
        ai.SetWaypointsParent(waypointParent);
        ai.StartMoving();

        activeCars[waypointParent].Add(car);
    }


    private void DisableCarsOutOfRange(GameObject waypointParent, List<Transform> activeWaypoints)
    {
        List<GameObject> toRemove = new List<GameObject>();

        foreach (GameObject car in activeCars[waypointParent])
        {
            bool stillActive = false;

            foreach (Transform waypoint in activeWaypoints)
            {
                if (Vector3.Distance(car.transform.position, waypoint.position) < radius)
                {
                    stillActive = true;
                    break;
                }
            }

            if (!stillActive)
            {
                car.SetActive(false); // 자동차 비활성화
                toRemove.Add(car);
            }
        }

        // 리스트에서 제거
        foreach (GameObject car in toRemove)
        {
            activeCars[waypointParent].Remove(car);
        }
    }
}
