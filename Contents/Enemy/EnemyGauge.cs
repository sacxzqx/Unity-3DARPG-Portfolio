using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 적의 체력바, 체간 게이지, 경고 아이콘 등의 HUD 요소를 관리하는 클래스
/// EnemyGaugeManager에서 관리하며, Enemy가 이를 제어함
/// </summary>
public class EnemyGauge
{
    public Slider HealthBar;
    public GameObject PostureGauge;
    public Image PostureFill;
    public GameObject AlertIcon;

    private float originalWidth;

    /// <summary>
    /// EnemyGauge 생성자: 외부에서 UI 요소를 주입받아 초기화
    /// </summary>
    public EnemyGauge(Slider healthBar, GameObject postureGauge, Image postureFill, GameObject alertIcon)
    {
        this.HealthBar = healthBar;
        this.PostureGauge = postureGauge;
        this.PostureFill = postureFill;
        this.AlertIcon = alertIcon;

        originalWidth = postureFill.rectTransform.sizeDelta.x;
    }

    /// <summary>
    /// 체력바 및 체간 게이지의 표시 여부를 설정함
    /// </summary>
    /// <param name="isActive">표시할지 여부</param>
    public void SetActive(bool isActive)
    {
        HealthBar.gameObject.SetActive(isActive);
        PostureGauge.SetActive(isActive);
    }

    public void ShowAlertIcon(bool show)
    {
        if (AlertIcon != null)
            AlertIcon.SetActive(show);
    }

    /// <summary>
    /// 체간 게이지의 너비를 비율에 따라 갱신함
    /// </summary>
    /// <param name="currentPosture">현재 체간 수치</param>
    /// <param name="maxPosture">최대 체간 수치</param>
    public void UpdatePosture(float currentPosture, float maxPosture)
    {
        float fillAmount = currentPosture / maxPosture;

        if (PostureFill != null)
        {
            PostureFill.rectTransform.sizeDelta = new Vector2(originalWidth * fillAmount, PostureFill.rectTransform.sizeDelta.y);
        }
    }
}
