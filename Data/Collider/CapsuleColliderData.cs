using UnityEngine;

/// <summary>
/// 플레이어의 캡슐 콜라이더 정보를 캐싱하고 관리하는 유틸리티 클래스  
/// 지면 체크, 경사면 보정, 점프/낙하 중심 계산 등에 활용되는 중심 위치와 수직 extents 데이터를 제공
/// </summary>
public class CapsuleColliderData
{
    public CapsuleCollider Collider {  get; private set; }
    public Vector3 ColliderCenterInLocalSpace { get; private set; }
    public Vector3 ColliderVerticalExtents { get; private set; }

    public void Initialize(GameObject gameObject)
    {
        if (Collider != null)
        {
            return;
        }

        Collider = gameObject.GetComponent<CapsuleCollider>();

        UpdateColliderData();
    }

    /// <summary>
    /// 콜라이더의 중심 및 수직 Extents 정보를 갱신 
    /// 콜라이더가 움직이거나 스케일이 변경될 경우 호출 필요
    /// </summary>
    public void UpdateColliderData()
    {
        ColliderCenterInLocalSpace = Collider.center;

        ColliderVerticalExtents = new Vector3(0f, Collider.bounds.extents.y, 0f);
    }
}
