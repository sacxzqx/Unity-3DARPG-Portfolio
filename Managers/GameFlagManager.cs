using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임의 다양한 플래그(예: 대사, 튜토리얼등)을 관리하는 클래스
/// </summary>
public class GameFlagManager : MonoBehaviour, ISavable
{
    public static GameFlagManager Instance { get; private set; }

    private HashSet<string> setFlags = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SaveManager.Instance.RegisterSavable(this);
    }

    private void OnDisable()
    {
        SaveManager.Instance.UnregisterSavable(this);
    }

    public bool IsFlagSet(string flagId)
    {
        return setFlags.Contains(flagId);
    }

    public void SetFlag(string flagId)
    {
        if (!setFlags.Contains(flagId))
        {
            setFlags.Add(flagId);
        }
    }

    public void SaveData(GameSaveData data)
    {
        data.GameFlags = new List<string>(setFlags);
    }

    public void LoadData(GameSaveData data)
    {
        setFlags = new HashSet<string>(data.GameFlags);
    }
}
