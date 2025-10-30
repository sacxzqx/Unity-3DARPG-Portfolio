using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� �� ��� ������ ������Ʈ Ǯ���� �����ϴ� ���� �̱��� �Ŵ���
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
            CreatePool(prefab, 5); // �⺻ ������� ����
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
            Debug.LogWarning($"Ǯ '{poolKey}'��(��) ã�� �� �����ϴ�. ������Ʈ�� �ı��մϴ�.");
            Destroy(objectToReturn);
        }
    }
}