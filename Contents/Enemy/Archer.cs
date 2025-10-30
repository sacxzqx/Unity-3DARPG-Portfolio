using UnityEngine;

/// <summary>
/// Enemy Ŭ������ ��ӹ޴� �ü� ���� Ŭ����
/// ȭ�� �߻� �� �տ� �� ȭ���� ǥ��/���� ó���� ���
/// </summary>
public class Archer : Enemy
{
    [Header("Archer Specifics")]
    [Tooltip("�տ� ��� ���� ��Ŀ� ȭ�� ���� ������Ʈ")]
    [SerializeField]  private GameObject arrowInHand;
    [SerializeField] private Transform arrowSpawnPoint;

    [Tooltip("������ �߻�� �߻�ü�� ȭ�� ������")]
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
            Debug.LogError("�ü��� ȭ�� ����(arrowInHand �Ǵ� arrowPrefab)�� �������� �ʾҽ��ϴ�!", this);
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