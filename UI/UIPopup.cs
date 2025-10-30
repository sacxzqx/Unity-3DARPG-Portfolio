using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� UI �˾� ���� Ŭ����. ����/�ݱ� �ִϸ��̼ǰ� ���̵� ��/�ƿ� ����
/// </summary>
public class UIPopup : MonoBehaviour
{
    [SerializeField] private GameObject popupCanvas;

    private Sequence fadeSequence;

    [Header("Optional Animations")]

    [SerializeField] private DOTweenAnimation doTweenOpenAnimation;
    [SerializeField] private DOTweenAnimation doTweenCloseAnimation;

    [Header("Optional UI Elements")]

    [Tooltip("Optional: ���̵� ��/�ƿ� ���� �� ���")]
    [SerializeField] private CanvasGroup popupGroup;

    public TextMeshProUGUI PopupText;
    public Button YesButton;
    public Button NoButton;

    /// <summary>
    /// �˾��� ���� ���� �ִϸ��̼��� ���
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
    /// �˾��� �ݰ� ���� �ִϸ��̼��� ���
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
    /// �˾��� ���� �ð� ������ �� �ڵ����� ������� �� (���̵� ��/�ƿ�)
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