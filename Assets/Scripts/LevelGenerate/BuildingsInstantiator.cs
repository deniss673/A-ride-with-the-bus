using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Splines;
using static UnityEngine.Splines.SplineInstantiate;
using Unity.VisualScripting;
using System.Linq;
using Unity.Mathematics;
using UnityEngine.UIElements;
public class BuildingsInstantiator : MonoBehaviour
{
    private GameObject _buildingsGameObject;
    private List<GameObject> _buildingsPrefabs;
    private string _path = "Map/buildings";
    private List<GameObject> _buildings;
    private Vector3 _offset;

    //DE FACUT SCRIPTUL DE LA 0 PENTRU A INSTANTIA OBIECTE



    void GetBuildingsPrefab()
    {
        _buildingsPrefabs = Resources.LoadAll<GameObject>(_path).ToArray().ToList();
    }


    public void PrepareInstantiator()
    {
        GetBuildingsPrefab();
        _buildingsGameObject = new GameObject("Buildings");
        _buildings = new List<GameObject>() { };
        _offset = new Vector3(0, 0, 15);
    }

    public void InstantiateBuildings(Vector3 knotPosition)
    {
        AddBuilding(knotPosition, true);
        AddBuilding(knotPosition, false);


    }
    private void AddBuilding(Vector3 knotPosition,bool isRightSide)
    {
        var buildingPosition = knotPosition + (isRightSide == true ? -_offset : _offset);
        SpawnBuilding(buildingPosition);
    }

    private void SpawnBuilding(Vector3 pos)
    {
        System.Random sysRand = new System.Random();
        int random = (int)(sysRand.NextDouble() * _buildingsPrefabs.Count - 0);

        var building = _buildingsPrefabs[random];
        
        var obj = Instantiate(building);

        obj.transform.parent = _buildingsGameObject.transform;
        obj.transform.localPosition = pos;

        _buildings.Add(obj);
    }

    public void MoveBuildings(float deltaSpeed, float? moveAlongZ = null)
    {
        var LocalToWorldMatrix = _buildingsGameObject.transform.localToWorldMatrix;
        var WorldToLocalMatrix = _buildingsGameObject.transform.worldToLocalMatrix;
        foreach (var obj in _buildings) 
        {
            float3 worldPos = math.mul(LocalToWorldMatrix, new float4(obj.transform.position, 1)).xyz;

            if (moveAlongZ!=null)
                worldPos.z += deltaSpeed;
            else
                worldPos.x -= deltaSpeed;

            obj.transform.position = math.mul(WorldToLocalMatrix, new float4(worldPos, 1)).xyz;
        }

    }


    public void RemoveBuilding() 
    {
        
    }

    public GameObject GetBuildingGameObj()
    {
        return _buildingsGameObject;
    }



}
