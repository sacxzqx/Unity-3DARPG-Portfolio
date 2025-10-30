using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ����Ʈ ����/�Ϸ� ��Ŀ�� �̴ϸ� �� ����ʿ� ǥ���ϴ� Ŭ����
/// UI ����, ����Ʈ ����, ���� ��ǥ ���� �������� ��Ŀ�� �������� ǥ��/������
/// </summary>
public class MinimapIconManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private MinimapMapper minimapMapper; // ���� ����

    [Header("Marker Prefabs")]
    [SerializeField] private GameObject canStartPrefab;
    [SerializeField] private GameObject canFinishPrefab;
    [SerializeField] private GameObject enemyMarkerPrefab;

    [Header("Icon Transforms")]
    [SerializeField] private RectTransform iconParent; // ��� �������� ������ �θ�
    [SerializeField] private RectTransform playerIcon;

    private Dictionary<string, GameObject> activeMarkers = new Dictionary<string, GameObject>();
    private Dictionary<IEnemy, GameObject> activeEnemyMarkers = new Dictionary<IEnemy, GameObject>();

    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        UpdatePlayerIcon();
        UpdateAllEnemyMarkersPosition();
    }

    private void OnEnable()
    {
        minimapMapper.OnMapperInitialized += UpdateAllQuestMarkers;
        GameEventsManager.Instance.QuestEvents.OnQuestStateChange += ShowQuestMarker;
        GameEventsManager.Instance.EnemyEvents.OnEnemySpawned += HandleEnemySpawned;
        GameEventsManager.Instance.EnemyEvents.OnEnemyDespawned += HandleEnemyDespawned;
    }

    private void OnDisable()
    {
        minimapMapper.OnMapperInitialized -= UpdateAllQuestMarkers;
        GameEventsManager.Instance.QuestEvents.OnQuestStateChange -= ShowQuestMarker;
        GameEventsManager.Instance.EnemyEvents.OnEnemySpawned -= HandleEnemySpawned;
        GameEventsManager.Instance.EnemyEvents.OnEnemyDespawned -= HandleEnemyDespawned;
    }

    /// <summary>
    /// ����Ʈ ���¿� ���� ���� �Ǵ� �Ϸ� ��Ŀ�� ǥ��
    /// </summary>
    public void ShowQuestMarker(Quest quest)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (quest.QuestInfo.SceneName != currentSceneName)
        {
            // ���� �ٸ� ���� ����Ʈ���, Ȥ�ö� ���������� �� ��Ŀ�� ����� ��� �Լ��� ����
            if (activeMarkers.ContainsKey(quest.QuestInfo.Id))
            {
                Destroy(activeMarkers[quest.QuestInfo.Id]);
                activeMarkers.Remove(quest.QuestInfo.Id);
            }
            return; // �ٸ� ���� ����Ʈ�̹Ƿ� �� �̻� �������� ����
        }

        if (activeMarkers.ContainsKey(quest.QuestInfo.Id))
        {
            Destroy(activeMarkers[quest.QuestInfo.Id]);
            activeMarkers.Remove(quest.QuestInfo.Id);
        }

        GameObject prefabToInstantiate = null;
        Vector3 markerWorldPos = Vector3.zero;

        switch (quest.QuestState)
        {
            case QuestState.CAN_START:
                prefabToInstantiate = canStartPrefab;
                markerWorldPos = quest.QuestInfo.StartNpcPosition;
                break;
            case QuestState.CAN_FINISH:
                prefabToInstantiate = canFinishPrefab;
                markerWorldPos = quest.QuestInfo.EndNpcPosition;
                break;
            default:
                return;
        }

        GameObject newMarker = Instantiate(prefabToInstantiate, iconParent);
        newMarker.SetActive(true);

        Vector2 mapPos = minimapMapper.ConvertWorldToMap(markerWorldPos);

        newMarker.GetComponent<RectTransform>().anchoredPosition = mapPos;

        activeMarkers.Add(quest.QuestInfo.Id, newMarker);
    }

    public void HideMarker(string questId)
    {
        if (activeMarkers.ContainsKey(questId))
        {
            Destroy(activeMarkers[questId]);
            activeMarkers.Remove(questId);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Loading")
        {
            return;
        }

        UpdateAllQuestMarkers();
    }

    private void UpdatePlayerIcon()
    {
        if (playerTransform == null || playerIcon == null || minimapMapper == null) return;

        // Mapper�� ���񽺸� �̿��� ��ǥ ��ȯ
        Vector2 mapPos = minimapMapper.ConvertWorldToMap(playerTransform.position);
        playerIcon.anchoredPosition = mapPos;

        // ȸ�� �� ������Ʈ
        float yRotation = playerTransform.eulerAngles.y;
        playerIcon.localRotation = Quaternion.Euler(0f, 0f, -yRotation);
    }

    private void HandleEnemySpawned(IEnemy enemy)
    {
        // �̹� ��ϵ� ���̸� ����
        if (activeEnemyMarkers.ContainsKey(enemy)) return;

        // ���ο� �������� �����ϰ� Dictionary�� �߰�
        GameObject newMarker = Instantiate(enemyMarkerPrefab, iconParent);
        newMarker.SetActive(true);
        activeEnemyMarkers.Add(enemy, newMarker);
    }
    
    private void HandleEnemyDespawned(IEnemy enemy)
    {
        // ��ϵ� ������ Ȯ��
        if (activeEnemyMarkers.TryGetValue(enemy, out GameObject marker))
        {
            // �������� �ı��ϰ� Dictionary���� ����
            Destroy(marker);
            activeEnemyMarkers.Remove(enemy);
        }
    }

    private void UpdateAllEnemyMarkersPosition()
    {
        if (playerTransform == null || minimapMapper == null) return;

        // ��ϵ� ��� ���� ���� �ݺ�
        foreach (var entry in activeEnemyMarkers)
        {
            IEnemy enemy = entry.Key;
            GameObject marker = entry.Value;

            if (enemy == null || marker == null) continue;

            // ���� ��ǥ�� �̴ϸ� ��ǥ�� ��ȯ�Ͽ� ����
            Vector2 mapPos = minimapMapper.ConvertWorldToMap(enemy.gameObject.transform.position);
            marker.GetComponent<RectTransform>().anchoredPosition = mapPos;
        }
    }

    private void UpdateAllQuestMarkers()
    {
        // ���� ��Ŀ�� ��� ����
        foreach (var marker in activeMarkers.Values)
        {
            Destroy(marker);
        }
        activeMarkers.Clear();

        if (QuestManager.Instance == null) return;
        // QuestManager�κ��� ��� ����Ʈ ����� ������ ShowQuestMarker�� ����
        foreach (Quest quest in QuestManager.Instance.GetAllQuests())
        {
            ShowQuestMarker(quest);
        }
    }
}