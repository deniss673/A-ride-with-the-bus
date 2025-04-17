using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wait For Combat", story: "[Agent] waits until it gets attacked by Player", category: "Action/Physics", id: "89498e7c09fae45303665bd4c230452c")]
public partial class WaitForCombatAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    private Collider _collider;
    protected override Status OnStart()
    {
        _collider = Agent.Value.GetComponent<Collider>();
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

