using UnityEngine;

/// <summary>
/// 하나의 퀘스트 정보를 담는 ScriptableObject
/// 이름, 요구 조건, 단계, 보상 등 퀘스트 메타데이터를 포함함
/// </summary>
[CreateAssetMenu(fileName = "QuestInfoSO", menuName = "QuestInfoSO", order = 1)] 
public class QuestSO : ScriptableObject
{
    [field: SerializeField]
    [Tooltip("퀘스트 고유 ID (파일 이름 기준 자동 설정됨)")]
    public string Id { get; private set; }

    [Header("General")]
    [Tooltip("UI에 표시될 퀘스트 이름")]
    public string DisplayName;
    [TextArea(4, 10)]
    public string QuestDescription;

    [Tooltip("퀘스트를 제공하는 씬과 NPC의 위치")]
    public string SceneName;
    public Vector3 StartNpcPosition;
    public Vector3 EndNpcPosition;

    [Header("Requirements")]
    [Tooltip("퀘스트 수락에 필요한 최소 레벨")]
    public int LevelRequirement;

    [Tooltip("선행 퀘스트 (완료해야 수락 가능)")]
    public QuestSO[] QuestPrerequisites;

    [Header("Steps")]
    [Tooltip("단계별 퀘스트 진행을 담당하는 프리팹 배열")]
    public GameObject[] QuestStepPrefabs;

    [Header("Rewards")]
    [Tooltip("완료 시 획득할 골드")]
    public int GoldReward;

    [Tooltip("완료 시 획득할 경험치")]
    public int ExperienceReward;

    /// <summary>
    /// ScriptableObject 이름을 자동으로 id 필드에 동기화
    /// </summary>
    private void OnValidate()
    {
#if UNITY_EDITOR
        Id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}