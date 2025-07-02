using UnityEngine;
using UnityEngine.SceneManagement;

public class CommandManager : MonoBehaviour
{

    ItemManager _itemManager;
    bool _droneActive = false;
    public Camera _mainCamera;
    void Start()
    {
        _itemManager = FindFirstObjectByType<ItemManager>();
    }

    void Update()
    {
        if (_itemManager == null) {
            _itemManager = FindFirstObjectByType<ItemManager>();
        }
        if (_itemManager != null) {
            ItemUses();
        }

    }


    void ItemUses()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _itemManager.GetHeal();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _itemManager.GetDamage();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _itemManager.UseBook();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneManager.LoadScene("Lost");
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            SceneManager.LoadScene("Win");
        }


        if (Input.GetKeyDown(KeyCode.C))
        {
            SetActiveCamera();
            FindFirstObjectByType<CameraDrone>().SetActive(!_droneActive);
            _droneActive = !_droneActive;
        }


        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            FindFirstObjectByType<Player>().SetInfiniteHealth();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            FindFirstObjectByType<Player>().SetHealthBack();
        }

    }


    void SetActiveCamera()
    {
        _mainCamera.enabled= _droneActive;
    }


}
