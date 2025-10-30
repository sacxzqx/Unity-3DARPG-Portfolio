using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using Unity.Services.Analytics.Internal;

/// <summary>
/// 지정된 Waypoint 경로를 따라 순찰하는 Action 노드
/// Waypoint 도달 시 대기 시간을 두고 방향 전환까지 지원
/// </summary>
public class PatrolWaypoint : Action
{
    private Enemy enemy;
    private NavMeshAgent agent;

    private List<Waypoint> patrolPoints;
    private int currentIndex = 0;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private bool forward = true;

    public float ReachThreshold = 0.5f; // 도착 판정에 사용할 거리 허용값

    public override void OnAwake()
    {
        enemy = GetComponent<Enemy>();
        agent = GetComponent<NavMeshAgent>();
    }

    public override void OnStart()
    {
        agent.updateRotation = true;
        LoadPatrolPoints();

        if (patrolPoints.Count == 0) return;

        // 상태 복원 및 초기화
        currentIndex = enemy.SavedWaypointIndex;
        forward = enemy.SavedWaypointForward;
        isWaiting = false;
        waitTimer = 0f;

        // 이동 지시 및 설정
        agent.SetDestination(patrolPoints[currentIndex].transform.position);
        agent.speed = 1.0f;
        agent.ResetPath();
        enemy.Anim.SetFloat("MoveY", 0f);
    }

    public override TaskStatus OnUpdate()
    {
        if (enemy.CurrentState != EnemyState.Idle || patrolPoints.Count == 0)
        {
            return TaskStatus.Failure;
        }

        if (isWaiting)
        {
            return HandleWaitState();
        }
        else
        {
            return HandleMovementAndArrival();
        }
    }

    public override void OnEnd()
    {
        agent.updateRotation = false;
        agent.ResetPath();
        isWaiting = false;
        waitTimer = 0f;

        enemy.Anim.SetFloat("MoveY", 0f);

        enemy.SavedWaypointIndex = currentIndex;
        enemy.SavedWaypointForward = forward;
    }

    private void CalculateNextIndex()
    {
        if (forward && currentIndex >= patrolPoints.Count - 1)
        {
            forward = false;
        }
        else if (!forward && currentIndex <= 0)
        {
            forward = true;
        }

        currentIndex += forward ? 1 : -1;
        currentIndex = Mathf.Clamp(currentIndex, 0, patrolPoints.Count - 1);
    }

    private void GoToNextPoint()
    {
        CalculateNextIndex();

        enemy.SavedWaypointIndex = currentIndex;
        enemy.SavedWaypointForward = forward;

        agent.SetDestination(patrolPoints[currentIndex].transform.position);
    }

    private void LoadPatrolPoints()
    {
        patrolPoints = new List<Waypoint>();
        foreach (Transform child in enemy.WaypointRoot)
        {
            Waypoint point = child.GetComponent<Waypoint>();
            if (point != null) patrolPoints.Add(point);
        }
    }

    private TaskStatus HandleWaitState()
    {
        enemy.Anim.SetFloat("MoveY", 0f);

        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0f)
        {
            isWaiting = false;
            GoToNextPoint();

            return TaskStatus.Running;
        }

        return TaskStatus.Running;
    }

    private TaskStatus HandleMovementAndArrival()
    {
        enemy.Anim.SetFloat("MoveY", 0.5f);

        if (!agent.pathPending && agent.remainingDistance <= ReachThreshold)
        {
            Waypoint current = patrolPoints[currentIndex];

            if (current.WaitTime > 0)
            {
                isWaiting = true;
                waitTimer = current.WaitTime;
            }
            else
            {
                GoToNextPoint();
            }
        }

        return TaskStatus.Running;
    }
}