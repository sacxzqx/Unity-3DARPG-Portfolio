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
    private bool isAutoSaving = false; // 중복 실행 방지 플래그

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
            Debug.LogWarning("세이브 파일이 없습니다.");
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
    /// 플레이어 사망 시에 마지막으로 로드한 위치의 세이브 슬롯을 로드하는 함수
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
        if (dataToLoad == null) return; // 로드 중이 아닐 땐 아무것도 안 함

        // 새 씬이 완전히 로드된 이 시점에서 모든 데이터를 적용
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
    /// 씬 로드 후 안전하게 오토 세이브를 실행하도록 외부에서 호출하는 함수.
    /// </summary>
    public void AutoSaveOnSceneLoad()
    {
        // 이미 오토 세이브가 진행 중이면 무시
        if (isAutoSaving) return;

        StartCoroutine(AutoSaveOnSceneLoadCoroutine());
    }

    private IEnumerator AutoSaveOnSceneLoadCoroutine()
    {
        isAutoSaving = true;

        // 현재 프레임의 모든 Start(), Update()가 끝날 때까지 기다려
        // 모든 초기화가 완료된 시점에 자동 저장을 실행하도록 함
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
