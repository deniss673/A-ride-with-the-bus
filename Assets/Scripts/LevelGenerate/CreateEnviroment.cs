using UnityEngine;

public class CreateEnviroment : MonoBehaviour
{
    GameObject terrain;
    GameObject player;

    void Start()
    {
        terrain=Instantiate(Resources.Load<GameObject>("Terrain"));
        player= Instantiate(Resources.Load<GameObject>("Player"));
        Instantiate(Resources.Load<GameObject>("Cube"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
