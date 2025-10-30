using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 적 체력/체간 게이지 및 경고 아이콘을 관리하는 싱글톤 매니저
/// EnemyGauge 객체를 풀링하여 생성 비용을 최소화
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
    /// 사용 가능한 EnemyGauge를 풀에서 꺼내거나 새로 생성
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
    /// 사용이 끝난 EnemyGauge를 풀로 반환
    /// </summary>
    /// <param name="gauge">반환할 UI 객체</param>
    public void ReturnEnemyGuage(EnemyGauge gauge)
    {
        // 이 게이지가 씬 전환 등으로 이미 파괴되었다면 아무것도 하지 않고 그냥 종료
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
