using UnityEngine;

/// <summary>
/// 상태(State) 패턴을 위한 기본 인터페이스
/// 각 상태별로 공통적으로 필요한 메서드들을 정의
/// </summary>
public interface IState
{
    public void EnterState();
    public void ExitState();
    public void HandleInput();
    public void Update();
    public void PhysicsUpdate();
    public void OnAnimationEnterEvent();
    public void OnAnimationExitEvent();
    public void OnAnimationTransitionEvent();
    public void OnTriggerEnter(Collider collider);
    public void OnTriggerExit(Collider collider);
}
