using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class npcController_Pavewalk : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator animator;

    public float minDistance = 10; // 거리 기준

    private int index = 0;
    private bool forward = true; // true: 정방향, false: 역방향

    public GameObject CROSSWALK; // Crosswalk
    public Transform[] CrosswalkPoints;

    private float timer = 0f;
    private int state = 0; // 0 = 차량 빨간불, 1 = 차량 초록불, 2 = 차량 노란불

    private bool isCrossing = false; // 보행자가 횡단보도에 있는지 여부

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // CROSSWALK에서 모든 자식 포인트 가져오기
        CrosswalkPoints = new Transform[CROSSWALK.transform.childCount];
        for (int i = 0; i < CrosswalkPoints.Length; i++)
        {
            CrosswalkPoints[i] = CROSSWALK.transform.GetChild(i);
        }
    }

    void Update()
    {
        crosswalk(); // 신호등 상태 확인
    }

    void crosswalk()
    {
        timer += Time.deltaTime;

        switch (state)
        {
            case 0: // 차량 빨간불, 보행자 초록불

                if (Vector3.Distance(transform.position, CrosswalkPoints[index].position) < minDistance)
                {
                    index = (index + 1) % CrosswalkPoints.Length; // 순환 구조로 이동
                }

                agent.SetDestination(CrosswalkPoints[index].position);
                animator.SetFloat("vertical", !agent.isStopped ? 1 : 0);

                
                if (timer > 10f) // 10초 후에 상태 전환
                {
                    timer = 0f;
                    state = 1; // 다음 상태로 변경
                }
                break;

            case 1: // 차량 초록불, 보행자 빨간불
            case 2: // 차량 노란불, 보행자 빨간불
                animator.SetFloat("vertical", 0);
                if ((state == 1 && timer > 17f) || (state == 2 && timer > 3f))
                {
                    timer = 0f;
                    state = (state == 1) ? 2 : 0; // 다음 상태로 전환
                }
                break;
        }
    }
}
