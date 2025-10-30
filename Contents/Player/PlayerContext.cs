using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 플레이어의 핵심 데이터와 상태머신, 컴포넌트 참조를 포함하는 마스터 컨텍스트 클래스
/// 모든 서브 시스템이 참조하는 중심 객체 역할
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class PlayerContext : MonoBehaviour, ISavable
{
    [field: Header("Data & Configuration")]
    [field: SerializeField] public PlayerSO Data { get; private set; }
    [field: SerializeField] public PlayerAnimationsData AnimationsData { get; private set; }
    [field: SerializeField] public PlayerLayerData LayerData { get; private set; }

    public Rigidbody Rigidbody { get; private set; }
    public Animator Anim { get; private set; }
    public PlayerInput Input { get; private set; }
    public PlayerStatus PlayerStatus { get; private set; }

    [Header("External Systems & Flow Control")]
    public PlayerMovementStateMachine MovementStateMachine;
    public PlayerActionStateMachine ActionStateMachine;
    public InputBuffer InputBuffer;
    public Interactions Interaction;
    public LockOn LockOn { get; private set; } // GetComponent로 가져오면 여기에 배치
    public SkillManager SkillManager { get; set; } // DI 대상
    public Transform MainCameraTransform { get; set; } // DI 대상

    [Header("Weapon & Collisions")]
    public Collider HitBoxCollider;
    public Collider ParryCollider;
    public Collider WeaponCollider;
    public GameObject ObjectOfWeaponSheath;
    public GameObject ObjectOfWeaponDrawn;
    public GameObject PlayerWeapon;

    [field: SerializeField] public PlayerCapsuleColliderUtility ColliderUtility { get; private set; }

    [Header("Runtime Status")]
    public SkillSO CurrentSkill;
    public PlayerStat Stat { get; private set; }
    public Transform LookPoint;


    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Anim = GetComponent<Animator>();

        Input = GetComponent<PlayerInput>();
        LockOn = GetComponent<LockOn>();
        SkillManager = FindAnyObjectByType<SkillManager>();

        ColliderUtility.Initialize(gameObject);
        ColliderUtility.CalculateCapsuleColliderDimensions();
        AnimationsData.Initialize();

        InputBuffer = GetComponent<InputBuffer>();
        PlayerStatus = GetComponent<PlayerStatus>();

        Stat = PlayerStatus.playerStat;

        MovementStateMachine = new PlayerMovementStateMachine(this);
        ActionStateMachine = new PlayerActionStateMachine(this, MovementStateMachine);
    }

    private void Start()
    {
        MovementStateMachine.SetState(MovementStateMachine.IdlingState);
        ActionStateMachine.SetState(ActionStateMachine.SheathedState);
    }

    private void OnEnable()
    {
        ActionStateMachine.SubscribeInputs();
        SaveManager.Instance.RegisterSavable(this);

        SceneManager.sceneUnloaded += OnSceneUnloaded; 
        SceneManager.sceneLoaded += OnSceneLoaded;     
    }
    
    private void OnDisable()
    {
        ActionStateMachine.UnsubscribeInputs();
        SaveManager.Instance.UnregisterSavable(this);

        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnValidate()
    {
        // 인스펙터 상에서 변경될 때 충돌 크기 재계산
        ColliderUtility.Initialize(gameObject);
        ColliderUtility.CalculateCapsuleColliderDimensions();
    }

    private void Update()
    {
        MovementStateMachine.HandleInput();
        MovementStateMachine.Update();
        ActionStateMachine.HandleInput();
        ActionStateMachine.Update();
    }

    private void FixedUpdate()
    {
        MovementStateMachine.PhysicsUpdate();
        ActionStateMachine.PhysicsUpdate();
    }

    private void OnTriggerEnter(Collider collider)
    {
        ActionStateMachine.OnTriggerEnter(collider);
        MovementStateMachine.OnTriggerEnter(collider);
    }

    private void OnTriggerExit(Collider other)
    {
        ActionStateMachine.OnTriggerExit(other);
        MovementStateMachine.OnTriggerExit(other);
    }

    public void OnMovementStateAnimationEnterEvent()
    {
        MovementStateMachine.OnAnimationEnterEvent();
    }

    public void OnMovementStateAnimationExitEvent()
    {
        MovementStateMachine.OnAnimationExitEvent();
    }

    public void OnMovementStateAnimationTransitionEvent()
    {
        MovementStateMachine.OnAnimationTransitionEvent();
    }

    public void OnActionStateAnimationEnterEvent()
    {
        ActionStateMachine.OnAnimationEnterEvent();
    }

    public void OnActionStateAnimationExitEvent()
    {
        ActionStateMachine.OnAnimationExitEvent();
    }

    public void OnActionStateAnimationTransitionEvent()
    {
        ActionStateMachine.OnAnimationTransitionEvent();
    }

    public void SetCurrentSkill(SkillSO skill)
    {
        CurrentSkill = skill;
    }

    private void OnSceneUnloaded(Scene current)
    {
        Rigidbody.isKinematic = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Loading")
        {
            return;
        }

        Rigidbody.isKinematic = false;
    }

    public void ResetAnimatorParameters(Animator animator)
    {
        if (animator == null) return;

        foreach (var param in animator.parameters)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(param.name, 0f);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(param.name, 0);
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(param.name, false);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    animator.ResetTrigger(param.name);
                    break;
            }
        }
    }

    public void SaveData(GameSaveData data)
    {
        data.PlayerData.SceneName = SceneManager.GetActiveScene().name;
        data.PlayerData.Position = transform.position;
    }

    public void LoadData(GameSaveData data)
    {
        transform.position = data.PlayerData.Position;

        MovementStateMachine.SetState(MovementStateMachine.IdlingState);
        ActionStateMachine.SetState(ActionStateMachine.SheathedState);

        ResetAnimatorParameters(Anim);
        Anim.Play("Idle 0", 0, 0f);
        Anim.SetBool(AnimationsData.IdleParameterHash, true);

        PlayerWeapon.transform.SetParent(ObjectOfWeaponSheath.transform);
        PlayerWeapon.transform.localPosition = Data.ActionData.WeaponTransformData.SheathedPosition;
        PlayerWeapon.transform.localRotation = Quaternion.Euler(Data.ActionData.WeaponTransformData.SheathedRotation);
    }
}
