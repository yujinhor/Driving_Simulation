using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    public GameObject[] trafficLightGroups; // 신호등 그룹 (부모 오브젝트들)
    public Transform vehicleTransform; // 차량의 Transform

    private float timer = 0f;
    private int state = 0; // 0 = 차량 빨간불, 1 = 차량 초록불, 2 = 차량 노란불

    void Start()
    {
        // 각 신호등 그룹의 초기 상태 설정
        foreach (GameObject group in trafficLightGroups)
        {
            SetTrafficLightState(group, 0); // 초기 상태를 차량 빨간불로 설정
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        switch (state)
        {
            case 0: // 차량 빨간불, 보행자 초록불
                if (timer > 10f) // 10초 후에 상태 전환
                {
                    timer = 0f;
                    state = 1; // 다음 상태로 변경
                    foreach (GameObject group in trafficLightGroups)
                    {
                        SetTrafficLightState(group, 1); // 차량 초록불로 전환
                    }
                }
                break;
            case 1: // 차량 초록불, 보행자 빨간불
                if (timer > 17f) // 17초 후에 상태 전환
                {
                    timer = 0f;
                    state = 2; // 다음 상태로 변경
                    foreach (GameObject group in trafficLightGroups)
                    {
                        SetTrafficLightState(group, 2); // 차량 노란불로 전환
                    }
                }
                break;
            case 2: // 차량 노란불, 보행자 빨간불
                if (timer > 3f) // 3초 후에 상태 전환
                {
                    timer = 0f;
                    state = 0; // 초기 상태로 돌아감
                    foreach (GameObject group in trafficLightGroups)
                    {
                        SetTrafficLightState(group, 0); // 차량 빨간불로 전환
                    }
                }
                break;
        }
    }

    void SetTrafficLightState(GameObject group, int newState)
    {
        // 그룹 내 자식 오브젝트 찾기 (새로운 이름 적용)
        Transform vehicleRedLight = group.transform.Find("Vehicle_Red");
        Transform vehicleYellowLight = group.transform.Find("Vehicle_Yellow");
        Transform vehicleGreenLight = group.transform.Find("Vehicle_Green");
        Transform pedestrianRedLight = group.transform.Find("Crosswalk_Red");
        Transform pedestrianGreenLight = group.transform.Find("Crosswalk_Green");

        // 그룹 내 AudioSource 컴포넌트 가져오기
        AudioSource audioSource = group.GetComponent<AudioSource>();

        // 모든 신호등 비활성화
        vehicleRedLight.gameObject.SetActive(false);
        vehicleYellowLight.gameObject.SetActive(false);
        vehicleGreenLight.gameObject.SetActive(false);
        pedestrianRedLight.gameObject.SetActive(true); // 기본 보행자 빨간불 켜기
        pedestrianGreenLight.gameObject.SetActive(false);

        // 새로운 상태에 따라 신호등 활성화 및 소리 제어
        switch (newState)
        {
            case 0: // 차량 빨간불, 보행자 초록불
                vehicleRedLight.gameObject.SetActive(true);
                pedestrianGreenLight.gameObject.SetActive(true);
                pedestrianRedLight.gameObject.SetActive(false);
                if (audioSource != null && !audioSource.isPlaying)
                {
                    audioSource.Play(); // 보행자 초록불일 때 소리 재생
                }
                break;
            case 1: // 차량 초록불, 보행자 빨간불
                vehicleGreenLight.gameObject.SetActive(true);
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop(); // 소리 정지
                }
                break;
            case 2: // 차량 노란불, 보행자 빨간불
                vehicleYellowLight.gameObject.SetActive(true);
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop(); // 소리 정지
                }
                break;
        }
    }


}
