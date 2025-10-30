using UnityEngine;

/// <summary>
/// �ϳ��� ����Ʈ ������ ��� ScriptableObject
/// �̸�, �䱸 ����, �ܰ�, ���� �� ����Ʈ ��Ÿ�����͸� ������
/// </summary>
[CreateAssetMenu(fileName = "QuestInfoSO", menuName = "QuestInfoSO", order = 1)] 
public class QuestSO : ScriptableObject
{
    [field: SerializeField]
    [Tooltip("����Ʈ ���� ID (���� �̸� ���� �ڵ� ������)")]
    public string Id { get; private set; }

    [Header("General")]
    [Tooltip("UI�� ǥ�õ� ����Ʈ �̸�")]
    public string DisplayName;
    [TextArea(4, 10)]
    public string QuestDescription;

    [Tooltip("����Ʈ�� �����ϴ� ���� NPC�� ��ġ")]
    public string SceneName;
    public Vector3 StartNpcPosition;
    public Vector3 EndNpcPosition;

    [Header("Requirements")]
    [Tooltip("����Ʈ ������ �ʿ��� �ּ� ����")]
    public int LevelRequirement;

    [Tooltip("���� ����Ʈ (�Ϸ��ؾ� ���� ����)")]
    public QuestSO[] QuestPrerequisites;

    [Header("Steps")]
    [Tooltip("�ܰ躰 ����Ʈ ������ ����ϴ� ������ �迭")]
    public GameObject[] QuestStepPrefabs;

    [Header("Rewards")]
    [Tooltip("�Ϸ� �� ȹ���� ���")]
    public int GoldReward;

    [Tooltip("�Ϸ� �� ȹ���� ����ġ")]
    public int ExperienceReward;

    /// <summary>
    /// ScriptableObject �̸��� �ڵ����� id �ʵ忡 ����ȭ
    /// </summary>
    private void OnValidate()
    {
#if UNITY_EDITOR
        Id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}