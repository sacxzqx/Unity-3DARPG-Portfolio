using DG.Tweening;
using UnityEngine;
using TMPro;

public class LevelUpEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem levelUpParticle;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float fadeDuration = 0.5f;

    [SerializeField] private GameObject levelUpUiPrefab;
    private CanvasGroup canvasGroup;

    private void OnEnable()
    {
        GameEventsManager.Instance.PlayerEvents.OnPlayerLevelUp += PlayerLevelUp;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.PlayerEvents.OnPlayerLevelUp -= PlayerLevelUp;
    }

    private void PlayerLevelUp(int newLevel)
    {
        if (levelUpUiPrefab != null)
        {
            AudioManager.Instance.PlaySFX("LevelUp");

            levelUpUiPrefab = Instantiate(levelUpUiPrefab);

            // CanvasGroup�� ����� ���̵� ȿ�� ����
            canvasGroup = levelUpUiPrefab.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = levelUpUiPrefab.AddComponent<CanvasGroup>();
            }

            TextMeshProUGUI levelUpText = levelUpUiPrefab.GetComponentInChildren<TextMeshProUGUI>();
            if (levelUpText != null)
            {
                levelUpText.text = "���� ��! ���� ����: " + newLevel;
            }

            // ó�� ��Ÿ�� �� ���̵� ��
            canvasGroup.alpha = 0; // ó������ �����ϰ� ����
            levelUpUiPrefab.SetActive(true);
            canvasGroup.DOFade(1, fadeDuration); // ���̵� ��

            // ���� �ð� �� ���̵� �ƿ� ����
            Invoke("FadeOutLevelUpPrefab", displayDuration);
        }
    }

    private void FadeOutLevelUpPrefab()
    {
        if (canvasGroup != null)
        {
            // ���̵� �ƿ� �� UI �ı�
            canvasGroup.DOFade(0, fadeDuration).OnComplete(() =>
            {
                Destroy(levelUpUiPrefab);
            });
        }
    }
}
