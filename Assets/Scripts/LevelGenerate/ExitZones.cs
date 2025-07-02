using UnityEngine;

public class ExitZones : MonoBehaviour
{

    BoxCollider _box;
    

    private void Start()
    {
        _box = GetComponent<BoxCollider>();
    }


    public Vector3 GetRandomPositionInExitZone()
    {
        Vector3 localCenter = _box.center;
        Vector3 localSize = _box.size;

        Vector3 localRandom = new Vector3(
            Random.Range(-localSize.x / 2f, localSize.x / 2f),
            Random.Range(-localSize.y / 2f, localSize.y / 2f),
            Random.Range(-localSize.z / 2f, localSize.z / 2f)
        );
        Vector3 worldPoint = _box.transform.TransformPoint(localCenter + localRandom);

        //var finalPos = transform.parent.parent.InverseTransformPoint(worldPoint);

        var finalPos = worldPoint;
        
        return finalPos;
    }
}
