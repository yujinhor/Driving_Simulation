using UnityEngine;
using UnityEngine.UI;

public class LaneDectectionUI : MonoBehaviour
{
    public LaneDetection laneDetection; // LaneDetection 스크립트 참조
    public RawImage leftImage; // 왼쪽 이미지
    public RawImage rightImage; // 오른쪽 이미지
    public Color normalColor = Color.white; // 기본 색상
    public Color activeColor = Color.red; // 활성화된 색상

    void Start()
    {
        if (laneDetection == null)
        {
            Debug.LogError("LaneDetection 스크립트가 설정되지 않았습니다!");
        }

        if (leftImage == null || rightImage == null)
        {
            Debug.LogError("UI 이미지가 설정되지 않았습니다!");
        }
    }

    void Update()
    {
        if (laneDetection == null || leftImage == null || rightImage == null)
            return;

        // LaneDetection에서 steeringAngle 값 가져오기
        float steeringAngle = laneDetection.GetSteeringAngle();

        // steeringAngle에 따라 이미지 색상 업데이트
        if (steeringAngle > 0)
        {
            // 왼쪽 이미지 활성화
            leftImage.color = activeColor;
            rightImage.color = normalColor;
        }
        else if (steeringAngle < 0)
        {
            // 오른쪽 이미지 활성화
            rightImage.color = activeColor;
            leftImage.color = normalColor;
        }
        else
        {
            // 기본 상태로 복귀
            leftImage.color = normalColor;
            rightImage.color = normalColor;
        }
    }
}
