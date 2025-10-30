using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어 이동 관련 공통 로직을 담당하는 베이스 이동 상태 클래스
/// 입력 처리, 물리 이동, 회전 처리, 감속 처리 등 기본 이동 기능을 정의
/// </summary>
public class PlayerMovementState : IState
{
    protected PlayerMovementStateMachine stateMachine;

    protected PlayerGroundedData movementData;
    protected PlayerAirborneData airborneData;

    private float currentMoveX = 0f;
    private float currentMoveY = 0f;
    private float moveSmoothTime = 0.1f; // 이동 보간 속도

    /// <summary>
    /// 상태 생성자
    /// 이동 및 공중 데이터를 초기화하고 회전 기본값 세팅
    /// </summary>
    public PlayerMovementState(PlayerMovementStateMachine playerMovementStateMachine)
    {
        stateMachine = playerMovementStateMachine;

        movementData = stateMachine.player.Data.GroundedData;
        airborneData = stateMachine.player.Data.AirborneData;

        InitializeData();
    }

    private void InitializeData()
    {
        SetBaseRotationData();
    }

    public virtual void EnterState()
    {
        AddInputActionsCallbacks();

        stateMachine.player.LockOn.OnLockOnDeactivated += HandleLockOnDeactivated;
    }

    public virtual void ExitState()
    {
        RemoveinputActionsCallback();

        stateMachine.player.LockOn.OnLockOnDeactivated -= HandleLockOnDeactivated;
    }

    public void HandleInput()
    {
        ReadMovementInput();
    }

    public virtual void Update()
    {
    }

    public virtual void PhysicsUpdate()
    {
        Move();
    }

    public virtual void OnAnimationEnterEvent()
    {
        
    }

    public virtual void OnAnimationExitEvent()
    {
        
    }

    public virtual void OnAnimationTransitionEvent()
    {
       
    }

    public virtual void OnTriggerEnter(Collider collider)
    {
        if (stateMachine.player.LayerData.IsGroundLayer(collider.gameObject.layer))
        {
            OnContactWithGround(collider);

            return;
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (stateMachine.player.LayerData.IsGroundLayer(collider.gameObject.layer))
        {
            OnContactWithGroundExited(collider);

            return;
        }
    }

    void ReadMovementInput()
    {
        stateMachine.ReusableData.MovementInput = stateMachine.player.Input.MovementActions.Movement.ReadValue<Vector2>();
    }

    private void Move()
    {
        if(stateMachine.ReusableData.MovementInput == Vector2.zero || stateMachine.ReusableData.MovementSpeedModifier == 0f || stateMachine.ReusableData.CanMove == false)
        {
            return;
        }

        if (stateMachine.player.LockOn.IsLockOn)
        {
            // 락온 상태일 때 부드럽게 방향 입력을 보간
            currentMoveX = Mathf.Lerp(currentMoveX, stateMachine.ReusableData.MovementInput.x, Time.deltaTime / moveSmoothTime);
            currentMoveY = Mathf.Lerp(currentMoveY, stateMachine.ReusableData.MovementInput.y, Time.deltaTime / moveSmoothTime);

            stateMachine.player.Anim.SetFloat("MoveX", currentMoveY);
            stateMachine.player.Anim.SetFloat("MoveY", currentMoveX);
        }

        Vector3 movementDirection = GetMovementInputDirection();

        float targetRotationYAngle = Rotate(movementDirection);

        Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle);

        float movementSpeed = GetMovementSpeed();

        Vector3 currentPlayerHorizontalVelocity = GetPlayerHorizontalVelocity();

        // 현재 속도를 무시하고 즉시 목표 속도로 변경하여 반응성을 높이기 위해 VelocityChange 모드를 사용
        stateMachine.player.Rigidbody.AddForce(targetRotationDirection * movementSpeed - currentPlayerHorizontalVelocity, ForceMode.VelocityChange);
    }

    /// <summary>
    /// 입력된 방향 벡터를 기반으로 목표 회전을 갱신하고, 실제 회전을 시작하도록 지시
    /// 이 메서드는 회전 과정을 관리하는 역할
    /// </summary>
    /// <param name="direction">목표로 할 이동 방향 벡터</param>
    /// <returns>계산된 최종 목표 회전 각도 (Y축)</returns>
    private float Rotate(Vector3 direction)
    {
        float directionAngle = UpdateTargetRotation(direction);

        RotateTowardsTargetRoation();

        return directionAngle;
    }

    /// <summary>
    /// 방향 벡터를 각도로 변환
    /// </summary>
    /// <param name="direction">이동 방향 벡터</param>
    /// <returns></returns>
    private float GetDirectionAngle(Vector3 direction)
    {
        float directionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        if (directionAngle < 0f)
        {
            directionAngle += 360f;
        }

        return directionAngle;
    }

