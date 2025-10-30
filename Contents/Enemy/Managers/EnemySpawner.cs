using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float respawnDelay = 10f;

    private void OnEnable()
    {
        GameEventsManager.Instance.EnemyEvents.OnEnemyDiedForRespawn += RespawnRequest;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.EnemyEvents.OnEnemyDiedForRespawn -= RespawnRequest;
    }

    private void RespawnRequest(IEnemy deadEnemy, GameObject prefab, Transform waypointRoot)
    {
        StartCoroutine(ReactivateCoroutine(deadEnemy.gameObject, waypointRoot));
    }

    /// <summary>
    /// ������ �ð��� ���� ��, ���� ��Ȱ��ȭ�ϰ� �ùٸ� ��������Ʈ ���� ������ ��ġ�ϴ� �ڷ�ƾ
    /// </summary>
    private IEnumerator ReactivateCoroutine(GameObject enemyToReactivate, Transform waypointRoot)
    {
        yield return new WaitForSeconds(respawnDelay);

        if (enemyToReactivate == null) yield break;

        Vector3 respawnPosition = enemyToReactivate.transform.position;
        Quaternion respawnRotation = enemyToReactivate.transform.rotation;

        if (waypointRoot != null && waypointRoot.childCount > 0)
        {
            Transform firstWaypoint = waypointRoot.GetChild(0);
            respawnPosition = firstWaypoint.position;
            respawnRotation = firstWaypoint.rotation;
        }

        Collider enemyCollider = enemyToReactivate.GetComponent<Collider>();
        enemyToReactivate.transform.position = PositioningUtility.GetGroundedPosition(respawnPosition, enemyCollider);
        enemyToReactivate.transform.rotation = respawnRotation;

        enemyToReactivate.SetActive(true);

        Enemy enemyScript = enemyToReactivate.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.WaypointRoot = waypointRoot;
        }
    }
}
