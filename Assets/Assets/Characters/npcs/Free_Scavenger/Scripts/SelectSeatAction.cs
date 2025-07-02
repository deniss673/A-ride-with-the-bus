using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SelectSeat", story: "[Agent] selects [Seat]", category: "Action", id: "634614235123e7cb1ee13f95de81d71c")]
public partial class SelectSeatAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Seat;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Agent == null || Seat == null)
            return Status.Failure;

        var script = Agent.Value.GetComponent<NpcScript>();

        Seat.Value = script.SelectSeat();


        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

