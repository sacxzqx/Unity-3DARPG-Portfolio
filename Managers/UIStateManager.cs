using UnityEngine;
using UnityEngine.Rendering;

using BeautifyEffect = Beautify.Universal.Beautify;

/// <summary>
/// UI의 열림/닫힘 상태에 따라 게임의 TimeScale과 커서 상태를 제어하는 클래스
/// 메뉴나 인벤토리 등의 UI가 열릴 때 게임을 일시정지하고
/// UI가 닫히면 원래대로 되돌리는 역할을 함
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
    /// Alt 키를 누르기 시작했을 때 커서를 강제로 노출
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
            // UI가 열려있으면 Alt를 떼도 커서는 계속 보여야 함
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // 게임 플레이 중이면 커서를 잠그고 숨김
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    /// <summary>
    /// UI 열림/닫힘 이벤트에 따라 시간 정지와 마우스 커서 상태를 조절
    /// </summary>
    /// <param name="uIType">열리거나 닫힌 UI의 타입</param>
    /// <param name="isOpen">UI가 열렸는지 여부</param>
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
