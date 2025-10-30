using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 플레이어의 락온 시스템을 담당하는 클래스.
/// 주변의 적을 감지하여 가장 적절한 대상에게 락온하고,
/// 타겟 전환, 마커 표시, 카메라 전환 등을 처리
/// </summary>
public class LockOn : MonoBehaviour, IReset
{
    [Header("System References")]
    private PlayerContext playerContext;

    [Header("Markers")]
    public GameObject Marker;          // 락온 시 표시되는 마커 전체 오브젝트
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
    /// 락온 시스템 초기화
    /// GameManager에서 카메라와 마커 오브젝트를 연결
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
        // 락온 토글: 마우스 가운데 버튼 클릭
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

            // 마커 위치 갱신 및 카메라 방향 정렬
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
    /// 캐릭터를 락온 대상 방향으로 부드럽게 회전
    /// </summary>
    private void RotatePlayer()
    {
        Vector3 direction = lockOnEnemy.transform.position - transform.position;
        direction.y = 0;
        if (direction.sqrMagnitude < 0.0001f) return; // 방향 거의 없음: 무시

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        float angleDifference = Quaternion.Angle(playerContext.Rigidbody.rotation, lookRotation);

        // 각도 차이가 클 때만 회전
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

        // 카메라 회전값을 POV에 복사
        Quaternion lockOnRotation = LockOnCamera.transform.rotation;
        Vector3 angles = lockOnRotation.eulerAngles;
        pov.m_HorizontalAxis.Value = angles.y;
        pov.m_VerticalAxis.Value = angles.x;

        OnLockOnDeactivated?.Invoke();
    }

    /// <summary>
    /// 카메라 정면 기준 시야 안에 있는 가장 가까운 적 탐색
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
    /// 락온 대상 기준 좌/우 방향에 있는 적 중 가장 적합한 적 반환
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
    /// 화면상 위치를 기준으로 정렬된 적 리스트에서 다음 또는 이전 타겟을 찾아 순환하여 반환하는 함수
    /// </summary>
    /// <param name="direction">이동할 방향 (-1은 왼쪽, 1은 오른쪽)</param>
    private Enemy FindNextTarget(int direction)
    {
        // 플레이어 주변의 모든 적을 감지
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        if (colliders.Length <= 1) return null; // 현재 락온된 적 외에 다른 적이 없으면 종료

        List<Enemy> visibleEnemies = new List<Enemy>();
        foreach (var col in colliders)
        {
            // 락온 중인 적을 포함하여 모든 유효한 적을 리스트에 추가
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                visibleEnemies.Add(enemy);
            }
        }

        if (visibleEnemies.Count <= 1) return null;

        // 카메라 시점을 기준으로 모든 적을 왼쪽에서 오른쪽으로 정렬
        // Vector3.SignedAngle을 사용하여 카메라 정면을 기준으로 한 각도를 계산
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

        // 인덱스가 리스트의 끝을 넘어가면 처음으로 돌아감
        if (nextIndex >= visibleEnemies.Count)
        {
            nextIndex = 0;
        }
        // 인덱스가 리스트의 처음보다 작아지면 끝으로 돌아감
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
