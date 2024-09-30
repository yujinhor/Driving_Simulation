using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timeText; // TextMesh Pro 컴포넌트를 연결할 변수
    private float elapsedTime = 0f;  // 흐른 시간

    void Update()
    {
        // 시간 계산
        elapsedTime += Time.deltaTime;

        // 시간을 시:분:초 형식으로 포맷
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 1000F) % 1000F);

        // 텍스트 업데이트
        timeText.text = string.Format("Time: {0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
}

