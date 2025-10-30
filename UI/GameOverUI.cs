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

            .Append(mainImage.DOColor(Color.red, colorChangeDuration)) // ���� �̹����� ������
            .Join(echoImage.DOFade(0.5f, echoExpandDuration * 0.3f) // �ܻ��� ������ ��Ÿ����
                .SetEase(Ease.OutQuad) // �ε巴�� ��Ÿ������
                .OnComplete(() => {
                    // ��Ÿ�� ����, ������ ������鼭 Ŀ������
                    echoImage.DOFade(0f, echoExpandDuration * 0.7f) // ������ �ð� ���� ���̵� �ƿ�
                        .SetEase(Ease.InQuad); // ������ ���������
                }))
            .Join(echoImage.transform.DOScale(echoExpandScale, echoExpandDuration) // �ܻ��� Ŀ���鼭
                .SetEase(Ease.OutCubic)); // Ȯ Ŀ���� ������ ���� Ease �Լ� ���

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