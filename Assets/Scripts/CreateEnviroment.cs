using UnityEngine;

public class CreateEnviroment : MonoBehaviour
{
    public GameObject terrainPrefab;
    GameObject terrain;
    public GameObject playerPrefab;
    GameObject player;

    void Start()
    {
        terrain=Instantiate(terrainPrefab);
        player=Instantiate(playerPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
