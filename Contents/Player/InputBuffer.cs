using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어 입력을 일정 시간 동안 버퍼링하여 저장하는 클래스
/// 입력이 들어오면 일정 시간 후 자동으로 초기화
/// </summary>
public class InputBuffer : MonoBehaviour
{
    private PlayerInput playerInput;
    private string lastInput;

    [SerializeField] private float bufferTime = 0.7f;  // 입력을 유지할 시간

    private Coroutine inputTimerCoroutine;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        playerInput.OnInputReceived += BufferInput;
    }

    private void OnDestroy()
    {
        playerInput.OnInputReceived -= BufferInput;
    }

    /// <summary>
    /// 입력 호출 시 버퍼에 저장하고 일정 시간 후 초기화 처리
    /// </summary>
    /// <param name="input">입력된 키</param>
    private void BufferInput(string input)
    {
        lastInput = input;

        if (inputTimerCoroutine != null)
        {
            StopCoroutine(inputTimerCoroutine);
        }

        inputTimerCoroutine = StartCoroutine(ResetInputAfterTime(bufferTime));
    }

    /// <summary>
    /// 지정된 시간 후 입력 초기화
    /// </summary>
    /// <param name="time">대기 시간</param>
    private IEnumerator ResetInputAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        lastInput = null;
        inputTimerCoroutine = null;
    }

    /// <summary>
    /// 현재 버퍼에 저장된 입력을 외부에서 조회하여, 행동 로직에 활용할 때 사용
    /// </summary>
    public string GetLastInput()
    {
        return lastInput;
    }

    public void ClearLastInput()
    {
        lastInput = null;
    }
}