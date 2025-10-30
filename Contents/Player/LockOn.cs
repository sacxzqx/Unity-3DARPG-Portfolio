using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// �÷��̾��� ���� �ý����� ����ϴ� Ŭ����.
/// �ֺ��� ���� �����Ͽ� ���� ������ ��󿡰� �����ϰ�,
/// Ÿ�� ��ȯ, ��Ŀ ǥ��, ī�޶� ��ȯ ���� ó��
/// </summary>
public class LockOn : MonoBehaviour, IReset
{
    [Header("System References")]
    private PlayerContext playerContext;

    [Header("Markers")]
    public GameObject Marker;          // ���� �� ǥ�õǴ� ��Ŀ ��ü ������Ʈ
    public GameObject NormalMarker; 
    public GameObject ParryMarker;

    [Header("Cameras")]
    public Camera MainCamera;            
    public CinemachineVirtualCamera PlayerCamera;  
    public CinemachineVirtualCamera LockOnCamera; 
    private CinemachinePOV pov;

    [Header("Detection Settings")]
    [SerializeField] private LayerMask enemyLayer; 
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float frontOffset = 5f;

    [Header("Runtime State")]
    public bool IsLockOn = false;
    private Enemy lockOnEnemy;

    public event Action OnLockOnDeactivated;

    /// <summary>
    /// ���� �ý��� �ʱ�ȭ
    /// GameManager���� ī�޶�� ��Ŀ ������Ʈ�� ����
    /// </summary>
    public void Initialize(Camera mainCamera, CinemachineVirtualCamera playerCam, CinemachineVirtualCamera lockOnCam,
                                    GameObject marker, GameObject normalMarker, GameObject parryMarker)
    {
        MainCamera = mainCamera;
        PlayerCamera = playerCam;
        LockOnCamera = lockOnCam;
        Marker = marker;
        NormalMarker = normalMarker;
        ParryMarker = parryMarker;

        pov = PlayerCamera.GetCinemachineComponent<CinemachinePOV>();
    }
    private void Awake()
    {
        playerContext = GetComponent<PlayerContext>();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.EnemyEvents.OnEnemyDie += EnemyDeath;
        GameEventsManager.Instance.EnemyEvents.OnPostureBreak += UpdateParryMarker;
        GameEventsManager.Instance.EnemyEvents.OnRecoveryPosture += UpdateParryMarker;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.EnemyEvents.OnEnemyDie -= EnemyDeath;
        GameEventsManager.Instance.EnemyEvents.OnPostureBreak -= UpdateParryMarker;
        GameEventsManager.Instance.EnemyEvents.OnRecoveryPosture -= UpdateParryMarker;
    }

    private void Update()
    {
        // ���� ���: ���콺 ��� ��ư Ŭ��
        if (Input.GetMouseButtonDown(2))
        {
            if (IsLockOn)
            {
                DeactivateLockOn(lockOnEnemy);
            }
            else
            {
                var closetEnemy = FindClosestEnemy();
                if (closetEnemy != null)
                {
                    ActivateLockOn(closetEnemy);
                }
            }
        }

        if (IsLockOn)
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                var target = FindNextTarget(-1);
                if (target != null) ActivateLockOn(target);
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                var target = FindNextTarget(1);
                if (target != null) ActivateLockOn(target);
            }

