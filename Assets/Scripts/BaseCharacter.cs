using UnityEngine;

public class BaseCharacter : MonoBehaviour
{
    public int health { get; set; }
    public int movementSpeed { get; set; }
    public int damage { get; set; }
    public bool hostile {  get; set; }

    public BaseCharacter()
    {
        health = 100;
        movementSpeed = 5;
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
