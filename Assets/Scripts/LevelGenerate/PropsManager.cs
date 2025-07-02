using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class PropsManager : MonoBehaviour
{

    List<GameObject> _propsPrefabs = new List<GameObject>();
    List<GameObject> _treesPrefabs = new List<GameObject>();
    GameObject _lamp;
    string _path = "Map/streetProps";
    string _pathTrees = "Map/trees";
    string _pathLamp = "Map/lamp";
    float _forwardDistance = 20f;
    float _distance = 0;
    bool _right = false;
    StreetCreate _street;

    struct Pos
    {
        public Vector3 position;
        public Vector3 rotation;
    }

    public void Prepare(bool right)
    {
        _street = GetComponent<StreetCreate>();
        if (_street == null)
            return;
        GetDistance();
        GetPropsPrefab();

        SpawnAlongTheStreet(true, true);
        SpawnAlongTheStreet(true, false,false);
        SpawnAlongTheStreet(true, false,true);

        SpawnAlongTheStreet(false, true);
        SpawnAlongTheStreet(false, false, false);
        SpawnAlongTheStreet(false, false, true);

        _right = right;
    }

    void Update()
    {

    }

    void GetPropsPrefab()
    {
        _propsPrefabs = Resources.LoadAll<GameObject>(_path).ToArray().ToList();
        _treesPrefabs = Resources.LoadAll<GameObject>(_pathTrees).ToArray().ToList();
        _lamp = Resources.Load<GameObject>(_pathLamp);
    }

    GameObject GetRandomPrefab(bool trees)
    {
        if (trees)
        {
            var r = UnityEngine.Random.Range(0, _treesPrefabs.Count);
            return _treesPrefabs[r];
        }
        var random = UnityEngine.Random.Range(0, _propsPrefabs.Count);
        return _propsPrefabs[random];
    }

    void SpawnAlongTheStreet(bool right,bool isLamp, bool isTree= false)
    {
        var location = 0f;
        if (isLamp)
        {
            location = UnityEngine.Random.Range(5, 10);
        }

        while (location < _distance)
        {
            GameObject prop;
            if (isLamp)
            {
                prop = _lamp;
            }
            else
            {
                prop = GetRandomPrefab(isTree);
            }

            var ok = SpawnRandomObject(prop, ref location, right);

            if (!ok)
            {
                continue;
            }

            var size = prop.GetComponentInChildren<BoxCollider>().size.x;

            if (size > _distance - location)
            {
                return;
            }

            location += size + GetRandomDistance(prop.name);
        }
    }


    float GetRandomDistance(string name)
    {
        if (name.Contains("lamp"))
        {
            return UnityEngine.Random.Range(10, 20);
        }
        if (name.Contains("trees"))
        {
            return UnityEngine.Random.Range(2, 4);
        }
        return UnityEngine.Random.Range(5, 15);
    }
    bool SpawnRandomObject(GameObject prop,ref float location,bool right)
    {
        var pos = GetObjectOffset(ref location, prop.name,right);
        
        GameObject newObject = Instantiate(prop);
        newObject.transform.parent = gameObject.transform;
        newObject.transform.localPosition = pos.position;
        newObject.transform.eulerAngles = pos.rotation;
        while (GetOverlap(newObject))
        {
            location++;
            pos = GetObjectOffset(ref location, prop.name, right);
            newObject.transform.localPosition = pos.position;
            newObject.transform.eulerAngles = pos.rotation;
            Physics.SyncTransforms();
            if(_distance < location)
            {
                return false;
            }
        }

        return true;
    }

    Pos GetObjectOffset(ref float location,string name,bool right)
    {
        var forwardOffset = GetOffset(name);
        _street.GetSpline().Evaluate(location / _distance, out var splinePoint, out var tangentV, out var upVector);
        splinePoint = transform.InverseTransformPoint(splinePoint);

        Vector3 tan = tangentV;
        Vector3 position = splinePoint;

        Vector3 tangent = tan.normalized;


        tangent = transform.InverseTransformDirection(tangent);
        Vector3 normal = Vector3.Cross(transform.up, tangent).normalized;
        Vector3 lateralOffset = normal * forwardOffset * (right ? 1f : -1f);




        Vector3 finalPosition = position + lateralOffset;


        var rotation = Quaternion.LookRotation(tangent, transform.up);
        var y = rotation.eulerAngles.y;
        y += transform.eulerAngles.y;
        var rot = new Vector3(0, y, 0);

        rot.y -= 180 * (right ? 1 : 0);
        finalPosition.y += 30f;

        var pos = new Pos();
        pos.position = finalPosition;
        pos.rotation = rot;

        return pos;
    }

    void GetDistance()
    {
        _distance = _street.GetSpline().CalculateLength();
    }

    float GetOffset(string name)
    {
        if (name.Contains("tree"))
        {
            return 18;
        }
        if (name.Contains("lamp"))
        {
            return 14f;
        }
        return 25;
    }

    bool GetOverlap(GameObject _object)
    {
        var box = _object.GetComponentInChildren<BoxCollider>();
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
        if (hitColliders.Count() == 1)
            return false;

        foreach (var collider in hitColliders)
        {
            if (collider.gameObject != _object && !collider.name.ToLower().Contains("terrain"))
            {
                return true;
            }
        }
        return false;
    }


    


}
