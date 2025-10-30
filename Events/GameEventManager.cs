using UnityEngine;

/// <summary>
/// 게임 전역에서 사용되는 이벤트를 모아둔 싱글톤 매니저
/// 각 시스템에서 사용하는 이벤트 그룹을 구분하여 인스턴스화 하고
/// 필요한 곳에서 GameEventManager.instance로 접근하여 사용함
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