    /// <summary>
    /// 주어진 방향 각도에 현재 카메라의 Y축 회전을 더함
    /// 결과값은 0~360도 사이로 정규화
    /// </summary>
    /// <param name="angle">카메라 회전을 더할 기준 방향 각도 (예: 플레이어 입력 각도)</param>
    /// <returns>카메라 회전이 적용되고 정규화된 최종 방향 각도</returns>
    private float AddCameraRotationToAngle(float angle)
    {
        angle += stateMachine.player.MainCameraTransform.eulerAngles.y;

        if (angle > 360f)
        {
            angle -= 360f;
        }

        return angle;
    }

    /// <summary>
    /// 목표 회전 각도를 갱신하고, 회전 보간에 사용될 시간을 초기화
    /// </summary>
    /// <param name="targetAngle">목표 회전 각도</param>
    private void UpdateTargetRotationData(float targetAngle)
    {
        stateMachine.ReusableData.CurrentTargetRotation.y = targetAngle;

        stateMachine.ReusableData.DampedTargetRotationPassedTime.y = 0f;
    }

    protected void StartAnimation(int animationHash)
    {
        stateMachine.player.Anim.SetBool(animationHash, true);
    }

    protected void StopAnimation(int animationHash)
    {
        stateMachine.player.Anim.SetBool(animationHash, false);
    }

    /// <summary>
    /// 기본 회전 데이터를 설정
    /// </summary>
    protected void SetBaseRotationData()
    {
        stateMachine.ReusableData.RotationData = movementData.BaseRotationData;

        stateMachine.ReusableData.TimeToReachTargetRotation = stateMachine.ReusableData.RotationData.TargetRotationReachTime;
    }

    /// <summary>
    /// 현재 입력을 기반으로 이동 벡터를 반환
    /// </summary>
    /// <returns>현재 입력에 대한 이동 벡터</returns>
    protected Vector3 GetMovementInputDirection()
    {
        return new Vector3(stateMachine.ReusableData.MovementInput.x, 0f, stateMachine.ReusableData.MovementInput.y);
    }

    /// <summary>
    /// 현재 이동 속도를 계산하여 반환
    /// </summary>
    /// <returns>현재 이동 속도</returns>
    protected float GetMovementSpeed()
    {
        return movementData.BaseSpeed * stateMachine.ReusableData.MovementSpeedModifier * stateMachine.ReusableData.MovementOnSlopesSpeedModifier;
    }

    /// <summary>
    /// 플레이어의 수평 속도를 반환
    /// </summary>
    /// <returns>플레이어의 수평 속도</returns>
    protected Vector3 GetPlayerHorizontalVelocity()
    {
        Vector3 playerHorizontalVelocity = stateMachine.player.Rigidbody.velocity;

        playerHorizontalVelocity.y = 0f;

        return playerHorizontalVelocity;
    }

    /// <summary>
    /// 플레이어의 수직 속도를 반환
    /// </summary>
    /// <returns>플레이어의 수직 속도</returns>
    protected Vector3 GetPlayerVerticalVelocity()
    {
        return new Vector3(0f, stateMachine.player.Rigidbody.velocity.y, 0f);
    }

    /// <summary>
    /// 목표 회전 방향으로 부드럽게 회전
    /// </summary>
    protected void RotateTowardsTargetRoation()
    {
        float currentYAngle = stateMachine.player.Rigidbody.rotation.eulerAngles.y;

        if(currentYAngle == stateMachine.ReusableData.CurrentTargetRotation.y)
        {
            return;
        }

        float smoothedYAngle = Mathf.SmoothDampAngle(currentYAngle, stateMachine.ReusableData.CurrentTargetRotation.y, ref stateMachine.ReusableData.DampedTargetRotationCurrentVelocity.y, stateMachine.ReusableData.TimeToReachTargetRotation.y - stateMachine.ReusableData.DampedTargetRotationPassedTime.y);

        stateMachine.ReusableData.DampedTargetRotationPassedTime.y += Time.deltaTime;

        Quaternion targetRotation = Quaternion.Euler(0f, smoothedYAngle, 0f);


        // 락온 상태에서는 방향을 바꾸지 않음
        if (stateMachine.player.LockOn.IsLockOn == false)
        {
            stateMachine.player.Rigidbody.MoveRotation(targetRotation);
        }
    }

    private void HandleLockOnDeactivated()
    {
        // 목표 회전값을 현재 캐릭터가 실제로 바라보는 방향으로 강제 동기화
        UpdateTargetRotationData(stateMachine.player.Rigidbody.rotation.eulerAngles.y);

        // 추가로, 회전 속도까지 0으로 리셋하여 빠르게 회전해버리는 것을 막음
        stateMachine.ReusableData.DampedTargetRotationCurrentVelocity.y = 0f;
    }

