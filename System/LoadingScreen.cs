using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 로딩 씬에서 비동기 씬 로드를 수행하고,
/// 로딩 진행 바와 아이콘을 업데이트하며, 완료 후 키 입력 시 게임 활성화 및 전환을 수행하는 클래스
/// </summary>
public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private Image loadingIcon;

    private bool loadComplete = false;
    private AsyncOperation asyncLoad;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeInDuration = 1.0f;
    private bool textOnBoard = false;

    public static Vector3? PendingPlayerPosition = null;

    private static string sceneToLoad;

    private void Start()
    {
        GameEventsManager.Instance.InputEvents.SetPlayerInput(false);
        GameManager.Instance.DeActiveGameUI();

        StartCoroutine(LoadAsyncScene(sceneToLoad));
    }

    private void Update()
    {
        if(loadComplete && !textOnBoard)
        {
            StartCoroutine(FadeInRoutine());
            textOnBoard = true;
        }

        if(loadComplete && Input.anyKeyDown)
        {
            StartCoroutine(ActivateGameAndCleanInput());
        }
    }

    /// <summary>
    /// 외부에서 호출되어 지정된 씬을 로딩 씬을 통해 비동기 로드하는 정적 메서드
    /// </summary>
    public static void LoadScene(string sceneName, Vector3? position = null)
    {
        if (position != null)
        {
            PendingPlayerPosition = position;
        }
        sceneToLoad = sceneName;
        SceneManager.LoadScene("Loading");
    }

    /// <summary>
    /// 지정된 씬을 비동기 방식으로 로드하고 진행 상태를 업데이트
    /// </summary>
    IEnumerator LoadAsyncScene(string sceneName)
    {
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            progressBar.value = progress;
            loadingIcon.fillAmount = progress;

            if(asyncLoad.progress >= 0.9f)
            {
                loadComplete = true;
            }

            yield return null;
        }
    }

    IEnumerator FadeInRoutine()
    {
        float currentTime = 0f;

        while (currentTime < fadeInDuration)
        {
            currentTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, currentTime / fadeInDuration);
            yield return null;
        }
    }

    /// <summary>
    /// 씬 활성화 및 입력 버퍼를 제거하는 코루틴
    /// </summary>
    IEnumerator ActivateGameAndCleanInput()
    {
        GameEventsManager.Instance.InputEvents.SetPlayerInput(true);

        GameManager.Instance.ActivateGame();
        asyncLoad.allowSceneActivation = true;

        yield return null;
    }
}