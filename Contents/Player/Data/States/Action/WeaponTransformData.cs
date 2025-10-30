using System;
using UnityEngine;

[Serializable]
public class WeaponTransformData
{
    [Header("WeaponDrawnData")]
    [field: SerializeField] public Vector3 DrawnPosition;
    [field: SerializeField] public Vector3 DrawnRotation;

    [Header("WeaponSheathedData")]
    [field: SerializeField] public Vector3 SheathedPosition;
    [field: SerializeField] public Vector3 SheathedRotation;
}
