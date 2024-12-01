using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using UI = UnityEngine.UI;

public class TrafficLightDetection : MonoBehaviour
{

    public NNModel _model;
    public UI.RawImage _imageView;
    public Canvas canvas; // UI를 표시할 Canvas
    public Camera CaptureCamera; // Unity 카메라
    public RenderTexture renderTexture; // Unity 카메라 출력용 RenderTexture
    private List<GameObject> detectionBoxes = new List<GameObject>(); // 생성된 박스 관리


    private int _resizeLength = 640; // 리사이즈된 정사각형의 한 변의 길이
                                     // 라벨 정보
                                     // model.Metadata["names"]에도 유사한 값이 저장되어 있지만, JSON 문자열로 등록되어 있어 표준 기능으로 파싱할 수 없으므로 별도로 정의함.
    private readonly string[] _labels = {
        "car", "light", "null", "person"};

    // Start is called before the first frame update
    void Start()
    {
        // ONNX 모델 로드 및 워커 생성
        var model = ModelLoader.Load(_model);
        var worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);


        // Unity 카메라에 RenderTexture 연결
        CaptureCamera.targetTexture = renderTexture;
        _imageView.texture = renderTexture; // UI에서 실시간 카메라 뷰를 표시

        // 업데이트 코루틴 시작
        StartCoroutine(UpdateDetection());


        IEnumerator UpdateDetection()
        {
            while (true)
            {

                // RenderTexture를 Texture2D로 변환
                Texture2D capturedImage = CaptureCameraImage();

                if (capturedImage == null)
                {
                    Debug.LogError("Captured image is null. Check if RenderTexture is properly assigned.");
                    yield break;
                }

                // 이미지를 ONNX 모델에 입력
                var texture = ResizedTexture(capturedImage, _resizeLength, _resizeLength);
                Tensor inputTensor = new Tensor(texture, channels: 3);


                // 추론 실행
                worker.Execute(inputTensor);

                // 결과 분석
                Tensor output0 = worker.PeekOutput("output0");
                List<DetectionResult> ditects = ParseOutputs(output0, 0.3f, 0.75f);

                inputTensor.Dispose();
                output0.Dispose();

                // 결과 그리기
                // 축소된 이미지를 분석하고 있으므로, 결과를 원래 크기로 변환
                float scaleX = capturedImage.width / (float)_resizeLength;
                float scaleY = capturedImage.height / (float)_resizeLength;

                // UI 업데이트
                int activeCameraIndex = CameraManager.instance.GetCurrentCameraIndex();
                if (activeCameraIndex == 5) { AddBoxOutline(ditects, scaleX, scaleY); };
                AddBoxOutline_MiniCam(ditects, scaleX, scaleY, capturedImage);

                // 다음 실행까지 0.5초 대기
                yield return new WaitForSeconds(0.1f);
            }
        }

