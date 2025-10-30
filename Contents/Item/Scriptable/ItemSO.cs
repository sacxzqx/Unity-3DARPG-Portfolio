using System;
using UnityEngine;

public enum EquipType { Weapon, Armor }
public enum PotionType { Health, Mana, ManaRegen }
public enum ItemType { Consumable, Equipment }

/// <summary>
/// �ϳ��� ���� ���� ������ ��Ÿ���� ����ü  
/// ��: ���ݷ� +10, ü�� +20 ��
/// </summary>
[Serializable]
public struct StatModifier
{
    public StatType StatType; // ü��, ���� ��
    public float Value; // ����/������ ��
}

/// <summary>
/// ��� �������� ���� �Ӽ��� ���� �ֻ��� ScriptableObject Ŭ����  
/// �Һ���, ����� ���� ������ ������ ���� �����Ѵ�
/// </summary>
[CreateAssetMenu(menuName = "Item")]
public class ItemSO : ScriptableObject
{
    public string ItemName;
    public int ItemId;
    public Sprite Sprite;
    public int PurchasePrice;
    public int SellPrice;
    public bool Stackable;
    public ItemType ItemType;

    public virtual string Description => "������ ���� ������";
}