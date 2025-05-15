using NUnit.Framework;
using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.AppUI.Core;

public class StreetManager : MonoBehaviour
{
    private Vector3 _pos;
    private bool oneTime = false;
    private GameObject _streets;
    private int _index;
    private GameObject _currentSpline;
    private bool _onChange;
    private List<GameObject> _streetsGameObjects;
    private int _currentStreet;
    private GameObject _hydrant;
    private bool _lastObjIsStreet = false;
    private Quaternion _angle = Quaternion.identity;

    private int _dif=0;

    private GameObject _bus;
    private BusManager _busManager;

    private bool _stopDecelerate = false;
    private bool _stopAccelerate = false;


    private void Start()
    {
        _pos = new Vector3(206, 34, 57);
        _streets = new GameObject("Streets");
        _streets.transform.position = _pos;
        _index = 0;
        _streetsGameObjects = new List<GameObject>();
        _onChange = false;
        _hydrant = GameObject.Find("Hydrant");
        _angle = Quaternion.identity;
        _currentStreet = -1;
        _bus = GameObject.Find("bus");
        _busManager = _bus.GetComponent<BusManager>();
        _busManager.SetStreetManager(this);
        AddStreet();
    }

    private void Update()
    {
        CheckPositionOfCurrentStreet();

        var street = GetInterface();
        if (!street.DoneGenerate())
        {
            return;
        }
        MoveKnots();

        RotateSpline();
        _busManager.RotateWheels();
        if (_index == 5)
        {
            return;
        }

        AddRoadComponent();
    }

    #region Generation
    void AddRoadComponent()
    {
        if (_lastObjIsStreet)
        {
            AddIntersection();
        }
        else
        {
            AddStreet();
        }
    }

    void AddStreet()
    {
        var obj = new GameObject("Street " + _index);
        obj.transform.parent = _streets.transform;

        obj.transform.position = CalculateLastStreetPos();
        obj.transform.localRotation = CalculateRotation();

        obj.AddComponent<StreetCreate>();

       
        
        _streetsGameObjects.Add(obj);
        if (_currentStreet == -1)
        {
            _currentStreet = _index;
            obj.GetComponent<IRoadComponentsInterface>().SetIsCurrentStreet(true);
            _busManager.SetCurrentStreet(_streetsGameObjects[_currentStreet]);
        }
        if (_currentSpline == null)
        {
            _currentSpline = _streetsGameObjects[_index];
            _onChange = true;
        }

        _index++;
        _lastObjIsStreet = true;
    }

    void AddIntersection()
    {
        var obj = new GameObject("Intersection " + _index);

        obj.transform.parent = _streets.transform;

        obj.transform.position = CalculateLastStreetPos();
        obj.transform.rotation = CalculateRotation();

        obj.AddComponent<IntersectionCreate>();

        _streetsGameObjects.Add(obj);

        if (_currentSpline == null)
        {
            _currentSpline = _streetsGameObjects[_index];
            _onChange = true;
            obj.GetComponent<IRoadComponentsInterface>().SetIsCurrentStreet(true);
        }
        _index++;
        _lastObjIsStreet = false;

    }
    #endregion

    #region Misc
    IRoadComponentsInterface GetInterface()
    {
        _streetsGameObjects[_index - 1].TryGetComponent<IRoadComponentsInterface>(out var roadComponent);

        if (roadComponent != null)
        {
            return roadComponent;
        }
        else
        {
            Debug.LogError("There is no Road Component interface on this game object");
            return null;
        }

    }

    Vector3 CalculateLastStreetPos()
    {
        if(_index == 0)
        {
            return _pos;
        }

        var roadComponent = GetInterface();

        if (roadComponent != null)
        {
            return roadComponent.GetLastKnotPos();
        }
        else
        {
            return _pos;
        }
    }

    Quaternion CalculateRotation()
    {
        if (_index == 0)
        {
            return new Quaternion(0, 0, 0, 0);
        }

        var roadComponent = GetInterface();

        if (roadComponent != null)
        {
            return roadComponent.GetLastKnotsAngle();
        }
        else
        {
            return new Quaternion(0, 0, 0, 0);
        }

    }
    #endregion

