using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using static Unity.Burst.Intrinsics.X86.Avx;

#region Door
public class Door
{
    Vector3 closedPosition;
    GameObject door;
    bool rightSide;
    bool opened;

    Vector3 _openedFront;
    Vector3 _openedRight;


    public Door(GameObject doorGameObj, bool right)
    {
        door = doorGameObj;
        rightSide = right;
        opened = false;
        closedPosition = door.transform.localPosition;

        _openedFront = closedPosition - new Vector3(0, 3, 0);
        _openedRight = _openedFront + new Vector3(28, 0, 0) * (rightSide == true ? -1 : 1);

    }

    public void OpenDoor()
    {
        if (door.transform.localPosition.y != _openedFront.y)
            FrontMovement(true);

        if (door.transform.localPosition.y != _openedFront.y)
            return;

        if (door.transform.localPosition.x != _openedRight.x)
            RightMovement(true);

        if (door.transform.localPosition.x != _openedRight.x)
            return;

        opened = true;
    }

    public void CloseDoor()
    {
        if (door.transform.localPosition.x != closedPosition.x)
            RightMovement(false);

        if (door.transform.localPosition.x != closedPosition.x)
            return;

        if (door.transform.localPosition.y != closedPosition.y)
            FrontMovement(false);

        if (door.transform.localPosition.y != closedPosition.y)
            return;



        opened = false;
    }

    public bool IsOpened()
    {
        return opened;
    }


    void FrontMovement(bool open)
    {
        door.transform.localPosition = Vector3.MoveTowards(door.transform.localPosition, open ? _openedFront : closedPosition, Time.deltaTime * 2);
    }

    void RightMovement(bool open)
    {
        door.transform.localPosition = Vector3.MoveTowards(door.transform.localPosition, open ? _openedRight : _openedFront, Time.deltaTime * 7);
    }

    public GameObject GetDoorGameObject()
    {
        return door;
    }


}
#endregion
public class BusManager : MonoBehaviour
{

    public bool open = false;
    public bool close = false;

    List<Door> _doors = new List<Door>();
    List<BusStopManager> _busStops = new List<BusStopManager>();
    GameObject _currentStreet;
    BusStop _nextStop=null;
    SplineContainer _spline;
    StreetManager _streetManager;
    bool _gotInBusStop = false;
    float _time = 0f;
    bool _timerStarted = false;
    bool _accelerate = false;
    bool _createNavMesh = false;

    void Start()
    {
        CreateDoors();
    }

    public void SetStreetManager(StreetManager streetManager)
    {
        _streetManager = streetManager;
    }

