using UnityEngine;

public static class PositioningUtility
{
    /// <summary>
    /// ������ ���� ��ġ �Ʒ��� ���� ����� ���� ã��,
    /// ��� ������Ʈ�� ũ�⿡ �´� ��Ȯ�� ���� ��ġ�� ���
    /// </summary>
    /// <param name="basePosition">��ġ�� ã�� ������ (X, Z ��ǥ ���)</param>
    /// <param name="objectCollider">������ ������Ʈ�� Collider (���� ����)</param>
    /// <returns>���� ���� ��� ���� ���� ��ġ</returns>
    public static Vector3 GetGroundedPosition(Vector3 basePosition, Collider objectCollider)
    {
        Vector3 finalPosition = basePosition;

        Vector3 rayStartPoint = new Vector3(basePosition.x, basePosition.y + 50f, basePosition.z);

        if (Physics.Raycast(rayStartPoint, Vector3.down, out RaycastHit hit, 100f))
        {
            finalPosition = hit.point;
        }
        else
        {
            Debug.LogWarning($"��ġ {basePosition} �Ʒ����� ���� ã�� ���߽��ϴ�.");
        }

        if (objectCollider != null)
        {
            finalPosition.y += objectCollider.bounds.extents.y;
        }

        return finalPosition;
    }
}
