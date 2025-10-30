using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SaveLoadMode { Save, Load }

/// <summary>
/// ���̺� �ε� UI�� �����ϴ� Ŭ����
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
    /// ���̺� ���� Ŭ�� ��, ���̺� ���� UI�� �����ϴ� �Լ�
    /// </summary>
    public void OnSlotClicked(SaveSlot slot)
    {
        if (CurrentMode == SaveLoadMode.Save)
        {
            SaveManager.Instance.SaveGame(slot.slotIndex);

            // ���� ���� �ش� ���� ���� ����
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
            // StartScene������ �ΰ��� UI�� �����ϰ� Toggle�� ȣ������ ���� (TimeScale ����)
            if (SceneManager.GetActiveScene().name != "StartScene")
            {
                saveLoadUIPopup.Close();
                GameEventsManager.Instance.InputEvents.NotifyUIClosed();
            }

            GameEventsManager.Instance.UIEvents.CloseUI();
        }
    }
}
