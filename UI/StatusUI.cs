using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// 플레이어 스탯 상태창 UI를 제어하는 클래스.
/// 체력, 마나, 스태미나, 공격력, 방어력, 스탯포인트 등을 실시간으로 업데이트.
/// </summary>
public class StatusUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject contentParent;

    [Header("Stat Text Fields")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI manaRegenText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenceText;
    [SerializeField] private TextMeshProUGUI statPointsText;
    [SerializeField] private TextMeshProUGUI levelText;

    private void OnEnable()
    {
        GameEventsManager.Instance.InputEvents.OnStatusTogglePressed += StatusTogglePressed;
        GameEventsManager.Instance.PlayerEvents.OnHealthChanged += UpdateHealthText;
        GameEventsManager.Instance.PlayerEvents.OnManaChanged += UpdateManaText;
        GameEventsManager.Instance.PlayerEvents.OnManaRegenChanged += UpdateManaRegenText;
        GameEventsManager.Instance.PlayerEvents.OnStrengthChanged += UpdateAttackText;
        GameEventsManager.Instance.PlayerEvents.OnDefenseChanged += UpdateDefenceText;
        GameEventsManager.Instance.PlayerEvents.OnStatPointChanged += UpdateStatPoints;
        GameEventsManager.Instance.PlayerEvents.OnPlayerLevelChanged += UpdatePlayerLevel;
        GameEventsManager.Instance.PlayerEvents.OnPlayerLevelUp += UpdatePlayerLevel;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.InputEvents.OnStatusTogglePressed -= StatusTogglePressed;
        GameEventsManager.Instance.PlayerEvents.OnHealthChanged -= UpdateHealthText;
        GameEventsManager.Instance.PlayerEvents.OnManaChanged -= UpdateManaText;
        GameEventsManager.Instance.PlayerEvents.OnManaRegenChanged -= UpdateManaRegenText;
        GameEventsManager.Instance.PlayerEvents.OnStrengthChanged -= UpdateAttackText;
        GameEventsManager.Instance.PlayerEvents.OnDefenseChanged -= UpdateDefenceText;
        GameEventsManager.Instance.PlayerEvents.OnStatPointChanged -= UpdateStatPoints;
        GameEventsManager.Instance.PlayerEvents.OnPlayerLevelChanged -= UpdatePlayerLevel;
        GameEventsManager.Instance.PlayerEvents.OnPlayerLevelUp -= UpdatePlayerLevel;
    }

    private void StatusTogglePressed()
    {
        if (contentParent.activeInHierarchy)
        {
            HideUI();
        }
        else
        {
            ShowUI();
        }
    }

    private void ShowUI()
    {
        contentParent.SetActive(true);
    }

    private void HideUI()
    {
        contentParent.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void UpdateHealthText(float health)
    {
        healthText.text = "체력 : " + health.ToString();
    }

    public void UpdateManaText(float mana)
    {
        manaText.text = "마나 : " + mana.ToString();
    }

    public void UpdateManaRegenText(float manaRegen)
    {
        manaRegenText.text = "마나 회복률 : " + manaRegen.ToString();
    }

    public void UpdateAttackText(float attack)
    {
        attackText.text = "공격력 : " + attack.ToString();
    }

    public void UpdateDefenceText(float defence)
    {
        defenceText.text = "방어력 : " + defence.ToString();
    }

    private void UpdateStatPoints(int statPoint)
    {
        statPointsText.text = statPoint.ToString();
    }

    private void UpdatePlayerLevel(int level)
    {
        levelText.text = level.ToString();
    }
}
