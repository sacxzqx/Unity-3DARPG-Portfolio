using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private List<ISavable> savables = new();
    private GameSaveData dataToLoad;
    public bool IsLoading { get; private set; } = false; 

    private int lastUsedSlotIndex = -1;

    private const int AUTO_SAVE_SLOT_INDEX = 3;
    private bool isAutoSaving = false; // �ߺ� ���� ���� �÷���

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
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameEventsManager.Instance.PlayerEvents.OnPlayerDied += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameEventsManager.Instance.PlayerEvents.OnPlayerDied -= HandlePlayerDeath;
    }

    public void RegisterSavable(ISavable savable)
    {
        if (!savables.Contains(savable))
            savables.Add(savable);
    }

    public void UnregisterSavable(ISavable savable)
    {
        savables.Remove(savable);
    }

    public void SaveGame(int slotIndex)
    {
        GameSaveData data = new GameSaveData();

        data.MetaData.SceneName = SceneManager.GetActiveScene().name;
        data.MetaData.DateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        foreach (var savable  in savables)
        {
            savable.SaveData(data);
        }

        string path = GetSavePath(slotIndex);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        lastUsedSlotIndex = slotIndex;

    }

    public void LoadGame(int slotIndex)
    {
        Time.timeScale = 1f;

        IsLoading = true;
        string path = GetSavePath(slotIndex);

        if (!File.Exists(path))
        {
            Debug.LogWarning("���̺� ������ �����ϴ�.");
            return;
        }

        string json = File.ReadAllText(path);
        dataToLoad = JsonUtility.FromJson<GameSaveData>(json);

        lastUsedSlotIndex = slotIndex;

        Cursor.visible = false;
        SpawnManager.Instance.IsLoadingFromSave = true;
        GameManager.Instance.LoadScene(dataToLoad.MetaData.SceneName);
    }

    /// <summary>
    /// �÷��̾� ��� �ÿ� ���������� �ε��� ��ġ�� ���̺� ������ �ε��ϴ� �Լ�
    /// </summary>
    private void HandlePlayerDeath()
    {
        StartCoroutine(AutoLoadAfterDelay(10f));
    }

    private IEnumerator AutoLoadAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (lastUsedSlotIndex != -1)
        {
            LoadGame(lastUsedSlotIndex);
        }
        else
        {
            SceneManager.LoadScene("StartScene");
        }
    }

    public string GetSceneName(int slotIndex)
    {
        string path = GetSavePath(slotIndex);
        if (!File.Exists(path)) return "";

        string json = File.ReadAllText(path);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
        return data.PlayerData?.SceneName ?? "";
    }

    public string GetSaveTime(int slotIndex)
    {
        string path = GetSavePath(slotIndex);
        if (!File.Exists(path)) return "";

        return File.GetLastWriteTime(path).ToString("yyyy-MM-dd HH:mm:ss");
    }

    private GameSaveData LoadMetaOnly(int slotIndex)
    {
        string path = GetSavePath(slotIndex);
        if (!File.Exists(path)) return null;

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<GameSaveData>(json);
    }

    private string GetSavePath(int slotIndex)
    {
        return Path.Combine(Application.persistentDataPath, $"save_{slotIndex}.json");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (dataToLoad == null) return; // �ε� ���� �ƴ� �� �ƹ��͵� �� ��

        // �� ���� ������ �ε�� �� �������� ��� �����͸� ����
        ApplyLoadedData(dataToLoad);
        dataToLoad = null;

        StartCoroutine(ResetLoadingFlag());
    }

    private void ApplyLoadedData(GameSaveData data)
    {
        foreach (var savable in savables)
        {
            savable.LoadData(data);
        }
    }

    /// <summary>
    /// �� �ε� �� �����ϰ� ���� ���̺긦 �����ϵ��� �ܺο��� ȣ���ϴ� �Լ�.
    /// </summary>
    public void AutoSaveOnSceneLoad()
    {
        // �̹� ���� ���̺갡 ���� ���̸� ����
        if (isAutoSaving) return;

        StartCoroutine(AutoSaveOnSceneLoadCoroutine());
    }

    private IEnumerator AutoSaveOnSceneLoadCoroutine()
    {
        isAutoSaving = true;

        // ���� �������� ��� Start(), Update()�� ���� ������ ��ٷ�
        // ��� �ʱ�ȭ�� �Ϸ�� ������ �ڵ� ������ �����ϵ��� ��
        yield return new WaitForEndOfFrame();

        yield return null;

        SaveGame(AUTO_SAVE_SLOT_INDEX);

        isAutoSaving = false;
    }

    private IEnumerator ResetLoadingFlag()
    {
        yield return null;

        IsLoading = false;
    }
}
