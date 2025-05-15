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

    public void Prepare(bool right)
    {
        GetDistance();
        GetPropsPrefab();
        SpawnAlongTheBuilding(true);
        SpawnAlongTheBuilding(false,false);
        SpawnAlongTheBuilding(false, true);
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

    void SpawnAlongTheBuilding(bool isLamp, bool isTree=false)
    {
        var location = 0f;
        if (isLamp)
        {
            location =UnityEngine.Random.Range(5, 10);
        }

        while(location < _distance)
        {
            GameObject prop;
            if (isLamp) {
                prop = _lamp;
            }
            else {
                prop = GetRandomPrefab(isTree);
            }
           
            var ok=SpawnRandomObject(prop, ref location);

            if (!ok)
            {
                continue;
            }

            var size = prop.GetComponentInChildren<BoxCollider>().size.x;

            if(size > _distance - location)
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
    bool SpawnRandomObject(GameObject prop,ref float location)
    {
        var spawnPosition = GetObjectOffset(ref location, prop.name);
        
        GameObject newObject = Instantiate(prop, spawnPosition, gameObject.transform.rotation);
        newObject.transform.parent = gameObject.transform;
        while (GetOverlap(newObject))
        {
            location++;
            newObject.transform.position = GetObjectOffset(ref location,prop.name);
            Physics.SyncTransforms();
            if(_distance < location)
            {
                return false;
            }
        }

        return true;
    }

    Vector3 GetObjectOffset(ref float location,string name)
    {
        var forwardOffset = GetOffset(name);

        Vector3 spawnPosition = gameObject.transform.position + gameObject.transform.right * forwardOffset;
        var offset = location * gameObject.transform.forward;
        spawnPosition.y += 0.3f;
        spawnPosition += (_right ? -1 : 1) * offset;
        return spawnPosition;
    }

    void GetDistance()
    {
        _distance = GetComponent<BoxCollider>().size.z;
    }

    float GetOffset(string name)
    {
        if (name.Contains("tree"))
        {
            return 20f;
        }
        if (name.Contains("lamp"))
        {
            return 25f;
        }
        return 12f;
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