        //worker.Dispose();

    }

    private Texture2D CaptureCameraImage()
    {
        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;
        return texture;
    }

    private void AddBoxOutline(List<DetectionResult> detections, float scaleX, float scaleY)
    {
        // 이전 프레임의 선 삭제
        foreach (GameObject line in detectionBoxes)
        {
            Destroy(line);
        }
        detectionBoxes.Clear(); // 리스트 초기화

        // 캔버스 크기 확인
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;

        // 화면 비율 조정 (640x360 기준 비율)
        float referenceWidth = 640f;
        float referenceHeight = 360f;
        float scaleFactorX = canvasWidth / referenceWidth;
        float scaleFactorY = canvasHeight / referenceHeight;

        foreach (DetectionResult detection in detections)
        {
            // 박스 좌표 계산 (비율 반영)
            float x1 = detection.x1 * scaleX * scaleFactorX;
            float y1 = detection.y1 * scaleY * scaleFactorY;
            float x2 = detection.x2 * scaleX * scaleFactorX;
            float y2 = detection.y2 * scaleY * scaleFactorY;

            // UI 좌표 반영 (Y축 반전)
            float uiY1 = canvasHeight - y1;
            float uiY2 = canvasHeight - y2;

            // 클래스 ID에 따라 색상 결정
            Color lineColor = Color.red; // 기본 색상: 빨강
            if (detection.classId == 0) // 0번 클래스: "car"
            {
                lineColor = Color.blue; 
            }
            else if (detection.classId == 1) // 1번 클래스: "light"
            {
                lineColor = Color.green; 
            }
            else if (detection.classId == 2) // 2번 클래스: "person"
            {
                lineColor = Color.red;
            }
            else
            {
                continue;
            }

            // 4개의 선을 생성하여 박스 경계선을 그림
            CreateLine(new Vector2(x1, uiY1), new Vector2(x2, uiY1), lineColor); // 상단
            CreateLine(new Vector2(x2, uiY1), new Vector2(x2, uiY2), lineColor); // 우측
            CreateLine(new Vector2(x2, uiY2), new Vector2(x1, uiY2), lineColor); // 하단
            CreateLine(new Vector2(x1, uiY2), new Vector2(x1, uiY1), lineColor); // 좌측
        }
    }


    private void CreateLine(Vector2 start, Vector2 end, Color color)
    {
        // 선을 그릴 GameObject 생성
        GameObject lineObject = new GameObject("Line");
        lineObject.transform.SetParent(canvas.transform, false);

        // Image 컴포넌트 추가
        var image = lineObject.AddComponent<UI.Image>();
        image.color = color; // 선 색상 설정

        // RectTransform 설정
        RectTransform rectTransform = lineObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0); // Canvas 좌측 하단 기준
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.pivot = new Vector2(0, 0.5f); // 선의 중심을 Y축 중간으로 설정

        // 선의 길이와 두께 계산
        float length = Vector2.Distance(start, end);
        float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;

        rectTransform.sizeDelta = new Vector2(length, 2f); // 선의 길이와 두께 (두께 2)
        rectTransform.anchoredPosition = start; // 시작점 위치
        rectTransform.rotation = Quaternion.Euler(0, 0, angle); // 회전 설정

        // 생성된 선을 리스트에 추가
        detectionBoxes.Add(lineObject);
    }

    private void AddBoxOutline_MiniCam(List<DetectionResult> detections, float scaleX, float scaleY, Texture2D capturedImage)
    {
        var image = ResizedTexture(capturedImage, capturedImage.width, capturedImage.height);
        // 동일한 클래스는 동일한 색상이 되도록 설정
        Dictionary<int, Color> colorMap = new Dictionary<int, Color>();

        foreach (DetectionResult ditect in detections)
        {
            // 분석 결과 표시
            Debug.Log($"{_labels[ditect.classId]}: {ditect.score:0.00}");

            // 박스처럼 표시
            Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            if (colorMap.ContainsKey(ditect.classId))
            {
                color = colorMap[ditect.classId];
            }
            else
            {
                colorMap.Add(ditect.classId, color);
            }

            // 좌표 스케일 조정
            int x1 = (int)(ditect.x1 * scaleX);
            int x2 = (int)(ditect.x2 * scaleX);
            int y1 = (int)(ditect.y1 * scaleY);
            int y2 = (int)(ditect.y2 * scaleY);

            // 사각형의 상단, 하단, 좌측, 우측 그리기
            for (int x = x1; x <= x2; x++)
            {
                // 상단
                image.SetPixel(x, capturedImage.height - y1, color);
                // 하단
                image.SetPixel(x, capturedImage.height - y2, color);
            }
            for (int y = y1; y <= y2; y++)
            {
                // 좌측
                image.SetPixel(x1, capturedImage.height - y, color);
                // 우측
                image.SetPixel(x2, capturedImage.height - y, color);
            }

        }
        image.Apply();

        _imageView.texture = image;
    }

    // 라벨 화면에 출력 만들다 말았음
    private void AddLabelToDetection(DetectionResult ditect, float scaleX, float scaleY, Texture2D capturedImage)
    {
        // 박스 좌표 계산 (Texture2D 좌표에서 Canvas 좌표로 변환)
        float x1 = ditect.x1 * scaleX;
        float y1 = ditect.y1 * scaleY;
        float x2 = ditect.x2 * scaleX;
        float y2 = ditect.y2 * scaleY;

        // 박스 중심 좌표 계산
        float centerX = (x1 + x2) / 2;
        float centerY = (y1 + y2) / 2;

        // 좌표 상하 반전 (Texture2D는 좌상단 원점, Canvas는 좌하단 원점)
        centerY = capturedImage.height - centerY;

        // 라벨 GameObject 생성
        GameObject labelObject = new GameObject("DetectionLabel");
        labelObject.transform.SetParent(canvas.transform);

        // Text 컴포넌트 추가
        UI.Text label = labelObject.AddComponent<UI.Text>();
        label.text = $"{_labels[ditect.classId]}: {ditect.score:0.00}";
        label.fontSize = 14;
        label.color = Color.white; // 라벨 색상
        label.alignment = TextAnchor.MiddleCenter;

        // RectTransform 설정
        RectTransform rectTransform = label.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(150, 30); // 텍스트 박스 크기
        rectTransform.anchorMin = new Vector2(0, 1); // Canvas의 좌표계와 맞추기
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(centerX, centerY); // 박스 중심에 위치 설정
    }


    private List<DetectionResult> ParseOutputs(Tensor output0, float threshold, float iouThres)
    {
        // 검출 결과의 행 수
        int outputWidth = output0.shape.width;

        // 검출 결과로 채택할 후보
        List<DetectionResult> candidateDitects = new List<DetectionResult>();
        // 사용할 검출 결과
        List<DetectionResult> ditects = new List<DetectionResult>();

        for (int i = 0; i < outputWidth; i++)
        {
            // 검출 결과 분석
            var result = new DetectionResult(output0, i);
            // 점수가 기준 값 미만이면 무시
            if (result.score < threshold)
            {
                continue;
            }
            // 후보로 추가
            candidateDitects.Add(result);
        }

        // NonMaxSuppression 처리
        // 겹친 사각형 중 최대 점수를 가진 것을 채택
        while (candidateDitects.Count > 0)
        {
            int idx = 0;
            float maxScore = 0.0f;
            for (int i = 0; i < candidateDitects.Count; i++)
            {
                if (candidateDitects[i].score > maxScore)
                {
                    idx = i;
                    maxScore = candidateDitects[i].score;
                }
            }

            // 점수가 가장 높은 결과를 가져오고, 리스트에서 제거
            var cand = candidateDitects[idx];
            candidateDitects.RemoveAt(idx);

            // 채택할 결과에 추가
            ditects.Add(cand);

            List<int> deletes = new List<int>();
            for (int i = 0; i < candidateDitects.Count; i++)
            {
                // IOU 확인
                float iou = Iou(cand, candidateDitects[i]);
                if (iou >= iouThres)
                {
                    deletes.Add(i);
                }
            }
            for (int i = deletes.Count - 1; i >= 0; i--)
            {
                candidateDitects.RemoveAt(deletes[i]);
            }

        }

        return ditects;

    }


    // 객체의 겹침 정도 판단
    private float Iou(DetectionResult boxA, DetectionResult boxB)
    {
        if ((boxA.x1 == boxB.x1) && (boxA.x2 == boxB.x2) && (boxA.y1 == boxB.y1) && (boxA.y2 == boxB.y2))
        {
            return 1.0f;

        }
        else if (((boxA.x1 <= boxB.x1 && boxA.x2 > boxB.x1) || (boxA.x1 >= boxB.x1 && boxB.x2 > boxA.x1))
          && ((boxA.y1 <= boxB.y1 && boxA.y2 > boxB.y1) || (boxA.y1 >= boxB.y1 && boxB.y2 > boxA.y1)))
        {
            float intersection = (Mathf.Min(boxA.x2, boxB.x2) - Mathf.Max(boxA.x1, boxB.x1))
                * (Mathf.Min(boxA.y2, boxB.y2) - Mathf.Max(boxA.y1, boxB.y1));
            float union = (boxA.x2 - boxA.x1) * (boxA.y2 - boxA.y1) + (boxB.x2 - boxB.x1) * (boxB.y2 - boxB.y1) - intersection;
            return (intersection / union);
        }

        return 0.0f;
    }



    // 이미지 리사이즈 처리
    private static Texture2D ResizedTexture(Texture2D texture, int width, int height)
    {
        // RenderTexture에 쓰기
        var rt = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(texture, rt);
        // RenderTexture에서 데이터 가져오기
        var preRt = RenderTexture.active;
        RenderTexture.active = rt;
        var resizedTexture = new Texture2D(width, height);
        resizedTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        resizedTexture.Apply();
        RenderTexture.active = preRt;
        RenderTexture.ReleaseTemporary(rt);
        return resizedTexture;
    }

}

// 검출 결과
class DetectionResult
{
    public float x1 { get; }
    public float y1 { get; }
    public float x2 { get; }
    public float y2 { get; }
    public int classId { get; }
    public float score { get; }

    public DetectionResult(Tensor t, int idx)
    {
        // 검출 결과로 얻어지는 사각형의 좌표 정보는 0: 중심 x, 1: 중심 y, 2: 폭, 3: 높이
        // 좌표계를 좌상단 xy, 우하단 xy로 변환
        float halfWidth = t[0, 0, idx, 2] / 2;
        float halfHeight = t[0, 0, idx, 3] / 2;
        x1 = t[0, 0, idx, 0] - halfWidth;
        y1 = t[0, 0, idx, 1] - halfHeight;
        x2 = t[0, 0, idx, 0] + halfWidth;
        y2 = t[0, 0, idx, 1] + halfHeight;

        // 나머지 영역에는 각 클래스의 점수가 설정되어 있음
        // 최대값을 판단하여 설정
        int classes = t.shape.channels - 4;
        score = 0f;
        for (int i = 0; i < classes; i++)
        {
            float classScore = t[0, 0, idx, i + 4];
            if (classScore < score)
            {
                continue;
            }
            classId = i;
            score = classScore;
        }
    }

}
