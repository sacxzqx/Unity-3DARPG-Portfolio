using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// UIInitializer는 게임 시작 시 UI 프리팹들을 인스턴스화하고
/// 딕셔너리에 등록하여 전역적으로 접근 가능하도록 관리하는 초기화 관리자
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
    /// 특정 UI를 기본적으로 활성화
    /// HUD 및 월드맵 관련 UI 콘텐츠를 켜는 초기 활성화 단계에서 호출
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