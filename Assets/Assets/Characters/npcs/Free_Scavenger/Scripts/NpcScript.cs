using Unity.Behavior;
using UnityEngine;

public class NpcScript : MonoBehaviour
{
    private GetHit EventChannel;
    private int _health;
    private BehaviorGraphAgent _graph;


    private void Start()
    {
        _graph = GetComponent<BehaviorGraphAgent>();
        _graph.BlackboardReference.GetVariableValue<int>("Health", out _health);
        _graph.BlackboardReference.GetVariableValue<GetHit>("GetHit",out EventChannel);
    }

    public void TakeDamage(int value)
    {
        if (_health > 0)
        {
            Debug.Log("HIT");
            _health -= value;
            _graph.BlackboardReference.SetVariableValue<int>("Health", _health);
            EventChannel.SendEventMessage();
        }
            
    }


}
