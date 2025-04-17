using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/GetHit")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "GetHit", message: "GotHit", category: "Events", id: "6e4b0905e536db79eae4788545794871")]
public partial class GetHit : EventChannelBase
{
    public delegate void GetHitEventHandler();
    public event GetHitEventHandler Event; 

    public void SendEventMessage()
    {
        Event?.Invoke();
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        Event?.Invoke();
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        GetHitEventHandler del = () =>
        {
            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as GetHitEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as GetHitEventHandler;
    }
}

