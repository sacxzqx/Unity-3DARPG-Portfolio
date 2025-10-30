using UnityEngine;

/// <summary>
/// 각 씬에 배치되는 자동 저장용 클래스
/// </summary>
public class SceneInitializer : MonoBehaviour
{
    void Start()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.AutoSaveOnSceneLoad();
        }
    }
}