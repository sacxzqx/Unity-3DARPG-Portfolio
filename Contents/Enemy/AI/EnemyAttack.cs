using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class EnemyAttack : Action
{
    private Animator anim;

    private bool animationEnded = false;
    private float delayTimer = 0f;
    private float delayAfterAnim = 2f;

    public SharedFloat StrafeMoveChance;

    public override void OnStart()
    {
        anim = GetComponent<Animator>();
        anim.SetTrigger("EnemyAttack");

        animationEnded = false;
        delayTimer = 0f;

        // 공격 후 Strafe 확률 증가
        StrafeMoveChance.Value += 0.2f;
    }

    public override TaskStatus OnUpdate()
    {
        var state = anim.GetCurrentAnimatorStateInfo(0);

        if (!animationEnded)
        {
            if (state.IsName("Attack") && state.normalizedTime >= 0.80f)
            {
                animationEnded = true;
                delayTimer = 0f;
            }
            else
            {
                return TaskStatus.Running;
            }
        }

        delayTimer += Time.deltaTime;
        if (delayTimer >= delayAfterAnim)
        {
            delayTimer = 0f;
            animationEnded = false;

            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        delayTimer = 0f;
        anim.ResetTrigger("EnemyAttack");
        animationEnded = false;
    }
}
