using UnityEngine;

public class ExitGame : MonoBehaviour
{
    [SerializeField] private GameObject exitUIPanel;
    [SerializeField] private GameObject closePanel;
    [SerializeField] private CanvasGroup canvasGroup;
 
    public void ClikExit()
    {
        exitUIPanel.SetActive(true);
        if (closePanel != null ) closePanel.SetActive(false);
        if (canvasGroup != null ) canvasGroup.interactable = false;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ExitPanel()
    {
        exitUIPanel.SetActive(false);
        if (canvasGroup != null) canvasGroup.interactable = true;

        if (closePanel != null) GameEventsManager.Instance.InputEvents.NotifyUIClosed();
    }
}
