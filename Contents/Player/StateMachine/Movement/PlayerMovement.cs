/// <summary>
/// �÷��̾� �̵� ���� ���¸� �����ϴ� ���� ���¸ӽ�
/// �پ��� �̵� ���� �� ��ȯ�� ���
/// </summary>
public class PlayerMovementStateMachine : StateMachine
{
    public PlayerContext player { get; }
    public PlayerStateReusableData ReusableData { get; }

    public PlayerIdlingState IdlingState { get; }
    public PlayerWalkingState WalkingState { get; }
    public PlayerRunningState RunningState { get; }
    public PlayerSprintState SprintingState { get; }

    public PlayerLightStoppingState LightStoppingState { get; }
    public PlayerMediumStoppingState MediumStoppingState { get; }
    public PlayerHardStoppingState HardStoppingState { get; }

    public PlayerJumpingState JumpingState { get; }
    public PlayerFallingState FallingState { get; }

    public PlayerLightLandingState LightLandingState { get; }
    public PlayerHardLandingState HardLandingState { get; }

    public PlayerDashingState DashingState { get; }
    public PlayerRollingState RollingState { get; }

    public PlayerMovementStateMachine(PlayerContext player)
    {
        this.player = player;
        ReusableData = new PlayerStateReusableData();

        IdlingState = new PlayerIdlingState(this);
        DashingState = new PlayerDashingState(this);
    
        WalkingState = new PlayerWalkingState(this);
        RunningState = new PlayerRunningState(this);
        SprintingState = new PlayerSprintState(this);

        LightStoppingState = new PlayerLightStoppingState(this);
        MediumStoppingState = new PlayerMediumStoppingState(this);
        HardStoppingState = new PlayerHardStoppingState(this);

        JumpingState = new PlayerJumpingState(this);
        FallingState = new PlayerFallingState(this);

        LightLandingState = new PlayerLightLandingState(this);
        HardLandingState = new PlayerHardLandingState(this);
        RollingState = new PlayerRollingState(this);
    }
}
