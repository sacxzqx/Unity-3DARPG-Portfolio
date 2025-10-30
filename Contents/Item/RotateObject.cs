using UnityEngine;

/// <summary>
/// 아이템 오브젝트에 부착되어 회전 효과를 내는 클래스
/// </summary>
public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 50f;

    private void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
