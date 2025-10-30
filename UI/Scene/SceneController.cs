using UnityEngine;

public class SceneController : MonoBehaviour
{
    [Header("Player Spawn Points")]
    public Transform PlayerSpawnPoint; // 씬마다 지정된 플레이어의 디폴트 생성 위치

    void Start()
    {
        // 씬에 따라 초기 위치 설정 (플레이어 또는 다른 오브젝트)
        SetInitialPositions();
    }

    private void SetInitialPositions()
    {
        if (PlayerSpawnPoint != null)
        {
            // 플레이어의 위치를 지정된 spawn point로 설정
            GameManager.Instance.GameInitializer.PlayerInstance.transform.position = PlayerSpawnPoint.position;
        }
    }
}