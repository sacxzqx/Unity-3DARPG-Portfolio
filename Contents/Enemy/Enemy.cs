using BehaviorDesigner.Runtime;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// �� ĳ������ ��ü ���� �� �ൿ ������ �����ϴ� Ŭ����
/// ���� ����, ü��/ü�� ����, AI �ൿƮ��, UI HUD, ������ ��� ���� ���� ó����
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
        // ��Ʈ ����� �̵� ���� �״�� ����
        Vector3 nextPosition = Anim.rootPosition;
        nextPosition.y = Agent.nextPosition.y;
        transform.position = nextPosition;


        // ��Ʈ ����� ȸ�� ���� �����ϰ� �÷��̾ �ٶ󺸴� ȸ���� ���
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
            // �÷��̾ �ٶ� �ʿ䰡 ���� ���� ��Ʈ ����� ȸ���� �״�� ���
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

                // �ǰݽÿ��� ��� �����¼��� ��ȯ
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
    /// ���� �ൿ Ʈ���� �ߴ� �� �ٽ� �����ϴ� �޼���
    /// ���� Ʈ���� Stop�ϰ� �� Ʈ���� �������
    /// </summary>
    /// <param name="delay">Ʈ���� ��ü�ϱ� ���� �����ð�</param>
    IEnumerator ResetTreeAfterDelay(float delay)
    {
        behaviorTree.DisableBehavior();
        behaviorTree.ExternalBehavior = null;
        yield return new WaitForSeconds(delay);
        behaviorTree.ExternalBehavior = originalBehaviorAsset;
        Debug.Log("��ü �Ϸ�");
        behaviorTree.EnableBehavior();
    }

    /// <summary>
    /// ���� ���ظ� �Ծ��� �� ȣ��Ǵ� �޼���
    /// ü�� ����, �ǰ� �ִϸ��̼� ���, ��� ó������ ����
    /// </summary>
    /// <param name="damageAmount">���� ���ط�</param>
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
    /// ü�� ���ظ� �Ծ��� �� ȣ��Ǵ� �޼���
    /// ü�� ����, �ı� �� ų���� Ʈ����
    /// </summary>
    /// <param name="baseDamage">���� �ݿ� �� ü�� ���ط�</param>
    public void TakePostureDamage(int baseDamage)
    {
        StartCoroutine(ResetTreeAfterDelay(1f)); // Ʈ�� ���� (�¾����� ���� �ߴ�)

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
    /// �ִϸ��̼� �̺�Ʈ ���� ���� �Լ�
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
    /// ü�� �������� ���� �ε巴�� �����Ͽ� UI�� �ݿ��ϴ� �ڷ�ƾ
    /// ���ÿ� ü�� ��ġ�� ���� ���� ���ŵ�
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
    /// ���� �÷��̾ �������� �� ����Ǵ� ��� ���� ������
    /// ����ǥ UI, ��� �ִϸ��̼� ��� �� ���� AI ���� ��ȯ
    /// </summary>
    private IEnumerator AlertSequence()
    {
        // ��� ���� �� AI�� �۵����� �ʵ��� Ʈ�� ��Ȱ��ȭ
        behaviorTree.DisableBehavior();
        behaviorTree.ExternalBehavior = null;

        EnemyGauge.ShowAlertIcon(true);
        AudioManager.Instance.PlaySFX("Alerted");

        yield return new WaitForSeconds(1f); // ���� ������ ���� ���

        EnemyGauge.ShowAlertIcon(false);
        Anim.SetTrigger("Alert");

        // �ִϸ��̼� ���̸�ŭ ����Ͽ� ���� �Ϸ�
        yield return WaitForClipLength("Alert", 0.2f);

        // AI �� ���� ���·� ���� ��ȯ
        behaviorTree.ExternalBehavior = originalBehaviorAsset;
        behaviorTree.EnableBehavior();

        alertCoroutine = null;
    }

    /// <summary>
    /// ������ �ִϸ��̼� Ŭ���� ����Ǵ� ���� + ���� �ð���ŭ ����ϴ� �ڷ�ƾ
    /// �ִϸ��̼� �̸����� �˻� ����
    /// </summary>
    /// <param name="clipName">�ִϸ��̼� �̸�</param>
    /// <param name="buffer">���� �ð�</param>
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

        Debug.LogWarning($"Ŭ�� '{clipName}' �� ã�� �� �����ϴ�.");
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
            // ���� ������ ã�� ���� ���, �⺻ ��ġ�� ���
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
    /// ų���� �߻��� �÷��̾ ȣ���Ͽ� ���� �ൿ�� �ʱ�ȭ�ϰ� �÷��̾� ��ġ�� �̵���Ű�� �Լ�
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
    /// ������ �ð� ���� ���� ų���� ��ġ�� �ε巴�� �̵� �� ȸ����Ŵ
    /// ���� ��Ȯ���� ���� Lerp �� Slerp ���
    /// </summary>
    /// <param name="playerPosition">�÷��̾� ��ġ</param>
    /// <param name="playerRotation">�÷��̾� ȸ��</param>
    /// <param name="distance">�÷��̾�κ��� ������ �Ÿ�</param>
    /// <param name="duration">�̵� ���� �ð�</param>
    private IEnumerator MoveToPosition(Vector3 playerPosition, Quaternion playerRotation, float distance, float duration)
    {
        Vector3 startPos = transform.position;

        Vector3 targetPos = playerPosition + (playerRotation * Vector3.forward * distance);

        Quaternion startRot = transform.rotation;

        // ������ ���� ȸ�� ���� ���� ����
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
    /// �ִϸ��̼� Ŭ���� ������ ȣ��Ǿ�, ���� ���� ���� �ൿ Ʈ�� Task��
    /// �ִϸ��̼��� �������� �����ϴ� �Լ�
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