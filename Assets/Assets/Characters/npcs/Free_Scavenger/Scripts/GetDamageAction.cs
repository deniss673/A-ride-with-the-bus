using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GetDamage", story: "[Agent] [health] takes [damage]", category: "Action", id: "ca61dca7091a5e09a1442d12356b3c6d")]
public partial class GetDamageAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<int> Health;
    [SerializeReference] public BlackboardVariable<int> Damage;
    protected override Status OnStart()
    {
        Health -= Damage;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

