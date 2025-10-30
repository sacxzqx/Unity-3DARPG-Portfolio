using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �þ� �� Ÿ��(��: �÷��̾�)�� Ž���ϴ� �þ� �ý��� ������Ʈ
/// ���� �������� Ž���ϸ�, ���� ���°� �ٲ� ��� �̺�Ʈ�� �߻���Ŵ
/// </summary>
public class FieldOfView : MonoBehaviour
{
    /// <summary>
    /// ���� ���°� ����Ǿ��� �� ȣ��Ǵ� �̺�Ʈ
    /// (isVisible: ���� ���� ����, target: ������ ��ü)
    /// </summary>
    public event Action<bool, GameObject> OnVisibilityChanged;

    [SerializeField] private float viewRadius;

    [Range(0, 360)]
    [SerializeField] private float viewAngle;

    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] private List<Transform> visibleTargets = new List<Transform>(); // ���� ������ Ÿ�� ����Ʈ

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
    /// ���� �ð� �������� �þ� �� Ÿ���� ã�� �ڷ�ƾ
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
    /// �þ� �� Ÿ���� Ž���Ͽ� ����Ʈ�� �߰��ϰ�, ���� ���ΰ� �ٲ�� �̺�Ʈ�� �߻���Ŵ
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
    /// Ư�� ������ ���� ���ͷ� ��ȯ
    /// </summary>
    /// <param name="angleDegress">����</param>
    /// <param name="angleGlobal">���� ��ǥ ���� ����</param>
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
