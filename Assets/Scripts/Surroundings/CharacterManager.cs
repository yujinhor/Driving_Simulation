using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public ObjectPool objectPool; // Object Pool 참조
    public Transform mainCar; // 메인 카 Transform
    public float radius = 50f; // 메인 카 반경
    public GameObject waypointParent; // Waypoint Parent (GameObject)
    public int maxCharacters = 3; // 생성할 캐릭터의 최대 수

    private Transform[] waypoints; // Waypoints 배열
    private HashSet<Transform> usedWaypoints = new HashSet<Transform>(); // 사용된 Waypoints

    void Start()
    {
        // Waypoint Parent에서 모든 자식 Transform 가져오기
        Transform parentTransform = waypointParent.transform;
        waypoints = new Transform[parentTransform.childCount];
        for (int i = 0; i < parentTransform.childCount; i++)
        {
            waypoints[i] = parentTransform.GetChild(i);
        }
    }

    void Update()
    {
        List<Transform> activeWaypoints = GetActiveWaypoints();

        // 현재 활성화된 Waypoint에서 캐릭터 생성
        int charactersToSpawn = Mathf.Min(maxCharacters, activeWaypoints.Count);
        for (int i = 0; i < charactersToSpawn; i++)
        {
            Transform waypoint = activeWaypoints[Random.Range(0, activeWaypoints.Count)];
            if (!usedWaypoints.Contains(waypoint))
            {
                ActivateCharacterAtWaypoint(waypoint);
                usedWaypoints.Add(waypoint);
            }
        }
    }

    private List<Transform> GetActiveWaypoints()
    {
        List<Transform> activeWaypoints = new List<Transform>();
        foreach (Transform waypoint in waypoints)
        {
            if (Vector3.Distance(mainCar.position, waypoint.position) <= radius)
            {
                activeWaypoints.Add(waypoint);
            }
        }
        return activeWaypoints;
    }

    private void ActivateCharacterAtWaypoint(Transform waypoint)
    {
        GameObject character = objectPool.GetObject();
        character.transform.position = waypoint.position;
        CharacterAI ai = character.GetComponent<CharacterAI>();
        ai.SetWaypointsParent(waypointParent); // Waypoint Parent 전달
        ai.StartMoving();
    }
}
