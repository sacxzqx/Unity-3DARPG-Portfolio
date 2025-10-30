using System;
using UnityEngine;

/// <summary>
/// 경사로 및 단차 오르기 관련 설정값을 저장하는 데이터 클래스  
/// 플레이어가 경사면을 부드럽게 올라가거나 단을 타넘을 때 사용하는 물리 파라미터를 정의
/// </summary>
[Serializable]
public class SlopeData
{
    /// <summary>
    /// 현재 콜라이더 높이에서 계단이나 턱으로 인식할 수 있는 최대 비율  
    /// 예: 0.25 = 키의 25% 높이까지는 단차로 인식하고 오를 수 있음
    /// </summary>
    [field: SerializeField][field: Range(0f, 1f)] public float StepHeightPecentage { get; private set; } = 0.25f;

    /// <summary>
    /// 슬로프 앞방향으로 보내는 부유 감지용 Ray의 길이  
    /// 단차 감지나 공중에 떠있는지 여부를 판단하는 데 사용됨
    /// </summary>
    [field: SerializeField][field: Range(0f, 5f)] public float FloatRayDistacne { get; private set; } = 2f;

    /// <summary>
    /// 단차를 넘어갈 때 수직 보정에 사용할 힘의 크기  
    /// 단을 올라설 때 적용되는 임펄스 (ex: 위로 튕겨올리는 느낌)
    /// </summary>
    [field: SerializeField][field: Range(0f, 50f)] public float StepReachForce { get; private set; } = 25f;
}
