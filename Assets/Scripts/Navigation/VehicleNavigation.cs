using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VehicleNavigation : MonoBehaviour
{
    public Transform waypointsParent; // 경유지들이 있는 부모 오브젝트
    private List<Transform> allWaypoints = new List<Transform>(); // 자식 경유지 리스트
    private List<Transform> selectedWaypoints1 = new List<Transform>();
    private int Left_Waypoints = 0;
    private Transform finalDestination; // 최종 목적지
    public float minDistance = 10f; // 경유지와의 최소 거리 기준
    private float waypointDetectionRadius = 15f; // 경로와 경유지 사이의 최소 거리 기준
    private NavMeshAgent agent;
    private int currentWaypointIndex = 0; // 현재 목표 경유지 인덱스
    public LineRenderer vehiclePathLineRenderer; // 차량의 현재 위치에서 다음 경유지까지의 경로를 표시할 LineRenderer
    public LineRenderer waypointsPathLineRenderer; // 나머지 경유지들 간의 경로를 표시할 LineRenderer
    private float minDistanceFromVehicle = 15.0f; // 차량과 경유지 사이의 최소 거리 기준
    private Coroutine pathUpdateCoroutine;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // 부모 오브젝트의 모든 자식을 경유지 리스트에 추가
        if (waypointsParent != null)
        {
            foreach (Transform child in waypointsParent)
            {
                allWaypoints.Add(child);
            }
        }

        SetupLineRenderer(vehiclePathLineRenderer);
        SetupLineRenderer(waypointsPathLineRenderer);
    }


    private void SetupLineRenderer(LineRenderer lineRenderer)
    {
        // LineRenderer 기본 설정
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = 7f;
        lineRenderer.endWidth = 7f;
        lineRenderer.alignment = LineAlignment.View; // 카메라에 맞추어 정렬
    }

    // 목적지를 설정하는 함수
    public void SetDestination(Transform newDestination)
    {
        if (vehiclePathLineRenderer != null)
        {
            vehiclePathLineRenderer.enabled = true;
        }

        if (waypointsPathLineRenderer != null)
        {
            waypointsPathLineRenderer.enabled = true;
        }
        finalDestination = newDestination;
        UpdatePath(); // 초기 경로 설정
        DrawWaypointsPath(); // 경유지들 간의 경로를 설정
        StartPathUpdateRoutine(); // 반복 작업을 코루틴으로 실행
    }

    private void StartPathUpdateRoutine()
    {
        if (pathUpdateCoroutine != null)
        {
            StopCoroutine(pathUpdateCoroutine);
        }
        pathUpdateCoroutine = StartCoroutine(PathUpdateRoutine());
    }

    private IEnumerator PathUpdateRoutine()
    {
        while (true)
        {
            if (Left_Waypoints > 0)
            {
                UpdatePath();
                DrawWaypointsPath();
            }
            else
            {
                Debug.Log("No waypoints left. Path updates paused.");
                yield break; // 코루틴 종료
            }

            yield return new WaitForSeconds(1f); // 3초마다 실행
        }
    }

    private void Update()
    {

        if (Left_Waypoints > 0)
        {
            Transform targetWaypoint = selectedWaypoints1[0];
            agent.SetDestination(targetWaypoint.position);

            if (Vector3.Distance(transform.position, targetWaypoint.position) < minDistance)
            {
                UpdatePath(); // 경로 업데이트
                DrawWaypointsPath(); // 경유지들 간의 경로 다시 그리기
                Debug.Log("Arrived!!!!!!!!!");
            }
        }
        else if (Left_Waypoints == 0)
        {
            // 최종 목적지에 도착한 경우 LineRenderer 비활성화
            Debug.Log("Final destination reached. LineRenderers disabled.");
            DisableLineRenderers();
        }
    }

    private void DisableLineRenderers()
    {
        if (vehiclePathLineRenderer != null)
        {
            vehiclePathLineRenderer.enabled = false;
        }

        if (waypointsPathLineRenderer != null)
        {
            waypointsPathLineRenderer.enabled = false;
        }
    }


    // 차량에서 다음 경유지까지의 경로를 업데이트하는 함수
    private void UpdatePath()
    {
        selectedWaypoints1.Clear(); // 기존 리스트를 비움

        Vector3 nextWaypoint = finalDestination.position;
        // 차량이 NavMeshAgent로 생성한 경로의 각 코너를 확인하고, 경로 근처의 경유지를 선택
        NavMeshPath agentPath = new NavMeshPath();
        agent.CalculatePath(nextWaypoint, agentPath);

        if (agentPath.status == NavMeshPathStatus.PathComplete)
        {
            foreach (Vector3 corner in agentPath.corners)
            {
                //Debug.Log($"=== 코너 {corner} 검사 시작 ===");

                foreach (Transform waypoint in allWaypoints)
                {
                    float distance = Vector3.Distance(corner, waypoint.position);
                    //Debug.Log($"코너 {corner}와 경유지 {waypoint.name} 간 거리: {distance}");

                    if (distance < waypointDetectionRadius)
                    {
                        float distanceFromVehicle = Vector3.Distance(transform.position, waypoint.position);
                        //Debug.Log($"경유지 {waypoint.name}가 코너 {corner}에 가까움 (거리: {distance})");

                        // 차량과의 거리가 충분히 멀고 경로에 가까운 경유지들만 추가
                        if (distanceFromVehicle > minDistanceFromVehicle)
                        {
                            if (!selectedWaypoints1.Contains(waypoint))
                            {
                                selectedWaypoints1.Add(waypoint);
                                //Debug.Log($"경유지 {waypoint.name}가 경로에 가까워 선택됨.");
                            }
                        }
                        else
                        {
                            //Debug.Log($"경유지 {waypoint.name}는 차량과 너무 가까워 선택되지 않음.");
                        }
                    }
                }

                //Debug.Log($"=== 코너 {corner} 검사 종료 ===");
            }

            //Debug.Log("정렬되기전 경유지들:");
            foreach (Transform waypoint in selectedWaypoints1)
            {
                Debug.Log(waypoint.name);
            }
        }

        Left_Waypoints = selectedWaypoints1.Count;

        Debug.Log("Left_Waypoints:" + Left_Waypoints);

        if(Left_Waypoints == 0) { return; }

        // 차량 위치에서 다음 경유지까지의 경로만 NavMesh를 통해 계산
        Vector3 nextWaypoint2 = selectedWaypoints1[0].position;

        // 경로 계산
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(nextWaypoint2, path);

        if (path.corners.Length > 0)
        {
            // 코너 포인트가 있을 때 경로 설정
            vehiclePathLineRenderer.positionCount = path.corners.Length;
            vehiclePathLineRenderer.SetPositions(path.corners);
        }
        else
        {
            // 코너 포인트가 없을 때 차량 위치와 다음 경유지 위치를 직선으로 표시
            vehiclePathLineRenderer.positionCount = 2;
            vehiclePathLineRenderer.SetPosition(0, transform.position); // 현재 차량 위치
            vehiclePathLineRenderer.SetPosition(1, nextWaypoint2);      // 다음 경유지 위치
        }



    }

    // 경유지들 간의 직선 경로를 그리는 함수 (경로 상에 있는 경유지들만 선택)
    private void DrawWaypointsPath()
    {

        List<Vector3> waypointsPathPoints = new List<Vector3>();
        Vector3 nextWaypoint = finalDestination.position;
        // 차량이 NavMeshAgent로 생성한 경로의 각 코너를 확인하고, 경로 근처의 경유지를 선택
        NavMeshPath agentPath = new NavMeshPath();
        agent.CalculatePath(nextWaypoint, agentPath);

        if (agentPath.status == NavMeshPathStatus.PathComplete)
        {
            // 선택된 경유지들의 이름을 거리 순으로 출력
            Debug.Log("경유지순서:");
            foreach (Transform waypoint in selectedWaypoints1)
            {
                Debug.Log(waypoint.name);
            }

            // 선택된 경유지들을 순서대로 이어서 경로 그리기
            for (int i = 0; i < selectedWaypoints1.Count-1; i++)
            {
                Vector3 adjustedPosition = selectedWaypoints1[i].position;
                adjustedPosition.y += 3.0f; // Y축 오프셋 추가
                waypointsPathPoints.Add(adjustedPosition);
            }


            // 최종 목적지 추가
            Vector3 finalAdjustedPosition = finalDestination.position;
            finalAdjustedPosition.y += 3.0f; // Y축 오프셋 추가
            waypointsPathPoints.Add(finalAdjustedPosition);

            // 경유지 간 경로 LineRenderer에 경로 설정
            waypointsPathLineRenderer.positionCount = waypointsPathPoints.Count;
            waypointsPathLineRenderer.SetPositions(waypointsPathPoints.ToArray());
        }
    }


}
