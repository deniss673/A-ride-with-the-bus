using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using System.Threading.Tasks;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class StreetCreate : MonoBehaviour, IRoadComponentsInterface
{

    [Header("Scene Objects")]
    private SplineContainer _spline;
    private GameObject _splineGameObject;
    private Material roadMat;
    public GameObject hydrant;

    [Header("Positions")]
    private Vector3 _startPos;
    private Vector3 _newPos;
    private float _endPos;
    private BezierKnot _currentKnot;
    private BezierKnot _futureKnot;
    private bool _wentPast;

    private int _currentKnotIndex;
    private int _futureKnotIndex;



    [Header("Spawner variables")]
    private float _speed;
    private float _maxSpeed=0.15f;
    private float _minSpeed=0;
    private float _distance;
    private int _maxKnots;
    private int _startingKnots = 3;
    private float _lastTurn;
    private bool _needsRebuild;
    private float _turnChance;

    private bool _isCurrentStreet = false;
    private BuildingsManager _buildingsManager;
    

    void InstantiateSplines(ref GameObject obj, ref SplineContainer spline, int radius, Vector3 pos, Material mat, string name)
    {
        //Components
        obj = this.gameObject;
        obj.AddComponent<SplineContainer>();
        obj.AddComponent<SplineExtrude>();

        spline = obj.GetComponent<SplineContainer>();

        //Mesh
        obj.GetComponent<MeshFilter>().mesh = new Mesh();
        obj.GetComponent<MeshFilter>().mesh.MarkDynamic();
        spline.GetComponent<MeshRenderer>().material = mat;

        //Extrude
        var extrude = spline.GetComponent<SplineExtrude>();
        
        extrude.Radius = radius;
        extrude.Sides = 4;
        extrude.SegmentsPerUnit = 5;
        extrude.RebuildOnSplineChange = false;
        extrude.Container = obj.GetComponent<SplineContainer>();

        //Game object        
        obj.transform.localScale = new Vector3(1, 0.01f, 1);

        //To rebuild the mesh
        _needsRebuild = true;
        
    }

    void Start()
    {
        roadMat = Resources.Load<Material>("Map/road material");
        _splineGameObject = this.gameObject;
        hydrant = GameObject.Find("Hydrant");
        InstantiateSplines(ref _splineGameObject, ref _spline, 50, hydrant.transform.position, roadMat, "Road");


        _speed = 0.15f;
        _distance = 50;
        _endPos = -100;
        _startPos = new Vector3(0, 0, 0);
        _newPos = _startPos - new Vector3(_distance, 0, 0);

        System.Random sysRand = new System.Random();
        //_maxKnots = sysRand.Next(15, 31);
        _maxKnots = 7;
        
        _startingKnots = 3;

        _futureKnotIndex = 1;
        _currentKnotIndex = 0;
        _currentKnot = new BezierKnot();
        _futureKnot = new BezierKnot();

        _wentPast = false;
        _turnChance = 100;

        GenerateRoad();
        _buildingsManager = gameObject.AddComponent<BuildingsManager>();
        _buildingsManager.PrepareInstantiator();
    }

    void Update()
    {
        if (_spline.Spline.Knots.Count() > 1)
        {
            CheckKnots();
        }
    }

    #region Road Generation
    void GenerateRoad()
    {
        while (_spline.Spline.Knots.Count() < _maxKnots)
        {
            AddKnots();
        }
        if(_spline.Spline.Knots.Count() == _maxKnots && _needsRebuild)
        {
            var extrude = _splineGameObject.GetComponent<SplineExtrude>();
            extrude.Rebuild();
            _needsRebuild = false;
        }
    }

    void AddKnots()
    {
        var z = GenerateTurn();

        _newPos = _newPos + new Vector3(_distance, 0, z);

        var knot = new BezierKnot(_newPos);

        _spline.Spline.Add(knot);

        var count = _spline.Spline.Knots.Count();

        _spline.Spline.SetTangentMode(count - 1, TangentMode.AutoSmooth);

    }

    float GenerateTurn()
    {
        float z = _lastTurn;

        System.Random sysRand = new System.Random();
        var chance = sysRand.Next(0, 101);

        if (chance < _turnChance && _spline.Spline.Knots.Count() > _startingKnots)
        {
            var max = _distance + _lastTurn;
            var min = -_distance + _lastTurn;

            float random = UnityEngine.Random.Range(min, max);
            z = random;
            _turnChance = 40;
        }
        else
        {
            _turnChance += 5;
        }

        _lastTurn = z;

        return z;
    }
    #endregion

    #region Rotation Calc
    void CheckKnots()
    {
        var splineKnots = _spline.Spline.Knots.ToList();

        if (!_isCurrentStreet)
            return;

        if (_wentPast)
            return;

        if (BezierKnot.Equals(_currentKnot, new BezierKnot()) || BezierKnot.Equals(_futureKnot, new BezierKnot()))
        {
            _currentKnot = splineKnots[0];
            _futureKnot = splineKnots[1];
        }
        

        if (_futureKnotIndex == _spline.Spline.Knots.Count())
        {
            _wentPast = true;
            return;
        }

        _currentKnot = splineKnots[_currentKnotIndex];
        _futureKnot = splineKnots[_futureKnotIndex];

        var hydrantPos = hydrant.transform.position;
        var knotPos = _splineGameObject.transform.TransformPoint(_futureKnot.Position);


        if (knotPos.x < hydrantPos.x)
        {
            _currentKnotIndex++;
            _futureKnotIndex++;
        }
    }

    //FIX THIS SHIT
    public Quaternion RotateSpline()
    {
        var p1 = _futureKnot.Position;
        var p2 = _currentKnot.Position;
        p1 = transform.TransformPoint(p1);
        p2 = transform.TransformPoint(p2);

        Vector3 direction = p1 - p2;
        direction = direction.normalized;

        var streetDirection = transform.parent.forward;

        var angle2 = Vector3.SignedAngle(streetDirection, direction, Vector3.up);
        
        float angle = Mathf.Atan2(-direction.x, direction.z) * Mathf.Rad2Deg;
        var target = Quaternion.Euler(0, -angle2+90, 0);
       
        if (Mathf.Abs(direction.z)<0.05f)
            return Quaternion.identity;

        return target; 
    }

    #endregion 

    #region Interface Functions
    public void MoveKnots(float? diff = null)
    {
        if (_speed == _minSpeed)
        {
            return;
        }
        var pos = this.gameObject.transform.position;
        float deltaSpeed = diff != null ? (float)diff * Time.deltaTime * 3f : _speed;
        if (diff == null)
        {
            pos.x -= deltaSpeed;
        }
        else
        {
            pos.z -= deltaSpeed;
        }
        this.gameObject.transform.position = pos;
    }


    public bool DoneGenerate()
    {
        if (_maxKnots == 0)
            return false;

        return _maxKnots == _spline.Spline.Knots.Count();
    }

    public Vector3 GetLastKnotPos()
    {
        return transform.TransformPoint((Vector3)_spline.Spline.Knots.ToList()[_maxKnots - 1].Position);
    }

    public Quaternion GetLastKnotsAngle()
    {
        var knots = _spline.Spline.Knots.ToList();

        var p1 = knots[_maxKnots-2].Position;
        var p2 = knots[_maxKnots - 1].Position;

        p1 = transform.TransformPoint(p1);
        p2 = transform.TransformPoint(p2);
        
        Vector3 p = p2 - p1;
        var direction = transform.InverseTransformDirection(p);

        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        var target = Quaternion.Euler(0, angle - 90, 0);
        
        
        Quaternion fixedRotation = target * transform.localRotation;

        return fixedRotation;
    }

    public Vector3 GetCurrentKnot()
    {
        return transform.TransformPoint(_futureKnot.Position);
    }

    public Vector3 GetFutureKnot()
    {
        return transform.TransformPoint(_currentKnot.Position);
    }
    public bool WentPast(float pos)
    {
        return pos > transform.TransformPoint(_spline.Spline.Knots.Last().Position).x;
    }

    public void SetIsCurrentStreet(bool ok)
    {
        _isCurrentStreet = ok;
    }

    public SplineContainer GetSpline()
    {
        return GetComponent<SplineContainer>();
    }
    public List<BezierKnot> GetKnots()
    {
        return _spline.Spline.Knots.ToList();
    }

    public Vector3 GetNextKnot(Vector3 position)
    {

        float minDistance = float.MaxValue;
        int closestSegmentIndex = -1;

        var splineKnots = _spline.Spline.Knots.ToList();

        for (int i = 0; i < splineKnots.Count - 1; i++)
        {
            Vector3 a = splineKnots[i].Position;
            Vector3 b = splineKnots[i + 1].Position;

            Vector3 closestPoint = ClosestPointOnSegment(a, b, position);
            float distance = Vector3.Distance(position, closestPoint);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestSegmentIndex = i;
            }
        }

        if (closestSegmentIndex >= 0 && closestSegmentIndex + 1 < splineKnots.Count)
        {
            return splineKnots[closestSegmentIndex + 1].Position;
        }

        return splineKnots[splineKnots.Count - 1].Position;
    }


    public void Accelerate()
    {
        if(_speed < _maxSpeed) {
            _speed+= 0.01f;
        }
    }
    public void Deccelerate()
    {
        if (_speed > _minSpeed)
        {
            _speed= Mathf.Lerp(_speed, _minSpeed, 1f * Time.deltaTime);
        }
    }
    public float GetSpeed()
    {
        return _speed;
    }
    #endregion



    private Vector3 ClosestPointOnSegment(Vector3 a, Vector3 b, Vector3 point)
    {
        Vector3 ab = b - a;
        float t = Vector3.Dot(point - a, ab) / ab.sqrMagnitude;
        t = Mathf.Clamp01(t);
        return a + t * ab;
    }

}
