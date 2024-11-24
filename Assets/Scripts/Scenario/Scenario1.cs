using UnityEngine;
using UnityEngine.UI;

public class Scenario1 : MonoBehaviour
{
    public GameObject objectToSpawn; // 생성할 오브젝트
    public Transform vehicleFront; // 차량의 앞 위치
    public Rigidbody vehicleRigidbody; // 차량의 Rigidbody
    public Toggle spawnToggle; // Toggle UI
    private bool isSpawning = false; // 스폰 활성화 여부
    private float spawnInterval = 5f; // 스폰 주기
    private float spawnTimer = 0f; // 타이머
    private float baseSpawnDistance = 2f; // 기본 생성 거리
    private float speedMultiplier = 1f; // 속도에 따른 거리 배율

    void Start()
    {
        // Toggle 값 변경 이벤트 등록
        if (spawnToggle != null)
        {
            spawnToggle.onValueChanged.AddListener(OnToggleChanged);
        }
    }

    void Update()
    {
        // 스폰 활성화 시 타이머로 오브젝트 생성
        if (isSpawning)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                SpawnObject();
                spawnTimer = 0f; // 타이머 초기화
            }
        }
    }

    private void OnToggleChanged(bool value)
    {
        // Toggle 상태 변경 시 스폰 활성화 여부 업데이트
        isSpawning = value;
        if (!isSpawning)
        {
            spawnTimer = 0f; // 비활성화 시 타이머 초기화
        }
    }

    private void SpawnObject()
    {
        if (objectToSpawn != null && vehicleFront != null && vehicleRigidbody != null)
        {
            // 차량의 속도에 비례한 거리 계산
            float speed = vehicleRigidbody.velocity.magnitude; // 차량 속도 크기
            float dynamicDistance = baseSpawnDistance + (speed * speedMultiplier);

            // 생성 위치 계산
            Vector3 spawnPosition = vehicleFront.position + vehicleFront.forward * dynamicDistance;

            // 오브젝트 생성
            Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}
