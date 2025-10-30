using UnityEngine;

/// <summary>
/// 회복 물약 아이템을 정의하는 ScriptableObject 클래스
/// 체력, 마나, 기력 중 하나를 회복할 수 있으며, 아이템 사용 시 해당 능력치를 회복
/// </summary>
[CreateAssetMenu(fileName = "New Potion", menuName = "Item/Potion")]
public class PotionSO : ConsumableSO
{
    public int RecoveryAmount;
    public PotionType PotionType;

    /// <summary>
    /// 물약의 설명을 반환
    /// 예: "체력 50 회복"
    /// </summary>
    public override string Description
    {
        get
        {
            string effect = PotionType switch
            {
                PotionType.Health => "체력",
                PotionType.Mana => "마나",
                _ => "알 수 없는 효과"
            };
            return $"{effect} {RecoveryAmount} 회복";
        }
    }

    public override void Use(PlayerStat playerStat)
    {
        playerStat.RestoreStat(PotionType, RecoveryAmount);
        AudioManager.Instance.PlaySFX("Consume");
    }
}