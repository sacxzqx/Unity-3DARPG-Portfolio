using UnityEngine;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] private Button questLogButton;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button preferenceButton;

    private void Awake()
    {
        if (questLogButton != null)
            questLogButton.onClick.AddListener(() => GameEventsManager.Instance.InputEvents.QuestLogTogglePressed());

        if (inventoryButton != null)
            inventoryButton.onClick.AddListener(() => GameEventsManager.Instance.InputEvents.InventoryTogglePressed());

        if (saveButton != null)
            saveButton.onClick.AddListener(() => GameEventsManager.Instance.InputEvents.OpenSaveMenu());

        if (loadButton != null)
            loadButton.onClick.AddListener(() => GameEventsManager.Instance.InputEvents.OpenLoadMenu());

        if (preferenceButton != null)
            preferenceButton.onClick.AddListener(() => GameEventsManager.Instance.InputEvents.OpenPreference());
    }
}
