using BehaviorDesigner.Runtime;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 적 캐릭터의 전체 전투 및 행동 로직을 관리하는 클래스
/// 상태 전이, 체력/체간 관리, AI 행동트리, UI HUD, 아이템 드롭 등을 통합 처리함
/// </summary>
public class Enemy : MonoBehaviour, IEnemy, IDamageProvider
{
    [field: Header("Reference")]
    [SerializeField] private EnemySO enemyData;
    [SerializeField] private KillMoveData killMoveData;

    [Header("Spawning")]
    [SerializeField] private GameObject selfPrefab;

    [field: Header("UI Offset")]
    [SerializeField] private Vector3 hpBarOffset = new Vector3(0f, 2f, 0f);
    [SerializeField] private Vector3 postureOffset = new Vector3(0f, 2.2f, 0f);
    [SerializeField] private Vector3 alertOffset = new Vector3(0f, 2.5f, 0f);

    [SerializeField] private ExternalBehavior originalBehaviorAsset;

    public EnemySO EnemyData { get => enemyData; set => enemyData = value; }
    public KillMoveData KillMoveData { get => killMoveData; set => killMoveData = value; }

    [field: Header("Status")]
    public EnemyState CurrentState;
    private int currentHealth;
    private float currentPosture;
    private float targetHealth;
    private float targetPosture;
    public bool IsParryGuageFull = false;
    private bool isGaugeForced = false;
    private float lerpSpeed = 20f;

    [field: Header("UI")]
    public EnemyGauge EnemyGauge;
    [SerializeField] private float guageDuration = 3.0f;
    private Coroutine gaugeCoroutine = null;

    [field: Header("Component")]
    private Camera mainCamera;
    public Animator Anim { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public Collider HitBoxCollider { get; private set; }
    [field: SerializeField] public Collider WeaponCollider { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    private FieldOfView fov;
    private BehaviorTree behaviorTree;

    [field: Header("External Target")]
    [SerializeField] private GameObject itemPrefab;
    [field: SerializeField] public Transform Marker { get; private set; }
    [field: SerializeField] public Transform TargetPlayer { get; private set; }
    private Transform dropPoint;
    protected bool canRotateToPlayer = false;

    [field: Header("AI")]
    public Transform WaypointRoot;
    public int SavedWaypointIndex = 0;
    public bool SavedWaypointForward = true;
    private Coroutine alertCoroutine;
    private bool hasEverBeenInBattle = false;

    private void Awake()
    {
        mainCamera = Camera.main;
        Anim = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
        HitBoxCollider = GetComponent<BoxCollider>();
        Agent = GetComponent<NavMeshAgent>();
        fov = GetComponent<FieldOfView>();
        behaviorTree = GetComponent<BehaviorTree>();
        Anim.applyRootMotion = true;
    }

    void OnEnable()
    {
        Agent.updateRotation = false;
        GameEventsManager.Instance.EnemyEvents.EnemySpawned(this);
        GameEventsManager.Instance.EnemyEvents.OnRecoveryPosture += RecoverPosture;
        fov.OnVisibilityChanged += HandleVisibilityChanged;

        EnemyGauge = EnemyGaugeManager.Instance.GetEnemyGauge();
        if (EnemyGauge != null)
        {
            EnemyGauge.HealthBar.maxValue = enemyData.MaxHealth;
            EnemyGauge.HealthBar.value = currentHealth;
            targetPosture = 0;
            StartCoroutine(UpdatePostureBar());
        }

        currentHealth = enemyData.MaxHealth;
        targetHealth = currentHealth;
        currentPosture = 0;
        targetPosture = 0;
        IsParryGuageFull = false;
        isGaugeForced = false;

        HitBoxCollider.enabled = true;
        hasEverBeenInBattle = false;

        if (Agent != null && !Agent.enabled)
        {
            Agent.enabled = true;
        }

        if (Agent.isOnNavMesh)
        {
            Agent.Warp(transform.position);
            Agent.isStopped = false;
            Agent.ResetPath();
        }

        SetState(EnemyState.Idle, true);
        TargetPlayer = null;
        canRotateToPlayer = false;
        behaviorTree.EnableBehavior();
        behaviorTree.ExternalBehavior = originalBehaviorAsset;
    }

    void OnDisable()
    {
        GameEventsManager.Instance.EnemyEvents.EnemyDespawned(this);
        GameEventsManager.Instance.EnemyEvents.OnRecoveryPosture -= RecoverPosture;
        fov.OnVisibilityChanged -= HandleVisibilityChanged;

        if (EnemyGauge != null)
        {
            EnemyGaugeManager.Instance.ReturnEnemyGuage(EnemyGauge);
            EnemyGauge = null;
        }

        StopAllCoroutines();

        if (behaviorTree != null)
        {
            behaviorTree.DisableBehavior();
        }
    }

    private void Update()
    {
        if (EnemyGauge != null && EnemyGauge.HealthBar.gameObject.activeSelf)
        {
            EnemyGauge.HealthBar.transform.position = transform.position + hpBarOffset;
            EnemyGauge.PostureGauge.transform.position = transform.position + postureOffset;

            EnemyGauge.HealthBar.transform.LookAt(mainCamera.transform);
            EnemyGauge.PostureGauge.transform.LookAt(mainCamera.transform);
        }

        if (EnemyGauge.HealthBar.value != targetHealth)
        {
            EnemyGauge.HealthBar.value = Mathf.MoveTowards(EnemyGauge.HealthBar.value, targetHealth, Time.deltaTime * lerpSpeed);
        }
    }

    void OnAnimatorMove()
    {
        // 루트 모션의 이동 값은 그대로 적용
        Vector3 nextPosition = Anim.rootPosition;
        nextPosition.y = Agent.nextPosition.y;
        transform.position = nextPosition;


        // 루트 모션의 회전 값은 무시하고 플레이어를 바라보는 회전을 계산
        if (canRotateToPlayer && TargetPlayer != null)
        {
            Vector3 dir = (TargetPlayer.position - transform.position).normalized;
            dir.y = 0;

            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion lookRot = Quaternion.LookRotation(dir);
                Quaternion smoothRot = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 20f);
                transform.rotation = smoothRot;
            }
        }
        else
        {
            // 플레이어를 바라볼 필요가 없을 때는 루트 모션의 회전을 그대로 사용
            transform.rotation = Anim.rootRotation;
        }
    }

