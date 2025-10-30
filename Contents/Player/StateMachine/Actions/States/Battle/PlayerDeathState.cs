using UnityEngine;

public class PlayerDeathState : PlayerActionState
{
    public PlayerDeathState(PlayerActionStateMachine playerActionStateMachine) : base(playerActionStateMachine)
    {
    }

    public override void EnterState()
    {
        stateMachine.player.Input.DisablePlayerActions();
        StartAnimation(stateMachine.player.AnimationsData.PlayerDeath);
    }

    protected override void OnHitEnter(Collider collider)
    {
        // �ƹ��͵� �������� ����
    }
}
