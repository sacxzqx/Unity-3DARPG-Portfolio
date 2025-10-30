using UnityEngine;

public class Portal : MonoBehaviour, IInteractable
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private SpawnPointID destinationPointID; // �� ��Ż�� Ÿ�� ������ ����Ʈ �̸�

    public InteractionType InteractionType => InteractionType.Portal;

    public void Interact()
    {
        SpawnManager.NextSpawnPointID = destinationPointID;
        GameManager.Instance.LoadScene(sceneToLoad);
    }
}
