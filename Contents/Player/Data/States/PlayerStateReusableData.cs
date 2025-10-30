using UnityEngine;

/// <summary>
/// 플레이어 상태 간에 공유되는 일시적인 런타임 데이터를 저장하는 클래스
/// 이동 입력, 회전 타겟, 가속도, 감속 관련 변수들을 포함
/// </summary>
public class PlayerStateReusableData
{
    public bool CanMove { get; set; } = true;
    public bool IsWeaponEquipped { get; set; } = false;

    public Vector2 MovementInput { get; set; }
    public Vector3 Direction {  get; set; }
    public float MovementSpeedModifier { get; set; } = 1f;
    public float MovementOnSlopesSpeedModifier { get; set; } = 1f;
    public float MovementDecelerationForce { get; set; } = 1f;

    public bool ShouldWalk { get; set; }
    public bool ShouldSprint { get; set; }

    private Vector3 currentTargetRotation;
    private Vector3 timeToReachTargetRotation;
    private Vector3 dampedTargetRotationCurrentVelocity;
    private Vector3 dampedTargetRotationPassedTime;

    public ref Vector3 CurrentTargetRotation
    {
        get
        {
            return ref currentTargetRotation;
        }
    }

    public ref Vector3 TimeToReachTargetRotation
    {
        get
        {
            return ref timeToReachTargetRotation;
        }
    }
    public ref Vector3 DampedTargetRotationCurrentVelocity
    {
        get
        {
            return ref dampedTargetRotationCurrentVelocity;
        }
    }
    public ref Vector3 DampedTargetRotationPassedTime
    {
        get
        {
            return ref dampedTargetRotationPassedTime;
        }
    }

    public Vector3 CurrentJumpForce { get; set; }

    public PlayerRotationData RotationData { get; set; }
}
