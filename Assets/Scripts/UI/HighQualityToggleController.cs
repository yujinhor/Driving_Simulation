using UnityEngine;
using UnityEngine.UI;

public class LowQualityToggleController : MonoBehaviour
{
    public Toggle objectToggle; // UI Toggle
    public GameObject targetObject; // 활성화/비활성화할 오브젝트

    private void Start()
    {
        // Toggle의 상태가 변경될 때 이벤트 연결
        objectToggle.onValueChanged.AddListener(delegate { ToggleObject(objectToggle.isOn); });

        // 시작 시 초기 상태 설정
        targetObject.SetActive(objectToggle.isOn);
    }

    private void ToggleObject(bool isOn)
    {
        targetObject.SetActive(isOn);
    }
}
