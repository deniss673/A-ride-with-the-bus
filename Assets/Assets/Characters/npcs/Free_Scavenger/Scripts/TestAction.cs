using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TEST", story: "[Agent]", category: "Action", id: "4747614805f5c438a5f2666721c68588")]
public partial class TestAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    private bool hit;

    protected override Status OnStart()
    {
        hit = false;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerAttacks"))
        {
            hit = true;
        }
    }
}

