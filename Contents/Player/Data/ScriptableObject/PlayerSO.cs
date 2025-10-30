using UnityEngine;

/// <summary>
/// �÷��̾��� ���º� �̵� �����͸� �����ϴ� ScriptableObject
/// Grounded(����)�� Airborne(����) ���¿� ���� ������ ����
/// </summary>
[CreateAssetMenu(fileName = "Player", menuName = "Custom/Characters/Player")]
public class PlayerSO : ScriptableObject
{
    [field: SerializeField] public PlayerGroundedData GroundedData { get; private set; }
    [field: SerializeField] public PlayerAirborneData AirborneData { get; private set; }

    [field: SerializeField] public PlayerActionData ActionData { get; private set; }
}