    void LateUpdate()
    {
        if (EnemyGauge == null) return;

        if (EnemyGauge.AlertIcon != null && EnemyGauge.AlertIcon.activeSelf)
        {
            EnemyGauge.AlertIcon.transform.position = transform.position + alertOffset;
            EnemyGauge.AlertIcon.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }
    }

    private void SetState(EnemyState newState, bool immediate = false)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;

        switch (newState)
        {
            case EnemyState.Idle:
                canRotateToPlayer = false;
                DeActivateHealthBar();
                if (hasEverBeenInBattle) GameEventsManager.Instance.EnemyEvents.ExitBattleMusic();
                fov.SetViewDistance(150, 10f);
                break;

            case EnemyState.Battle:
                canRotateToPlayer = true;
                hasEverBeenInBattle = true;
                if (!immediate)
                {
                    if (alertCoroutine != null) StopCoroutine(alertCoroutine);
                    alertCoroutine = StartCoroutine(AlertSequence());
                }

                // 피격시에는 즉시 전투태세로 전환
                ActivateGauge();
                GameEventsManager.Instance.EnemyEvents.EnterBattleMusic();
                fov.SetViewDistance(360, 25f);
                break;
        }
    }

    private void HandleVisibilityChanged(bool isVisible, GameObject player)
    {
        if (isVisible)
        {
            TargetPlayer = player.transform;

            if (CurrentState != EnemyState.Battle)
            {
                SetState(EnemyState.Battle);
            }
        }
        else
        {
            if (CurrentState != EnemyState.Idle)
            {
                SetState(EnemyState.Idle);
            }

            TargetPlayer = null;
        }
    }

    /// <summary>
    /// 적의 행동 트리를 중단 후 다시 시작하는 메서드
    /// 기존 트리를 Stop하고 새 트리를 재시작함
    /// </summary>
    /// <param name="delay">트리를 교체하기 위한 지연시간</param>
    IEnumerator ResetTreeAfterDelay(float delay)
    {
        behaviorTree.DisableBehavior();
        behaviorTree.ExternalBehavior = null;
        yield return new WaitForSeconds(delay);
        behaviorTree.ExternalBehavior = originalBehaviorAsset;
        Debug.Log("교체 완료");
        behaviorTree.EnableBehavior();
    }

    /// <summary>
    /// 적이 피해를 입었을 때 호출되는 메서드
    /// 체력 감소, 피격 애니메이션 재생, 사망 처리까지 포함
    /// </summary>
    /// <param name="damageAmount">입은 피해량</param>
    public virtual void TakeDamage(int damageAmount)
    {
        if (alertCoroutine != null)
        {
            StopCoroutine(alertCoroutine);
            alertCoroutine = null;

            if (EnemyGauge != null) EnemyGauge.ShowAlertIcon(false);
        }

        Anim.ResetTrigger("Alert");

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, enemyData.MaxHealth);

        if (currentHealth <= 0)
        {
            Die(this);
            return;
        }

        targetHealth = currentHealth;

        SetState(EnemyState.Battle, true);

        if (WeaponCollider != null)
        {
            WeaponCollider.enabled = false;
        }

        Anim.SetTrigger("EnemyHit");
        AudioManager.Instance.PlaySFX("EnemyHit");

        if (isGaugeForced) 
            StartCoroutine(ResetTreeAfterDelay(1f));

        if (gaugeCoroutine != null) 
            StopCoroutine(gaugeCoroutine);
    }

    /// <summary>
    /// 체간 피해를 입었을 때 호출되는 메서드
    /// 체간 증가, 파괴 시 킬무브 트리거
    /// </summary>
    /// <param name="baseDamage">내성 반영 전 체간 피해량</param>
    public void TakePostureDamage(int baseDamage)
    {
        StartCoroutine(ResetTreeAfterDelay(1f)); // 트리 리셋 (맞았으니 동작 중단)

        int actualDamage = Mathf.Max(0, baseDamage - enemyData.PostureResistance);

        targetPosture = Mathf.Clamp(currentPosture + actualDamage, 0, 200);

        StartCoroutine(UpdatePostureBar());

        if (targetPosture >= 200)
        {
            IsParryGuageFull = true;

            AudioManager.Instance.PlaySFX("KillMoveNotice");

            GameEventsManager.Instance.EnemyEvents.PostureBreak(this);
        }
    }

    public void HandleParry()
    {
        Anim.SetTrigger("Enemy_Parried");

        if (WeaponCollider != null)
        {
            WeaponCollider.enabled = false;
        }

        TakePostureDamage(100);
    }

    private void RecoverPosture()
    {
        IsParryGuageFull = false;

        float recoverAmount = 50;
        targetPosture = Mathf.Max(0, targetPosture - recoverAmount);

        StartCoroutine(UpdatePostureBar());
    }

    /// <summary>
    /// 애니메이션 이벤트 전용 헬퍼 함수
    /// </summary>
    public void Die()
    {
        Die(this);
    }

    public void Die(IEnemy enemy)
    {
        GameEventsManager.Instance.EnemyEvents.ExitBattleMusic();

        GameEventsManager.Instance.EnemyEvents.EnemyDie(this);
        GameEventsManager.Instance.EnemyEvents.EnemyDiedForRespawn(this, selfPrefab, WaypointRoot);

        if (EnemyGauge != null) 
            EnemyGauge.ShowAlertIcon(false);

        DropItem();

        gameObject.SetActive(false);
    }

    IEnumerator ActivateGuageForSeconds(float seconds)
    {
        EnemyGauge.SetActive(true);

        yield return new WaitForSeconds(seconds);

        EnemyGauge.SetActive(false);
        gaugeCoroutine = null;
    }

    public void ActivateGauge()
    {
        isGaugeForced = true;
        EnemyGauge.SetActive(true);
    }

    public void DeActivateHealthBar()
    {
        isGaugeForced = false;
        EnemyGauge.SetActive(false);
    }

    /// <summary>
    /// 체간 게이지의 값을 부드럽게 보간하여 UI에 반영하는 코루틴
    /// 동시에 체간 수치에 따라 색상도 갱신됨
    /// </summary>
    private IEnumerator UpdatePostureBar()
    {
        float duration = 0.4f;
        float elapsed = 0f;

        float startValue = currentPosture;
        float targetValue = targetPosture;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newWidth = Mathf.Lerp(startValue, targetValue, elapsed / duration);

            EnemyGauge.PostureFill.rectTransform.sizeDelta = new Vector2(newWidth, EnemyGauge.PostureFill.rectTransform.sizeDelta.y);

            UpdatePostureColor(targetPosture / 200);

            yield return null;
        }

        EnemyGauge.PostureFill.rectTransform.sizeDelta = new Vector2(targetValue, EnemyGauge.PostureFill.rectTransform.sizeDelta.y);

        currentPosture = targetPosture;
    }

    private void UpdatePostureColor(float fillRatio)
    {
        if (fillRatio < 0.3f)
        {
            EnemyGauge.PostureFill.color = Color.green;
        }
        else if (fillRatio < 0.7f)
        {
            EnemyGauge.PostureFill.color = Color.yellow;
        }
        else
        {
            EnemyGauge.PostureFill.color = Color.red;
        }
    }

    /// <summary>
    /// 적이 플레이어를 감지했을 때 실행되는 경고 연출 시퀀스
    /// 느낌표 UI, 경고 애니메이션 출력 후 전투 AI 모드로 전환
    /// </summary>
    private IEnumerator AlertSequence()
    {
        // 경고 연출 중 AI가 작동하지 않도록 트리 비활성화
        behaviorTree.DisableBehavior();
        behaviorTree.ExternalBehavior = null;

        EnemyGauge.ShowAlertIcon(true);
        AudioManager.Instance.PlaySFX("Alerted");

        yield return new WaitForSeconds(1f); // 연출 템포를 위한 대기

        EnemyGauge.ShowAlertIcon(false);
        Anim.SetTrigger("Alert");

        // 애니메이션 길이만큼 대기하여 연출 완료
        yield return WaitForClipLength("Alert", 0.2f);

        // AI 및 전투 상태로 최종 전환
        behaviorTree.ExternalBehavior = originalBehaviorAsset;
        behaviorTree.EnableBehavior();

        alertCoroutine = null;
    }

    /// <summary>
    /// 지정한 애니메이션 클립이 재생되는 길이 + 여유 시간만큼 대기하는 코루틴
    /// 애니메이션 이름으로 검색 수행
    /// </summary>
    /// <param name="clipName">애니메이션 이름</param>
    /// <param name="buffer">버퍼 시간</param>
    private IEnumerator WaitForClipLength(string clipName, float buffer = 0.1f)
    {
        foreach (var clip in Anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                yield return new WaitForSeconds(clip.length + buffer);
                yield break;
            }
        }

        Debug.LogWarning($"클립 '{clipName}' 을 찾을 수 없습니다.");
    }

    private void DropItem()
    {
        Vector3 dropPosition = transform.position;

        RaycastHit hit;
        if (Physics.Raycast(dropPosition, Vector3.down, out hit, Mathf.Infinity))
        {
            dropPosition = hit.point + new Vector3(0, 1f, 0);
        }
        else
        {
            // 만약 지면을 찾지 못한 경우, 기본 위치로 드롭
            dropPosition = transform.position + new Vector3(0, -2f, 0);
        }

        GameObject itemObject = Instantiate(itemPrefab, dropPosition, itemPrefab.transform.rotation);

        ItemObject itemScript = itemObject.GetComponent<ItemObject>();
        if (itemScript != null)
        {
            itemScript.SetDroppedItem(enemyData.DropItems[0]);
        }
    }

    public void TriggerKillmove()
    {
        if (killMoveData != null)
        {
            GetComponent<Animator>().SetTrigger(killMoveData.EnemyAnimationTrigger);
        }
    }

    /// <summary>
    /// 킬무브 발생시 플레이어가 호출하여 적의 행동을 초기화하고 플레이어 위치로 이동시키는 함수
    /// </summary>
    public void MoveToKillMovePosition(Vector3 playerPosition, Quaternion playerRotation)
    {
        behaviorTree.DisableBehavior();

        if (Agent.isOnNavMesh)
        {
            Agent.isStopped = true;
            Agent.enabled = false;
        }

        StartCoroutine(MoveToPosition(playerPosition, playerRotation, KillMoveData.Distance, KillMoveData.Duration));
    }

    /// <summary>
    /// 지정된 시간 동안 적을 킬무브 위치로 부드럽게 이동 및 회전시킴
    /// 연출 정확도를 위해 Lerp 및 Slerp 사용
    /// </summary>
    /// <param name="playerPosition">플레이어 위치</param>
    /// <param name="playerRotation">플레이어 회전</param>
    /// <param name="distance">플레이어로부터 떨어질 거리</param>
    /// <param name="duration">이동 지속 시간</param>
    private IEnumerator MoveToPosition(Vector3 playerPosition, Quaternion playerRotation, float distance, float duration)
    {
        Vector3 startPos = transform.position;

        Vector3 targetPos = playerPosition + (playerRotation * Vector3.forward * distance);

        Quaternion startRot = transform.rotation;

        // 설정에 따라 회전 반전 여부 결정
        Quaternion targetRot = killMoveData.ReverseRotation
            ? playerRotation * Quaternion.Euler(0, 180, 0)
            : playerRotation;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
    }

    /// <summary>
    /// 애니메이션 클립의 끝에서 호출되어, 현재 실행 중인 행동 트리 Task에
    /// 애니메이션이 끝났음을 전달하는 함수
    /// </summary>
    public void AnimationFinishedEvent()
    {
        if (behaviorTree != null)
        {
            behaviorTree.SendEvent("AnimationFinished");
        }
    }

    public int GetDamageAmount()
    {
        return EnemyData.AttackPower;
    }
}