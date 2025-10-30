using UnityEngine;
using System.Collections;

/// <summary>
/// ���� ���� �� ���� �̺�Ʈ�� ���� ���� BGM�� �Ϲ� BGM�� �ڵ����� ��ȯ�ϴ� ������ Ŭ����
/// </summary>
public class BattleMusicManager : MonoBehaviour, IReset
{
    private int activeBattleCount = 0;
    private string previousBGM;
    private string battleBGM = "Battle";

    private Coroutine switchBackCoroutine = null; // BGM ��ȯ �ڷ�ƾ�� ������ ����

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
    /// ������ �ð���ŭ ��ٸ� ��, ���� BGM���� ��ȯ�ϴ� �ڷ�ƾ
    /// </summary>
    private IEnumerator SwitchBackToPreviousBGMWithDelay()
    {
        yield return new WaitForSeconds(5f);

        // ��ٸ��� ���� ���ο� ������ ���۵��� �ʾҴٸ� BGM ��ȯ ����
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
