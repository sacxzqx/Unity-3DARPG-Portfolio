using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour, IReset
{
    [SerializeField] private Image mainImage;
    [SerializeField] private Image echoImage;

    [Header("Animation Settings")]
    [SerializeField] private float initialDelay = 1.0f;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float colorChangeDuration = 0.1f;
    [SerializeField] private float echoExpandScale = 1.2f;
    [SerializeField] private float echoExpandDuration = 0.4f;
    [SerializeField] private float echoFadeOutDuration = 1.0f;

    private Sequence activeSequence;

    private void Awake()
    {
        GameEventsManager.Instance.PlayerEvents.OnPlayerDied += PlayerDeathEffect;
    }

    private void OnDestroy()
    {
        GameEventsManager.Instance.PlayerEvents.OnPlayerDied -= PlayerDeathEffect;
    }

    public void PlayerDeathEffect()
    {
        mainImage.color = new Color(0.5f, 0.5f, 0.5f, 0f); 
        echoImage.color = new Color(1f, 0f, 0f, 0f);

        echoImage.rectTransform.anchoredPosition = mainImage.rectTransform.anchoredPosition;
        echoImage.transform.localScale = Vector3.one; 

        gameObject.SetActive(true);

        activeSequence = DOTween.Sequence();
        Color solidGray = new Color(0.5f, 0.5f, 0.5f, 1f);

        activeSequence.AppendInterval(initialDelay)
            .Append(mainImage.DOColor(solidGray, fadeInDuration))
            .AppendInterval(2f)
            .AppendCallback(() => AudioManager.Instance.PlaySFX("PlayerDeath"))

            .Append(mainImage.DOColor(Color.red, colorChangeDuration)) // 메인 이미지는 빨갛게
            .Join(echoImage.DOFade(0.5f, echoExpandDuration * 0.3f) // 잔상은 빠르게 나타나고
                .SetEase(Ease.OutQuad) // 부드럽게 나타나도록
                .OnComplete(() => {
                    // 나타난 직후, 서서히 사라지면서 커지도록
                    echoImage.DOFade(0f, echoExpandDuration * 0.7f) // 나머지 시간 동안 페이드 아웃
                        .SetEase(Ease.InQuad); // 빠르게 사라지도록
                }))
            .Join(echoImage.transform.DOScale(echoExpandScale, echoExpandDuration) // 잔상은 커지면서
                .SetEase(Ease.OutCubic)); // 확 커지는 느낌을 위해 Ease 함수 사용

        activeSequence.SetUpdate(true);
    }

    public void ResetBeforeSceneLoad()
    {
        gameObject.SetActive(false);
    }

    public void ResetAfterSceneLoad()
    {
    }
}