using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 파티클 이펙트의 풀링 시스템을 담당하는 싱글톤 매니저 클래스
/// Resources 폴더에서 파티클을 로드하고 재사용하여 성능을 최적화
/// </summary>
public class ParticleManager : MonoBehaviour, IReset
{
    public static ParticleManager Instance { get; private set; }

    private Dictionary<string, List<ParticleSystem>> particlePools;

    private Dictionary<string, ParticleSystem> particlePrefabs;

    public int InitialSize = 5;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        particlePools = new Dictionary<string, List<ParticleSystem>>();
        particlePrefabs = new Dictionary<string, ParticleSystem>();
    }

    /// <summary>
    /// 지정된 이름의 파티클을 특정 위치에 재생. 없으면 풀을 생성하고 추가함
    /// </summary>
    /// <param name="particleName">파티클 이름 (Resources/Particles 하위)</param>
    /// <param name="position">월드 좌표</param>
    /// <param name="emitterTransform">기준이 되는 트랜스폼 (기본값: 자기 자신)</param>
    /// <param name="offsetPosition">기준 위치에서의 상대 위치</param>
    /// <param name="offsetRotation">기준 회전에서의 상대 회전</param>
    public void PlayParticle(
        string particleName,
        Vector3 position,
        Transform emitterTransform = null,
        Vector3? offsetPosition = null,
        Vector3? offsetRotation = null 
    )
    {
        if (emitterTransform == null)
        {
            emitterTransform = transform;
        }

        if (!particlePools.ContainsKey(particleName))
        {
            ParticleSystem loadedParticle = Resources.Load<ParticleSystem>("Particles/" + particleName);

            if (loadedParticle != null)
            {
                particlePrefabs[particleName] = loadedParticle;
                List<ParticleSystem> pools = new List<ParticleSystem>();
                for (int i = 0; i < InitialSize; i++)
                {
                    ParticleSystem newParticle = Instantiate(loadedParticle);
                    newParticle.gameObject.SetActive(false);
                    pools.Add(newParticle);
                }
                particlePools[particleName] = pools;
            }
            else
            {
                Debug.LogError("일치하는 파티클이 존재하지 않음 " + particleName);
                return;
            }
        }

        List<ParticleSystem> pool = particlePools[particleName];
        ParticleSystem particle = GetParticleFromPool(pool, particlePrefabs[particleName]);

        if (particle != null)
        {
            Vector3 finalPosition = position + emitterTransform.TransformDirection(offsetPosition ?? Vector3.zero);
            particle.transform.position = finalPosition;

            Quaternion finalRotation = emitterTransform.rotation * Quaternion.Euler(offsetRotation ?? Vector3.zero);
            particle.transform.rotation = finalRotation;

            StartCoroutine(PlayAndDisable(particle));
        }
    }

    private IEnumerator PlayAndDisable(ParticleSystem particle)
    {
        particle.gameObject.SetActive(true);
        particle.Play();

        yield return new WaitForSeconds(particle.main.duration);

        particle.gameObject.SetActive(false);
        particle.transform.position = Vector3.zero;
        particle.transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// 풀에서 비활성화된 파티클을 찾거나 새로 생성하여 반환
    /// </summary>
    private ParticleSystem GetParticleFromPool(List<ParticleSystem> pool, ParticleSystem prefab)
    {
        foreach (ParticleSystem particle in pool)
        {
            if (!particle.gameObject.activeInHierarchy)
            {
                return particle;
            }
        }

        ParticleSystem newParticle = Instantiate(prefab);
        newParticle.gameObject.SetActive(false);
        pool.Add(newParticle);
        return newParticle;
    }

    public void ResetBeforeSceneLoad()
    {
    }

    public void ResetAfterSceneLoad()
    {
        particlePools.Clear();
        particlePrefabs.Clear();
    }
}