    /// <summary>
    /// 입력된 이동 방향을 기준으로 목표 회전 각도를 계산하고 업데이트
    /// 카메라 방향을 추가로 고려할 수 있음
    /// </summary>
    /// <param name="direction">플레이어 이동 입력 방향</param>
    /// <param name="shouldConsiderCameraRotation">카메라 방향을 함께 고려할지 여부 (기본값: true)</param>
    /// <returns>계산된 최종 목표 회전 각도</returns>
    protected float UpdateTargetRotation(Vector3 direction, bool shouldConsiderCameraRotation = true)
    {
        float directionAngle = GetDirectionAngle(direction);

        if(shouldConsiderCameraRotation)
        {
            directionAngle = AddCameraRotationToAngle(directionAngle);
        }

        if (directionAngle != stateMachine.ReusableData.CurrentTargetRotation.y)
        {
            UpdateTargetRotationData(directionAngle);
        }

        return directionAngle;
    }

    /// <summary>
    /// 목표 회전 각도에 맞춰 방향 벡터를 계산
    /// </summary>
    /// <param name="targetAngle">목표 회전 각도</param>
    /// <returns>회전 각도에 따른 방향 벡터</returns>
    protected Vector3 GetTargetRotationDirection(float targetAngle)
    {
        return Quaternion.Euler(0f,targetAngle, 0f) * Vector3.forward;
    }

    protected void ResetVelocity()
    {
        stateMachine.player.Rigidbody.velocity = Vector3.zero;
    }

    /// <summary>
    /// 수직 속도를 제거하고, 수평 속도만 유지
    /// </summary>
    protected void ResetVerticalVelocity()
    {
        Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();

        stateMachine.player.Rigidbody.velocity =playerHorizontalVelocity;
    }

    protected virtual void AddInputActionsCallbacks()
    {
        stateMachine.player.Input.MovementActions.WalkToggle.started += OnWalkToggleStarted;
    }

    protected virtual void RemoveinputActionsCallback()
    {
        stateMachine.player.Input.MovementActions.WalkToggle.started -= OnWalkToggleStarted;
    }

    protected void DecelerateHorizontally()
    {
        Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();

        stateMachine.player.Rigidbody.AddForce(-playerHorizontalVelocity * stateMachine.ReusableData.MovementDecelerationForce, ForceMode.Acceleration);
    }

    protected void DecelerateVertically()
    {
        Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();

        stateMachine.player.Rigidbody.AddForce(-playerVerticalVelocity * stateMachine.ReusableData.MovementDecelerationForce, ForceMode.Acceleration);
    }

    /// <summary>
    /// 수평 방향으로 이동 중인지 검사
    /// </summary>
    /// <param name="minimumMagnitude">수평 방향 이동을 판단하기 위한 최소 속도 임계값</param>
    /// <returns>수평 방향으로 이동 중이면 true, 그렇지 않으면 false</returns>
    protected bool IsMovingHorizontally(float minimumMagnitude = 0.1f)
    {
        Vector3 playerHorizontaVelocity = GetPlayerHorizontalVelocity();

        Vector2 playerHorizontalMovement = new Vector2(playerHorizontaVelocity.x, playerHorizontaVelocity.z);

        return playerHorizontalMovement.magnitude > minimumMagnitude;
    }

    /// <summary>
    /// 현재 위쪽으로 이동중인지 여부를 반환
    /// </summary>
    /// <param name="minimumVelocity">위 방향 이동을 판단하기 위한 최소 속도 임계값</param>
    /// <returns>위로 이동 중이면 true, 그렇지 않으면 false</returns>
    protected bool IsMovingUp(float minimumVelocity = 0.1f)
    {
        return GetPlayerVerticalVelocity().y > minimumVelocity;
    }

    /// <summary>
    /// 현재 아래쪽으로 이동중인지 여부를 반환
    /// </summary>
    /// <param name="minimumVelocity">아래 방향 이동을 판단하기 위한 최소 속도 임계값</param>
    /// <returns>아래로 이동 중이면 true, 그렇지 않으면 false</returns>
    protected bool IsMovingDown(float minimumVelocity = 0.1f)
    {
        return GetPlayerVerticalVelocity().y < -minimumVelocity;
    }

    protected virtual void OnContactWithGround(Collider collider)
    {
    }

    protected virtual void OnContactWithGroundExited(Collider collider)
    {
    }

    /// <summary>
    /// 걷기 토글 키를 눌렀을 때 호출
    /// </summary>
    /// <param name="context">New Input System에 의해 전달되는 입력 액션의 컨텍스트 정보</param>
    protected virtual void OnWalkToggleStarted(InputAction.CallbackContext context)
    {
        stateMachine.ReusableData.ShouldWalk = !stateMachine.ReusableData.ShouldWalk;
    }
}
