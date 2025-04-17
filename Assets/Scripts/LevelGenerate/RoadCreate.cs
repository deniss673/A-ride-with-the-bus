using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using System.Threading.Tasks;

public class RoadCreate : MonoBehaviour
{

    [Header("Scene Objects")]
    private SplineContainer _spline;
    private GameObject _splineGameObject;
    private Material roadMat;
    private Material pavamentMat;
    public GameObject hydrant;

    [Header("Positions")]
    private Vector3 _startPos;
    private Vector3 _newPos;
    private float _endPos;
    private BezierKnot _currentKnot;
    private BezierKnot _futureKnot;

    private int _currentKnotIndex;
    private int _futureKnotIndex;




    [Header("Time releated")]
    private float _interval;
    private float _currentTime;

    [Header("Spawner variables")]
    private float _speed;
    private float _distance;
    private int _maxKnots;
    private int _startingKnots = 5;
    private float _lastTurn;



    BuildingsInstantiator _buildingsInstantiator;


    void InstantiateSplines(ref GameObject  obj,ref SplineContainer spline, int radius, Vector3 pos, Material mat,string name) 
    {
        obj = new GameObject(name);
        obj.AddComponent<SplineContainer>();
        obj.AddComponent<SplineExtrude>();

        spline = obj.GetComponent<SplineContainer>();


        spline.GetComponent<MeshRenderer>().material = mat;
        var extrude = spline.GetComponent<SplineExtrude>();

        extrude.Radius = radius;
        extrude.Sides = 4;
        extrude.SegmentsPerUnit = 5;

        extrude.RebuildOnSplineChange = false;



        obj.transform.localScale = new Vector3(1, 0.01f, 1);
        extrude.Container = obj.GetComponent<SplineContainer>();

        obj.GetComponent<MeshFilter>().mesh = new Mesh();

        obj.GetComponent<MeshFilter>().mesh.MarkDynamic();

        obj.transform.position = pos;

        _buildingsInstantiator=obj.AddComponent<BuildingsInstantiator>();

        _buildingsInstantiator.PrepareInstantiator();

        var obj2=_buildingsInstantiator.GetBuildingGameObj();
        obj2.transform.position = pos;
    }

    void Start()
    {
        roadMat = Resources.Load<Material>("Map/road material");
        pavamentMat = Resources.Load<Material>("Map/Pavament");

        InstantiateSplines(ref _splineGameObject, ref _spline, 6, hydrant.transform.position, roadMat, "Road");

        _endPos = -100;        
        _startPos = new Vector3(0, 0,0);
        _newPos = _startPos;

        _interval = 3;
        _currentTime = 0;

        _speed = 0.025f;
        _distance = 15;
        _maxKnots = 20;
        _startingKnots = 20;

        _futureKnotIndex = 1;
        _currentKnotIndex = 0;
        _currentKnot = new BezierKnot();
        _futureKnot = new BezierKnot();

    }

    void Update()
    {
        UpdateTime();

        MoveKnots();

        VerifyNumberOfKnots();
        if (_spline.Spline.Knots.Count()>1)
        {
            CheckKnots();

            RotateSpline();

            RecenterTheSpline();
        }
    }

    void VerifyNumberOfKnots()
    {
        
        var knots = _spline.Spline.Knots.ToList();

        if (knots.Count == 0)
        {
            return;
        }
        if (knots[0].Position.x < _endPos)
        {
            knots.RemoveAt(0);
            _spline.Spline.Knots = knots;
            _currentKnotIndex--;
            _futureKnotIndex--;
        }

    }

    void UpdateTime()
    {
        if (_currentTime < _interval && _spline.Spline.Knots.Count()> _startingKnots)
        {
            _currentTime = _currentTime + Time.deltaTime;
        }
        else if(_spline.Spline.Knots.Count() < _maxKnots )
        {
            AddKnots();
            _currentTime = 0;
        }
    }


