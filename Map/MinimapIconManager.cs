using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 퀘스트 시작/완료 마커를 미니맵 및 월드맵에 표시하는 클래스
/// UI 상태, 퀘스트 상태, 월드 좌표 등을 기준으로 마커를 동적으로 표시/제거함
/// </summary>
public class MinimapIconManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private MinimapMapper minimapMapper; // 계산기 참조

    [Header("Marker Prefabs")]
    [SerializeField] private GameObject canStartPrefab;
    [SerializeField] private GameObject canFinishPrefab;
    [SerializeField] private GameObject enemyMarkerPrefab;

    [Header("Icon Transforms")]
    [SerializeField] private RectTransform iconParent; // 모든 아이콘이 생성될 부모
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
    /// 퀘스트 상태에 따라 시작 또는 완료 마커를 표시
    /// </summary>
    public void ShowQuestMarker(Quest quest)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (quest.QuestInfo.SceneName != currentSceneName)
        {
            // 만약 다른 씬의 퀘스트라면, 혹시라도 남아있을지 모를 마커를 지우고 즉시 함수를 종료
            if (activeMarkers.ContainsKey(quest.QuestInfo.Id))
            {
                Destroy(activeMarkers[quest.QuestInfo.Id]);
                activeMarkers.Remove(quest.QuestInfo.Id);
            }
            return; // 다른 씬의 퀘스트이므로 더 이상 진행하지 않음
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

        // Mapper의 서비스를 이용해 좌표 변환
        Vector2 mapPos = minimapMapper.ConvertWorldToMap(playerTransform.position);
        playerIcon.anchoredPosition = mapPos;

        // 회전 값 업데이트
        float yRotation = playerTransform.eulerAngles.y;
        playerIcon.localRotation = Quaternion.Euler(0f, 0f, -yRotation);
    }

    private void HandleEnemySpawned(IEnemy enemy)
    {
        // 이미 등록된 적이면 무시
        if (activeEnemyMarkers.ContainsKey(enemy)) return;

        // 새로운 아이콘을 생성하고 Dictionary에 추가
        GameObject newMarker = Instantiate(enemyMarkerPrefab, iconParent);
        newMarker.SetActive(true);
        activeEnemyMarkers.Add(enemy, newMarker);
    }
    
    private void HandleEnemyDespawned(IEnemy enemy)
    {
        // 등록된 적인지 확인
        if (activeEnemyMarkers.TryGetValue(enemy, out GameObject marker))
        {
            // 아이콘을 파괴하고 Dictionary에서 제거
            Destroy(marker);
            activeEnemyMarkers.Remove(enemy);
        }
    }

    private void UpdateAllEnemyMarkersPosition()
    {
        if (playerTransform == null || minimapMapper == null) return;

        // 등록된 모든 적에 대해 반복
        foreach (var entry in activeEnemyMarkers)
        {
            IEnemy enemy = entry.Key;
            GameObject marker = entry.Value;

            if (enemy == null || marker == null) continue;

            // 월드 좌표를 미니맵 좌표로 변환하여 적용
            Vector2 mapPos = minimapMapper.ConvertWorldToMap(enemy.gameObject.transform.position);
            marker.GetComponent<RectTransform>().anchoredPosition = mapPos;
        }
    }

    private void UpdateAllQuestMarkers()
    {
        // 기존 마커를 모두 제거
        foreach (var marker in activeMarkers.Values)
        {
            Destroy(marker);
        }
        activeMarkers.Clear();

        if (QuestManager.Instance == null) return;
        // QuestManager로부터 모든 퀘스트 목록을 가져와 ShowQuestMarker를 실행
        foreach (Quest quest in QuestManager.Instance.GetAllQuests())
        {
            ShowQuestMarker(quest);
        }
    }
}