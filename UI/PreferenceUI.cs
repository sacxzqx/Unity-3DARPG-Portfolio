using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreferenceUI : MonoBehaviour
{
    [SerializeField] private GameObject contentParent;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        exitButton.onClick.AddListener(() => HideMenu());
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.InputEvents.OnPreferenceRequested += Toggle;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.InputEvents.OnPreferenceRequested -= Toggle;
    }

    public void Toggle()
    {
        if (contentParent.activeInHierarchy)
        {
            HideMenu();
        }
        else
        {
            ShowMenu();
        }
    }

    private void ShowMenu()
    {
        contentParent.SetActive(true);
    }

    private void HideMenu()
    {
        if (contentParent.gameObject.activeSelf)
        {
            if (SceneManager.GetActiveScene().name != "StartScene")
            {
                GameEventsManager.Instance.InputEvents.NotifyUIClosed();
                contentParent.SetActive(false);
            }

            GameEventsManager.Instance.UIEvents.CloseUI();
        }
    }
}
