using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� �⺻ �ɷ�ġ �� ��� ���� ���� �����ϴ� ������ ��ü
/// Enemy �ν��Ͻ����� �����Ǿ� ���� �� ���� ������ ������
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
