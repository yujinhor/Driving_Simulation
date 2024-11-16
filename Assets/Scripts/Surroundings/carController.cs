using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class carController : MonoBehaviour
{
    public NavMeshAgent agent;
    //public Animator animator;

    public GameObject PATH; // Pavewalk
    public Transform[] PathPoints;

    public float minDistance = 10;

    public int index = 0;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //animator = GetComponent<Animator>();

        // PATH에서 모든 자식 포인트 가져오기
        PathPoints = new Transform[PATH.transform.childCount];
        for (int i = 0; i < PathPoints.Length; i++)
        {
            PathPoints[i] = PATH.transform.GetChild(i);
        }
    }

    void Update() {
        roam();
    }

    void roam() {
        // 다음 PathPoint로 이동
        if (Vector3.Distance(transform.position, PathPoints[index].position) < minDistance)
        {
            index = (index + 1) % PathPoints.Length; // 순환 구조로 이동
        }

        agent.SetDestination(PathPoints[index].position);
        //animator.SetFloat("vertical", !agent.isStopped ? 1 : 0); // 애니메이션 동기화
    }
}
