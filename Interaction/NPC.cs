using UnityEngine;

/// <summary>
/// 플레이어와 상호작용 가능한 NPC의 행동을 정의한 클래스
/// </summary>
public class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] private string npcId = "레이저";

    [SerializeField] private int defaultDialogueId = 1;
    [SerializeField] private ItemSO[] itemsForSale;

    private bool isFirstTimeMeeting = true;

    private Animator anim;

    public InteractionType InteractionType => InteractionType.NPC;

    public int DialogueIndex
    {
        get { return defaultDialogueId; }
        set { defaultDialogueId = value; }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Interact()
    {
        anim.SetTrigger("Interaction");

        DialogueManager.Instance.StartConversationWith(npcId);
        InitializeShop();
    }

    private void InitializeShop()
    {
        GameEventsManager.Instance.ShopEvents.InitializeShop(itemsForSale);
    }
}