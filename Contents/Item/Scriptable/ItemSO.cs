using System;
using UnityEngine;

public enum EquipType { Weapon, Armor }
public enum PotionType { Health, Mana, ManaRegen }
public enum ItemType { Consumable, Equipment }

/// <summary>
/// 하나의 스탯 보정 정보를 나타내는 구조체  
/// 예: 공격력 +10, 체력 +20 등
/// </summary>
[Serializable]
public struct StatModifier
{
    public StatType StatType; // 체력, 마나 등
    public float Value; // 증가/감소할 값
}

/// <summary>
/// 모든 아이템의 공통 속성을 가진 최상위 ScriptableObject 클래스  
/// 소비형, 장비형 등의 아이템 유형을 공통 관리한다
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

    public virtual string Description => "설명이 없는 아이템";
}