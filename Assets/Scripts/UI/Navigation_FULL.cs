using UnityEngine;
using UnityEngine.UI;

public class Navigation_FULL : MonoBehaviour
{
    public Toggle controlToggle; // UI 토글
    public Transform destination1; // 첫 번째 목적지 위치
    public Transform destination2; // 두 번째 목적지 위치
    public Transform destination3; // 세 번째 목적지
    public Transform destination4; // 두 번째 목적지 위치
    public Transform destination5; // 세 번째 목적지 위치
    public VehicleNavigation vehicleNavigation;

    private void Start()
    {
        if (controlToggle != null)
        {
            // 초기 상태를 토글에 맞추고 이벤트를 추가
            gameObject.SetActive(controlToggle.isOn);
            controlToggle.onValueChanged.AddListener(SetVisibility);
        }
    }

    private void SetVisibility(bool isVisible)
    {
        // 토글 상태에 따라 오브젝트 활성화/비활성화
        gameObject.SetActive(isVisible);
    }

    public void SetDestination1()
    {
        vehicleNavigation.SetDestination(destination1);
    }

    public void SetDestination2()
    {
        vehicleNavigation.SetDestination(destination2);
    }

    public void SetDestination3()
    {
        vehicleNavigation.SetDestination(destination3);
    }
    public void SetDestination4()
    {
        vehicleNavigation.SetDestination(destination4);
    }
    public void SetDestination5()
    {
        vehicleNavigation.SetDestination(destination5);
    }
}
