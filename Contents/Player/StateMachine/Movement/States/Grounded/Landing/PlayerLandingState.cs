using UnityEngine.InputSystem;

/// <summary>
/// 플레이어의 착지(Landing) 상태를 관리하는 클래스
/// 착지 애니메이션을 재생하며, 다른 입력 반응은 처리하지 않음
/// </summary>
public class PlayerLandingState : PlayerGroundedState
{
    public PlayerLandingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        StartAnimation(stateMachine.player.AnimationsData.LandingParameterHash);
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAnimation(stateMachine.player.AnimationsData.LandingParameterHash);
    }

    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
        // 착지 상태에서는 별도의 이동 취소 처리 없이 무시
    }
}
