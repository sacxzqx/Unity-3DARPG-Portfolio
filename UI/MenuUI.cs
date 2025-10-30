using DG.Tweening;
using UnityEngine;

/// <summary>
/// �޴� UI�� �����̵� �� ���̵� �ִϸ��̼����� ����ϴ� Ŭ����
/// DOTween�� ����Ͽ� �ε巯�� ��ȯ�� ����
/// </summary>
public class MenuUI : MonoBehaviour
{
    [SerializeField] private GameObject contentParent;

    [SerializeField] private RectTransform menuPanelRect; // �޴� �г��� RectTransform
    [SerializeField] private CanvasGroup menuCanvasGroup; // �޴� �г��� CanvasGroup

    public float slideDuration = 0.5f;
    public float fadeDuration = 0.5f;

    private Vector2 offScreenPosition;
    private Vector2 onScreenPosition;

    private void OnEnable()
    {
        GameEventsManager.Instance.InputEvents.OnMenuTogglePressed += Toggle;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.InputEvents.OnMenuTogglePressed -= Toggle;
    }

    private void Start()
    {
        onScreenPosition = menuPanelRect.anchoredPosition;
        offScreenPosition = new Vector2(-menuPanelRect.rect.width, menuPanelRect.anchoredPosition.y);

        menuPanelRect.anchoredPosition = offScreenPosition;
        menuCanvasGroup.alpha = 0f;
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

        // DOTween Sequence�� ����Ͽ� �����̵�� ���̵带 ���ÿ� ����
        Sequence sequence = DOTween.Sequence();
        sequence.Join(menuPanelRect.DOAnchorPos(onScreenPosition, slideDuration).SetEase(Ease.OutExpo)).SetUpdate(true)
                .Join(menuCanvasGroup.DOFade(1f, fadeDuration).SetUpdate(true));
    }

    private void HideMenu()
    {
        // DOTween Sequence�� ����Ͽ� �����̵�� ���̵带 ���ÿ� ����
        Sequence sequence = DOTween.Sequence();
        sequence.Join(menuPanelRect.DOAnchorPos(offScreenPosition, slideDuration).SetEase(Ease.InExpo).SetUpdate(true))
                .Join(menuCanvasGroup.DOFade(0f, fadeDuration)).SetUpdate(true)
                .OnComplete(() =>
                {
                    contentParent.SetActive(false);
                });
    }
}
