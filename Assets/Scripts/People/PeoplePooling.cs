using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeoplePooling : MonoBehaviour
{
    public GameObject casual_Male_K; // Man 프리팹
    public GameObject casual_Female_K; // Woman 프리팹
    public GameObject MainCar; // MainCar (씬에 있는 에셋)
    public int poolSize = 10; // 풀링할 객체 수
    public float spawnRadius = 30.0f; // 객체가 생성될 반경
    public float objectMoveSpeed = 2.0f; // 객체의 이동 속도

    private List<GameObject> objectPool; // 풀링된 객체 목록
    private List<Vector3> relativePositions; // MainCar 기준 상대 위치
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

        // Object Pool 및 상대 위치 목록 초기화
        objectPool = new List<GameObject>();
        relativePositions = new List<Vector3>();

        for (int i = 0; i < poolSize; i++)
        {
            // Man과 Woman 프리팹을 번갈아 풀링
            GameObject prefabToUse = useManPrefab ? casual_Male_K : casual_Female_K;
            useManPrefab = !useManPrefab; // 다음 번에는 다른 프리팹을 사용

            GameObject obj = Instantiate(prefabToUse);
            Debug.Log("Instantiated: " + obj.name); // 객체의 이름을 출력
            obj.SetActive(false); // 초기 상태는 비활성화
            objectPool.Add(obj);

            // MainCar 주변의 랜덤 위치 설정
            Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
            randomPosition.y = 0; // y축은 고정 (2D 평면에서 이동)
            relativePositions.Add(randomPosition);
        }
    }

    void Update()
    {
        if (MainCar == null) return; // MainCar가 없으면 업데이트 중단

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = objectPool[i];

            // 객체가 비활성화되어 있으면 활성화
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                Debug.Log("Activating object: " + obj.name); // 객체 이름 출력
            }

            // MainCar 주변으로 객체 이동 (상대적 위치 유지)
            Vector3 targetPosition = MainCar.transform.position + relativePositions[i];
            obj.transform.position = Vector3.Lerp(obj.transform.position, targetPosition, Time.deltaTime * objectMoveSpeed);

            Debug.Log("Object position: " + obj.transform.position); // 객체 위치 정보 출력
        }
    }
}
