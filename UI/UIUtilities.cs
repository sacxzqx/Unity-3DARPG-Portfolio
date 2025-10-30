using UnityEngine;

/// <summary>
/// UI 관련 공통 기능을 제공하는 유틸리티 클래스
/// </summary>
public static class UIUtilities
{
    /// <summary>
    /// 대상 GameObject가 null이 아닐 때만 활성화 또는 비활성화 처리
    /// </summary>
    /// <param name="uiObject">조작할 UI GameObject</param>
    /// <param name="isActive">true: 활성화, false: 비활성화</param>
    public static void SetUIActive(GameObject uiObject, bool isActive)
    {
        if(uiObject != null)
        {
            uiObject.SetActive(isActive);
        }
    }
}