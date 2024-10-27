using UnityEngine;

public class Movement : Player
{
    private Rigidbody rb;
    private bool isGrounded=false;
    private float rotationX;
    private float rotationY;
    public float sensitivity;
    private bool isSprinting;

    private float staminaTimer;
   


    private void Start()
    {
        sensitivity = 50f;
        rb=GetComponent<Rigidbody>();
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
            if (Input.GetKeyDown(KeyCode.Space) && stamina>20)
            {
                rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
                stamina = stamina - 20;
            }
        }
    }

    void PlayerMovement()
    {
        var direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.Translate(direction * movementSpeed * Time.deltaTime);
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
        if (isGrounded && !isSprinting && stamina<100)
        {
            stamina=stamina+10*Time.deltaTime;
            
        }
        if (stamina < 0)
        {
            stamina = 0;
        }
        if (isSprinting && stamina > 0)
        {
            staminaTimer += Time.deltaTime;
            if(staminaTimer > 0.2f)
            {
                staminaTimer= 0;
                stamina = stamina - 3;
            }
        }

    }

    void Sprint()
    {
        if (isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && stamina > 0)
            {
                movementSpeed = 30;
                isSprinting = true;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                movementSpeed = 5;
                isSprinting = false;
            }
        }
    }


}
