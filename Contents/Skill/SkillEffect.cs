using System.Collections;
using UnityEngine;

/// <summary>
/// 스킬 이펙트 오브젝트에 부착되어, 일정 시간 동안 콜라이더를 활성화하고
/// 충돌 시 적에게 데미지를 가하는 컴포넌트
/// </summary>
public class SkillEffect : MonoBehaviour
{
    private SkillSO skill;
    private ParticleSystem skillParticle;

    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private float enableTime = 0f;
    [SerializeField] private float enableDuration = 0.4f;

    private void Awake()
    {
        skillParticle = GetComponent<ParticleSystem>();
    }

    public void Initialize(SkillSO skill)
    {
        this.skill = skill;
    }

    private void OnEnable()
    {
        if (skill == null) return;

        skillParticle.Play();
        switch (skill.Type)
        {
            case SkillType.Attack:
                if (boxCollider != null)
                {
                    StartCoroutine(ActivateCollider(enableTime, enableDuration));
                }
                break;
            case SkillType.Buff:
                GameEventsManager.Instance.PlayerEvents.ApplyBuff(skill);
                break;
        }
        StartCoroutine(ReturnToPoolAfterDelay(skillParticle.main.duration));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            int damage = GameEventsManager.Instance.EnemyEvents.EnemySkillHit(skill);

            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    private IEnumerator ActivateCollider(float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        boxCollider.enabled = true;

        yield return new WaitForSeconds(duration);
        boxCollider.enabled = false;
    }

    private IEnumerator ReturnToPoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ObjectPooler.Instance.ReturnToPool(gameObject);
    }
}