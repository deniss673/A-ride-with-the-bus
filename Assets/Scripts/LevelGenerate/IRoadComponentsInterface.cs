using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public interface IRoadComponentsInterface
{
    public void MoveKnots(float? diff = null);
    public Quaternion GetLastKnotsAngle();
    public Vector3 GetLastKnotPos();
    public bool DoneGenerate();
    public Vector3 GetCurrentKnot();
    public Vector3 GetFutureKnot();
    public Quaternion RotateSpline();
    public bool WentPast(float pos);
    public void SetIsCurrentStreet(bool ok);
    public List<BezierKnot> GetKnots();
    public SplineContainer GetSpline();
    public void Accelerate();
    public void Deccelerate();
    public float GetSpeed();
}
