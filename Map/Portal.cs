using UnityEngine;

public class Portal : MonoBehaviour, IInteractable
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private SpawnPointID destinationPointID; // 이 포탈을 타면 도착할 게이트 이름

    public InteractionType InteractionType => InteractionType.Portal;

    public void Interact()
    {
        SpawnManager.NextSpawnPointID = destinationPointID;
        GameManager.Instance.LoadScene(sceneToLoad);
    }
}
