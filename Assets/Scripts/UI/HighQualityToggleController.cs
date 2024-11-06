using UnityEngine;
using UnityEngine.UI;

public class LowQualityToggleController : MonoBehaviour
{
    public Toggle objectToggle; // UI Toggle
    public GameObject targetObject; // 활성화/비활성화할 오브젝트

    private void Start()
    {
        // targetObject가 할당되지 않은 경우 초기화 중단
        if (targetObject == null)
        {
            Debug.LogWarning("Target Object is not assigned, skipping initialization.");
            return;
        }

        // Toggle의 상태가 변경될 때 이벤트 연결
        objectToggle.onValueChanged.AddListener(delegate { ToggleObject(objectToggle.isOn); });

        // 시작 시 초기 상태 설정
        targetObject.SetActive(objectToggle.isOn);
    }

    private void ToggleObject(bool isOn)
    {
        if (targetObject != null)
        {
            targetObject.SetActive(isOn);
        }
    }
}
