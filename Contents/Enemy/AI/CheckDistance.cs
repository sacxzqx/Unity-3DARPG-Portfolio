using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class CheckDistance : Conditional
{
    public SharedGameObject TargetGameObject;
    public SharedBool IsChasing;

    public float DistanceToCheck = 2f;
    public float DistanceToReturn = 10f;

    public override TaskStatus OnUpdate()
    {
        if (TargetGameObject == null || TargetGameObject.Value == null)
        {
            return TaskStatus.Failure;
        }

        float sqrDistance = (transform.position - TargetGameObject.Value.transform.position).sqrMagnitude;

        if (sqrDistance > DistanceToReturn)
        {
            IsChasing.Value = false;
            return TaskStatus.Failure;
        }

        if (sqrDistance >= DistanceToCheck)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}