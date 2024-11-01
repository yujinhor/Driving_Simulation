using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeoplePooling : MonoBehaviour
{
    public GameObject casual_Male_K; // Man 프리팹
    public GameObject casual_Female_K; // Woman 프리팹
    public GameObject MainCar; // MainCar (씬에 있는 에셋)
    public int poolSize = 50; // 풀링할 객체 수
    public float spawnRadius = 5.0f; // 객체가 생성될 반경
    public float objectMoveSpeed = 2.0f; // 객체의 이동 속도

    private List<GameObject> objectPool; // 풀링된 객체 목록
    private bool useManPrefab = true; // Man과 Woman 프리팹을 번갈아 생성하는 플래그

    void Start()
    {
        // MainCar가 씬에 있는지 확인
        if (MainCar == null)
        {
            Debug.LogError("MainCar not assigned! Please assign MainCar in the Inspector.");
            return;
        }

        // 프리팹이 제대로 할당되어 있는지 확인
        if (casual_Male_K == null || casual_Female_K == null)
        {
            Debug.LogError("Casual_Male_K or Casual_Female_K prefab not assigned! Please assign them in the Inspector.");
            return;
        }

        // Object Pool 초기화
        objectPool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            // Man과 Woman 프리팹을 번갈아 풀링
            GameObject prefabToUse = useManPrefab ? casual_Male_K : casual_Female_K;
            useManPrefab = !useManPrefab; // 다음 번에는 다른 프리팹을 사용

            GameObject obj = Instantiate(prefabToUse);
            obj.SetActive(false); // 초기 상태는 비활성화
            objectPool.Add(obj);
        }
    }

    void Update()
    {
        if (MainCar == null)
        {
            Debug.LogError("MainCar is missing.");
            return;
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = objectPool[i];

            // 객체가 비활성화되어 있으면 활성화
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
            }

            // MainCar 주변의 랜덤 목표 위치 생성
            Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
            randomPosition.y = 0; // y축은 고정 (2D 평면에서 이동)
            Vector3 targetPosition = MainCar.transform.position + randomPosition;

            // MoveTowards를 사용해 목표 위치로 점진적으로 이동
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetPosition, Time.deltaTime * objectMoveSpeed);
        }
    }
}
