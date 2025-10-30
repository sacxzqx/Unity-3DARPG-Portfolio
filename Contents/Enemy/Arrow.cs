using UnityEngine;
using System.Collections;

/// <summary>
/// ȭ�� �߻�ü�� �ൿ�� �����ϴ� Ŭ����
/// Ȱ��ȭ�Ǹ� ������ �ӵ��� �����ϸ�, �浹 �� �Ǵ� ���� �ð� �� ObjectPooler�� �ڵ����� �ݳ�
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Arrow : MonoBehaviour, IDamageProvider
{
    [Header("Stats")]
    [SerializeField] private float speed = 40f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifeTime = 5f;

    private int currentDamage;

    private Rigidbody rb;
    private Coroutine returnCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if (returnCoroutine != null) StopCoroutine(returnCoroutine);
        returnCoroutine = StartCoroutine(ReturnToPoolAfterTime(lifeTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ReturnToPoolDelayed());
        }
    }

    private IEnumerator ReturnToPoolDelayed()
    {
        yield return new WaitForEndOfFrame();

        ReturnToPool();
    }

    public void Fire(Transform target, int attackPower)
    {
        currentDamage = attackPower;

        if (target == null)
        {
            rb.velocity = transform.forward * speed;
            return;
        }
        Vector3 targetCenter = target.position + Vector3.up * 0.9f;
        Vector3 direction = targetCenter - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction.normalized);
        rb.velocity = transform.forward * speed;
    }

    private IEnumerator ReturnToPoolAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        ObjectPooler.Instance.ReturnToPool(this.gameObject);
    }

    public int GetDamageAmount()
    {
        return currentDamage;
    }
}