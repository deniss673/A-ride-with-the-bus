using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class IntersectionCreate : MonoBehaviour, IRoadComponentsInterface
{

    private SplineContainer _spline;

    private SplineExtrude _extrude;
    private Material _material;
    
    private float _speed = 0.15f;
    private float _minSpeed = 0.0f;
    private float _maxSpeed = 0.15f;
    private bool _doneGenerating = false;
    private bool _isCurrentStreet = false;
    private int _type = 0;
    private string _pathType1= "Map/crossIntersection";
    private string _pathType2 = "Map/crossIntersection3";
    private string _pathType3 = "Map/crossIntersection3rotated";

    private string _materialPath = "";


    void Start()
    {
        InstantiateIntersection();
        _doneGenerating = true; 
        _isCurrentStreet = false;
    }

    void Update()
    {
        
    }

    #region Insntatiate intersection
    void InstantiateIntersection()
    {
        _spline = gameObject.AddComponent<SplineContainer>();
        //var knot1 = new BezierKnot();
        //var knot2 = new BezierKnot();

        var knot1 = new BezierKnot(new float3(0, 0, 0), new float3(0, 0, 0), new float3(0, 0, 0));
        var knot2 = new BezierKnot(new float3(5, 0, 0), new float3(0, 0, 0), new float3(0, 0, 0));
        knot1.Position = new Vector3(0, 0, 0);
        knot2.Position = new Vector3(50, 0, 0);
        
        _spline.Spline.Add(knot1);
        _spline.Spline.Add(knot2);
        ChooseType();
        GetMaterialPath();
        AddMesh();
        AddRoads();
        AddProps();
    }


    void ChooseType()
    {
        _type = UnityEngine.Random.Range(1, 3);
    }

    void GetMaterialPath()
    {
        if (_type == 2)
        {
            _materialPath = _pathType2;
        }
        else if(_type == 3)
        {
            _materialPath = _pathType3;
        }
        else
        {
            _materialPath = _pathType1;
        }
    }

    void AddMesh()
    {
        _material = Resources.Load<Material>(_materialPath);
        _extrude = gameObject.AddComponent<SplineExtrude>(); 
        GetComponent<MeshFilter>().mesh = new Mesh();
        GetComponent<MeshFilter>().mesh.MarkDynamic();
        gameObject.GetComponent<MeshRenderer>().material = _material;

        _extrude.Radius = 50;
        _extrude.Sides = 4;
        _extrude.SegmentsPerUnit = 5;
        _extrude.Container = _spline;

        _extrude.RebuildOnSplineChange = false;
        gameObject.transform.localScale = new Vector3(1, 0.01f, 1);

        _extrude.Rebuild();
    }

    void AddProps()
    {
        if (_type != 1)
        {
            var comp=gameObject.AddComponent<BuildingsManager>();
            comp.PrepareInstantiator(true, _type == 2 ? true : false);
        }
    }

    void AddRoads()
    {
        if (_type == 2)
        {
            CreateRoad(0, true);
        }
        else if (_type == 3)
        {
            CreateRoad(0, false);
        }
        else
        {
            CreateRoad(0, true);
            CreateRoad(1, false);
        }
        
    }

    void CreateRoad(int index, bool left)
    {
        var obj = new GameObject($"Street {index} of intersection");
        obj.transform.parent = transform;

        obj.transform.localPosition = GetPosition(left);
        obj.transform.localRotation = GetRotation(left);
        Physics.SyncTransforms();
        obj.AddComponent<StreetCreate>();
    }

    Vector3 GetPosition(bool left)
    {
        var p1 = _spline.Spline.Knots.ToList()[0];
        var p2 = _spline.Spline.Knots.ToList()[1];

        var x = (p2.Position.x - p1.Position.x)/2;
        var z = p1.Position.z + (left ? 50 : -50);


        return new Vector3(x, 0, z);
    }

    Quaternion GetRotation(bool left)
    {
        return Quaternion.Euler(0, left ? -90 : 90, 0);
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
        //float deltaSpeed = diff != null ? (float)diff * Time.deltaTime * 0.6f : _speed;
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

    public Quaternion GetLastKnotsAngle()
    {
        return Quaternion.Euler(0, this.gameObject.transform.localEulerAngles.y, 0);
    }

    public Vector3 GetLastKnotPos()
    {
        return this.gameObject.transform.TransformPoint(_spline.Spline.Knots.ToList()[1].Position);
    }

    public bool DoneGenerate()
    {
        return _doneGenerating;
    }
    public Vector3 GetCurrentKnot()
    {
        return transform.TransformPoint(_spline.Spline.Knots.ToList()[1].Position);
    }
    public Vector3 GetFutureKnot()
    {
        return transform.TransformPoint(_spline.Spline.Knots.ToList()[0].Position);
    }

    public Quaternion RotateSpline()
    {
        return Quaternion.identity;
    }

    public bool WentPast(float pos)
    {

        return pos > transform.TransformPoint(_spline.Spline.Knots.Last().Position).x;
    }
    public void SetIsCurrentStreet(bool ok)
    {
        _isCurrentStreet = ok;
    }

    public List<BezierKnot> GetKnots()
    {
        return _spline.Spline.Knots.ToList();
    }

    public SplineContainer GetSpline()
    {
        return GetComponent<SplineContainer>();
    }
    public void Accelerate()
    {
        if (_speed < _maxSpeed)
        {
            _speed += 0.01f;
        }
    }
    public void Deccelerate()
    {
        if (_speed > _minSpeed)
        {
            _speed = Mathf.Lerp(_speed,_minSpeed , 1f * Time.deltaTime);
        }
    }

    public float GetSpeed()
    {
        return _speed;
    }
    #endregion
}
