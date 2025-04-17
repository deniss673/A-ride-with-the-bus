using UnityEngine;

public interface IRoadComponentsInterface
{
    public void MoveKnots(float? diff = null);
    public Quaternion GetLastKnotsAngle();
    public Vector3 GetLastKnotPos();
    public bool DoneGenerate();
    public Vector3 GetCurrentKnot();
    public Quaternion RotateSpline();
    public bool WentPast(float pos);
    public void SetIsCurrentStreet(bool ok);
}
