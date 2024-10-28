using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody rb;
    private bool isGrounded=false;
    private float rotationX;
    private float rotationY;
    public float sensitivity;
    private bool isSprinting;

    private float staminaTimer;
    private Player player;
    private float sprintCooldown;

    private int movementSpeed=5;
    private int sprintSpeed=30;


    private void Start()
    {
        sensitivity = 50f;
        rb=GetComponent<Rigidbody>();
        player = GetComponent<Player>();
    }

    public void Update()
    {
        PlayerMovement();

        CheckGround();

        Rotate();

        StaminaManager();

        Sprint();

        
    }



    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }

    void CheckGround()
    {
        if (isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space) && player.stamina>20)
            {
                rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
                player.stamina = player.stamina - 20;
            }
        }
    }

    void PlayerMovement()
    {
        var direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.Translate(direction * player.movementSpeed * Time.deltaTime);
    }

    void Rotate()
    {
        rotationX -= Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity *-1;
        //rotationY += Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
        //rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        transform.localEulerAngles = new Vector3(0, rotationX, 0);
    }

    void StaminaManager()
    {
        if (isGrounded && !isSprinting)
        {
            player.stamina=player.stamina +10*Time.deltaTime;
            
        }
        if (player.stamina < 20)
        {
            sprintCooldown = 1f;
        }
        if (isSprinting)
        {
            staminaTimer += Time.deltaTime;
            if(staminaTimer > 0.05f)
            {
                staminaTimer= 0;
                player.stamina = player.stamina - 1;
            }
        }
        if(sprintCooldown>0 && !isSprinting)
        {
            sprintCooldown -= Time.deltaTime;
        }

    }

    void Sprint()
    {
        if (isGrounded && !(sprintCooldown>0))
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && player.stamina > 0)
            {
                SetSprint(true);
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                SetSprint(false);
            }
        }
        else if(player.stamina<1)
        {
            SetSprint(false);
        }
    }

    void SetSprint(bool value)
    {
        isSprinting = value;
        player.movementSpeed = value ? sprintSpeed : movementSpeed;
    }



}
