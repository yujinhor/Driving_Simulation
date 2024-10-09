using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CameraCapture : MonoBehaviour
{
    public Camera captureCamera; // 캡처할 카메라
    public int imageWidth = 1280;
    public int imageHeight = 720;
    public float captureInterval = 0.5f; // 0.5초마다 한 장씩 저장

    private string folderPath;

    void Start()
    {
        // 1. 폴더 생성 (현재 프로젝트 경로 아래에 "CapturedImages" 폴더 생성)
        folderPath = Path.Combine(Application.dataPath, "CapturedImages");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log($"폴더가 생성되었습니다: {folderPath}");
        }

        // 2. 캡처 코루틴 시작
        StartCoroutine(CaptureRoutine());
    }

    IEnumerator CaptureRoutine()
    {
        while (true)
        {
            CaptureImage();
            yield return new WaitForSeconds(captureInterval);
        }
    }

    void CaptureImage()
    {
        // 1. RenderTexture 생성 및 카메라에 할당
        RenderTexture renderTexture = new RenderTexture(imageWidth, imageHeight, 24);
        captureCamera.targetTexture = renderTexture;

        // 2. 카메라 렌더링
        captureCamera.Render();

        // 3. RenderTexture에서 Texture2D로 변환
        RenderTexture.active = renderTexture;
        Texture2D screenShot = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
        screenShot.Apply();

        // 4. RenderTexture 및 카메라 연결 해제
        captureCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        // 5. 파일 이름 생성 (타임스탬프 사용)
        string timeStamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
        string filePath = Path.Combine(folderPath, $"Capture_{timeStamp}.png");

        // 6. Texture2D 데이터를 PNG 파일로 변환 및 저장
        byte[] bytes = screenShot.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
        Debug.Log($"이미지가 {filePath}에 저장되었습니다.");
    }
}
