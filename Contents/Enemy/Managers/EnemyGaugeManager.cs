using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �� ü��/ü�� ������ �� ��� �������� �����ϴ� �̱��� �Ŵ���
/// EnemyGauge ��ü�� Ǯ���Ͽ� ���� ����� �ּ�ȭ
/// </summary>
public class EnemyGaugeManager : MonoBehaviour, IReset
{
    public static EnemyGaugeManager Instance { get; private set; }

    public Slider HealthBarPrefab;
    public GameObject PostureGaugePrefab;
    public GameObject AlertIconPrefab;

    private Queue<EnemyGauge> gaugePool = new Queue<EnemyGauge>();
    private List<EnemyGauge> allCreatedGauges = new List<EnemyGauge>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ��� ������ EnemyGauge�� Ǯ���� �����ų� ���� ����
    /// </summary>
    public EnemyGauge GetEnemyGauge()
    {
        if (gaugePool.Count > 0)
        {
            var gauge = gaugePool.Dequeue();
            gauge.SetActive(true);
            return gauge;
        }
        else
        {
            Slider newHealthBar = Instantiate(HealthBarPrefab, transform);
            GameObject newPostureGauge = Instantiate(PostureGaugePrefab, transform);
            GameObject newAlertIcon = Instantiate(AlertIconPrefab, transform);

            newHealthBar.gameObject.SetActive(false);
            newPostureGauge.SetActive(false);
            newAlertIcon.SetActive(false);

            Image postureFill = newPostureGauge.transform.Find("Fill").GetComponent<Image>();
            var newGauge = new EnemyGauge(newHealthBar, newPostureGauge, postureFill, newAlertIcon);

            allCreatedGauges.Add(newGauge);

            return newGauge;
        }
    }

    /// <summary>
    /// ����� ���� EnemyGauge�� Ǯ�� ��ȯ
    /// </summary>
    /// <param name="gauge">��ȯ�� UI ��ü</param>
    public void ReturnEnemyGuage(EnemyGauge gauge)
    {
        // �� �������� �� ��ȯ ������ �̹� �ı��Ǿ��ٸ� �ƹ��͵� ���� �ʰ� �׳� ����
        if (gauge == null || gauge.HealthBar == null)
        {
            return;
        }

        gauge.SetActive(false);
        gaugePool.Enqueue(gauge);
    }

    public void ResetBeforeSceneLoad()
    {
        foreach (var gauge in allCreatedGauges)
        {
            if (gauge != null)
            {
                if (gauge.HealthBar != null) Destroy(gauge.HealthBar.gameObject);
                if (gauge.PostureGauge != null) Destroy(gauge.PostureGauge);
                if (gauge.AlertIcon != null) Destroy(gauge.AlertIcon);
            }
        }

        allCreatedGauges.Clear();
        gaugePool.Clear();
    }

    public void ResetAfterSceneLoad()
    {
    }
}
