using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetPassive", story: "[Self] sets [pasive] check last [bool]", category: "Action", id: "80dd88a63f7542389349bc14402076fc")]
public partial class SetPassiveAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<bool> Pasive;
    [SerializeReference] public BlackboardVariable<bool> Bool;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var script = Self.Value.GetComponent<NpcScript>();

        if (Pasive.Value && !Bool.Value)
        {
            Pasive.Value = script.GetPasive();
            Bool.Value = true;

        }


        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

