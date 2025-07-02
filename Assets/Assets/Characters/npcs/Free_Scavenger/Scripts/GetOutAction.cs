using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GetOut", story: "[Agent] selects an exit [zone]", category: "Action", id: "2ff130aa6a47f80f27c34de265b13ae6")]
public partial class GetOutAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> Zone;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var script = Agent.Value.GetComponent<NpcScript>();
        if (script.GetExitPos() == Vector3.zero)
            return Status.Running;

        Zone.Value = script.GetExitPos();

        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

