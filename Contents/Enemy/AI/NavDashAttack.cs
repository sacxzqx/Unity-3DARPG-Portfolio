using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using Unity.Services.Analytics.Internal;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 플레이어를 향해 돌진하여 대시 공격을 수행하는 Action 노드
/// 이동 -> 애니메이션 실행 -> 후딜레이 처리까지 통합 제어
/// </summary>
public class NavDashAttack : Action
{
    private NavMeshAgent agent;
    private Animator anim;
    private Enemy enemy;

    private bool hasAttacked = false;

    private float delayAfterAnim = 1f;
    private bool animationEnded = false;
    private float delayTimer = 0f;

    private Coroutine moveSpeedCoroutine;

    public SharedFloat StrafeMoveChance;

    public override void OnStart()
    {
        agent = GetDefaultGameObject(gameObject).GetComponent<NavMeshAgent>();
        agent.updateRotation = true;
        anim = agent.GetComponent<Animator>();
        enemy = agent.GetComponent<Enemy>();

        delayTimer = 0f;
        hasAttacked = false;
        animationEnded = false;
        anim.SetFloat("MoveY", 0);

        agent.speed = 2.0f;

        moveSpeedCoroutine = enemy.StartCoroutine(LerpMoveSpeed(0f, 1f, 0.2f));

        StrafeMoveChance.Value += 0.2f;
    }

    public override TaskStatus OnUpdate()
    {
        if (enemy.TargetPlayer == null)
            return TaskStatus.Failure;

        if (hasAttacked)
        {
            return HandlePostAttackDelay();
        }
        else
        {
            return HandleDashMovement();
        }
    }

    public override void OnEnd()
    {
        agent.updateRotation = false;
        delayTimer = 0f;
        animationEnded = false;
        anim.SetFloat("MoveY", 0);

        if (moveSpeedCoroutine != null)
        {
            enemy.StopCoroutine(moveSpeedCoroutine);
            moveSpeedCoroutine = null;
        }
    }

    private TaskStatus HandleDashMovement()
    {
        agent.SetDestination(enemy.TargetPlayer.position);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            anim.SetTrigger("DashAttack");
            agent.isStopped = true;
            hasAttacked = true;
            return TaskStatus.Running; // 상태 전환 후 다음 틱까지 대기
        }

        agent.isStopped = false;
        anim.SetFloat("MoveY", agent.speed);
        return TaskStatus.Running;
    }

    private TaskStatus HandlePostAttackDelay()
    {
        var state = anim.GetCurrentAnimatorStateInfo(0);

        if (!animationEnded)
        {
            if (state.IsName("DashAttack") && state.normalizedTime >= 0.80f)
            {
                anim.SetFloat("MoveY", 0f);
                animationEnded = true;
            }
            return TaskStatus.Running;
        }

        delayTimer += Time.deltaTime;
        if (delayTimer >= delayAfterAnim)
        {
            delayTimer = 0f;
            hasAttacked = false;
            animationEnded = false;
            return TaskStatus.Success;
        }

        return TaskStatus.Running; // 딜레이 중
    }

    private IEnumerator LerpMoveSpeed(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float value = Mathf.Lerp(from, to, t / duration);
            anim.SetFloat("MoveY", value);
            yield return null;
        }

        anim.SetFloat("MoveY", to);
    }
}
