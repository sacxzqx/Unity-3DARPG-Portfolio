using UnityEngine;

/// <summary>
/// 애니메이션 이벤트를 통해 상태 전이, 파티클 및 사운드 이펙트를 트리거하는 컴포넌트
/// 애니메이션 타임라인에 연결되어 PlayerContext의 상태머신과 효과를 제어
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
            Debug.LogWarning("현재 스킬 또는 스킬의 EffectPrefab이 설정되어 있지 않습니다.");
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
