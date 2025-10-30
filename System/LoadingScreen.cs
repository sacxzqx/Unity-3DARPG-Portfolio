using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// �ε� ������ �񵿱� �� �ε带 �����ϰ�,
/// �ε� ���� �ٿ� �������� ������Ʈ�ϸ�, �Ϸ� �� Ű �Է� �� ���� Ȱ��ȭ �� ��ȯ�� �����ϴ� Ŭ����
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
    /// �ܺο��� ȣ��Ǿ� ������ ���� �ε� ���� ���� �񵿱� �ε��ϴ� ���� �޼���
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
    /// ������ ���� �񵿱� ������� �ε��ϰ� ���� ���¸� ������Ʈ
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
    /// �� Ȱ��ȭ �� �Է� ���۸� �����ϴ� �ڷ�ƾ
    /// </summary>
    IEnumerator ActivateGameAndCleanInput()
    {
        GameEventsManager.Instance.InputEvents.SetPlayerInput(true);

        GameManager.Instance.ActivateGame();
        asyncLoad.allowSceneActivation = true;

        yield return null;
    }
}