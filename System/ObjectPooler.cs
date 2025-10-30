using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 내 모든 종류의 오브젝트 풀링을 관리하는 범용 싱글톤 매니저
/// </summary>
public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance { get; private set; }

    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        Instance = this;
    }

    public void CreatePool(GameObject prefab, int initialSize)
    {
        string poolKey = prefab.name;

        if (poolDictionary.ContainsKey(poolKey)) return;

        Queue<GameObject> newPool = new Queue<GameObject>();
        for (int i = 0; i < initialSize; i++)
        {
            GameObject newObj = Instantiate(prefab, transform);
            newObj.SetActive(false);
            newPool.Enqueue(newObj);
        }
        poolDictionary.Add(poolKey, newPool);
    }

    public GameObject GetFromPool(GameObject prefab, Vector3 position, Quaternion rotation, bool isOpen = true)
    {
        string poolKey = prefab.name;

        if (!poolDictionary.ContainsKey(poolKey))
        {
            CreatePool(prefab, 5); // 기본 사이즈로 생성
        }

        if (poolDictionary[poolKey].Count == 0)
        {
            GameObject newObj = Instantiate(prefab, transform);
            poolDictionary[poolKey].Enqueue(newObj);
        }

        GameObject objectToSpawn = poolDictionary[poolKey].Dequeue();
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        
        if (isOpen) objectToSpawn.SetActive(true);

        return objectToSpawn;
    }

    public void ReturnToPool(GameObject objectToReturn)
    {
        string poolKey = objectToReturn.name.Replace("(Clone)", "").Trim();

        if (poolDictionary.ContainsKey(poolKey))
        {
            objectToReturn.SetActive(false);
            poolDictionary[poolKey].Enqueue(objectToReturn);
        }
        else
        {
            Debug.LogWarning($"풀 '{poolKey}'을(를) 찾을 수 없습니다. 오브젝트를 파괴합니다.");
            Destroy(objectToReturn);
        }
    }
}