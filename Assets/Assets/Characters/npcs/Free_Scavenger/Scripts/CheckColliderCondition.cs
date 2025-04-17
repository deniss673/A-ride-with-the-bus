using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckCollider", story: "Check if [collider] its from player hits", category: "Conditions", id: "0b778f61238271980a84ba9383b4dd06")]
public partial class CheckColliderCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Collider;

    public override bool IsTrue()
    {
        if (Collider == null)
        {
            return false;
        }
        if(Collider.Value.CompareTag("PlayerAttacks"))
            return true;
        return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
