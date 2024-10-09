using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;  // 싱글톤 관리- 전역적 접근
    public Camera[] cameras;
    private int currentCameraIndex = 0;
    public Image[] uiImages; // UI 이미지 배열 (카메라에 대응하는 이미지들)

    public Color normalColor = new Color(1f, 1f, 1f, 1f); // 원래 색상 (밝은 상태)
    public Color darkColor = new Color(0.5f, 0.5f, 0.5f, 1f); // 어두운 색상 (눌린 상태)

    void Awake()
    {
        // 싱글톤 패턴으로 카메라 매니저 전역 접근 설정
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환시에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(false);
        }

        if (cameras.Length > 0)
        {
            cameras[0].gameObject.SetActive(true);
        }
    }

    void Update()
    {
        // 숫자 1 ~ 5 키로 카메라 전환
        if (Input.GetKeyDown(KeyCode.Alpha1)) { SwitchCamera(0); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { SwitchCamera(1); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { SwitchCamera(2); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { SwitchCamera(3); }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            cameras[4].transform.position = cameras[0].transform.position;
            SwitchCamera(4);

        }
    }

    public void SwitchCamera(int cameraIndex)
    {
        if (cameraIndex >= 0 && cameraIndex < cameras.Length)
        {
            cameras[currentCameraIndex].gameObject.SetActive(false);
            cameras[cameraIndex].gameObject.SetActive(true);
            currentCameraIndex = cameraIndex;
        }

        UpdateUIImageColors(cameraIndex);
    }
    void UpdateUIImageColors(int buttonIndex)
    {
        // 모든 이미지의 색상을 원래 색으로 설정한 후
        for (int i = 0; i < uiImages.Length; i++)
        {
            uiImages[i].color = normalColor;
        }

        // 활성화된 카메라에 해당하는 이미지 색상만 어둡게 변경
        uiImages[buttonIndex].color = darkColor;
    }
    public int GetCurrentCameraIndex()
    {
        return currentCameraIndex; // 현재 카메라 인덱스 반환
    }
}
