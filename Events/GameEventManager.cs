using UnityEngine;

/// <summary>
/// ���� �������� ���Ǵ� �̺�Ʈ�� ��Ƶ� �̱��� �Ŵ���
/// �� �ý��ۿ��� ����ϴ� �̺�Ʈ �׷��� �����Ͽ� �ν��Ͻ�ȭ �ϰ�
/// �ʿ��� ������ GameEventManager.instance�� �����Ͽ� �����
/// </summary>
public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager Instance { get; private set; }

    public QuestEvents QuestEvents { get; private set; }
    public PlayerEvents PlayerEvents { get; private set; }
    public EnemyEvents EnemyEvents { get; private set; }
    public InputEvents InputEvents { get; private set; }
    public GoldEvents GoldEvents { get; private set; }
    public DialogueEvents DialogueEvents { get; private set; }
    public ItemEvents ItemEvents { get; private set; }
    public ShopEvents ShopEvents { get; private set; }
    public UIEvents UIEvents { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); 

        QuestEvents = new QuestEvents();
        PlayerEvents = new PlayerEvents();
        EnemyEvents = new EnemyEvents();
        InputEvents = new InputEvents();
        GoldEvents = new GoldEvents();
        DialogueEvents = new DialogueEvents();
        ItemEvents = new ItemEvents();
        ShopEvents = new ShopEvents();
        UIEvents = new UIEvents();
    }
}
