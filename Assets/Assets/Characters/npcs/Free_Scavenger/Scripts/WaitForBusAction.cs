using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaitForBus", story: "[Agent] waits fro bus", category: "Action", id: "a59c5886c9ea01d59756a96b30f61de5")]
public partial class WaitForBusAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Agent == null )
            return Status.Failure;

        var script = Agent.Value.GetComponent<NpcScript>();

        script.WaitForBus();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

