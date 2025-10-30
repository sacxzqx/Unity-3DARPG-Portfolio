using UnityEngine;

/// <summary>
/// 플레이어의 상태별 이동 데이터를 저장하는 ScriptableObject
/// Grounded(지상)와 Airborne(공중) 상태에 대한 정보를 포함
/// </summary>
[CreateAssetMenu(fileName = "Player", menuName = "Custom/Characters/Player")]
public class PlayerSO : ScriptableObject
{
    [field: SerializeField] public PlayerGroundedData GroundedData { get; private set; }
    [field: SerializeField] public PlayerAirborneData AirborneData { get; private set; }

    [field: SerializeField] public PlayerActionData ActionData { get; private set; }
}
