using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 시야 내 타겟(예: 플레이어)을 탐지하는 시야 시스템 컴포넌트
/// 일정 간격으로 탐지하며, 감지 상태가 바뀔 경우 이벤트를 발생시킴
/// </summary>
public class FieldOfView : MonoBehaviour
{
    /// <summary>
    /// 감지 상태가 변경되었을 때 호출되는 이벤트
    /// (isVisible: 현재 감지 여부, target: 감지된 객체)
    /// </summary>
    public event Action<bool, GameObject> OnVisibilityChanged;

    [SerializeField] private float viewRadius;

    [Range(0, 360)]
    [SerializeField] private float viewAngle;

    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] private List<Transform> visibleTargets = new List<Transform>(); // 현재 감지된 타겟 리스트

    private GameObject player;

    private Coroutine findTargetsCoroutine;

    private void OnEnable()
    {
        if (findTargetsCoroutine != null)
        {
            StopCoroutine(findTargetsCoroutine);
        }
        findTargetsCoroutine = StartCoroutine(FindTargetsWithDelay(0.2f));
    }

    private void OnDisable()
    {
        if (findTargetsCoroutine != null)
        {
            StopCoroutine(findTargetsCoroutine);
            findTargetsCoroutine = null;
        }
    }

    /// <summary>
    /// 일정 시간 간격으로 시야 내 타겟을 찾는 코루틴
    /// </summary>
    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    /// <summary>
    /// 시야 내 타겟을 탐지하여 리스트에 추가하고, 감지 여부가 바뀌면 이벤트를 발생시킴
    /// </summary>
    void FindVisibleTargets()
    {
        bool previousVisible = visibleTargets.Count > 0;
        visibleTargets.Clear();

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);


        for(int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.transform.position);

                if (!Physics.Raycast(transform.position, dirToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }

        if (previousVisible != (visibleTargets.Count > 0))
        {
            if (visibleTargets.Count > 0)
            {
                player = visibleTargets[0].gameObject;
            }
            OnVisibilityChanged?.Invoke(visibleTargets.Count > 0, player);
        }
    }

    /// <summary>
    /// 특정 각도를 방향 벡터로 변환
    /// </summary>
    /// <param name="angleDegress">각도</param>
    /// <param name="angleGlobal">전역 좌표 기준 여부</param>
    public Vector3 DirFromAngle(float angleDegress, bool angleGlobal)
    {
        if (!angleGlobal)
        {
            angleDegress += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin((-angleDegress + 90) * Mathf.Deg2Rad), 0, Mathf.Cos(-angleDegress + 90) * Mathf.Deg2Rad);
    }

    public void SetViewDistance(float newAngle, float newDistance)
    {
        viewAngle = newAngle;
        viewRadius = newDistance;
    }

    void OnDrawGizmos()
    {
        Vector3 forward = transform.forward;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-viewAngle / 2, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(viewAngle / 2, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * forward;
        Vector3 rightRayDirection = rightRayRotation * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, leftRayDirection * viewRadius);
        Gizmos.DrawRay(transform.position, rightRayDirection * viewRadius);
    }
}
