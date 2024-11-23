using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;  // 싱글톤으로 전역 접근을 위한 변수
    public Camera[] cameras;  // 사용할 카메라 배열
    private int currentCameraIndex = 0;  // 현재 활성화된 카메라 인덱스
    public Image[] uiImages; // UI 이미지 배열 (각 카메라에 대응되는 이미지)

    public Color normalColor = new Color(1f, 1f, 1f, 1f); // 기본 색상 (밝은 상태)
    public Color darkColor = new Color(0.5f, 0.5f, 0.5f, 1f); // 어두운 색상 (눌린 상태)

    void Awake()
    {
        // 싱글톤 패턴 설정
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 객체 유지
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 있으면 새로운 객체 파괴
        }
    }

    void Start()
    {
        // 모든 카메라를 초기화 시 비활성화
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(false);
        }

        // 첫 번째 카메라를 활성화
        if (cameras.Length > 0)
        {
            cameras[0].gameObject.SetActive(true);
        }
    }

    void Update()
    {
        // 숫자 키 1~5로 카메라 전환
        if (Input.GetKeyDown(KeyCode.Alpha1)) { SwitchCamera(0); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { SwitchCamera(1); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { SwitchCamera(2); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { SwitchCamera(3); }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            // 카메라 5번의 위치를 카메라 1번 위치로 설정 후 전환
            cameras[4].transform.position = cameras[0].transform.position;
            SwitchCamera(4);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6)) { SwitchCamera(5); }
    }

    public void SwitchCamera(int cameraIndex)
    {
        // 유효한 인덱스일 경우 카메라 전환
        if (cameraIndex >= 0 && cameraIndex < cameras.Length)
        {
            cameras[currentCameraIndex].gameObject.SetActive(false); // 현재 카메라 비활성화
            cameras[cameraIndex].gameObject.SetActive(true); // 새로운 카메라 활성화
            currentCameraIndex = cameraIndex; // 현재 카메라 인덱스 업데이트
        }

        UpdateUIImageColors(cameraIndex); // UI 이미지 색상 업데이트
    }

    void UpdateUIImageColors(int buttonIndex)
    {
        // 모든 UI 이미지를 기본 색상으로 초기화
        for (int i = 0; i < uiImages.Length; i++)
        {
            uiImages[i].color = normalColor;
        }

        // 활성화된 카메라에 대응하는 UI 이미지 색상 변경
        if (buttonIndex >= 0 && buttonIndex < uiImages.Length)
        {
            uiImages[buttonIndex].color = darkColor;
        }
    }

    void DisableCameras(int start, int end)
    {
        // 특정 범위의 카메라들을 비활성화
        for (int i = start - 1; i < end; i++)
        {
            if (i >= 0 && i < cameras.Length)
            {
                cameras[i].gameObject.SetActive(false);
            }
        }
    }

    public int GetCurrentCameraIndex()
    {
        return currentCameraIndex; // 현재 카메라 인덱스를 반환
    }
}
