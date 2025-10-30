using Cinemachine;
using System.Collections;
using UnityEngine;

public class PostureBreakCameraEffect : MonoBehaviour
{
    [SerializeField] private float slowMotionTime = 1.2f;

    private CinemachineVirtualCamera virtualCam;
    private CinemachineFramingTransposer framingTransposer;

    private Coroutine zoomCoroutine;

    private void Awake()
    {
        virtualCam = GetComponent<CinemachineVirtualCamera>();
        framingTransposer = virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.EnemyEvents.OnPostureBreak += FocusOnEnemy;
        GameEventsManager.Instance.PlayerEvents.OnKillMoveStarted += StopCameraEffectByKillMove;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.EnemyEvents.OnPostureBreak -= FocusOnEnemy;
        GameEventsManager.Instance.PlayerEvents.OnKillMoveStarted -= StopCameraEffectByKillMove;
    }

    private IEnumerator ZoomAndSlowMotion()
    {
        float originalDistance = framingTransposer.m_CameraDistance;
        float targetDistance = Mathf.Clamp(originalDistance * 0.5f, 2f, 8f);
        float duration = 0.3f;

        yield return ZoomCamera(originalDistance, targetDistance, duration);

        Time.timeScale = 0.2f;
        yield return new WaitForSeconds(slowMotionTime * Time.timeScale);

        // 줌아웃 & 타임스케일 복구
        GameEventsManager.Instance.EnemyEvents.PostureRecovery();
        Time.timeScale = 1.0f;
    }

    private IEnumerator ZoomCamera(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            framingTransposer.m_CameraDistance = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
    }

    private void FocusOnEnemy(Enemy enemy)
    {
        zoomCoroutine = StartCoroutine(ZoomAndSlowMotion());
    }

    /// <summary>
    /// 킬무브가 실행될 시에 코루틴을 즉시 중지하고 타임스케일을 원래대로 되돌리는 함수
    /// </summary>
    private void StopCameraEffectByKillMove()
    {
        if (zoomCoroutine == null) return;
        StopCoroutine(zoomCoroutine);
        zoomCoroutine = null;

        Time.timeScale = 1.0f;
    }
}
