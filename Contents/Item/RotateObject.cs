using UnityEngine;

/// <summary>
/// ������ ������Ʈ�� �����Ǿ� ȸ�� ȿ���� ���� Ŭ����
/// </summary>
public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 50f;

    private void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
