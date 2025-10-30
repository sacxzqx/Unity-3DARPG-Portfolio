using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ���� �����(BGM)�� ȿ����(SFX)�� �����ϴ� �̱��� ����� �Ŵ��� Ŭ����
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("#BGM")]
    public float BgmVolume;
    AudioSource bgmSource;
    float fadeDuration = 1.5f;

    [Header("#SFX")]
    public float SfxVolume = 1.0f;
    AudioSource[] sfxSource;
    public int Channels = 10;

    private Dictionary<string, AudioClip> audioClips;
    private int channelIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        Initialize();
    }

    void Initialize()
    {
        // ����� �÷��̾� �ʱ�ȭ
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmSource = bgmObject.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;
        bgmSource.loop = true;
        bgmSource.volume = BgmVolume;

        // ȿ���� �÷��̾� �ʱ�ȭ
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxSource = new AudioSource[Channels];

        for (int i = 0; i < sfxSource.Length; i++)
        {
            sfxSource[i] = sfxObject.AddComponent<AudioSource>();
            sfxSource[i].playOnAwake = false;
            sfxSource[i].volume = SfxVolume;
        }

        audioClips = new Dictionary<string, AudioClip>();
    }

    /// <summary>
    /// ������ �̸��� BGM�� ���̵� ��/�ƿ��� �Բ� ���
    /// </summary>
    public void PlayBGM(string bgmName, float fadeDuration = 1f)
    {
        if (bgmSource.clip != null && bgmSource.clip.name == bgmName && bgmSource.isPlaying)
            return; // �̹� ���� ���� ��� ���̸� ����

        StartCoroutine(FadeAndPlayBGM(bgmName));
    }

    private IEnumerator FadeAndPlayBGM(string bgmName)
    {
        float t = 0f;
        float startVolume = bgmSource.volume;

        // 1. ���̵� �ƿ�
        // ���� ��� ���� BGM�� ������ 0���� ����
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }
        bgmSource.Stop();

        // 2. ���ο� BGM �ε� �� ĳ��
        // ���� �ε� �� ����� Ŭ���� �޸𸮿� ĳ���ϴ� ����
        AudioClip newClip;
        if (!audioClips.TryGetValue(bgmName, out newClip))
        {
            newClip = Resources.Load<AudioClip>("Audio/" + bgmName);
            if (newClip != null)
            {
                audioClips[bgmName] = newClip;
            }
            else
            {
                Debug.LogError("BGM ������ ã�� �� �����ϴ�: " + bgmName);
                yield break;
            }
        }

        bgmSource.clip = newClip;
        bgmSource.Play();

        // 3. ���̵� ��
        // ���ο� BGM�� ������ ������ �ִ� �������� Ű��
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0, BgmVolume, t / fadeDuration);
            yield return null;
        }
        bgmSource.volume = BgmVolume;
    }

    public void PlaySFX(string sfxName)
    {
        if (audioClips.TryGetValue(sfxName, out AudioClip clip))
        {
            PlaySFXClip(clip);
        }
        else
        {
            AudioClip loadedClip = Resources.Load<AudioClip>("Audio/" + sfxName);
            if (loadedClip != null)
            {
                audioClips[sfxName] = loadedClip;
                PlaySFXClip(loadedClip);
            }
            else
            {
                Debug.LogError("SFX ������ ã�� �� �����ϴ�: " + sfxName);
            }
        }
    }

    /// <summary>
    /// ������ ����� Ŭ���� �ϳ��� ä�ο��� ���
    /// </summary>
    private void PlaySFXClip(AudioClip clip)
    {
        int loopIndex = channelIndex % Channels;

        if (!sfxSource[loopIndex].isPlaying)
        {
            sfxSource[loopIndex].clip = clip;
            sfxSource[loopIndex].Play();
            channelIndex++;
        }
    }

    public void SetBGMVolume(float volume)
    {
        BgmVolume = volume;
        bgmSource.volume = BgmVolume;
        PlayerPrefs.SetFloat("BGMVolume", BgmVolume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        SfxVolume = volume;
        foreach (var source in sfxSource)
        {
            source.volume = SfxVolume;
        }
        PlayerPrefs.SetFloat("SFXVolume", SfxVolume);
        PlayerPrefs.Save();
    }

    public void StopSFX(string sfxName)
    {
        foreach (var source in sfxSource)
        {
            if (source.clip != null && source.clip.name == sfxName && source.isPlaying)
            {
                source.Stop();
                break;
            }
        }
    }

    public string GetCurrentBGMName()
    {
        if (bgmSource != null && bgmSource.isPlaying && bgmSource.clip != null)
        {
            return bgmSource.clip.name;
        }

        return string.Empty;
    }

    public void LoadVolumeSetting()
    {
        BgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1.0f);
        SfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
    }
}