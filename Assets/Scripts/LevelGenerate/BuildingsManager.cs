using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Splines;
using System.Linq;
using Unity.VisualScripting;
using Unity.AI.Navigation;

public class BuildingsManager : MonoBehaviour
{
    private List<GameObject> _buildingsPrefabs;
    private string _path = "Map/buildings";
    private List<Building> _buildings = new List<Building>();
    private Vector3 _startPos;
    private IRoadComponentsInterface _street;
    private List<BezierKnot> _knots;
    private Vector3 _endPos;
    private bool _isIntersection = false;
    


    void GetBuildingsPrefab()
    {
        _buildingsPrefabs = Resources.LoadAll<GameObject>(_path).ToArray().ToList();
        foreach (GameObject go in _buildingsPrefabs)
        {
            Building building = new Building(go);
            _buildings.Add(building);
        }
    }


    public void PrepareInstantiator(bool isIntersection = false, bool isOnRight = false)
    {
        _isIntersection = isIntersection;
        GetBuildingsPrefab();
        if (_isIntersection)
        {
            _street = transform.GetComponent<StreetCreate>();
        }
        else
        {
            _street = transform.GetComponent<IntersectionCreate>();
        }
        _street = transform.GetComponent<IRoadComponentsInterface>();
        _knots = _street.GetKnots();
        _startPos = _knots[1].Position;
        _endPos = _knots.Last().Position;
        if (!_isIntersection)
        {
            gameObject.AddComponent<BusStopManager>();
            SpawnBuildingsOnStreet(false);
            SpawnBuildingsOnStreet(true);
        }
        else
        {
            SpawnBuildingsOnStreet(isOnRight);
        }

        var comp = transform.AddComponent<NavMeshSurface>();
        comp.collectObjects = CollectObjects.Children;
        comp.BuildNavMesh();

    }

    void SpawnBuildingsOnStreet(bool right)
    {
        var spline = _street.GetSpline();
        float splineLength = spline.CalculateLength();

        if (_isIntersection)
        {
            splineLength = 50;
        }

        float distance = 1f;



        Physics.SyncTransforms();

        while (distance < splineLength-10)
        {
            var buildingPrefab = GetRandomBuilding();
            GameObject building = Instantiate(buildingPrefab.GetObject());
            
            var debug = building.AddComponent<DebugClass>();

            building.transform.parent = transform;

            var size = building.GetBuildingBounds();
            var box = building.GetComponent<BoxCollider>();
            if (right && distance == 1)
            {
                distance = size.x;
            }
            bool notSpawned = true;

            if (distance + size.x > splineLength-10)
                return;

            while (notSpawned && distance < splineLength - 1)
            {
                if (!spline.Evaluate(distance / splineLength, out var splinePoint, out var tangentV, out var upVector))
                {
                    UnityEngine.Debug.LogWarning("Spline evaluate failed at distance: " + distance);
                    break;
                }

                splinePoint = transform.InverseTransformPoint(splinePoint);

                Vector3 tan = tangentV;
                Vector3 position = splinePoint;

                Vector3 tangent = tan.normalized;


                tangent = transform.InverseTransformDirection(tangent);
                Vector3 normal = Vector3.Cross(transform.up, tangent).normalized;
                Vector3 lateralOffset = normal * 40 * (right ? 1f : -1f);




                Vector3 finalPosition = position + lateralOffset;


                building.transform.localPosition = finalPosition;
                var rotation = Quaternion.LookRotation(tangent, transform.up);
                var y = rotation.eulerAngles.y;
                y += transform.eulerAngles.y;


                building.transform.rotation = Quaternion.Euler(0, y, 0);

                var yRot = building.transform.localEulerAngles;

                yRot.y -= 180 * (right ? 1 : 0);
                building.transform.localEulerAngles = yRot;



                debug.normal = normal;
                debug.tangent = tangent;
                debug.position = finalPosition;
                debug.distance = distance;
                debug.x = size.z;

                if (GetOverlap(building))
                {
                    distance++;
                }
                else
                {
                    notSpawned=false;
                    var propsmanager=building.AddComponent<PropsManager>();
                    propsmanager.Prepare(right);
                }
            }
            distance++;
        }
    }

    bool GetOverlap(GameObject _object)
    {
        var box = _object.GetComponent<BoxCollider>();
        Physics.SyncTransforms();
        if (box == null)
        {
            Debug.LogWarning("GameObject-ul nu are BoxCollider.");
            return false;
        }
        Vector3 center = box.transform.TransformPoint(box.center);
        Vector3 halfExtents = Vector3.Scale(box.size * 0.5f, box.transform.lossyScale);
        Quaternion orientation = box.transform.rotation;

        Collider[] hitColliders = Physics.OverlapBox(center, halfExtents, orientation);
        int i = 0;

        foreach (var collider in hitColliders)
        {
            if (collider.gameObject != _object)
            {
                return true;
            }
        }
        return false;
    }

    Building GetRandomBuilding()
    {
        var random = UnityEngine.Random.Range(0, _buildings.Count);
        return _buildings[random];
    }



}
