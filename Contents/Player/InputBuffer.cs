using System.Collections;
using UnityEngine;

/// <summary>
/// �÷��̾� �Է��� ���� �ð� ���� ���۸��Ͽ� �����ϴ� Ŭ����
/// �Է��� ������ ���� �ð� �� �ڵ����� �ʱ�ȭ
/// </summary>
public class InputBuffer : MonoBehaviour
{
    private PlayerInput playerInput;
    private string lastInput;

    [SerializeField] private float bufferTime = 0.7f;  // �Է��� ������ �ð�

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
    /// �Է� ȣ�� �� ���ۿ� �����ϰ� ���� �ð� �� �ʱ�ȭ ó��
    /// </summary>
    /// <param name="input">�Էµ� Ű</param>
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
    /// ������ �ð� �� �Է� �ʱ�ȭ
    /// </summary>
    /// <param name="time">��� �ð�</param>
    private IEnumerator ResetInputAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        lastInput = null;
        inputTimerCoroutine = null;
    }

    /// <summary>
    /// ���� ���ۿ� ����� �Է��� �ܺο��� ��ȸ�Ͽ�, �ൿ ������ Ȱ���� �� ���
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