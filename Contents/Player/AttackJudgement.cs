using UnityEngine;

/// <summary>
/// �÷��̾��� ���� �ݶ��̴��� �����Ǿ� �ݶ��̴��� ���� Ʈ���Ÿ� ���� Ŭ����
/// </summary>
public class AttackJudgement : MonoBehaviour
{
    [SerializeField] PlayerContext playerContext;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemyHealth = other.gameObject.GetComponent<Enemy>();

            int playerDamage = Mathf.RoundToInt(playerContext.Stat.Stats.Strength.CurrentValue);

            enemyHealth.TakeDamage(playerDamage);
        }
    }
}
