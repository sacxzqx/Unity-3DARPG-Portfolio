using UnityEngine;
using UnityEngine.Rendering;

using BeautifyEffect = Beautify.Universal.Beautify;

/// <summary>
/// UI�� ����/���� ���¿� ���� ������ TimeScale�� Ŀ�� ���¸� �����ϴ� Ŭ����
/// �޴��� �κ��丮 ���� UI�� ���� �� ������ �Ͻ������ϰ�
/// UI�� ������ ������� �ǵ����� ������ ��
/// </summary>
public class UIStateManager : MonoBehaviour
{
    [SerializeField] private UICursor.CursorActions altCursorAction;
    [SerializeField] private Volume globalVolume;

    private BeautifyEffect beautifyEffect;

    private void Awake()
    {
        if (globalVolume != null && globalVolume.profile.TryGet<BeautifyEffect>(out beautifyEffect))
        {
        }
        
        altCursorAction = new UICursor().Cursor;

        altCursorAction.cursor.started += _ => HandleAltPress();
        altCursorAction.cursor.canceled += _ => HandleAltRelease();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.InputEvents.OnUIStateChange += HandleUIState;

        altCursorAction.cursor.Enable();
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.InputEvents.OnUIStateChange -= HandleUIState;

        altCursorAction.cursor.Disable();
    }

    /// <summary>
    /// Alt Ű�� ������ �������� �� Ŀ���� ������ ����
    /// </summary>
    private void HandleAltPress()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HandleAltRelease()
    {
        if (Time.timeScale == 0f)
        {
            // UI�� ���������� Alt�� ���� Ŀ���� ��� ������ ��
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // ���� �÷��� ���̸� Ŀ���� ��װ� ����
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    /// <summary>
    /// UI ����/���� �̺�Ʈ�� ���� �ð� ������ ���콺 Ŀ�� ���¸� ����
    /// </summary>
    /// <param name="uIType">�����ų� ���� UI�� Ÿ��</param>
    /// <param name="isOpen">UI�� ���ȴ��� ����</param>
    private void HandleUIState(UIType uIType, bool isOpen)
    {
        if (isOpen)
        {
            beautifyEffect.blurIntensity.value = 0.7f;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            beautifyEffect.blurIntensity.value = 0f;
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
