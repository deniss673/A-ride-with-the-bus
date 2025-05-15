using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/Sit")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "Sit", message: "sits on [seat]", category: "Events", id: "f4096af333779a3dcc70d2a909056368")]
public partial class Sit : EventChannelBase
{
    public delegate void SitEventHandler(GameObject seat);
    public event SitEventHandler Event; 

    public void SendEventMessage(GameObject seat)
    {
        Event?.Invoke(seat);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<GameObject> seatBlackboardVariable = messageData[0] as BlackboardVariable<GameObject>;
        var seat = seatBlackboardVariable != null ? seatBlackboardVariable.Value : default(GameObject);

        Event?.Invoke(seat);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        SitEventHandler del = (seat) =>
        {
            BlackboardVariable<GameObject> var0 = vars[0] as BlackboardVariable<GameObject>;
            if(var0 != null)
                var0.Value = seat;

            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as SitEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as SitEventHandler;
    }
}

