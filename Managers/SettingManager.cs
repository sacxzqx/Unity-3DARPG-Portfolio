using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 환경 설정을 관리하는 클래스
/// BGM 및 SFX 볼륨 슬라이더를 AudioManager와 연동하여 실시간 조정 및 저장 가능
/// </summary>
public class SettingManager : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            bgmSlider.value = AudioManager.Instance.BgmVolume;
            sfxSlider.value = AudioManager.Instance.SfxVolume;
        }

        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        InitializeResolutionOptions();
    }

    private void InitializeResolutionOptions()
    {
        resolutions = Screen.resolutions
            .Select(res => new Resolution { width = res.width, height = res.height })
            .Distinct(new ResolutionComparer())
        .ToArray();
        resolutionDropdown.ClearOptions();

        var options = resolutions.Select(r => $"{r.width} x {r.height}").ToList();
        resolutionDropdown.AddOptions(options);

        int savedIndex = FindSavedResolutionIndex();
        resolutionDropdown.value = savedIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    public void SetBGMVolume(float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetBGMVolume(volume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(volume);
        }
    }

    private int FindSavedResolutionIndex()
    {
        int savedWidth = PlayerPrefs.GetInt("ScreenWidth", Screen.currentResolution.width);
        int savedHeight = PlayerPrefs.GetInt("ScreenHeight", Screen.currentResolution.height);

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == savedWidth && resolutions[i].height == savedHeight)
                return i;
        }

        return 0;
    }

    public void SetResolution(int index)
    {
        var resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt("ScreenWidth", resolution.width);
        PlayerPrefs.SetInt("ScreenHeight", resolution.height);
        PlayerPrefs.Save();
    }
}
