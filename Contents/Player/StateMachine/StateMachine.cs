using System.Diagnostics;
using UnityEngine;

/// <summary>
/// 상태를 관리하는 기본 추상 클래스
/// 상태 전환과 현재 상태의 업데이트 호출을 담당
/// </summary>
public abstract class StateMachine
{
    protected IState currentState;
    protected IState formerState;

    public IState CurrentState { get { return currentState; } }
    public IState FormerState { get { return formerState; } }

    /// <summary>
    /// 현재 상태를 새로운 상태로 변경
    /// 이전 상태는 formerState로 기록
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
