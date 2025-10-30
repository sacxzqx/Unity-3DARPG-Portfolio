using UnityEngine;

public class SceneController : MonoBehaviour
{
    [Header("Player Spawn Points")]
    public Transform PlayerSpawnPoint; // ������ ������ �÷��̾��� ����Ʈ ���� ��ġ

    void Start()
    {
        // ���� ���� �ʱ� ��ġ ���� (�÷��̾� �Ǵ� �ٸ� ������Ʈ)
        SetInitialPositions();
    }

    private void SetInitialPositions()
    {
        if (PlayerSpawnPoint != null)
        {
            // �÷��̾��� ��ġ�� ������ spawn point�� ����
            GameManager.Instance.GameInitializer.PlayerInstance.transform.position = PlayerSpawnPoint.position;
        }
    }
}