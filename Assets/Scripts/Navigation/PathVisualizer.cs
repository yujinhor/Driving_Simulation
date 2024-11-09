using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer), typeof(NavMeshAgent))]
public class PathVisualizer : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private NavMeshAgent agent;
    public float yOffset = 1f;  // NavMesh 위에서 Y축 오프셋

    void Start()
    {
        // Line Renderer 및 NavMeshAgent 참조 가져오기
        lineRenderer = GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;

        lineRenderer.positionCount = agent.path.corners.Length;
        for (int i = 0; i < agent.path.corners.Length; i++)
        {
            Vector3 position = agent.path.corners[i];
            position.y += yOffset;  // Y축에 오프셋 추가
            lineRenderer.SetPosition(i, position);
        }

        // 초기 경로 그리기
        UpdatePathLine();
    }

    void Update()
    {
        // NavMeshAgent의 경로가 업데이트될 때마다 선을 다시 그림
        if (agent.hasPath)
        {
            UpdatePathLine();
            lineRenderer.positionCount = agent.path.corners.Length;
            for (int i = 0; i < agent.path.corners.Length; i++)
            {
                Vector3 position = agent.path.corners[i];
                position.y += yOffset;  // Y축에 오프셋 추가
                lineRenderer.SetPosition(i, position);
            }
        }
    }

    void UpdatePathLine()
    {
        // NavMeshAgent의 경로 점을 가져와 Line Renderer에 할당
        NavMeshPath path = agent.path;
        lineRenderer.positionCount = path.corners.Length;

        // 각 점을 Line Renderer에 설정
        for (int i = 0; i < path.corners.Length; i++)
        {
            lineRenderer.SetPosition(i, path.corners[i]);
        }
    }
}
