using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 내의 배경음(BGM)과 효과음(SFX)을 관리하는 싱글톤 오디오 매니저 클래스
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
        // 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmSource = bgmObject.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;
        bgmSource.loop = true;
        bgmSource.volume = BgmVolume;

        // 효과음 플레이어 초기화
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
    /// 지정된 이름의 BGM을 페이드 인/아웃과 함께 재생
    /// </summary>
    public void PlayBGM(string bgmName, float fadeDuration = 1f)
    {
        if (bgmSource.clip != null && bgmSource.clip.name == bgmName && bgmSource.isPlaying)
            return; // 이미 같은 곡이 재생 중이면 리턴

        StartCoroutine(FadeAndPlayBGM(bgmName));
    }

    private IEnumerator FadeAndPlayBGM(string bgmName)
    {
        float t = 0f;
        float startVolume = bgmSource.volume;

        // 1. 페이드 아웃
        // 현재 재생 중인 BGM의 볼륨을 0으로 줄임
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }
        bgmSource.Stop();

        // 2. 새로운 BGM 로드 및 캐싱
        // 파일 로드 및 오디오 클립을 메모리에 캐싱하는 로직
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
                Debug.LogError("BGM 파일을 찾을 수 없습니다: " + bgmName);
                yield break;
            }
        }

        bgmSource.clip = newClip;
        bgmSource.Play();

        // 3. 페이드 인
        // 새로운 BGM의 볼륨을 설정된 최대 볼륨까지 키움
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
                Debug.LogError("SFX 파일을 찾을 수 없습니다: " + sfxName);
            }
        }
    }

    /// <summary>
    /// 지정된 오디오 클립을 하나의 채널에서 재생
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