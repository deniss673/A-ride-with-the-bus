using UnityEngine;
using UnityEngine.Splines;

public class BusStopManager : MonoBehaviour
{

    private GameObject _busStop;
    private string _path = "Map/busStop";
    private float _distance;
    private SplineContainer _spline;
    private float _splineLength;
    private GameObject _busStopGo;
    public bool show = false;

    void Start()
    {
        Prepare();
    }

    private void Update()
    {
        if (show)
        {
            show = false;
        }
    }

    void Prepare()
    {
        _busStop = Resources.Load<GameObject>(_path);

        _spline = GetComponent<IRoadComponentsInterface>().GetSpline();

        _splineLength = _spline.CalculateLength();
        _distance = UnityEngine.Random.Range(10, _splineLength);

        SpawnBusStop();
    }

    void SpawnBusStop()
    {
        _spline.Evaluate(_distance / _splineLength, out var splinePoint, out var tangentV, out var upVector);
        var go = Instantiate(_busStop);
        go.AddComponent<BusStop>();
        go.transform.parent = transform;

        splinePoint = transform.InverseTransformPoint(splinePoint);

        Vector3 tan = tangentV;
        Vector3 position = splinePoint;

        Vector3 tangent = tan.normalized;


        tangent = transform.InverseTransformDirection(tangent);
        Vector3 normal = Vector3.Cross(transform.up, tangent).normalized;
        Vector3 lateralOffset = normal * 22;




        Vector3 finalPosition = position + lateralOffset;


        go.transform.localPosition = finalPosition;
        var rotation = Quaternion.LookRotation(tangent, transform.up);
        var y = rotation.eulerAngles.y;
        y += transform.eulerAngles.y;


        go.transform.rotation = Quaternion.Euler(0, y, 0);

        var yRot = go.transform.localEulerAngles;

        yRot.y -= 180;
        go.transform.localEulerAngles = yRot;

        /*        var npc = Instantiate(Resources.Load<GameObject>("Map/npc1"));
                npc.transform.SetParent(transform, true);
                npc.transform.localPosition = finalPosition;
                var streetCreateScript = GetComponent<StreetCreate>();*/


        _busStopGo = go;
    }

    public Vector3 GetRandomPositionInBusStop()
    {
        var boxGo = _busStopGo.transform.GetChild(2);

        var box = boxGo.GetComponent<BoxCollider>();
        Vector3 localCenter = box.center;
        Vector3 localSize = box.size;

        Vector3 localRandom = new Vector3(
            Random.Range(-localSize.x / 2f, localSize.x / 2f),
            Random.Range(-localSize.y / 2f, localSize.y / 2f),
            Random.Range(-localSize.z / 2f, localSize.z / 2f)
        );
        Vector3 worldPoint = box.transform.TransformPoint(localCenter + localRandom);

        var finalPos = transform.InverseTransformPoint(worldPoint);

        return finalPos;
    }



}