    MoveKnots CreateMoveKnotsJob(Transform transform,float? diff,Spline spline,NativeArray<float3> knotPositions)
    {
        var bezierKnots = spline.Knots.ToList();
        bool hasDiff = diff.HasValue;
        float deltaSpeed = hasDiff ? (float)diff * Time.deltaTime * _speed : _speed;

        for (int i = 0; i < bezierKnots.Count; i++)
        {
            knotPositions[i] = bezierKnots[i].Position;
        }

        var moveKnotsJob = new MoveKnots
        {
            KnotPositions =knotPositions,
            DeltaSpeed = deltaSpeed,
            MoveAlongZ = hasDiff,
            LocalToWorldMatrix = transform.localToWorldMatrix,
            WorldToLocalMatrix = transform.worldToLocalMatrix
        };
        return moveKnotsJob;
    }

    void ParalelizeJobs(float? diff)
    {
        var transform1 = _splineGameObject.transform;
        var spline1 = _spline.Spline;
        
        var count = spline1.Knots.Count();

        NativeArray<float3> knotPositions1 = new NativeArray<float3>(count, Allocator.TempJob);
        var job1 = CreateMoveKnotsJob(transform1, diff, spline1,knotPositions1);



        JobHandle handle1 = job1.Schedule(count, 64);
        handle1.Complete();

        var bezierKnots1 = spline1.Knots.ToList();
        for (int i = 0; i < count; i++)
        {
            var knot1 = bezierKnots1[i];
            knot1.Position = knotPositions1[i];
            spline1.SetKnot(i, knot1);
        }

        knotPositions1.Dispose();
        

    }

    void MoveKnots(float? diff = null)
    {
        ParalelizeJobs(diff);
        _buildingsInstantiator.MoveBuildings(_speed, diff);
    }


    void AddKnots()
    {
        var z = GenerateTurn();

        _newPos = _newPos + new Vector3(_distance, 0, z);

        var knot = new BezierKnot(_newPos);

        _spline.Spline.Add(knot);

        var count = _spline.Spline.Knots.Count();

        _spline.Spline.SetTangentMode(count-1,TangentMode.AutoSmooth);

        _buildingsInstantiator.InstantiateBuildings(knot.Position);

    }

    float GenerateTurn()
    {
        float z=_lastTurn;

        
        if (UnityEngine.Random.Range(0, 100) < 40 && _spline.Spline.Knots.Count()> _startingKnots)
        {
            System.Random sysRand = new System.Random();
            var max = _lastTurn >0 ? _distance + _lastTurn : _distance-_lastTurn;
            var min = _lastTurn >0 ? _distance + _lastTurn : _distance-_lastTurn;
            
            float random = (float)(sysRand.NextDouble() * max - min);
            z = random;
        }

        _lastTurn = z;
        
        return z;
    }

    void CheckKnots()
    {
        if(BezierKnot.Equals(_currentKnot,new BezierKnot())|| BezierKnot.Equals(_futureKnot, new BezierKnot()))
        {
            _currentKnot = _spline.Spline.Knots.ToList()[0];
            _futureKnot = _spline.Spline.Knots.ToList()[1];
        }
        _currentKnot = _spline.Spline.Knots.ToList()[_currentKnotIndex];
        _futureKnot = _spline.Spline.Knots.ToList()[_futureKnotIndex];

        if (_futureKnot.Position.x <0.5f)
        {
            _currentKnotIndex++;
            _futureKnotIndex++;
        }
    }
    void RotateSpline()
    {
        var p2 = _futureKnot.Position;
        var p1 = _currentKnot.Position;
        var ty = p2.z - p1.z;
        var tx = p2.x - p1.x;

        float angle = Mathf.Atan2(ty,tx);
        var y = _splineGameObject.transform.rotation.y;


        var target = Quaternion.Euler(0,angle*Mathf.Rad2Deg,0);

        _splineGameObject.transform.rotation = Quaternion.Slerp(_splineGameObject.transform.rotation,target, Time.deltaTime);
        var obj = _buildingsInstantiator.GetBuildingGameObj();
        obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, target, Time.deltaTime);
    }

    void RecenterTheSpline()
    {
        Vector3 worldPos = _splineGameObject.transform.TransformPoint(_futureKnot.Position);

        if (worldPos.z >hydrant.transform.position.z+0.5f || worldPos.z < hydrant.transform.position.z - 0.5f)
        {
            var dif =  hydrant.transform.position.z- worldPos.z;
            MoveKnots(dif);
        }
    }

}

