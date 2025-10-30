using UnityEngine;

/// <summary>
/// ȸ�� ���� �������� �����ϴ� ScriptableObject Ŭ����
/// ü��, ����, ��� �� �ϳ��� ȸ���� �� ������, ������ ��� �� �ش� �ɷ�ġ�� ȸ��
/// </summary>
[CreateAssetMenu(fileName = "New Potion", menuName = "Item/Potion")]
public class PotionSO : ConsumableSO
{
    public int RecoveryAmount;
    public PotionType PotionType;

    /// <summary>
    /// ������ ������ ��ȯ
    /// ��: "ü�� 50 ȸ��"
    /// </summary>
    public override string Description
    {
        get
        {
            string effect = PotionType switch
            {
                PotionType.Health => "ü��",
                PotionType.Mana => "����",
                _ => "�� �� ���� ȿ��"
            };
            return $"{effect} {RecoveryAmount} ȸ��";
        }
    }

    public override void Use(PlayerStat playerStat)
    {
        playerStat.RestoreStat(PotionType, RecoveryAmount);
        AudioManager.Instance.PlaySFX("Consume");
    }
}