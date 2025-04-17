using Cinemachine;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Player :  BaseCharacter
{

    public GameObject PlayerUIprefab;
    GameObject PlayerUI;
    private Animator _anim;
    private CinemachineFreeLook _camera;




    [Header("Movement Variables")]
    private Rigidbody rb;
    private bool isGrounded = false;
    private float rotationX;
    private float rotationY;
    public float sensitivity;
    private bool isSprinting;
    private float _targetX=0;

    private float staminaTimer;
    private float sprintCooldown;

    private int sprintSpeed = 6;
    private readonly float MinStamina = 0;
    private readonly float MaxStamina = 100;
    private float _stamina;
    public float stamina
    {
        get { return _stamina; }
        set
        {
            _stamina = Mathf.Clamp(value, MinStamina, MaxStamina);
        }
    }

    [Header("Combat Variables")]
    private float combatTimer;
    private bool isFighting=false;
    private float lastPunchTime;
    private int punchComboStage = 0;
    private float currentPunchClipLength;
    private bool isAttacking;
    private bool hittedSomething=false;
    void Start()
    {

        //DE PUS INTR-UN SCRIPT SEPARAT
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        PlayerUI = Instantiate(Resources.Load<GameObject>("PlayerUI"));
        _anim = GetComponent<Animator>();
        sensitivity = 50f;
        rb = GetComponent<Rigidbody>();
        lastPunchTime = Time.time;
        currentPunchClipLength = 0;
        isAttacking = false;
        isFighting = false;
        _camera = GameObject.FindGameObjectWithTag("camera").GetComponent<CinemachineFreeLook>();
    }

    public Player()
    {
        damage = 10;
        hostile = true;
        _stamina = 100;
        movementSpeed = 3;
    }

    private void Update()
    {
        SetStamina();

        CombatLogic();

        MovementLogic();


    }


    #region Movement

    void SetStamina()
    {
        PlayerUI.GetComponent<Stats>().ChangeSlider(stamina);
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
            if (Input.GetKeyDown(KeyCode.Space) && stamina > 20)
            {
                rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
                stamina = stamina - 20;
            }
        }
    }

    void PlayerMovement()
    {
        var direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        var directionVector = direction * movementSpeed;

        direction = direction * movementSpeed * Time.deltaTime;

        transform.Translate(direction);

        _anim.SetFloat("forward", directionVector.z,0.15f,Time.deltaTime);
        _anim.SetFloat("right", directionVector.x,0.15f, Time.deltaTime);

    }


    void Rotate()
    {
        //rotationX -= Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity * -1;
        //rotationY += Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
        //rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        if (_targetX == 0)
        {
            rotationX = _camera.m_XAxis.Value;
        }
        else
        {
            rotationX = Mathf.Lerp(_camera.m_XAxis.Value, _targetX, Time.deltaTime * 2f);
            _camera.m_XAxis.Value = Mathf.Lerp(_camera.m_XAxis.Value, _targetX, Time.deltaTime * 2f);
        }

        transform.localEulerAngles = new Vector3(0, rotationX, 0);

        if(_camera.m_XAxis.Value + 10 > _targetX && _camera.m_XAxis.Value - 10 < _targetX && _targetX != 0)
        {
            _targetX = 0;
        }
    }

    void StaminaManager()
    {
        if (isGrounded && !isSprinting)
        {
            stamina = stamina + 10 * Time.deltaTime;

        }
        if (stamina < 20)
        {
            sprintCooldown = 1f;
        }
        if (isSprinting)
        {
            staminaTimer += Time.deltaTime;
            if (staminaTimer > 0.05f)
            {
                staminaTimer = 0;
                stamina = stamina - 1;
            }
        }
        if (sprintCooldown > 0 && !isSprinting)
        {
            sprintCooldown -= Time.deltaTime;
        }

    }

    void Sprint()
    {
        if (isGrounded && !(sprintCooldown > 0))
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && stamina > 0)
            {
                SetSprint(true);
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                SetSprint(false);
            }
        }
        else if (stamina < 1)
        {
            SetSprint(false);
        }
    }

    void SetSprint(bool value)
    {
        isSprinting = value;
        movementSpeed = value ? sprintSpeed : movementSpeed;
    }


    void MovementLogic()
    {
        if (!isAttacking)
        {
            PlayerMovement();
            Sprint();
        }

        CheckGround();

        Rotate();

        StaminaManager();

        
    }
    #endregion

    #region Combat

    public void UpdateCurrentPunchClipLength()
    {
        AnimationClip[] clips = _anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if(clip.name == "punch" + punchComboStage)
                currentPunchClipLength = clip.length/1.5f;
        }
    }
    void Punch()
    {
        LockInEnemy();
        hittedSomething = false;
        if (punchComboStage == 3)
        {
            punchComboStage = 0;
        }
        punchComboStage++;
        isFighting = true;

        lastPunchTime = Time.time;

        

        _anim.ResetTrigger("isPunching1");
        _anim.ResetTrigger("isPunching2");
        _anim.ResetTrigger("isPunching3");

        var animationTrigger = "isPunching" + punchComboStage;


        _anim.SetTrigger(animationTrigger);
        _anim.SetTrigger("combat");

        UpdateCurrentPunchClipLength();


    }

    void Combo()
    {
        if (Time.time - lastPunchTime > currentPunchClipLength-0.1f)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Punch();
                combatTimer = 3;
            }
        }
        if (punchComboStage != 0 && Time.time - lastPunchTime > 1.5f) {
            punchComboStage = 0;
        }
    }


    void CombatLogic()
    {
        Combo();

        if (isFighting) {
            combatTimer -= Time.deltaTime;
        }
        if (combatTimer < 0 && isFighting) { 
            isFighting = false;
            _anim.SetBool("isFighting",false);
        }
        

    }

    public void SetAttacking(bool ok)
    {
        isAttacking = ok;
    }

    void LockInEnemy()
    {
        List<Collider> enemies = Physics.OverlapSphere(transform.position, 1.5f).Where(obj => obj.gameObject.tag == "NPC").ToList();

        

        var lockedIn = enemies
            .Select(e => e.transform)
            .Where(t => IsInFront(t))
            .OrderBy(t => Vector3.SqrMagnitude(t.position - transform.position))
            .FirstOrDefault();

        if (enemies.Count == 0 || lockedIn==null)
            return;

        Vector3 direction = lockedIn.position - transform.position;
        direction.y = 0; 

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Vector3 eulerRotation = targetRotation.eulerAngles;

        _targetX = eulerRotation.y;

        if (_targetX > 180)
        {
            _targetX = -(360 - _targetX);
        }

    }

    private bool IsInFront(Transform target)
    {
        Vector3 toTarget = (target.position - transform.position).normalized;
        return Vector3.Dot(transform.forward, toTarget) > Mathf.Cos(90 * Mathf.Deg2Rad);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("NPC") && isAttacking && hittedSomething == false)
        {
            other.gameObject.GetComponent<NpcScript>().TakeDamage(20);
            hittedSomething = true;
        }
    }




    #endregion
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        List<BoxCollider> boxes = new List<BoxCollider>();
        boxes.AddRange(GetComponents<BoxCollider>());
        boxes.AddRange(GetComponentsInChildren<BoxCollider>());
        foreach(var box in boxes)
        {
            if (box != null)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(box.center, box.size);
            }
        }
            
    }
}
