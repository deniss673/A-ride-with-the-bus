using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DebugClass : MonoBehaviour
{
    public bool showSize = false;
    public Vector3 tangent;
    public Vector3 normal;
    public float distance;
    public Vector3 position;
    public float x;
    public float curve;


    public bool testOverlap = false;

    private void Update()
    {
        if (showSize)
        {
            showSize = false;
            var building = gameObject;
            var size = building.GetBuildingBounds();
            float projectedLength = Mathf.Abs(Vector3.Dot(building.transform.forward, tangent.normalized)) * size.z;
            float width = Mathf.Abs(Vector3.Dot(normal, size));
            UnityEngine.Debug.Log($"Width= {projectedLength}");
            UnityEngine.Debug.Log($"Size= {size}");
            Debug.Log($"Distance = {distance}");
            Debug.Log($"Position = {position}");
            Debug.Log($"Position of x = {x}");
            Debug.Log($"Curve = {curve}");
        }
        if (testOverlap) {
            testOverlap = false;
            GetOverlap();
        }
    }

    public bool GetOverlap()
    {
        var _object = this.gameObject;
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
        int i = 0;


        while (i < hitColliders.Length)
        {
            Debug.Log("Hit : " + hitColliders[i].name + i);
            i++;
        }
        Debug.Log("TESTED");
        return false;
    }



}
