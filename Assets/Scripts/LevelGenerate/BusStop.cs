using UnityEngine;

public class BusStop : MonoBehaviour
{

    private bool _stopped = false;
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public void SetStopped()
    {
        _stopped = true;
    }

    public bool IsStopped()
    {
        return _stopped;
    }
}
