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

            // CanvasGroup을 사용해 페이드 효과 적용
            canvasGroup = levelUpUiPrefab.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = levelUpUiPrefab.AddComponent<CanvasGroup>();
            }

            TextMeshProUGUI levelUpText = levelUpUiPrefab.GetComponentInChildren<TextMeshProUGUI>();
            if (levelUpText != null)
            {
                levelUpText.text = "레벨 업! 현재 레벨: " + newLevel;
            }

            // 처음 나타날 때 페이드 인
            canvasGroup.alpha = 0; // 처음에는 투명하게 설정
            levelUpUiPrefab.SetActive(true);
            canvasGroup.DOFade(1, fadeDuration); // 페이드 인

            // 일정 시간 후 페이드 아웃 시작
            Invoke("FadeOutLevelUpPrefab", displayDuration);
        }
    }

    private void FadeOutLevelUpPrefab()
    {
        if (canvasGroup != null)
        {
            // 페이드 아웃 후 UI 파괴
            canvasGroup.DOFade(0, fadeDuration).OnComplete(() =>
            {
                Destroy(levelUpUiPrefab);
            });
        }
    }
}
