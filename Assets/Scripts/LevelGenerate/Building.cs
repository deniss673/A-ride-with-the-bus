using UnityEngine;
using UnityEngine.UIElements;

public class Building
{
    GameObject _object;

    public Building (GameObject Object)
    {
        _object = Object;
    }

    public GameObject GetObject()
    {
        return _object;
    }

    public Vector3 GetBounds()
    {
        return _object.GetBuildingBounds();
    }

    public bool GetOverlap()
    {
        var box = _object.GetComponent<BoxCollider>();
        if (box == null)
        {
            Debug.LogWarning("GameObject-ul nu are BoxCollider.");
            return false;
        }

        Vector3 center = box.transform.TransformPoint(box.center);
        Vector3 halfExtents = Vector3.Scale(box.size * 0.5f, box.transform.lossyScale);
        Quaternion orientation = box.transform.rotation;
        Collider[] hitColliders = Physics.OverlapBox(center, halfExtents, orientation);

        foreach (var collider in hitColliders)
        {
            if (collider.gameObject != _object)
            {
                return true;
            }
        }
        return false;
    }

}


public static class BuildingHelper
{
    public static Vector3 GetBuildingBounds(this GameObject _object)
    {
        Collider collider = _object.GetComponent<Collider>();
        if (collider != null)
        {
            Bounds bounds = collider.bounds;
            Vector3 size = bounds.size;
            return size;
        }
        return Vector3.zero;
    }

    public static float GetBuildingWidth(this GameObject _object, Vector3 normal)
    {
        var box = _object.GetComponent<BoxCollider>();

        Transform t = box.transform;

        Vector3 size = box.size;
        Vector3 scale = t.lossyScale;

        Vector3 worldSize = Vector3.Scale(size, scale);

        Vector3 right = t.rotation * Vector3.right;
        Vector3 up = t.rotation * Vector3.up;
        Vector3 forward = t.rotation * Vector3.forward;

        float width =
            Mathf.Abs(Vector3.Dot(normal, right)) * worldSize.x +
            Mathf.Abs(Vector3.Dot(normal, up)) * worldSize.y +
            Mathf.Abs(Vector3.Dot(normal, forward)) * worldSize.z;

        return width;

    }

    public static bool GetOverlap(this GameObject _object)
    {
        var box = _object.GetComponent<BoxCollider>();
        if (box == null)
        {
            Debug.LogWarning("GameObject-ul nu are BoxCollider.");
            return false;
        }
      
        Vector3 center = box.transform.TransformPoint(box.center);
        Vector3 halfExtents = Vector3.Scale(box.size * 0.5f, box.transform.lossyScale);
        Quaternion orientation = box.transform.rotation; 
        Collider[] hitColliders = Physics.OverlapBox(center, halfExtents, orientation);

        foreach (var collider in hitColliders)
        {
            if(collider.gameObject != _object)
            {
                return true;
            }
        }
        return false;
    }

    static public void Combine(this GameObject _object)
    {
        MeshFilter[] meshFilters = _object.GetComponentsInChildren<MeshFilter>();
        var combineList = new System.Collections.Generic.List<CombineInstance>();
        Material sharedMaterial = null;

        foreach (MeshFilter mf in meshFilters)
        {
            if (mf == _object.GetComponent<MeshFilter>()) continue; 
            if (mf.sharedMesh == null) continue; 
            var renderer = mf.GetComponent<MeshRenderer>();
            if (renderer == null) continue; 
            if (!renderer.enabled) continue;

            CombineInstance ci = new CombineInstance
            {
                mesh = mf.sharedMesh,
                transform = mf.transform.localToWorldMatrix
            };
            combineList.Add(ci);

            if (sharedMaterial == null)
                sharedMaterial = renderer.sharedMaterial;

            mf.gameObject.SetActive(false);
        }

        if (combineList.Count == 0)
        {
            Debug.LogWarning("Nu s-au gasit meshuri valide pentru combinare.");
            return;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; 
        combinedMesh.CombineMeshes(combineList.ToArray(), true, true);

        _object.GetComponent<MeshFilter>().mesh = combinedMesh;
        _object.GetComponent<MeshRenderer>().material = sharedMaterial;
    }
}