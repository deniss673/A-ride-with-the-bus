using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SitAction", story: "[Self] sits on [Seat]", category: "Action", id: "befd7bf3641e5d217d9626643e6e1735")]
public partial class SitAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Seat;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Self == null || Seat == null)
            return Status.Failure;

        var script = Self.Value.GetComponent<NpcScript>();

        script.TakeSeat(Seat.Value.GetComponent<Seat>());


        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

