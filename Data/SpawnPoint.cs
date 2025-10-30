using UnityEngine;

public enum SpawnPointID
{
    Default,
    Forest_Entrance,
    Village_From_Forest,
    SavePoint_01
}

public class SpawnPoint : MonoBehaviour
{
    [Tooltip("이 스폰 지점의 고유 ID. SpawnManager가 이 ID를 통해 지점을 찾음")]
    [SerializeField] private SpawnPointID pointID;

    public SpawnPointID PointID => pointID;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
        Gizmos.color = Color.white;
    }
}
