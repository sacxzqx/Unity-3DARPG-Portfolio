using System.Diagnostics;
using UnityEngine;

/// <summary>
/// ���¸� �����ϴ� �⺻ �߻� Ŭ����
/// ���� ��ȯ�� ���� ������ ������Ʈ ȣ���� ���
/// </summary>
public abstract class StateMachine
{
    protected IState currentState;
    protected IState formerState;

    public IState CurrentState { get { return currentState; } }
    public IState FormerState { get { return formerState; } }

    /// <summary>
    /// ���� ���¸� ���ο� ���·� ����
    /// ���� ���´� formerState�� ���
    /// </summary>
    public void SetState(IState newState)
    {
        formerState = currentState;

        currentState?.ExitState();

        currentState = newState;

        currentState.EnterState();
    }
    
    public void Update()
    {
        currentState?.Update();
    }

    public void PhysicsUpdate()
    {
        currentState?.PhysicsUpdate();
    }

    public void HandleInput()
    {
        currentState?.HandleInput();
    }

    public void OnAnimationEnterEvent()
    {
        currentState?.OnAnimationEnterEvent();
    }

    public void OnAnimationExitEvent()
    {
        currentState?.OnAnimationExitEvent();
    }

    public void OnAnimationTransitionEvent()
    {
        currentState?.OnAnimationTransitionEvent();
    }

    public void OnTriggerEnter(Collider collider)
    {
        currentState?.OnTriggerEnter(collider);
    }

    public void OnTriggerExit(Collider collider)
    {
        currentState?.OnTriggerExit(collider);
    }
}
