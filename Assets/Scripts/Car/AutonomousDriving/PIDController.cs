using UnityEngine;

public class PIDController
{
    private float kp; // 비례 게인
    private float ki; // 적분 게인
    private float kd; // 미분 게인

    private float integral; // 적분 누적 값
    private float previousError; // 이전 오차 값

    private float outputMin; // 출력 최소값
    private float outputMax; // 출력 최대값

    public PIDController(float kp, float ki, float kd, float outputMin, float outputMax)
    {
        this.kp = kp;
        this.ki = ki;
        this.kd = kd;
        this.outputMin = outputMin;
        this.outputMax = outputMax;

        this.integral = 0f;
        this.previousError = 0f;
    }

    public float Update(float error, float deltaTime)
    {
        // 적분 항 계산
        integral += error * deltaTime;

        // 미분 항 계산
        float derivative = (error - previousError) / deltaTime;

        // PID 출력 계산
        float output = kp * error + ki * integral + kd * derivative;


        // 출력 제한
        output = Mathf.Clamp(output, outputMin, outputMax);

        // 이전 오차 업데이트
        previousError = error;

        return output;
    }
}
