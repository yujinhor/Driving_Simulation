using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public GameObject[] obstacles; // 제어할 장애물 오브젝트 배열
    public float redLightDuration = 10f; // 차량 빨간불 지속 시간
    public float greenLightDuration = 17f; // 차량 초록불 지속 시간
    public float yellowLightDuration = 3f; // 차량 노란불 지속 시간

    private float timer = 0f; // 타이머
    private int state = 0; // 0 = 차량 빨간불, 1 = 차량 초록불, 2 = 차량 노란불

    void Start()
    {
        SetObstacleState(true); // 초기 상태: 장애물 활성화
    }

    void Update()
    {
        timer += Time.deltaTime;

        //switch (state)
        //{
        //    case 0: // 차량 빨간불
        //        if (timer > redLightDuration)
        //        {
        //            timer = 0f;
        //            state = 1; // 다음 상태로 전환 (초록불)
        //            SetObstacleState(false); // 장애물 비활성화
        //        }
        //        break;

        //    case 1: // 차량 초록불
        //        if (timer > greenLightDuration)
        //        {
        //            timer = 0f;
        //            state = 2; // 다음 상태로 전환 (노란불)
        //        }
        //        break;

        //    case 2: // 차량 노란불
        //        if (timer > yellowLightDuration)
        //        {
        //            timer = 0f;
        //            state = 0; // 초기 상태로 돌아감 (빨간불)
        //            SetObstacleState(true); // 장애물 활성화
        //        }
        //        break;
        //}

        switch (state)
        {
            case 0: // 차량 빨간불, 보행자 초록불
                if (timer > 10f) // 10초 후에 상태 전환
                {
                    timer = 0f;
                    state = 1; // 다음 상태로 변경
                    foreach (GameObject group in obstacles)
                    {
                        SetObstacleState(false); // 차량 초록불로 전환
                    }
                }
                break;
            case 1: // 차량 초록불, 보행자 빨간불
                if (timer > 17f) // 17초 후에 상태 전환
                {
                    timer = 0f;
                    state = 2; // 다음 상태로 변경
                    foreach (GameObject group in obstacles)
                    {
                        SetObstacleState(false); // 차량 노란불로 전환
                    }
                }
                break;
            case 2: // 차량 노란불, 보행자 빨간불
                if (timer > 3f) // 3초 후에 상태 전환
                {
                    timer = 0f;
                    state = 0; // 초기 상태로 돌아감
                    foreach (GameObject group in obstacles)
                    {
                        SetObstacleState(true); // 차량 빨간불로 전환
                    }
                }
                break;
        }
    }

    void SetObstacleState(bool active)
    {
        // 장애물 활성화 또는 비활성화
        foreach (GameObject obstacle in obstacles)
        {
            if (obstacle != null) // 장애물이 존재하는 경우만 처리
            {
                obstacle.SetActive(active);
            }
        }
    }
}
