using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ���� ȭ���� ĸó�� Texture2D�� �����ϰų�,
/// �� ���� �� �ڵ����� BGM�� ����ϴ� �̱��� ScreenManager Ŭ����
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
    /// ���� �������� �������� ��ü ȭ���� ĸó�Ͽ� Texture2D�� ����
    /// </summary>
    IEnumerator CaptureScreen()
    {
        // �ؽ�ó �ڷ��� ������ ����
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        yield return new WaitForEndOfFrame();

        // ȭ���� �ȼ� �����͸� �о �ؽ�óȭ �ϴ� ����
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