    #region Movement and rotation
    void CheckPositionOfCurrentStreet()
    {
        if(_currentStreet == -1)
        {
            return;
        }
        if (_currentStreet == _streetsGameObjects.Count - 1)
        {
            return;
        }

        var comp = _streetsGameObjects[_currentStreet].GetComponent<IRoadComponentsInterface>();

        if (_streetsGameObjects[_currentStreet+1].transform.position.x < _hydrant.transform.position.x && comp.WentPast(_hydrant.transform.position.x))
        {
            
            _currentStreet++;
            _onChange = true;
            comp.SetIsCurrentStreet(false);
            _currentSpline = _streetsGameObjects[_currentStreet]; 
            comp = _streetsGameObjects[_currentStreet].GetComponent<IRoadComponentsInterface>();
            comp.SetIsCurrentStreet(true);
            _busManager.SetCurrentStreet(_streetsGameObjects[_currentStreet]);
        }
        
    }

    void CheckAngle()
    {
        if (_streets.transform.rotation == _angle && _angle!= Quaternion.identity)
        {
            _angle = Quaternion.identity;
        }
    }

    void RotateSpline()
    {
        var spline = _currentSpline.GetComponent<IRoadComponentsInterface>();
        var target = spline.RotateSpline();
        
        if (_angle == Quaternion.identity)
            _angle = target;
        if (target == Quaternion.identity)
        {
            _busManager.CalculateBusRotation(_hydrant.transform.position, spline.GetCurrentKnot());
            CheckAngle();
            return;
        }
        _busManager.RotateBus(0);



        _streets.transform.rotation = Quaternion.Slerp(_streets.transform.rotation, _angle, Time.deltaTime * 5);
        CheckAngle();
    }


    public void Accelerate()
    {
        _stopDecelerate = false;
        if (_stopAccelerate)
        {
            return;
        }
        foreach (var street in _streetsGameObjects)
        {
            street.GetComponent<IRoadComponentsInterface>().Accelerate();
        }
    }
    public void Decelerate()
    {
        _stopAccelerate = false;
        
        if (_stopDecelerate)
            return;
        foreach (var street in _streetsGameObjects)
        {
            street.GetComponent<IRoadComponentsInterface>().Deccelerate();
        }
    }

    public bool IsStopped()
    {
        foreach (var street in _streetsGameObjects)
        {
            var speed = street.GetComponent<IRoadComponentsInterface>().GetSpeed();
            if (speed > 0.00001f) {
                return false;
            }
        }
        _stopDecelerate = true;
        return true;
    }

    public bool IsMaxSpeed()
    {
        foreach (var street in _streetsGameObjects)
        {
            var speed = street.GetComponent<IRoadComponentsInterface>().GetSpeed();
            if (speed > 0.14f)
            {
                return false;
            }
        }
        _stopAccelerate = true;
        return true;
    }

    void MoveKnots()
    {
        foreach(var street in _streetsGameObjects)
        {
            street.GetComponent<IRoadComponentsInterface>().MoveKnots();
        }

        MoveAlongZ();
    }
    void MoveAlongZ()
    {
        var spline = _currentSpline.GetComponent<IRoadComponentsInterface>();
        var pos = spline.GetCurrentKnot();

        var hydrantPos = _hydrant.transform.position.z;

        if (Mathf.Abs(pos.z - hydrantPos) > 0.5f)
        {
            var dif = pos.z - hydrantPos > 0 ? 1 : -1;
            
            _busManager.RotateBus(-dif * 5);
            
            foreach (var street in _streetsGameObjects)
            {
                street.GetComponent<IRoadComponentsInterface>().MoveKnots(dif);
            }
            _dif = dif;
            
        }
        else
        {
            if (_dif != 0)
            {
                _busManager.RotateBus(_dif * 5);
                _dif = 0;
            }
        }
    }
    #endregion


    #region Bus functions
    
    



    #endregion


}
