using UnityEngine;

/// <summary>
/// UI ���� ���� ����� �����ϴ� ��ƿ��Ƽ Ŭ����
/// </summary>
public static class UIUtilities
{
    /// <summary>
    /// ��� GameObject�� null�� �ƴ� ���� Ȱ��ȭ �Ǵ� ��Ȱ��ȭ ó��
    /// </summary>
    /// <param name="uiObject">������ UI GameObject</param>
    /// <param name="isActive">true: Ȱ��ȭ, false: ��Ȱ��ȭ</param>
    public static void SetUIActive(GameObject uiObject, bool isActive)
    {
        if(uiObject != null)
        {
            uiObject.SetActive(isActive);
        }
    }
}