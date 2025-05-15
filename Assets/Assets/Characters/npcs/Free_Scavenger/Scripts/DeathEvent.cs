using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/DeathEvent")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "DeathEvent", message: "[Agent] dies", category: "Events", id: "28289d85206776b5952fd06391e44637")]
public partial class DeathEvent : EventChannelBase
{
    public delegate void DeathEventEventHandler(GameObject Agent);
    public event DeathEventEventHandler Event; 

    public void SendEventMessage(GameObject Agent)
    {
        Event?.Invoke(Agent);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<GameObject> AgentBlackboardVariable = messageData[0] as BlackboardVariable<GameObject>;
        var Agent = AgentBlackboardVariable != null ? AgentBlackboardVariable.Value : default(GameObject);

        Event?.Invoke(Agent);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        DeathEventEventHandler del = (Agent) =>
        {
            BlackboardVariable<GameObject> var0 = vars[0] as BlackboardVariable<GameObject>;
            if(var0 != null)
                var0.Value = Agent;

            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as DeathEventEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as DeathEventEventHandler;
    }
}

