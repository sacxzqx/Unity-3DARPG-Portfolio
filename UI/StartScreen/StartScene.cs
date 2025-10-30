using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 타이틀 화면을 관리하는 클래스
/// </summary>
public class StartScene : MonoBehaviour
{
    [SerializeField] private SaveLoadUI saveLoadUI;
    [SerializeField] private UIPopup loadUIPopup;
    [SerializeField] private UIPopup settingUIPopup;

    [SerializeField] private GameObject preference_Canvas;
    [SerializeField] private GameObject titleScreen_Canvas;

    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private void OnEnable()
    {
        GameEventsManager.Instance.UIEvents.OnUIClosed += ExitLoadPanel;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.UIEvents.OnUIClosed -= ExitLoadPanel;
    }

    private void Start()
    {
        GameEventsManager.Instance.InputEvents.SetPlayerInput(false);

        if (AudioManager.Instance != null)
        {
            bgmSlider.value = AudioManager.Instance.BgmVolume;
            sfxSlider.value = AudioManager.Instance.SfxVolume;
        }
    }

    private void OnDestroy()
    {
        GameEventsManager.Instance.InputEvents.SetPlayerInput(true);
    }

    /// <summary>
    /// 타이틀 화면의 새 게임 버튼에 할당될 함수
    /// </summary>
    public void StartNewGame()
    {
        ScreenManager.Instance.LoadScreenTexture();
        SpawnManager.NextSpawnPointID = SpawnPointID.Default;
        AudioManager.Instance.PlaySFX("GameStart");
        GameManager.Instance.LoadScene("NewGame2");
        Cursor.visible = false;
    }

    /// <summary>
    /// 타이틀 화면의 불러오기 버튼에 할당될 함수
    /// </summary>
    public void LoadGame()
    {
        saveLoadUI.Open(SaveLoadMode.Load);
        titleScreen_Canvas.gameObject.SetActive(false);
    }

    public void ExitLoadPanel()
    {
        loadUIPopup.Close();
        titleScreen_Canvas.gameObject.SetActive(true);
    }

    /// <summary>
    /// 타이틀 화면의 환경 설정 버튼에 할당될 함수
    /// </summary>
    public void OpenSetting()
    {
        settingUIPopup.Open();
        titleScreen_Canvas.gameObject.SetActive(false);
    }

    public void SetPreference()
    {
        preference_Canvas.SetActive(true);
        titleScreen_Canvas.SetActive(false);
    }

    public void CloseSetting()
    {
        settingUIPopup.Close();
        titleScreen_Canvas.gameObject.SetActive(true);
    }
}
