using Cinemachine;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

/// <summary>
/// 게임 전반의 초기화 및 주요 시스템 참조를 관리하는 싱글톤 클래스
/// UI, 카메라, 입력, 스킬 시스템 등을 연결하여 플레이 가능한 상태로 설정함
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Initializers")]
    public GameInitializer GameInitializer;  // 플레이어, 카메라 등 게임 오브젝트 생성 초기화 담당
    [SerializeField] private UIInitializer uiInitializer; // UI 캔버스 프리팹 생성 초기화 담당

    [Header("Managers")]
    [SerializeField] private UIInputManager inputManager; // UI 입력 처리 관리자
    [SerializeField] private SkillManager skillManager; // 스킬 관련 매니저

    private CinemachineBrain cinemachineBrain;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    void Start()
    {
        uiInitializer.Initialize();
        GameInitializer.Initialize();
        SetUp();
        AudioManager.Instance.LoadVolumeSetting();

        if (GameInitializer.MainCameraInstance != null)
        {
            cinemachineBrain = GameInitializer.MainCameraInstance.GetComponent<CinemachineBrain>();
        }
    }

    /// <summary>
    /// 게임 플레이가 활성화되는 시점에 호출 (UI 및 플레이어 활성화 등)
    /// </summary>
    public void ActivateGame()
    {
        GameInitializer.ActiveGame();
        uiInitializer.ActivateUI();
    }

    public void DeActiveGameUI()
    {
        uiInitializer.DeActivateUI();
    }

    /// <summary>
    /// 주요 시스템 연결 및 의존성 주입
    /// </summary>
    public void SetUp()
    {
        // 락온 마커 연결 (일반/패링)
        GameObject lockOnCanvas = uiInitializer.UIDictionary["LockOn_Canvas"];
        Transform canvasTransform = lockOnCanvas.transform;
        GameObject normalMarker = canvasTransform.Find("normalMarker")?.gameObject;
        GameObject parryMarker = canvasTransform.Find("parryMarker")?.gameObject;

        // 플레이어의 락온 시스템 초기화
        GameInitializer.PlayerInstance.GetComponent<LockOn>().Initialize(
            GameInitializer.MainCameraInstance.GetComponent<Camera>(),
            GameInitializer.PlayerCameraInstance.GetComponent<CinemachineVirtualCamera>(),
            GameInitializer.LockOnCameraInstance.GetComponent<CinemachineVirtualCamera>(),
            lockOnCanvas,
            normalMarker,
            parryMarker
        );

        // 두 카메라 모두 플레이어의 LookPoint를 따라가도록 설정
        GameInitializer.PlayerCameraInstance.GetComponent<CinemachineVirtualCamera>().Follow = GameInitializer.PlayerInstance.transform.Find("CameraLookPoint").transform;
        GameInitializer.LockOnCameraInstance.GetComponent<CinemachineVirtualCamera>().Follow = GameInitializer.PlayerInstance.transform.Find("CameraLookPoint").transform;

        // 플레이어의 Context에 메인 카메라 참조 전달
        GameInitializer.PlayerInstance.GetComponent<PlayerContext>().MainCameraTransform = GameInitializer.MainCameraInstance.transform;

        // 인터랙션 시스템에 카메라 및 대화 텍스트 UI 연결
        GameInitializer.PlayerInstance.GetComponentInChildren<Interactions>().MainCamera = GameInitializer.MainCameraInstance.GetComponent<Camera>();
        GameInitializer.PlayerInstance.GetComponentInChildren<Interactions>().DialogueText = uiInitializer.UIDictionary["Interaction_Canvas"].transform.Find("dialogueText").GetComponent<TextMeshProUGUI>();

        // UI 입력 시스템 초기화
        inputManager.Initialize(GameInitializer.PlayerInstance.GetComponent<PlayerInput>());

        // 스킬 시스템 연결 및 초기화
        var playerContext = GameInitializer.PlayerInstance.GetComponent<PlayerContext>();
        var skillManager = uiInitializer.UIDictionary["Skill_Canvas"].GetComponentInChildren<SkillManager>();

        playerContext.SkillManager = skillManager;
        skillManager.Initialize(GameInitializer.PlayerInstance.GetComponent<PlayerStatus>(), playerContext);
    }

    public void LoadScene(string sceneName)
    {
        var resettableObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IReset>();
        foreach (var obj in resettableObjects)
        {
            obj.ResetBeforeSceneLoad();
        }

        LoadingScreen.LoadScene(sceneName);
    }

    private void OnSceneUnloaded(Scene current)
    {
        if (cinemachineBrain != null)
        {
            cinemachineBrain.enabled = false;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var resettableObjects = FindObjectsOfType<MonoBehaviour>(true)
                                .OfType<IReset>();

        foreach (var obj in resettableObjects)
        {
            obj.ResetAfterSceneLoad();
        }

        if (scene.name == "Loading")
        {
            return;
        }

        if (cinemachineBrain != null)
        {
            cinemachineBrain.enabled = true;
        }
    }
}