using UnityEngine;

public class Player :  BaseCharacter
{
    public float stamina { get; set; }
    void Start()
    {

    }

    public Player()
    {
        damage = 10;
        hostile = true;
        stamina = 100;
    }

}
