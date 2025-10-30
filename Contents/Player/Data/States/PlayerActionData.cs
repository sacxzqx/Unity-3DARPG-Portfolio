using System;
using UnityEngine;

[Serializable]
public class PlayerActionData
{
    [Header("KillMoveData")]
    public float KillMoveDetectionRange = 3f;
    public float KillMoveDetectionRadius = 1f;
    public float KillMoveRaycastHeight = 1.5f;

    [Header("DefenseData")]
    public float BlockKnockbackForce = 1.2f;

    [Header("무기 위치")]
    public WeaponTransformData WeaponTransformData;
}
