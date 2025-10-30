using UnityEngine;

/// <summary>
/// Enemy 클래스를 상속받는 궁수 전용 클래스
/// 화살 발사 및 손에 쥔 화살의 표시/숨김 처리를 담당
/// </summary>
public class Archer : Enemy
{
    [Header("Archer Specifics")]
    [Tooltip("손에 쥐고 있을 장식용 화살 게임 오브젝트")]
    [SerializeField]  private GameObject arrowInHand;
    [SerializeField] private Transform arrowSpawnPoint;

    [Tooltip("실제로 발사될 발사체용 화살 프리팹")]
    [SerializeField]  private GameObject arrowPrefab;

    public void ShowArrowInHand()
    {
        if (arrowInHand != null)
        {
            arrowInHand.SetActive(true);
        }
    }

    public void HideArrowInHand()
    {
        if (arrowInHand != null)
        {
            arrowInHand.SetActive(false);
        }
    }

    public void FireArrow()
    {
        if (arrowInHand == null || arrowPrefab == null)
        {
            Debug.LogError("궁수의 화살 참조(arrowInHand 또는 arrowPrefab)가 설정되지 않았습니다!", this);
            return;
        }

        Vector3 spawnPosition = arrowSpawnPoint.transform.position;
        Quaternion spawnRotation = arrowSpawnPoint.transform.rotation;

        GameObject arrowObject = ObjectPooler.Instance.GetFromPool(arrowPrefab, spawnPosition, spawnRotation);
        Arrow arrowScript = arrowObject.GetComponent<Arrow>();

        if (arrowScript != null)
        {
            arrowScript.Fire(TargetPlayer, EnemyData.AttackPower);
        }

        HideArrowInHand();

        AudioManager.Instance.PlaySFX("ArrowRelease");
    }

    public override void TakeDamage(int damageAmount)
    {
        base.TakeDamage(damageAmount);

        HideArrowInHand();
    }
}