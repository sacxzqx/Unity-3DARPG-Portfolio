using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �������ͽ� â���� ���� ������ �ϴ� Ŭ����
/// </summary>
public class StatController : MonoBehaviour
{
    [SerializeField] private GameObject increaseButtons;

    [SerializeField] private Button hpIncreasButton;
    [SerializeField] private Button manaIncreaseButton;
    [SerializeField] private Button manaRegenIncreaseButton;
    [SerializeField] private Button strengthIncreaseButton;
    [SerializeField] private Button defenseIncreaseButton;

    private void Awake()
    {
        hpIncreasButton.onClick.AddListener(() => IncreaseStat(StatType.Health));
        manaIncreaseButton.onClick.AddListener(() => IncreaseStat(StatType.Mana));
        manaRegenIncreaseButton.onClick.AddListener(() => IncreaseStat(StatType.ManaRegen));
        strengthIncreaseButton.onClick.AddListener(() => IncreaseStat(StatType.Strength));
        defenseIncreaseButton.onClick.AddListener(() => IncreaseStat(StatType.Defense));
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.PlayerEvents.OnStatPointChanged += ButtonActivator;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.PlayerEvents.OnStatPointChanged -= ButtonActivator;
    }

    public void IncreaseStat(StatType statType)
    {
        switch (statType)
        {
            case StatType.Health:
                GameEventsManager.Instance.PlayerEvents.IncreasPlayerStat(StatType.Health);
                break;
            case StatType.Mana:
                GameEventsManager.Instance.PlayerEvents.IncreasPlayerStat(StatType.Mana);
                break;
            case StatType.ManaRegen:
                GameEventsManager.Instance.PlayerEvents.IncreasPlayerStat(StatType.ManaRegen);
                break;
            case StatType.Strength:
                GameEventsManager.Instance.PlayerEvents.IncreasPlayerStat(StatType.Strength);
                break;
            case StatType.Defense:
                GameEventsManager.Instance.PlayerEvents.IncreasPlayerStat(StatType.Defense);
                break;
        }

        GameEventsManager.Instance.PlayerEvents.UseStatPoint(1);
    }   

    private void ButtonActivator(int currentStatPoint)
    {
        increaseButtons.gameObject.SetActive(currentStatPoint > 0);
    }
}
