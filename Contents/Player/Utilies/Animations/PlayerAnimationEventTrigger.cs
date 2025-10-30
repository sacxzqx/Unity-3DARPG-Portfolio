using UnityEngine;

/// <summary>
/// �ִϸ��̼� �̺�Ʈ�� ���� ���� ����, ��ƼŬ �� ���� ����Ʈ�� Ʈ�����ϴ� ������Ʈ
/// �ִϸ��̼� Ÿ�Ӷ��ο� ����Ǿ� PlayerContext�� ���¸ӽŰ� ȿ���� ����
/// </summary>
public class PlayerAnimationEventTrigger : MonoBehaviour
{
    private PlayerContext player;

    private void Awake()
    {
        player = GetComponent<PlayerContext>();
    }

    public void TriggerOnMovementStateAnimationEnterEvent()
    {
        player.OnMovementStateAnimationEnterEvent();
    }

    public void TriggerOnMovementStateAnimationExitEvent()
    {
        player.OnMovementStateAnimationExitEvent();
    }

    public void TriggerOnMovementStateAnimationTransitionEvent()
    {
        player.OnMovementStateAnimationTransitionEvent();
    }

    public void TriggerOnActionStateAnimationEnterEvent()
    {
        player.OnActionStateAnimationEnterEvent();
    }

    public void TriggerOnActionStateAnimationExitEvent()
    {
        player.OnActionStateAnimationExitEvent();
    }

    public void TriggerOnActionStateAnimationTransitionEvent()
    {
        player.OnActionStateAnimationTransitionEvent();
    }

    public void TriggerSkillEffect()
    {
        if (player.CurrentSkill == null || player.CurrentSkill.EffectPrefab == null)
        {
            Debug.LogWarning("���� ��ų �Ǵ� ��ų�� EffectPrefab�� �����Ǿ� ���� �ʽ��ϴ�.");
            return;
        }
        GameObject effectInstance = ObjectPooler.Instance.GetFromPool(
                player.CurrentSkill.EffectPrefab,
                transform.position,
                transform.rotation,
                false
            );

        SkillEffect skillEffect = effectInstance.GetComponent<SkillEffect>();
        skillEffect.Initialize(player.CurrentSkill);

        Transform effectTransform = effectInstance.transform;
        effectTransform.SetParent(transform);
        effectTransform.localPosition = player.CurrentSkill.OffsetPosition;
        effectTransform.localRotation = Quaternion.Euler(player.CurrentSkill.OffsetRotation);

        effectTransform.SetParent(null);
        effectInstance.SetActive(true);
    }

    public void PlaySFX(string clipName)
    {
        AudioManager.Instance.PlaySFX(clipName);
    }
}
