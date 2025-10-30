using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SaveLoadMode { Save, Load }

/// <summary>
/// 세이브 로드 UI를 제어하는 클래스
/// </summary>
public class SaveLoadUI : MonoBehaviour
{
    [SerializeField] private GameObject contentParent;
    [SerializeField] private UIPopup saveLoadUIPopup;

    [SerializeField] private Button exitButton;

    public SaveLoadMode CurrentMode { get; private set; }
    public SaveSlot[] Slots;

    private void OnEnable()
    {
        GameEventsManager.Instance.InputEvents.OnSaveMenuRequested += HandleSaveRequest;
        GameEventsManager.Instance.InputEvents.OnLoadMenuRequested += HandleLoadRequest;

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.InputEvents.OnSaveMenuRequested -= HandleSaveRequest;
        GameEventsManager.Instance.InputEvents.OnLoadMenuRequested -= HandleLoadRequest;

        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(OnExitButtonClicked);
        }
    }

    private void HandleSaveRequest() => Toggle(SaveLoadMode.Save);
    private void HandleLoadRequest() => Toggle(SaveLoadMode.Load);

    public void Toggle(SaveLoadMode mode)
    {
        if (contentParent.gameObject.activeSelf && CurrentMode == mode)
        {
            saveLoadUIPopup.Close();
            return;
        }

        Open(mode);
    }

    public void Open(SaveLoadMode mode)
    {
        CurrentMode = mode;
        contentParent.gameObject.SetActive(true);

        for (int i = 0; i <Slots.Length; i++)
        {
            Slots[i].Initialize(i, this);
            string sceneName = SaveManager.Instance.GetSceneName(i);
            string time = SaveManager.Instance.GetSaveTime(i);
            Slots[i].SetSlotData(sceneName, time);
        }

        saveLoadUIPopup.Open();
    }


    /// <summary>
    /// 세이브 슬롯 클릭 시, 세이브 슬롯 UI를 갱신하는 함수
    /// </summary>
    public void OnSlotClicked(SaveSlot slot)
    {
        if (CurrentMode == SaveLoadMode.Save)
        {
            SaveManager.Instance.SaveGame(slot.slotIndex);

            // 저장 직후 해당 슬롯 정보 갱신
            string sceneName = SaveManager.Instance.GetSceneName(slot.slotIndex);
            string time = SaveManager.Instance.GetSaveTime(slot.slotIndex);
            slot.SetSlotData(sceneName, time);
        }
        else
        {
            saveLoadUIPopup.Close();
            GameEventsManager.Instance.InputEvents.NotifyUIClosed();
            AudioManager.Instance.PlaySFX("GameStart");
            SaveManager.Instance.LoadGame(slot.slotIndex);
        }
    }

    public void OnExitButtonClicked()
    {
        if (contentParent.gameObject.activeSelf)
        {
            // StartScene에서는 인게임 UI와 무관하게 Toggle을 호출하지 않음 (TimeScale 문제)
            if (SceneManager.GetActiveScene().name != "StartScene")
            {
                saveLoadUIPopup.Close();
                GameEventsManager.Instance.InputEvents.NotifyUIClosed();
            }

            GameEventsManager.Instance.UIEvents.CloseUI();
        }
    }
}
