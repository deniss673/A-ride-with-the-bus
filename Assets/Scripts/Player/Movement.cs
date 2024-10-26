using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 3f;
    private Rigidbody rb;
    private bool isGrounded=false;
    private float rotationX;
    private float rotationY;
    public float sensitivity;

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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
            }
        }
    }

    void PlayerMovement()
    {
        var direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    void Rotate()
    {
        rotationX -= Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity *-1;
        //rotationY += Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
        //rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        transform.localEulerAngles = new Vector3(0, rotationX, 0);
    }


}
