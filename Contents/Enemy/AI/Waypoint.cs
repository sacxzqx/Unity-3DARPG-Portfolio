using UnityEngine;

/// <summary>
/// 순찰 경로에서 적이 도달하는 웨이포인트 정보
/// 도달 시 정지 시간을 포함할 수 있음
/// </summary>
public class Waypoint : MonoBehaviour
{
    [Tooltip("이 웨이포인트에서 대기할 시간 (초)")]
    public float WaitTime = 0f;
}
