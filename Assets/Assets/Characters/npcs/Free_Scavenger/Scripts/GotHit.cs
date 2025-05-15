using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/GotHit")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "GotHit", message: "[Agent] got hit with [number] damage", category: "Events", id: "11a43c07c3a4851a832ba3f811fcdd95")]
public partial class GotHit : EventChannelBase
{
    public delegate void GotHitEventHandler(GameObject Agent, int number);
    public event GotHitEventHandler Event; 

    public void SendEventMessage(GameObject Agent, int number)
    {
        Event?.Invoke(Agent, number);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<GameObject> AgentBlackboardVariable = messageData[0] as BlackboardVariable<GameObject>;
        var Agent = AgentBlackboardVariable != null ? AgentBlackboardVariable.Value : default(GameObject);

        BlackboardVariable<int> numberBlackboardVariable = messageData[1] as BlackboardVariable<int>;
        var number = numberBlackboardVariable != null ? numberBlackboardVariable.Value : default(int);

        Event?.Invoke(Agent, number);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        GotHitEventHandler del = (Agent, number) =>
        {
            BlackboardVariable<GameObject> var0 = vars[0] as BlackboardVariable<GameObject>;
            if(var0 != null)
                var0.Value = Agent;

            BlackboardVariable<int> var1 = vars[1] as BlackboardVariable<int>;
            if(var1 != null)
                var1.Value = number;

            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as GotHitEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as GotHitEventHandler;
    }
}

