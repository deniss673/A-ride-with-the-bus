using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 3f;
    private Rigidbody rb;
    private bool isGrounded=false;


    private void Start()
    {
        rb=GetComponent<Rigidbody>();
    }

    public void Update()
    {
        var direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.Translate(direction*moveSpeed*Time.deltaTime);

        if (isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(Vector3.up * 200);
            }
        }

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


}
