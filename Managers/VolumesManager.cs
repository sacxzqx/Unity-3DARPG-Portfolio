using UnityEngine;
using UnityEngine.Rendering;

public class VolumesManagers : MonoBehaviour
{
    public static VolumesManagers Instance { get; private set; }

    public Volume volume;
    public VolumeProfile normalProfile;
    public VolumeProfile skillProfile;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ActiveSkillEffect()
    {
        if (volume != null)
        {
            volume.profile = skillProfile;
            Debug.Log(Shader.Find("7_CVD_Achromatopsia 1"));

        }
    }

    public void DeactiveEffect()
    {
        if(volume != null)
        {
            volume.profile = normalProfile;
        }
    }
}
