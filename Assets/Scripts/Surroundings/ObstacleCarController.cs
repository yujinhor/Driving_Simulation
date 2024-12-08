using UnityEngine;
using TrafficSimulation;

public class ObstacleCarController : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("Trigger radius for detecting the main car")]
    public float detectionRadius = 20f; // 감지 반경

    [Tooltip("Reference to the main car")]
    public Transform mainCar; // 메인 차량의 Transform을 Inspector에서 직접 연결

    private VehicleAI vehicleAI; // VehicleAI 참조
    private WheelDrive wheelDrive; // WheelDrive 참조
    private bool isActivated = false; // 차량이 활성화 상태인지 확인

    void Start()
    {
        // VehicleAI 및 WheelDrive 컴포넌트 찾기
        vehicleAI = GetComponent<VehicleAI>();
        wheelDrive = GetComponent<WheelDrive>();

        if (vehicleAI == null || wheelDrive == null)
        {
            Debug.LogError("VehicleAI or WheelDrive component not found!");
            return;
        }

        // 초기 상태로 비활성화
        vehicleAI.enabled = false;
        wheelDrive.enabled = false;
    }

    void Update()
    {
        // 이미 활성화된 경우 더 이상 감지 로직 실행 안 함
        if (isActivated || mainCar == null)
            return;

        // 메인 차량과의 거리 계산
        float distanceToMainCar = Vector3.Distance(transform.position, mainCar.position);

        // 반경 내에 메인 차량이 들어오면 VehicleAI 및 WheelDrive 활성화
        if (distanceToMainCar <= detectionRadius)
        {
            isActivated = true;
            Debug.Log("Main car detected! Activating obstacle car.");
            ActivateVehicle();
        }
    }

    // VehicleAI 및 WheelDrive 활성화
    private void ActivateVehicle()
    {
        if (vehicleAI != null) vehicleAI.enabled = true;
        if (wheelDrive != null) wheelDrive.enabled = true;
        Debug.Log("VehicleAI and WheelDrive activated.");
    }
}
