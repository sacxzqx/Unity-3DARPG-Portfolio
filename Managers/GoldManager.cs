using UnityEngine;

public class GoldManager : MonoBehaviour, ISavable
{
    public static GoldManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private int startGold = 10;

    public int CurrentGold { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        CurrentGold = startGold;
    }

    private void OnEnable()
    {
        SaveManager.Instance.RegisterSavable(this);

        GameEventsManager.Instance.GoldEvents.OnGoldGained += GoldGained;
    }

    private void OnDisable()
    {
        SaveManager.Instance.UnregisterSavable(this);

        GameEventsManager.Instance.GoldEvents.OnGoldGained -= GoldGained;
    }

    private void Start()
    {
        GameEventsManager.Instance.GoldEvents.GoldChange(CurrentGold);
    }

    private void GoldGained(int gold)
    {
        CurrentGold += gold;
    }

    public void SaveData(GameSaveData data)
    {
        data.PlayerData.Gold = CurrentGold;
    }

    public void LoadData(GameSaveData data)
    {
        CurrentGold = data.PlayerData.Gold;
        GameEventsManager.Instance.GoldEvents.GoldChange(CurrentGold);
    }
}
