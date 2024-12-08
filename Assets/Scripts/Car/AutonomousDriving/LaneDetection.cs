using System.Collections;
using UnityEngine;
using OpenCvSharp; // OpenCV 네임스페이스 사용

public class LaneDetection : MonoBehaviour
{
    public Camera captureCamera; // 차량에 부착된 카메라
    public RenderTexture renderTexture; // 카메라에 연결된 RenderTexture
    public float handleSensitivity = 0.5f; // 핸들 민감도 조정

    private float steeringAngle;

    void Start()
    {
        if (captureCamera == null)
        {
            Debug.LogError("CaptureCamera가 설정되지 않았습니다!");
            return;
        }

        if (renderTexture == null)
        {
            Debug.LogError("RenderTexture가 설정되지 않았습니다!");
            return;
        }

        StartCoroutine(CaptureRoutine());
    }

    // 주기적으로 이미지를 캡처하고 차선에 따른 핸들 값을 계산하는 코루틴
    IEnumerator CaptureRoutine()
    {
        while (true)
        {
            // RenderTexture에서 이미지를 읽어옴
            Texture2D image = ReadRenderTexture(renderTexture);

            // 이미지를 처리하여 차선에 따른 조향 값을 계산
            steeringAngle = CalculateSteeringAngle(image);

            yield return new WaitForSeconds(0.05f); // 0.05초마다 데이터 처리
        }
    }

    // RenderTexture에서 Texture2D를 생성하는 함수
    Texture2D ReadRenderTexture(RenderTexture rt)
    {
        // RenderTexture 활성화
        RenderTexture.active = rt;

        // RenderTexture 데이터를 Texture2D로 복사
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new UnityEngine.Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        RenderTexture.active = null;

        return tex;
    }

    // 차선 검출을 위한 조향각 계산 함수
    float CalculateSteeringAngle(Texture2D image)
    {
        if (image == null)
        {
            Debug.LogWarning("이미지가 비어 있습니다!");
            return 0;
        }

        // OpenCV를 사용해 이미지 분석
        byte[] imageBytes = image.EncodeToPNG();
        Mat matImage = Mat.FromImageData(imageBytes, ImreadModes.Color);

        // 흑백 변환
        Mat gray = new Mat();
        Cv2.CvtColor(matImage, gray, ColorConversionCodes.BGR2GRAY);

        // 엣지 검출
        Mat edges = new Mat();
        Cv2.Canny(gray, edges, 50, 150);

        // 차선 검출: HoughLinesP를 사용해 직선을 검출
        LineSegmentPoint[] lines = Cv2.HoughLinesP(edges, 1, Mathf.PI / 180, 50, 50, 10);

        // 차선 중앙 계산
        float centerX = renderTexture.width / 2f;
        float laneCenterX = 0;
        int laneCount = 0;

        foreach (var line in lines)
        {
            laneCenterX += (line.P1.X + line.P2.X) / 2;
            laneCount++;
        }

        if (laneCount > 0)
        {
            laneCenterX /= laneCount;
        }
        else
        {
            Debug.LogWarning("차선을 찾지 못했습니다!");
            return 0;
        }

        // 차량 중심과 차선 중심의 차이를 기반으로 핸들 조작 각도 계산
        float offset = laneCenterX - centerX;
        float normalizedOffset = handleSensitivity * offset / (renderTexture.width / 2f);

        // 조향각 계산 및 범위 제한
        float calculatedSteering = -handleSensitivity * normalizedOffset;
        calculatedSteering = Mathf.Clamp(calculatedSteering, -1f, 1f);

        return calculatedSteering;

        //return -steeringAngle;
    }

    // 외부에서 조향각을 조회할 수 있는 함수
    public float GetSteeringAngle()
    {
        Debug.Log($"Steering Angle: {steeringAngle}");
        return steeringAngle/2;
    }
}

/*
using System.Collections;
using UnityEngine;
using OpenCvSharp;

public class LaneDetection : MonoBehaviour
{
    public Camera captureCamera; // 차량에 부착된 카메라
    public RenderTexture renderTexture; // 카메라에 연결된 RenderTexture
    public float handleSensitivity = 0.5f; // 핸들 민감도 조정

    private PIDController pidController; // PID 제어기
    private float steeringAngle;

    void Start()
    {
        if (captureCamera == null)
        {
            Debug.LogError("CaptureCamera가 설정되지 않았습니다!");
            return;
        }

        if (renderTexture == null)
        {
            Debug.LogError("RenderTexture가 설정되지 않았습니다!");
            return;
        }

        // PIDController 초기화 (PI 제어: kd=0)
        pidController = new PIDController(1.0f, 0.1f, 0.0f, -1.0f, 1.0f);

        StartCoroutine(CaptureRoutine());
    }

    IEnumerator CaptureRoutine()
    {
        while (true)
        {
            // RenderTexture에서 이미지를 읽어옴
            Texture2D image = ReadRenderTexture(renderTexture);

            // 차선 중심으로부터의 오프셋 계산
            float laneOffset = CalculateLaneOffset(image);

            // PID 제어기로 조향각 계산
            steeringAngle = pidController.Update(laneOffset, 0.1f); // deltaTime: 0.1초

            yield return new WaitForSeconds(0.1f); // 0.1초마다 처리
        }
    }


    Texture2D ReadRenderTexture(RenderTexture rt)
    {
        // RenderTexture 활성화
        RenderTexture.active = rt;

            // RenderTexture 데이터를 Texture2D로 복사
            Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new UnityEngine.Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();

            RenderTexture.active = null;

            return tex;
        }

    float CalculateLaneOffset(Texture2D image)
    {
        if (image == null)
        {
            Debug.LogWarning("이미지가 비어 있습니다!");
            return 0;
        }

        byte[] imageBytes = image.EncodeToPNG();
        Mat matImage = Mat.FromImageData(imageBytes, ImreadModes.Color);

        Mat gray = new Mat();
        Cv2.CvtColor(matImage, gray, ColorConversionCodes.BGR2GRAY);

        Mat edges = new Mat();
        Cv2.Canny(gray, edges, 50, 150);

        LineSegmentPoint[] lines = Cv2.HoughLinesP(edges, 1, Mathf.PI / 180, 50, 50, 10);

        float centerX = renderTexture.width / 2f;
        float laneCenterX = 0;
        int laneCount = 0;

        foreach (var line in lines)
        {
            laneCenterX += (line.P1.X + line.P2.X) / 2;
            laneCount++;
        }

        if (laneCount > 0)
        {
            laneCenterX /= laneCount;
        }
        else
        {
            Debug.LogWarning("차선을 찾지 못했습니다!");
            return 0;
        }

        return (laneCenterX - centerX) / (renderTexture.width / 2f);
    }

    public float GetSteeringAngle()
    {
        Debug.Log($"Steering Angle: {steeringAngle}");
        return steeringAngle;
    }
}
*/
