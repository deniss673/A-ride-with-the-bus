using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Threading.Tasks;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SitAction", story: "[Agent] sits on [seat]", category: "Action", id: "9a77e18807b8ebb3661f16dbeebb1722")]
public partial class SitAction : Action
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

        script.TakeASeat(Seat);


        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

