using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Player :  BaseCharacter
{
    private float _stamina;
    public float stamina {
        get { return _stamina; }
        set
        {
            _stamina = Mathf.Clamp(value, MinStamina, MaxStamina);
        } 
    }
    private float MinStamina = 0;
    private float MaxStamina = 100;
    public GameObject PlayerUIprefab;
    GameObject PlayerUI;
    void Start()
    {
        PlayerUI = Instantiate(PlayerUIprefab);
    }

    public Player()
    {
        damage = 10;
        hostile = true;
        _stamina = 100;
    }

    private void Update()
    {
        SetStamina();
    }

    void SetStamina()
    {
        PlayerUI.GetComponent<Stats>().ChangeSlider(stamina);
    }

}