    // Update is called once per frame
    void Update()
    {
        if (open)
        {
            OpenDoors();
        }
        if (close)
        {
            CloseDoors();
        }

        CheckBusPosition();
        GotInBusStation();


        StartOpen();

        if (_timerStarted)
            Timer();

        if (_accelerate)
            Accelerate();
        /*if (_createNavMesh)
        {
            _createNavMesh = false;
            var navMesh = _currentStreet.AddComponent<NavMeshSurface>();
            navMesh.BuildNavMesh();
        }*/

    }
    Vector3 GetBusBounds()
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            Bounds bounds = collider.bounds;
            Vector3 size = bounds.size;
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;
            return size;
        }
        return Vector3.zero;

    }
    #region Rotation
    public void CalculateBusRotation(Vector3 middleOfSpline, Vector3 knotPos)
    {
        var size = GetBusBounds();
        var busPos = transform.position;
        busPos.z = middleOfSpline.z;
        busPos.x += size.x;

        knotPos.y = busPos.y;


        var direction = (knotPos - busPos).normalized;

        if (busPos.x > knotPos.x - size.x  /*|| Mathf.Abs(knotPos.z-busPos.z) < 0.6f*/)
        {
            return;
        }

        var angle = Vector3.SignedAngle(transform.right, direction, Vector3.forward);
        var angle2 = Quaternion.LookRotation(direction, Vector3.forward);



        RotateBus(angle2.eulerAngles.y - 90);
    }

    public void RotateBus(float angle)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, angle, 0), Time.deltaTime * 2);
    }

    public void RotateWheels()
    {
        var wheels = transform.GetChild(0).GetChild(0).gameObject;

        for (int i = 0; i < wheels.transform.childCount; i++)
        {

            var wheel = wheels.transform.GetChild(i).gameObject;

            if (i > 1)
            {
                wheel = wheel.transform.GetChild(0).gameObject;
            }

            var angles = wheel.transform.localEulerAngles;
            angles.y--;
            wheel.transform.localEulerAngles = angles;
        }

    }
    #endregion

    #region Door functions
    void CreateDoors()
    {
        var doors = transform.GetChild(0).Find("FirstDoor").Find("DoorComp");

        var door1 = doors.GetChild(0).gameObject;
        var door2 = doors.GetChild(1).gameObject;

        Door d = new Door(door1, true);
        Door d2 = new Door(door2, false);


        _doors.Add(d);
        _doors.Add(d2);

        var doors2 = transform.GetChild(0).Find("SecondDoor").Find("DoorComp");

        door1 = doors2.GetChild(0).gameObject;
        door2 = doors2.GetChild(1).gameObject;

        Door d3 = new Door(door1, true);
        Door d4 = new Door(door2, false);

        _doors.Add(d3);
        _doors.Add(d4);

        var doors3 = transform.GetChild(0).Find("ThirdDoor").Find("DoorComp");

        door1 = doors3.GetChild(0).gameObject;

        Door d5 = new Door(door1, false);

        _doors.Add(d5);
    }

    void StartOpen()
    {
        if (_nextStop == null)
            return;
        if (_gotInBusStop && !_nextStop.IsStopped())
        {
            _createNavMesh = true;
            open = true;
            _nextStop.SetStopped();
        }
    }

    void OpenDoors()
    {
        foreach (var door in _doors)
        {
            door.OpenDoor();
        }
        var ok = true;
        foreach (var door in _doors)
        {
            ok = door.IsOpened() && ok;
        }
        if (ok)
        {
            open = false;
            _timerStarted = true;
        }
    }

    void Timer()
    {
        _time += Time.deltaTime;

        if (_time > 8f)
        {
            _timerStarted = false;
            close = true;
            _time = 0;
        }
    }
    public void CloseDoors()
    {
        foreach (var door in _doors)
        {
            door.CloseDoor();
        }
        var ok = false;
        foreach (var door in _doors)
        {
            ok = door.IsOpened() || ok;
        }
        if (!ok)
        {
            close = false; 
            ResetBusStop();
            /*var mesh = _currentStreet.GetComponent<NavMeshSurface>();
            Destroy(mesh);*/
        }
    }
    #endregion

    #region Bus Stop Functions

    public void SetCurrentStreet(GameObject obj)
    {
        _currentStreet = obj;
        var comp = obj.GetComponentInChildren<BusStop>();

        if(comp != null)
        {
            _nextStop = comp;
            _spline = obj.GetComponent<IRoadComponentsInterface>().GetSpline();
        }
        else
        {
            _nextStop = null;
            _spline = null;
        }
    }

    void GetComp()
    {
        if (_currentStreet != null)
        {
            var comp = _currentStreet.GetComponentInChildren<BusStop>();
            if (comp != null)
            {
                _nextStop = comp;
                _spline = _currentStreet.GetComponent<IRoadComponentsInterface>().GetSpline();
            }
        }
    }

    void ResetBusStop()
    {
        _currentStreet = null;
        _nextStop = null;
        _spline = null;
        _accelerate = true;
        _gotInBusStop = false;
    }

    void Accelerate()
    {
        _streetManager.Accelerate();
    }

    void CheckBusPosition()
    {
        if(_nextStop == null)
        {
            GetComp();
            return;
        }
        if (_accelerate)
            _accelerate = false;
        var test = _nextStop.transform;
        var pos = _nextStop.transform.position;
        
        
        Debug.Log(Vector3.Distance(transform.position, _nextStop.transform.position));

        var distance = Vector3.Distance(transform.position, pos);
        var busStoped = _streetManager.IsStopped();
        if ( distance<= 5 && busStoped)
        {
            return;
        }

        if (distance <= 35 && !_nextStop.IsStopped())
        {
            _accelerate = false;
            _streetManager.Decelerate();
        }
    }

    void GotInBusStation()
    {
        if (_accelerate)
            return;
        var busStoped = _streetManager.IsStopped();
        if (busStoped)
            _gotInBusStop = true;
    }


    public GameObject GetBusStation()
    {
        if (_nextStop == null)
            return null;
        if (!_gotInBusStop)
            return null;
        return _currentStreet;
    }

    public List<GameObject> GetDoors()
    {
        var doors = new List<GameObject>();

        foreach (var door in _doors)
        {
            doors.Add(door.GetDoorGameObject());
        }

        return doors;
    }
    #endregion

}
