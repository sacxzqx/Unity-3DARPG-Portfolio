using System;
using UnityEngine;

/// <summary>
/// 플레이어가 정지할 때의 감속 강도를 유형별로 정의하는 데이터 클래스
/// </summary>
[Serializable]
public class PlayerStopData 
{
    [field: SerializeField][field: Range(0f, 15f)] public float LightDecelerationForce { get; private set; } = 5f;
    [field: SerializeField][field: Range(0f, 15f)] public float MediumDecelerationForce { get; private set; } = 6.5f;
    [field: SerializeField][field: Range(0f, 15f)] public float HardDecelerationForce { get; private set; } = 5f;

}
