using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 공통 UI 팝업 제어 클래스. 열기/닫기 애니메이션과 페이드 인/아웃 지원
/// </summary>
public class UIPopup : MonoBehaviour
{
    [SerializeField] private GameObject popupCanvas;

    private Sequence fadeSequence;

    [Header("Optional Animations")]

    [SerializeField] private DOTweenAnimation doTweenOpenAnimation;
    [SerializeField] private DOTweenAnimation doTweenCloseAnimation;

    [Header("Optional UI Elements")]

    [Tooltip("Optional: 페이드 인/아웃 구현 시 사용")]
    [SerializeField] private CanvasGroup popupGroup;

    public TextMeshProUGUI PopupText;
    public Button YesButton;
    public Button NoButton;

    /// <summary>
    /// 팝업을 열고 등장 애니메이션을 재생
    /// </summary>
    public void Open()
    {
        if (popupCanvas != null)
        {
            UIUtilities.SetUIActive(popupCanvas, true);
            if (doTweenOpenAnimation != null)
            {
                doTweenOpenAnimation.DORestart(true);
            }
        }
    }

    /// <summary>
    /// 팝업을 닫고 퇴장 애니메이션을 재생
    /// </summary>
    public void Close()
    {
        if (popupCanvas != null)
        {
            if (doTweenCloseAnimation != null)
            {
                doTweenCloseAnimation.DORestart();
                doTweenCloseAnimation.onComplete.AddListener(() =>
                {
                    UIUtilities.SetUIActive(popupCanvas, false);
                });
            }
            else
            {
                UIUtilities.SetUIActive(popupCanvas, false);
            }
        }
    }

    /// <summary>
    /// 팝업을 일정 시간 보여준 뒤 자동으로 사라지게 함 (페이드 인/아웃)
    /// </summary>
    public void FadePopupInAndOut(float fadeTime = 0.5f, float visibleDuration = 1f)
    {
        if (fadeSequence != null && fadeSequence.IsActive())
        {
            return;
        }

        if (popupCanvas != null && !popupCanvas.activeSelf)
        {
            Open();
        }

        fadeSequence?.Kill();

        popupGroup.alpha = 0f;
        popupGroup.interactable = false;
        popupGroup.blocksRaycasts = false;

        fadeSequence = DOTween.Sequence()
            .SetUpdate(true)
            .Append(popupGroup.DOFade(1, fadeTime))
            .AppendCallback(() =>
            {
                popupGroup.interactable = true;
                popupGroup.blocksRaycasts = true;
            })
            .AppendInterval(visibleDuration)
            .Append(popupGroup.DOFade(0, fadeTime))
            .AppendCallback(() =>
            {
                popupGroup.interactable = false;
                popupGroup.blocksRaycasts = false;
                Close();
            });
    }

    public void OnCloseAnimationFinished()
    {
        UIUtilities.SetUIActive(popupCanvas, false);
    }
}