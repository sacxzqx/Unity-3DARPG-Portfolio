using UnityEngine;

/// <summary>
/// �� ���� ��ġ�Ǵ� �ڵ� ����� Ŭ����
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