using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GetOfBuss", story: "[Self] leaves bus", category: "Action", id: "1091b4772939348d69bfcf1d791028e2")]
public partial class GetOfBussAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var script = Self.Value.GetComponent<NpcScript>();
        if (!script.GetOutOfBus())
            return Status.Running;

        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

