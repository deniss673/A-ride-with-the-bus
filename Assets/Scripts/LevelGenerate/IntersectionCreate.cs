using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class IntersectionCreate : MonoBehaviour, IRoadComponentsInterface
{

    private SplineContainer _spline;

    private SplineExtrude _extrude;
    private Material _material;
    
    private float _speed = 0.025f;
    private bool _doneGenerating = false;
    private bool _isCurrentStreet = false;

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
        var knot1 = new BezierKnot();
        var knot2 = new BezierKnot();

        knot1.Position = new Vector3(0, 0, 0);
        knot2.Position = new Vector3(15, 0, 0);

        _spline.Spline.Add(knot1);
        _spline.Spline.Add(knot2);
        AddMesh();
        AddRoads();
    }

    void AddMesh()
    {
        _material = Resources.Load<Material>("Map/crossIntersection");
        _extrude = gameObject.AddComponent<SplineExtrude>(); 
        GetComponent<MeshFilter>().mesh = new Mesh();
        GetComponent<MeshFilter>().mesh.MarkDynamic();
        gameObject.GetComponent<MeshRenderer>().material = _material;

        _extrude.Radius = 6;
        _extrude.Sides = 4;
        _extrude.SegmentsPerUnit = 5;
        _extrude.Container = _spline;

        _extrude.RebuildOnSplineChange = false;
        gameObject.transform.localScale = new Vector3(1, 0.01f, 1);

        _extrude.Rebuild();
    }

    void AddRoads()
    {
        CreateRoad(0, true);
        CreateRoad(1, false);
    }

    void CreateRoad(int index, bool left)
    {
        var obj = new GameObject($"Street {index} of intersection");
        obj.transform.parent = transform;

        obj.AddComponent<StreetCreate>();
        obj.transform.localPosition = GetPosition(left);
        obj.transform.localRotation = GetRotation(left);
    }

    Vector3 GetPosition(bool left)
    {
        var p1 = _spline.Spline.Knots.ToList()[0];
        var p2 = _spline.Spline.Knots.ToList()[1];

        var x = (p2.Position.x - p1.Position.x)/2;
        var z = p1.Position.z + (left ? 6 : -6);


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
        var pos = this.gameObject.transform.position;
        float deltaSpeed = diff != null ? (float)diff * Time.deltaTime * 0.6f : _speed;
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

    public Quaternion RotateSpline()
    {
        return Quaternion.identity;
    }

    public bool WentPast(float pos)
    {
        return pos > _spline.Spline.Knots.Last().Position.x;
    }
    public void SetIsCurrentStreet(bool ok)
    {
        _isCurrentStreet = ok;
    }
    #endregion
}
