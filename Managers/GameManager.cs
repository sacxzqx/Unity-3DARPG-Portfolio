using Cinemachine;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

/// <summary>
/// ���� ������ �ʱ�ȭ �� �ֿ� �ý��� ������ �����ϴ� �̱��� Ŭ����
/// UI, ī�޶�, �Է�, ��ų �ý��� ���� �����Ͽ� �÷��� ������ ���·� ������
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Initializers")]
    public GameInitializer GameInitializer;  // �÷��̾�, ī�޶� �� ���� ������Ʈ ���� �ʱ�ȭ ���
    [SerializeField] private UIInitializer uiInitializer; // UI ĵ���� ������ ���� �ʱ�ȭ ���

    [Header("Managers")]
    [SerializeField] private UIInputManager inputManager; // UI �Է� ó�� ������
    [SerializeField] private SkillManager skillManager; // ��ų ���� �Ŵ���

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
    /// ���� �÷��̰� Ȱ��ȭ�Ǵ� ������ ȣ�� (UI �� �÷��̾� Ȱ��ȭ ��)
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
    /// �ֿ� �ý��� ���� �� ������ ����
    /// </summary>
    public void SetUp()
    {
        // ���� ��Ŀ ���� (�Ϲ�/�и�)
        GameObject lockOnCanvas = uiInitializer.UIDictionary["LockOn_Canvas"];
        Transform canvasTransform = lockOnCanvas.transform;
        GameObject normalMarker = canvasTransform.Find("normalMarker")?.gameObject;
        GameObject parryMarker = canvasTransform.Find("parryMarker")?.gameObject;

        // �÷��̾��� ���� �ý��� �ʱ�ȭ
        GameInitializer.PlayerInstance.GetComponent<LockOn>().Initialize(
            GameInitializer.MainCameraInstance.GetComponent<Camera>(),
            GameInitializer.PlayerCameraInstance.GetComponent<CinemachineVirtualCamera>(),
            GameInitializer.LockOnCameraInstance.GetComponent<CinemachineVirtualCamera>(),
            lockOnCanvas,
            normalMarker,
            parryMarker
        );

        // �� ī�޶� ��� �÷��̾��� LookPoint�� ���󰡵��� ����
        GameInitializer.PlayerCameraInstance.GetComponent<CinemachineVirtualCamera>().Follow = GameInitializer.PlayerInstance.transform.Find("CameraLookPoint").transform;
        GameInitializer.LockOnCameraInstance.GetComponent<CinemachineVirtualCamera>().Follow = GameInitializer.PlayerInstance.transform.Find("CameraLookPoint").transform;

        // �÷��̾��� Context�� ���� ī�޶� ���� ����
        GameInitializer.PlayerInstance.GetComponent<PlayerContext>().MainCameraTransform = GameInitializer.MainCameraInstance.transform;

        // ���ͷ��� �ý��ۿ� ī�޶� �� ��ȭ �ؽ�Ʈ UI ����
        GameInitializer.PlayerInstance.GetComponentInChildren<Interactions>().MainCamera = GameInitializer.MainCameraInstance.GetComponent<Camera>();
        GameInitializer.PlayerInstance.GetComponentInChildren<Interactions>().DialogueText = uiInitializer.UIDictionary["Interaction_Canvas"].transform.Find("dialogueText").GetComponent<TextMeshProUGUI>();

        // UI �Է� �ý��� �ʱ�ȭ
        inputManager.Initialize(GameInitializer.PlayerInstance.GetComponent<PlayerInput>());

        // ��ų �ý��� ���� �� �ʱ�ȭ
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