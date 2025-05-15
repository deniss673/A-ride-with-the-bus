using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ChooseRandomSeat", story: "[Agent] choose random [seat] from all [seats]", category: "Action", id: "4c05f4ffd08edec1619b5c8872462020")]
public partial class ChooseRandomSeatAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Seat;
    [SerializeReference] public BlackboardVariable<List<GameObject>> Seats;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Seat.Value == null)
        {
            int choice = UnityEngine.Random.Range(0, Seats.Value.Count);
            Seat.Value = Seats.Value[choice];
            Debug.Log(Seat.Value);
            return Status.Success;
        }
        else
        {
            return Status.Success;
        }
    }

    protected override void OnEnd()
    {
    }
}

