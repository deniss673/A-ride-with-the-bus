using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NpcSpawner : MonoBehaviour
{
    private float _maxNumber;
    private List<GameObject> _npcs;
    void Start()
    {
        SetVariables();
        InstantiateNpcs();
    }
    void Update()
    {
        
    }

    void InstantiateNpcs()
    {
        for (int i = 0; i < _maxNumber; i++)
        {
            SpawnNpc();
        }
    }

    void SpawnNpc()
    {
        var go = GetRandomNpc();

        var instGo = Instantiate(go);
        instGo.transform.parent = transform;
    }

    GameObject GetRandomNpc()
    {
        var random = UnityEngine.Random.Range(0, _npcs.Count);

        return _npcs[random];
        
    }

    void SetVariables()
    {
        _npcs = Resources.LoadAll<GameObject>("Map/npcs").ToList();
        _maxNumber = Random.Range(10, 30);
    }


}
