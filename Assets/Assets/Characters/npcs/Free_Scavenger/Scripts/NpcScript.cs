using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NpcScript : MonoBehaviour
{
    private GetHit EventChannel;
    private int _health;
    private BehaviorGraphAgent _graph;
    private Animator _anim;
    private int _chance;
    private GameObject _bus;
    private GameObject _door;
    private bool _goto = false;

    private void Start()
    {
        _graph = GetComponent<BehaviorGraphAgent>();
        _graph.BlackboardReference.GetVariableValue<int>("Health", out _health);
        _graph.BlackboardReference.GetVariableValue<GetHit>("GetHit",out EventChannel);
        _anim = GetComponent<Animator>();
        _chance = UnityEngine.Random.Range(0, 100);
        _bus = GameObject.Find("bus");
    }

    private void Update()
    {
        if (_goto)
        {
            GoTo();
        }
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

    public void TakeASeat(GameObject obj)
    {
        var boxCollider = obj.GetComponentInChildren<BoxCollider>();
        var center = boxCollider.transform.TransformPoint(boxCollider.center);

        transform.position = center + obj.transform.right;


        transform.eulerAngles = obj.transform.eulerAngles;
        Debug.Log(obj.transform.eulerAngles);
        _anim.SetTrigger("sit");
    }



    public void WaitForBus()
    {
        if (_chance > 50)
            return;

        
        var street = _bus.GetComponent<BusManager>().GetBusStation();

        if (!street == transform.parent)
        {
            return;
        }
        else
        {
            GetInBus();
        }
    }
    
    void GetInBus()
    {
        var doors = _bus.GetComponent<BusManager>().GetDoors();

        var doorIndex = UnityEngine.Random.Range(0, doors.Count);

        _door = doors[doorIndex];
        _goto = true;

    }

    void GoTo()
    {
        if (_door == null)
            return;
        var agent = GetComponent<NavMeshAgent>();
        var pos = _door.transform.position;
        agent.SetDestination(pos + new Vector3(20,0,10));
    }

}
