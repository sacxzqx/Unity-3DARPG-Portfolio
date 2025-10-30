using UnityEngine;

/// <summary>
/// ����(State) ������ ���� �⺻ �������̽�
/// �� ���º��� ���������� �ʿ��� �޼������ ����
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
