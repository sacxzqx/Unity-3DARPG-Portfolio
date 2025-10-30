using UnityEngine;
using System.Collections;

/// <summary>
/// 전투 진입 및 종료 이벤트에 따라 전투 BGM과 일반 BGM을 자동으로 전환하는 관리자 클래스
/// </summary>
public class BattleMusicManager : MonoBehaviour, IReset
{
    private int activeBattleCount = 0;
    private string previousBGM;
    private string battleBGM = "Battle";

    private Coroutine switchBackCoroutine = null; // BGM 전환 코루틴을 저장할 변수

    private void OnEnable()
    {
        GameEventsManager.Instance.EnemyEvents.OnEnemyEnterBattle += OnEnemyEnterBattle;
        GameEventsManager.Instance.EnemyEvents.OnEnemyExitBattle += OnEnemyExitBattle;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.EnemyEvents.OnEnemyEnterBattle -= OnEnemyEnterBattle;
        GameEventsManager.Instance.EnemyEvents.OnEnemyExitBattle -= OnEnemyExitBattle;
    }

    private void OnEnemyEnterBattle()
    {
        if (switchBackCoroutine != null)
        {
            StopCoroutine(switchBackCoroutine);
            switchBackCoroutine = null;
        }

        if (activeBattleCount == 0 && AudioManager.Instance.GetCurrentBGMName() != battleBGM)
        {
            previousBGM = AudioManager.Instance.GetCurrentBGMName();
            AudioManager.Instance.PlayBGM(battleBGM);
        }

        activeBattleCount++;
    }

    private void OnEnemyExitBattle()
    {
        activeBattleCount = Mathf.Max(0, activeBattleCount - 1);

        if (activeBattleCount == 0)
        {
            switchBackCoroutine = StartCoroutine(SwitchBackToPreviousBGMWithDelay());
        }
    }

    /// <summary>
    /// 지정된 시간만큼 기다린 후, 이전 BGM으로 전환하는 코루틴
    /// </summary>
    private IEnumerator SwitchBackToPreviousBGMWithDelay()
    {
        yield return new WaitForSeconds(5f);

        // 기다리는 동안 새로운 전투가 시작되지 않았다면 BGM 전환 실행
        AudioManager.Instance.PlayBGM(previousBGM);

        switchBackCoroutine = null;
    }

    public void ResetBeforeSceneLoad()
    {
        previousBGM = null;
        if (switchBackCoroutine != null)
        {
            StopCoroutine(switchBackCoroutine);
            switchBackCoroutine = null;
        }
    }

    public void ResetAfterSceneLoad()
    {
        activeBattleCount = 0;
    }
}
