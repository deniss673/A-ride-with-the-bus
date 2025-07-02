using UnityEngine;

public class Seat : MonoBehaviour
{
    private BoxCollider _seat;
    private bool _occupied = false;
    public bool debug;

    void Start()
    {
        _seat = GetComponent<BoxCollider>();
    }

    void Update()
    {
        if (debug)
        {
            debug = false;
            Debug.Log(GetCenter());
        }
    }


    public Vector3 GetCenter()
    {
        var pos = _seat.center;


        return _seat.transform.TransformPoint(pos) - 0.2f * transform.right;
    }

    public float RotationAngle()
    {
        return transform.eulerAngles.y - 90;
    }

    public bool GetOccupied()
    {
        return _occupied;
    }
    public void SetOccupied()
    {
        _occupied = !_occupied;
    }
}
