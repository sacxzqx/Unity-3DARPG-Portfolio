using UnityEngine;

/// <summary>
/// 킬무브 연출에 필요한 이동, 회전, 애니메이션 트리거 정보를 담는 ScriptableObject
/// 적마다 다양한 킬무브 연출을 설정할 수 있도록 분리된 데이터 자산
/// </summary>
[CreateAssetMenu(fileName = "NewKillMoveData", menuName = "KillMove/KillMoveData")]
public class KillMoveData : ScriptableObject
{
    [Header("킬무브 이동 설정")]
    [Tooltip("플레이어와 적 사이의 거리")]
    public float Distance = 1.5f;

    [Tooltip("킬무브 시작 위치로 이동하는 시간")]
    public float Duration = 0.3f;

    [Header("회전 설정")]
    [Tooltip("적이 플레이어를 바라보도록 180도 회전할지 여부")]
    public bool ReverseRotation = true;

    [Header("애니메이션 정보")]
    [Tooltip("플레이어 측 애니메이션 트리거 이름")]
    public string PlayerAnimationTrigger;

    [Tooltip("적 측 애니메이션 트리거 이름")]
    public string EnemyAnimationTrigger;
}
