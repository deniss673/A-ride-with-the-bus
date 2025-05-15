using Unity.VisualScripting;
using UnityEngine;

public class TestingOverlap : MonoBehaviour
{
    public bool overLap = false;
    void Start()
    {
        
    }

    void Update()
    {
        if( overLap)
        {
            overLap = false;
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
        Collider[] hitColliders = Physics.OverlapBox(_object.transform.position, _object.transform.localScale / 2, Quaternion.identity);
        int i = 0;
        while (i < hitColliders.Length)
        {
            //Output all of the collider names
            Debug.Log("Hit : " + hitColliders[i].name + i);
            //Increase the number of Colliders in the array
            
            i++;
        }
        Debug.Log("TESTED");
        return false;
    }
}
