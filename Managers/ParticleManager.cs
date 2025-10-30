using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// ��ƼŬ ����Ʈ�� Ǯ�� �ý����� ����ϴ� �̱��� �Ŵ��� Ŭ����
/// Resources �������� ��ƼŬ�� �ε��ϰ� �����Ͽ� ������ ����ȭ
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
    /// ������ �̸��� ��ƼŬ�� Ư�� ��ġ�� ���. ������ Ǯ�� �����ϰ� �߰���
    /// </summary>
    /// <param name="particleName">��ƼŬ �̸� (Resources/Particles ����)</param>
    /// <param name="position">���� ��ǥ</param>
    /// <param name="emitterTransform">������ �Ǵ� Ʈ������ (�⺻��: �ڱ� �ڽ�)</param>
    /// <param name="offsetPosition">���� ��ġ������ ��� ��ġ</param>
    /// <param name="offsetRotation">���� ȸ�������� ��� ȸ��</param>
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
                Debug.LogError("��ġ�ϴ� ��ƼŬ�� �������� ���� " + particleName);
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
    /// Ǯ���� ��Ȱ��ȭ�� ��ƼŬ�� ã�ų� ���� �����Ͽ� ��ȯ
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
