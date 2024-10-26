using UnityEngine;

public class CameraScript : MonoBehaviour
{

    GameObject player;
    private float rotationX;
    private float rotationY;
    public float sensitivity;


    private bool firstPerson=false;
    private float smoothness=0.125f;

    private void Start()
    {
        sensitivity = 50f;
        player = GameObject.FindGameObjectWithTag("Player");
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        FollowPlayer();
        ChangeCamera();
        
    }

    void FollowPlayer()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            transform.position = player.transform.position + new Vector3(0, 1.5f, -3);
        }
        float offsetY;
        float offsetZ;
        if (firstPerson)
        {
            offsetY = 1.7f;
            offsetZ = 0;
        }
        else
        {
            offsetY = 2;
            offsetZ = 3;
        }
        var playerPosition = player.transform.position - player.transform.forward * offsetZ + player.transform.up * offsetY;
        transform.position = Vector3.Lerp(transform.position, playerPosition, smoothness);
        transform.localEulerAngles = player.transform.localEulerAngles;
    }

    void ChangeCamera()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            firstPerson = !firstPerson;
        }
    }


}
