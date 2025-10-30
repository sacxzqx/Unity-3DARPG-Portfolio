using DG.Tweening;
using UnityEngine;

/// <summary>
/// 메뉴 UI를 슬라이드 및 페이드 애니메이션으로 토글하는 클래스
/// DOTween을 사용하여 부드러운 전환을 구현
/// </summary>
public class MenuUI : MonoBehaviour
{
    [SerializeField] private GameObject contentParent;

    [SerializeField] private RectTransform menuPanelRect; // 메뉴 패널의 RectTransform
    [SerializeField] private CanvasGroup menuCanvasGroup; // 메뉴 패널의 CanvasGroup

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

        // DOTween Sequence를 사용하여 슬라이드와 페이드를 동시에 실행
        Sequence sequence = DOTween.Sequence();
        sequence.Join(menuPanelRect.DOAnchorPos(onScreenPosition, slideDuration).SetEase(Ease.OutExpo)).SetUpdate(true)
                .Join(menuCanvasGroup.DOFade(1f, fadeDuration).SetUpdate(true));
    }

    private void HideMenu()
    {
        // DOTween Sequence를 사용하여 슬라이드와 페이드를 동시에 실행
        Sequence sequence = DOTween.Sequence();
        sequence.Join(menuPanelRect.DOAnchorPos(offScreenPosition, slideDuration).SetEase(Ease.InExpo).SetUpdate(true))
                .Join(menuCanvasGroup.DOFade(0f, fadeDuration)).SetUpdate(true)
                .OnComplete(() =>
                {
                    contentParent.SetActive(false);
                });
    }
}
