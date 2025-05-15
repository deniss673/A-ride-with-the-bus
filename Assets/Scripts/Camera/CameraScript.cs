using UnityEngine;

public class CameraScript : MonoBehaviour
{

    GameObject player;
    private float rotationX;
    private float rotationY;
    public float sensitivity;


    private bool firstPerson=false;
    private float smoothness=0.125f;
    private Vector3 _currentVelocity = Vector3.zero;
    private Vector3 _minOffset = new Vector3(0.5f,0.5f,0.5f);

    
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
            transform.position = Vector3.Lerp(transform.position,player.transform.position + new Vector3(0, 2.5f, -3), Time.deltaTime);
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
        var targetPosition = Vector3.SmoothDamp(transform.position, playerPosition, ref _currentVelocity, smoothness);


        transform.position = Vector3.SmoothDamp(transform.position, playerPosition, ref _currentVelocity, smoothness);
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
