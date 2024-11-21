using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public ObjectPool objectPool; // Object Pool 참조
    public Transform mainCar; // MainCar Transform
    public float radius = 50f; // MainCar 반경
    public GameObject[] waypointParents; // 모든 WaypointParent 그룹
    public int maxCharactersPerParent = 10; // 각 Parent 그룹당 최대 캐릭터 수

    private Dictionary<GameObject, List<GameObject>> activeCharacters = new Dictionary<GameObject, List<GameObject>>();

    void Start()
    {
        // 각 WaypointParent 그룹 초기화
        foreach (var parent in waypointParents)
        {
            activeCharacters[parent] = new List<GameObject>();
        }
    }

    void Update()
    {
        foreach (var parent in waypointParents)
        {
            List<Transform> activeWaypoints = GetActiveWaypoints(parent);

            // 이미 생성된 캐릭터의 수가 초과하지 않도록 관리
            while (activeCharacters[parent].Count < maxCharactersPerParent && activeWaypoints.Count > 0)
            {
                Transform waypoint = activeWaypoints[Random.Range(0, activeWaypoints.Count)];
                CreateCharacterAtWaypoint(parent, waypoint);
                activeWaypoints.Remove(waypoint); // 중복 생성 방지
            }

            // 반경에서 벗어난 캐릭터 비활성화
            DisableCharactersOutOfRange(parent, activeWaypoints);
        }
    }

    private List<Transform> GetActiveWaypoints(GameObject waypointParent)
    {
        List<Transform> activeWaypoints = new List<Transform>();
        Transform parentTransform = waypointParent.transform;

        foreach (Transform waypoint in parentTransform)
        {
            float distance = Vector3.Distance(mainCar.position, waypoint.position);
            if (distance <= radius) // MainCar 반경 내 확인
            {
                activeWaypoints.Add(waypoint);
            }
        }

        return activeWaypoints;
    }

    private void CreateCharacterAtWaypoint(GameObject waypointParent, Transform waypoint)
    {
        GameObject character = objectPool.GetObject();
        character.transform.position = waypoint.position;
        CharacterAI ai = character.GetComponent<CharacterAI>();
        ai.SetWaypointsParent(waypointParent); // 해당 Parent 설정
        ai.StartMoving();

        // 활성 캐릭터 리스트에 추가
        activeCharacters[waypointParent].Add(character);
    }

    private void DisableCharactersOutOfRange(GameObject waypointParent, List<Transform> activeWaypoints)
    {
        List<GameObject> toRemove = new List<GameObject>();

        foreach (GameObject character in activeCharacters[waypointParent])
        {
            bool stillActive = false;

            foreach (Transform waypoint in activeWaypoints)
            {
                if (Vector3.Distance(character.transform.position, waypoint.position) < radius)
                {
                    stillActive = true;
                    break;
                }
            }

            if (!stillActive)
            {
                character.SetActive(false); // 캐릭터 비활성화
                toRemove.Add(character);
            }
        }

        // 리스트에서 제거
        foreach (GameObject character in toRemove)
        {
            activeCharacters[waypointParent].Remove(character);
        }
    }
}
