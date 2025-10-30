using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// UIInitializer�� ���� ���� �� UI �����յ��� �ν��Ͻ�ȭ�ϰ�
/// ��ųʸ��� ����Ͽ� ���������� ���� �����ϵ��� �����ϴ� �ʱ�ȭ ������
/// </summary>
public class UIInitializer : MonoBehaviour
{
    [Header("UI Prefabs")]
    public List<GameObject> UIPrefabs;

    public Dictionary<string, GameObject> UIDictionary = new Dictionary<string, GameObject>();

    public void Initialize()
    {
        foreach (var prefab in UIPrefabs)
        {
            GameObject uiInstance = Instantiate(prefab, transform);
            UIDictionary[prefab.name] = uiInstance;
        }
    }

    /// <summary>
    /// Ư�� UI�� �⺻������ Ȱ��ȭ
    /// HUD �� ����� ���� UI �������� �Ѵ� �ʱ� Ȱ��ȭ �ܰ迡�� ȣ��
    /// </summary>
    public void ActivateUI()
    {
        UIDictionary["HUD_Canvas"].transform.Find("HudUI/ContentParent").gameObject.SetActive(true);
        UIDictionary["HUD_Canvas"].transform.Find("HudUI/Sliders").gameObject.SetActive(true);
        UIDictionary["Map_Canvas"].transform.Find("MapUI/ContentParent").gameObject.SetActive(true);
    }

    public void DeActivateUI()
    {
        UIDictionary["HUD_Canvas"].transform.Find("HudUI/ContentParent").gameObject.SetActive(false);
        UIDictionary["HUD_Canvas"].transform.Find("HudUI/Sliders").gameObject.SetActive(false);
        UIDictionary["Map_Canvas"].transform.Find("MapUI/ContentParent").gameObject.SetActive(false);
    }
}