using UnityEngine;

/// <summary>
/// 플레이어 애니메이션과 관련된 제어 로직을 처리하는 컴포넌트
/// 애니메이션 이벤트를 통해 상태 전이, 회전 제어, 입력 큐 처리 등을 수행
/// </summary>
public class PlayerAnimationController : MonoBehaviour
{
    private PlayerContext playerContext;

    // 애니메이션 도중 회전 허용 여부
    public bool IsRotationEnabled = false;

    private void Start()
    {
        playerContext = GetComponent<PlayerContext>();
    }

    private void FixedUpdate()
    {
        if (IsRotationEnabled)
        {
            RotatePlayer();
        }
    }

    /// <summary>
    /// 무기를 뽑은 상태로 전환 (WeaponDrawnState)
    /// 애니메이션 종료 시 호출되어 상태머신과 애니메이터 상태를 동기화
    /// </summary>
    public void EnableWeaponDrawnState()
    {
        if (playerContext.ActionStateMachine.CurrentState == playerContext.ActionStateMachine.ConversationState ||
            playerContext.ActionStateMachine.CurrentState == playerContext.ActionStateMachine.SheathedState) return;

        if (playerContext.ActionStateMachine.CurrentState != playerContext.ActionStateMachine.WeaponDrawnState)
        {
            playerContext.ActionStateMachine.SetState(playerContext.ActionStateMachine.WeaponDrawnState); // 애니메이션이 끝나면 WeaponDrawnState로 전환
            playerContext.MovementStateMachine.SetState(playerContext.MovementStateMachine.IdlingState);
            playerContext.Anim.SetBool(playerContext.AnimationsData.MovingParameterHash, false);
        }
    }

    /// <summary>
    /// 무기를 집어넣은 상태로 전환 (SheathedState)
    /// 애니메이션 종료 시 호출되어 상태머신과 애니메이터 상태를 동기화
    /// </summary>
    public void EnableWeaponSheathedState()
    {
        if (playerContext.ActionStateMachine.CurrentState != playerContext.ActionStateMachine.SheathedState)
        {
            playerContext.ActionStateMachine.SetState(playerContext.ActionStateMachine.SheathedState);    
        }
    }

    /// <summary>
    /// 입력 버퍼로부터 마지막 입력을 꺼내 해당 동작 애니메이션을 재생
    /// </summary>
    public void MoveNextAnimation()
    {
        string lastInputName = playerContext.InputBuffer.GetLastInput();

        switch(lastInputName)
        {
            case "Attack": 
                playerContext.Anim.SetTrigger("OnCloseAttackCombo");
                break;
            case "Parry":
                playerContext.Anim.ResetTrigger("OnCloseAttackCombo");
                playerContext.ActionStateMachine.SetState(playerContext.ActionStateMachine.DefenseState);
                playerContext.Anim.SetBool("IsParrying", true);
                break;
        }

        playerContext.InputBuffer.ClearLastInput();
    }

    public void EnableRotation()
    {
        IsRotationEnabled = true;
    }

    public void DisableRotation()
    {
        IsRotationEnabled = false;
    }

    /// <summary>
    /// 현재 이동 방향 기준으로 캐릭터를 부드럽게 회전시킴
    /// 애니메이션 도중 회전이 필요한 경우 사용
    /// </summary>
    public void RotatePlayer()
    {
        if (playerContext.ActionStateMachine.ReusableData.Direction.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(playerContext.ActionStateMachine.ReusableData.Direction.x, playerContext.ActionStateMachine.ReusableData.Direction.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            playerContext.Rigidbody.rotation = Quaternion.Slerp(playerContext.Rigidbody.rotation, targetRotation, Time.deltaTime * 5); // 5는 회전 속도 파라미터
        }
    }
}