            // ��Ŀ ��ġ ���� �� ī�޶� ���� ����
            Transform markerPosition = lockOnEnemy.Marker;
            Marker.transform.position = markerPosition.position;
            Marker.transform.LookAt(MainCamera.transform.position);
        }
    }

    private void FixedUpdate()
    {
        if (IsLockOn)
        {
            RotatePlayer();
        }
    }

    /// <summary>
    /// ĳ���͸� ���� ��� �������� �ε巴�� ȸ��
    /// </summary>
    private void RotatePlayer()
    {
        Vector3 direction = lockOnEnemy.transform.position - transform.position;
        direction.y = 0;
        if (direction.sqrMagnitude < 0.0001f) return; // ���� ���� ����: ����

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        float angleDifference = Quaternion.Angle(playerContext.Rigidbody.rotation, lookRotation);

        // ���� ���̰� Ŭ ���� ȸ��
        if (angleDifference > 3f)
        {
            playerContext.Rigidbody.MoveRotation(Quaternion.Slerp(
                playerContext.Rigidbody.rotation,
                lookRotation,
                Time.deltaTime * 10f
            ));
        }
    }

    private void ActivateLockOn(Enemy enemy)
    {
        if (lockOnEnemy != null && lockOnEnemy != enemy)
        {
            lockOnEnemy.DeActivateHealthBar();
        }

        lockOnEnemy = enemy;

        LockOnCamera.Priority = 20;
        PlayerCamera.Priority = 10;

        IsLockOn = true;
        Marker.SetActive(true);
        enemy.ActivateGauge();

        UpdateParryMarker(enemy);

        playerContext.Anim.SetBool("isLockOn", true);
        LockOnCamera.LookAt = enemy.transform;
    }

    private void DeactivateLockOn(IEnemy enemy)
    {
        RotatePlayer();

        lockOnEnemy = null;

        LockOnCamera.Priority = 10;
        PlayerCamera.Priority = 20;
        LockOnCamera.LookAt = null;

        IsLockOn = false;
        Marker.SetActive(false);
        Marker.transform.SetParent(null);

        enemy.DeActivateHealthBar();

        playerContext.Anim.SetBool("isLockOn", false);

        // ī�޶� ȸ������ POV�� ����
        Quaternion lockOnRotation = LockOnCamera.transform.rotation;
        Vector3 angles = lockOnRotation.eulerAngles;
        pov.m_HorizontalAxis.Value = angles.y;
        pov.m_VerticalAxis.Value = angles.x;

        OnLockOnDeactivated?.Invoke();
    }

    /// <summary>
    /// ī�޶� ���� ���� �þ� �ȿ� �ִ� ���� ����� �� Ž��
    /// </summary>
    private Enemy FindClosestEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + MainCamera.transform.forward * frontOffset, detectionRadius, enemyLayer);
        lockOnEnemy = null;
        float closestDistance = Mathf.Infinity;
        float halfFieldOfView = MainCamera.fieldOfView * 0.5f;

        foreach (Collider col in colliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                Transform enemyTransform = col.transform;
                Vector3 cameraToEnemy = enemyTransform.position - MainCamera.transform.position;
                float angle = Vector3.Angle(MainCamera.transform.forward, cameraToEnemy);

                if (angle <= halfFieldOfView)
                {
                    float distanceToEnemy = Vector3.Distance(MainCamera.transform.position, enemyTransform.position);

                    if (distanceToEnemy < closestDistance)
                    {
                        lockOnEnemy = enemy;
                        closestDistance = distanceToEnemy;
                    }
                }
            }
        }

        return lockOnEnemy;
    }

    /// <summary>
    /// ���� ��� ���� ��/�� ���⿡ �ִ� �� �� ���� ������ �� ��ȯ
    /// </summary>
    private Enemy GetRelativeDirectionalTarget(int direction)
    {
        Collider[] hits = Physics.OverlapSphere(lockOnEnemy.transform.position, detectionRadius, enemyLayer);

        float bestScore = float.MinValue;
        Enemy bestTarget = null;

        foreach (var hit in hits)
        {
            Enemy candidate = hit.GetComponent<Enemy>();
            if (candidate == null || candidate == lockOnEnemy) continue;

            Vector3 toCandidate = (candidate.transform.position - lockOnEnemy.transform.position).normalized;
            float dot = Vector3.Dot(lockOnEnemy.transform.position * direction, toCandidate);

            if (dot < 0) continue;

            float score = dot / (Vector3.SqrMagnitude(candidate.transform.position - lockOnEnemy.transform.position) + 0.01f);
            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = candidate;
            }
        }

        return bestTarget;
    }

    /// <summary>
    /// ȭ��� ��ġ�� �������� ���ĵ� �� ����Ʈ���� ���� �Ǵ� ���� Ÿ���� ã�� ��ȯ�Ͽ� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <param name="direction">�̵��� ���� (-1�� ����, 1�� ������)</param>
    private Enemy FindNextTarget(int direction)
    {
        // �÷��̾� �ֺ��� ��� ���� ����
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        if (colliders.Length <= 1) return null; // ���� ���µ� �� �ܿ� �ٸ� ���� ������ ����

        List<Enemy> visibleEnemies = new List<Enemy>();
        foreach (var col in colliders)
        {
            // ���� ���� ���� �����Ͽ� ��� ��ȿ�� ���� ����Ʈ�� �߰�
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                visibleEnemies.Add(enemy);
            }
        }

        if (visibleEnemies.Count <= 1) return null;

        // ī�޶� ������ �������� ��� ���� ���ʿ��� ���������� ����
        // Vector3.SignedAngle�� ����Ͽ� ī�޶� ������ �������� �� ������ ���
        Vector3 cameraForwardOnPlane = MainCamera.transform.forward;
        cameraForwardOnPlane.y = 0;

        visibleEnemies.Sort((a, b) => {
            Vector3 dirToA = a.transform.position - transform.position;
            Vector3 dirToB = b.transform.position - transform.position;
            dirToA.y = 0;
            dirToB.y = 0;

            float angleA = Vector3.SignedAngle(cameraForwardOnPlane, dirToA, Vector3.up);
            float angleB = Vector3.SignedAngle(cameraForwardOnPlane, dirToB, Vector3.up);

            return angleA.CompareTo(angleB);
        });

        int currentIndex = visibleEnemies.IndexOf(lockOnEnemy);

        int nextIndex = currentIndex + direction;

        // �ε����� ����Ʈ�� ���� �Ѿ�� ó������ ���ư�
        if (nextIndex >= visibleEnemies.Count)
        {
            nextIndex = 0;
        }
        // �ε����� ����Ʈ�� ó������ �۾����� ������ ���ư�
        else if (nextIndex < 0)
        {
            nextIndex = visibleEnemies.Count - 1;
        }

        return visibleEnemies[nextIndex];
    }

    private void UpdateParryMarker(Enemy enemy)
    {
        if (lockOnEnemy == null) return;

        if (lockOnEnemy.IsParryGuageFull)
        {
            NormalMarker.SetActive(false);
            ParryMarker.SetActive(true);
        }
        else
        {
            NormalMarker.SetActive(true);
            ParryMarker.SetActive(false);
        }
    }

    private void UpdateParryMarker()
    {
        NormalMarker.SetActive(true);
        ParryMarker.SetActive(false);
    }

    private void EnemyDeath(IEnemy enemy)
    {
        if (lockOnEnemy != null)
        {
            DeactivateLockOn(enemy);
        }
    }

    public void ResetBeforeSceneLoad()
    {
        if (!IsLockOn) return;

        IsLockOn = false;
        lockOnEnemy = null;

        if (PlayerCamera != null) PlayerCamera.Priority = 20;
        if (LockOnCamera != null)
        {
            LockOnCamera.Priority = 10;
            LockOnCamera.LookAt = null;
        }

        if (Marker != null) Marker.SetActive(false);

        if (playerContext != null && playerContext.Anim != null)
        {
            playerContext.Anim.SetBool("isLockOn", false);
        }

        if (LockOnCamera != null && pov != null)
        {
            Quaternion lockOnRotation = LockOnCamera.transform.rotation;
            Vector3 angles = lockOnRotation.eulerAngles;
            pov.m_HorizontalAxis.Value = angles.y;
            pov.m_VerticalAxis.Value = angles.x;
        }
    }

    public void ResetAfterSceneLoad()
    {
    }
}
