using System.Collections;
using UnityEngine;

/// <summary>
/// ��ų ����Ʈ ������Ʈ�� �����Ǿ�, ���� �ð� ���� �ݶ��̴��� Ȱ��ȭ�ϰ�
/// �浹 �� ������ �������� ���ϴ� ������Ʈ
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