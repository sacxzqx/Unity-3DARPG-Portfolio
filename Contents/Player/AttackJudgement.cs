using UnityEngine;

/// <summary>
/// 플레이어의 무기 콜라이더에 부착되어 콜라이더를 통해 트리거를 돕는 클래스
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
