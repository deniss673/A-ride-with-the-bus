using System.Collections.Generic;
using System.Linq;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

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
    private SplineContainer _street;
    private float _t;
    private bool _faceDirection;
    private float _movementSpeed = 3;
    private float _randomOffset;
    private bool _right;
    private bool _isWaiting = false;
    private bool _isAtDoor = false;
    private Vector3 _originalPos = Vector3.zero;
    private bool _isSitting = false;
    private int _exit = 1;
    private int _currentStreet = 0;

    public bool sit = false;
    public bool test = false;
    public bool up = false;

    private List<Seat> _seats;
    private Seat _seat = null;
    private Vector3 _exitPos = Vector3.zero;
    private bool _gettingOut = false;
    private bool _wasPassive = true;
    private bool _isTalking = false;

    public bool debug = false;


    public int item1;
    public int item2;
    public int item3;
    public int item4;
    public int item5;
    public int item6;

    private void Start()
    {
        _graph = GetComponent<BehaviorGraphAgent>();
        _graph.BlackboardReference.GetVariableValue<int>("Health", out _health);
        _graph.BlackboardReference.GetVariableValue<GetHit>("GetHit",out EventChannel);
        _anim = GetComponent<Animator>();
        _chance = UnityEngine.Random.Range(0, 100);
        _randomOffset = Random.Range(20, 30);
        _right = Random.Range(0, 100) > 50;
        _faceDirection = Random.Range(0, 100) > 50;
        _bus = GameObject.Find("bus");
        _exit = Random.Range(1, 10);


        item1 = Random.Range(1, 3);
        item2 = Random.Range(1, 3);
        item3 = Random.Range(1, 3);
        item4 = Random.Range(1, 3);
        item5 = Random.Range(1, 3);
        item6 = Random.Range(1, 3);

        Spawn();
        
    }

    public bool IsWalking()
    {
        return !_isWaiting && !_isTalking;
    }

    public void SetTalking()
    {
        _isTalking = true;
        _anim.SetTrigger("talk");
    }

    private bool CheckDistance()
    {
        var allNPCs = GameObject.FindGameObjectsWithTag("NPC");
        foreach (var npc in allNPCs)
        {
            if (npc == this.gameObject) continue;

            if (!npc.GetComponent<NpcScript>().IsWalking())
            {
                continue;
            }

            if (Vector3.Distance(transform.position, npc.transform.position) <= 1)
            {
                Vector3 lookDirection = npc.transform.position - transform.position;
                npc.GetComponent<NpcScript>().SetTalking();
                lookDirection.y = 0; 
                if (lookDirection != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(lookDirection);
                return true;
            }
                
        }
        return false;
    }

    private void Talk()
    {
        if (CheckDistance())
        {
            SetTalking();
        }


    }


    private void Update()
    {
        ReadyForTrade();
        if (debug)
        {
            debug = false;
            Debug2();
        }

        if (_isSitting)
        {
            CheckBusStop();
        }

        if (sit)
        {
            sit = false;
            //TakeSeat();
        }
        if (_gettingOut)
        {
            GetOut();
        }
        if (up)
        {
            up = false;
            SitUp();
        }
        if (test)
        {

        }
        else
        {
            if (_goto)
            {
                GoTo();
            }
            if (!_isWaiting && !_isTalking)
            {
                Walk();
            }
            else if (_isTalking)
            {
                
            }
            else
            {
                WaitForBus();
            }
            
        }


    }


    void Spawn()
    {
        if (_chance > 30)
        {
            SpawnToWalk();
        }
        else
        {
            SpawnToWaitForBus();
        }
    }
    
    void SpawnToWaitForBus()
    {
        _isWaiting = true;

        var busStop = transform.parent.GetComponent<BusStopManager>();

        var spawnPosition = busStop.GetRandomPositionInBusStop();


        transform.localPosition = spawnPosition;

        
    }

    public void TakeDamage(int value)
    {
        if (_health > 0)
        {
            Debug.Log("HIT");
            _health -= value;

            if (_isSitting)
            {
                _isSitting = false;
                GetComponent<BehaviorGraphAgent>().enabled = true;
                GetComponent<NavMeshAgent>().enabled = true;
                
            }
            GetComponent<BehaviorGraphAgent>().Restart();
            _graph.BlackboardReference.SetVariableValue<int>("Health", _health);
            EventChannel.SendEventMessage();


            if (_health <= 0)
            {
                GameObject.FindFirstObjectByType<ItemManager>().DropItem();
            }

        }
    }



    public bool GotBook()
    {
        if (!GetComponent<BehaviorGraphAgent>().enabled)
            return false;
        var blackboard =
            GetComponent<BehaviorGraphAgent>().BlackboardReference;
        blackboard.GetVariableValue("Passive", out bool passive);
        if (!passive)
        {
            blackboard.SetVariableValue("Passive", true);
            GetComponent<BehaviorGraphAgent>().Restart();
            return true;
        }
        return false;
    }

    public void SetPassiveAfterBook()
    {
        var blackboard =
        GetComponent<BehaviorGraphAgent>().BlackboardReference;
        blackboard.SetVariableValue("Passive", false);
        GetComponent<BehaviorGraphAgent>().Restart();
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
        var street = _bus.GetComponent<BusManager>().GetBusStation();

        if (street == null)
            return;
        if (street.transform != transform.parent)
        {
            return;
        }
        else if(_door == null)
        {
            GetDoorPosition();
        }
    }
    
    void GetDoorPosition()
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

        var pos = _door.transform.position - new Vector3(0, 0, 2);
        if (_isAtDoor)
        {
            var got = GetInBus();
            if (got)
            {
                _anim.SetFloat("SpeedMagnitude", 0, 0.15f, Time.deltaTime);
                _goto = false;
                _currentStreet = GameObject.FindAnyObjectByType<BusManager>().GetBusStationNumber();
                AddComponents();
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, pos, _movementSpeed * Time.deltaTime);
            _anim.SetFloat("SpeedMagnitude", _movementSpeed, 0.15f, Time.deltaTime);
            if (Vector3.Distance(transform.position, pos) < 0.1f)
            {
                _isAtDoor = true;
            }
        }
    }

    bool GetInBus()
    {
        var pos = _door.transform.position + new Vector3(0, 0, 1);

        transform.position = Vector3.MoveTowards(transform.position, pos, _movementSpeed * Time.deltaTime);
        _anim.SetFloat("SpeedMagnitude", _movementSpeed, 0.15f, Time.deltaTime);

        if(Vector3.Distance(transform.position, pos) < 0.1f)
        {
            return true;
        }
        return false;

    }

    void AddComponents()
    {
        gameObject.GetComponent<BehaviorGraphAgent>().enabled = true;
        gameObject.GetComponent<NavMeshAgent>().enabled = true;
        _exit = Random.Range(3, 5);
        var bus = GameObject.Find("bus");
        transform.parent = bus.transform;
    }

    public void SpawnToWalk(bool wasInBus=false)
    {
        _street = transform.parent.GetComponent<SplineContainer>();
        var spline = _street.Spline;

        float maxDistance = spline.GetLength();
        
        float random = UnityEngine.Random.Range(0, maxDistance);
        var t = random / maxDistance;
        if (wasInBus)
        {
            SplineUtility.GetNearestPoint(spline, transform.position, out _, out random);
            _right = true;
            t = random;
        }
        else
        {
            _right = Random.Range(0, 100) > 50;
        }

        spline.Evaluate(t, out var pos, out var tan, out var up);
        Vector3 tangent = tan;

        tangent = transform.InverseTransformDirection(tangent);
        tangent = tangent.normalized * (_faceDirection ? 1 : -1);
        Vector3 normal = Vector3.Cross(Vector3.up, tangent).normalized;
        Vector3 lateralOffset = normal * _randomOffset * (_right ? 1f : -1f);


        Vector3 targetPosition = (Vector3)pos + lateralOffset;
        targetPosition.y += 50;
        _t = t*maxDistance;
        if(!wasInBus)
            transform.localPosition = targetPosition;
        
        transform.forward = ((Vector3)tangent).normalized * (_faceDirection ? 1f : -1f);
    }


    private Vector3 target;
    public void Walk()
    {
        var spline = _street.Spline;

        _t += _movementSpeed/3 * Time.deltaTime * (_faceDirection ? 1 : -1);
        _t = Mathf.Clamp(_t, 0, spline.GetLength());

        if(_t == spline.GetLength() || _t == 0)
        {
            _faceDirection = !_faceDirection;
            return;
        }

        spline.Evaluate(_t / spline.GetLength(), out var position, out var tan, out var up);

        Vector3 tangent = tan;
        tangent = tangent.normalized * (_faceDirection ? 1 : -1);

        //tangent = transform.InverseTransformDirection(tangent);
        Vector3 normal = Vector3.Cross(transform.up, tangent).normalized;
        Vector3 lateralOffset = normal * _randomOffset * (_right ? 1f : -1f);




        Vector3 targetPosition = (Vector3)position + lateralOffset;


        if (IsObstacleAhead(out Vector3 avoidanceDirection))
        {
            float avoidanceOffset = 3.0f;
            targetPosition += avoidanceDirection * avoidanceOffset;
        }


        transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, _movementSpeed * Time.deltaTime);
        Quaternion targetRotation = Quaternion.LookRotation(tangent, transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);


        _anim.SetFloat("SpeedMagnitude", _movementSpeed, 0.15f, Time.deltaTime);
        Talk();
    }
    private bool IsObstacleAhead(out Vector3 avoidanceDirection)
    {
        Vector3 forward = transform.forward;
        Vector3 origin = transform.position;
        float obstacleDetectionDistance = 5f;

        if (Physics.Raycast(origin, forward, out RaycastHit hit, obstacleDetectionDistance))
        {
            Vector3 right = transform.right;
            if (!Physics.Raycast(origin, right, obstacleDetectionDistance))
            {
                avoidanceDirection = right;
                return true;
            }

            Vector3 left = -transform.right;
            if (!Physics.Raycast(origin, left, obstacleDetectionDistance))
            {
                avoidanceDirection = left;
                return true;
            }

            avoidanceDirection = Vector3.zero;
            return false;
        }

        avoidanceDirection = Vector3.zero;
        return false;
    }

    
    public GameObject SelectSeat()
    {
        _seats = GameObject.FindObjectsByType<Seat>(FindObjectsSortMode.None).Where(m => !m.GetOccupied()).ToList();

        if (_seats.Count != 0)
        {
            _seat = _seats[Random.Range(0, _seats.Count)];

            _seat.SetOccupied();

        }



        return _seat.gameObject;

    }

    public void TakeSeat(Seat seat)
    {
        _isSitting = true;
        var pos = seat.GetCenter();
        //pos = transform.parent.InverseTransformPoint(pos);

        _originalPos = transform.position;
        var originalPos = transform.position;
        //pos.y = originalPos.y;
        pos.y -= 0.30f;

        var navmesh = GetComponent<NavMeshAgent>();
        navmesh.enabled = false;
        transform.position = pos;

        var rotation = transform.eulerAngles;
        rotation.y = seat.RotationAngle();
        transform.eulerAngles = rotation;

        Debug.Log(rotation.y);

        _anim.SetTrigger("sit");
        
    }


    private void SitUp()
    {
        if (_seat == null) {
            return;
        }
        _isSitting = false;
        _seat.SetOccupied();
        _anim.SetTrigger("sitUp");



    }

    public void SitUpOnAnimationEnd()
    {
        transform.position = _originalPos;
        var agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;

        var exitZones = GameObject.FindObjectsByType<ExitZones>(FindObjectsSortMode.None);

        var exit = exitZones[Random.Range(0, exitZones.Count())];

        _exitPos = exit.GetRandomPositionInExitZone();
    }

    private void CheckBusStop()
    {
        var busStationNumber = GameObject.FindAnyObjectByType<BusManager>().GetBusStationNumber();
        if(busStationNumber != _currentStreet)
        {
            _exit--;
            _currentStreet++;
        }
        if (_exit == 0)
        {
            SitUp();
        }
    }


    public Vector3 GetExitPos()
    {
        return _exitPos;
    }
    private Vector3 _pos;
    public bool GetOutOfBus()
    {
        var street = _bus.GetComponent<BusManager>().GetBusStation();

        if(street == null)
        {
            return false;
        }

        _gettingOut = true;
        _pos = transform.position - new Vector3(0, 0, (Random.Range(10, 15)));
        GetComponent<BehaviorGraphAgent>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
        return true;
    }


    private void GetOut()
    {
        var street = _bus.GetComponent<BusManager>().GetBusStation();

        

        transform.parent = null;

        if (Vector3.Distance(transform.position, _pos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _pos, _movementSpeed * Time.deltaTime);
            _anim.SetFloat("SpeedMagnitude", _movementSpeed, 0.15f, Time.deltaTime);
            return;
        }

        if (street == null)
            Destroy(this);
        transform.parent = street.transform;


        _isWaiting = false;
        SpawnToWalk(true);
        _gettingOut = false;
    }



    public bool GetPasive()
    {
        if (tag != "NPC")
        {
            return false;
        }
        return true;
    }


    public bool GetPassiveGraph()
    {
        var blackboard =
        GetComponent<BehaviorGraphAgent>().BlackboardReference;
        blackboard.GetVariableValue("Passive", out bool passive);
        return passive;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private bool playerInRange = false;

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void ReadyForTrade()
    {
        if (!GetPassiveGraph())
        {
            return;
        }
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            var obj = Instantiate(Resources.Load<GameObject>("Trade"));
            obj.GetComponent<Trade>().Create(item1, item2, item3, item4, item5, item6);
            Time.timeScale = 0;
        }
    }

    private void Debug2()
    {
        Debug.Log(_right);
        Debug.Log(_exit);
    }
}
