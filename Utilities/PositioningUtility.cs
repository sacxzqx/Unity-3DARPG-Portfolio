using UnityEngine;

public static class PositioningUtility
{
    /// <summary>
    /// 지정된 기준 위치 아래의 가장 가까운 땅을 찾아,
    /// 대상 오브젝트의 크기에 맞는 정확한 스폰 위치를 계산
    /// </summary>
    /// <param name="basePosition">위치를 찾을 기준점 (X, Z 좌표 사용)</param>
    /// <param name="objectCollider">스폰될 오브젝트의 Collider (높이 계산용)</param>
    /// <returns>땅에 발이 닿는 최종 스폰 위치</returns>
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
            Debug.LogWarning($"위치 {basePosition} 아래에서 땅을 찾지 못했습니다.");
        }

        if (objectCollider != null)
        {
            finalPosition.y += objectCollider.bounds.extents.y;
        }

        return finalPosition;
    }
}
