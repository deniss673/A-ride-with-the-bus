using UnityEngine;

public class CameraDrone : MonoBehaviour
{
    public Camera droneCamera;
    public float droneSpeed = 10f;
    public float droneRotationSpeed = 100f;
    bool _active = false;


    void Start()
    {
        
    }

    void Update()
    {
        if (_active)
            HandleDroneControls();
    }

    public void SetActive(bool active)
    {
        _active = true;
        droneCamera.enabled = active;
    }


    void HandleDroneControls()
    {
        float moveHorizontal = Input.GetAxis("Horizontal"); 
        float moveVertical = Input.GetAxis("Vertical");     

        Vector3 forward = droneCamera.transform.forward;
        Vector3 right = droneCamera.transform.right;
        forward.y = 0f;
        right.y = 0f;

        Vector3 direction = (forward * moveVertical + right * moveHorizontal).normalized;

        if (Input.GetKey(KeyCode.Keypad8)) 
            droneCamera.transform.position += Vector3.up * droneSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Keypad2)) 
            droneCamera.transform.position -= Vector3.up * droneSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Keypad7)) 
            droneCamera.transform.position += droneCamera.transform.forward * droneSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.Keypad1))
            droneCamera.transform.position -= droneCamera.transform.forward * droneSpeed * Time.deltaTime;

        droneCamera.transform.position += direction * droneSpeed * Time.deltaTime;

        float rotationY = 0f;
        if (Input.GetKey(KeyCode.Keypad4))
            rotationY = -1f;
        if (Input.GetKey(KeyCode.Keypad6))
            rotationY = 1f;

        droneCamera.transform.Rotate(0f, rotationY * droneRotationSpeed * Time.deltaTime, 0f);
    }
}
