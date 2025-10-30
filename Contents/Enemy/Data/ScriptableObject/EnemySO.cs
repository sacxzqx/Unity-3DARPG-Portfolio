using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적의 기본 능력치 및 드롭 정보 등을 정의하는 데이터 객체
/// Enemy 인스턴스에서 참조되어 전투 및 보상 정보를 제어함
/// </summary>
[CreateAssetMenu(menuName = "Enemy/EnemyData")]
public class EnemySO : ScriptableObject
{
    public string Name;
    public int AttackPower;
    public int MaxHealth;
    public int PostureResistance;
    public int Experience;
    public List<ItemSO> DropItems;
}
