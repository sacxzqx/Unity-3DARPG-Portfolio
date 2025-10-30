using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 현재 화면을 캡처해 Texture2D로 저장하거나,
/// 씬 시작 시 자동으로 BGM을 재생하는 싱글톤 ScreenManager 클래스
/// </summary>
public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }

    public Texture2D ScreenTexture;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().isLoaded) 
        {
            AudioManager.Instance.PlayBGM(SceneManager.GetActiveScene().name);
        }
    }

    /// <summary>
    /// 현재 프레임을 기준으로 전체 화면을 캡처하여 Texture2D로 저장
    /// </summary>
    IEnumerator CaptureScreen()
    {
        // 텍스처 자료형 변수를 생성
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        yield return new WaitForEndOfFrame();

        // 화면의 픽셀 데이터를 읽어서 텍스처화 하는 과정
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
        texture.Apply();
        ScreenTexture = texture;
    }

    public void LoadScreenTexture()
    {
        StartCoroutine(CaptureScreen());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Loading") return;

        AudioManager.Instance.PlayBGM(scene.name);
    }
}
