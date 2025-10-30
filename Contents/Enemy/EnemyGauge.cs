using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� ü�¹�, ü�� ������, ��� ������ ���� HUD ��Ҹ� �����ϴ� Ŭ����
/// EnemyGaugeManager���� �����ϸ�, Enemy�� �̸� ������
/// </summary>
public class EnemyGauge
{
    public Slider HealthBar;
    public GameObject PostureGauge;
    public Image PostureFill;
    public GameObject AlertIcon;

    private float originalWidth;

    /// <summary>
    /// EnemyGauge ������: �ܺο��� UI ��Ҹ� ���Թ޾� �ʱ�ȭ
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
    /// ü�¹� �� ü�� �������� ǥ�� ���θ� ������
    /// </summary>
    /// <param name="isActive">ǥ������ ����</param>
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
    /// ü�� �������� �ʺ� ������ ���� ������
    /// </summary>
    /// <param name="currentPosture">���� ü�� ��ġ</param>
    /// <param name="maxPosture">�ִ� ü�� ��ġ</param>
    public void UpdatePosture(float currentPosture, float maxPosture)
    {
        float fillAmount = currentPosture / maxPosture;

        if (PostureFill != null)
        {
            PostureFill.rectTransform.sizeDelta = new Vector2(originalWidth * fillAmount, PostureFill.rectTransform.sizeDelta.y);
        }
    }
}
