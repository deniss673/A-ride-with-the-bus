using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Player :  BaseCharacter
{

    public GameObject PlayerUIprefab;
    GameObject PlayerUI;
    private Animator _anim;


    [Header("Movement Variables")]
    private Rigidbody rb;
    private bool isGrounded = false;
    private float rotationX;
    private float rotationY;
    public float sensitivity;
    private bool isSprinting;

    private float staminaTimer;
    private float sprintCooldown;

    private int sprintSpeed = 8;
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

    void Start()
    {
        PlayerUI = Instantiate(Resources.Load<GameObject>("PlayerUI"));
        _anim = GetComponent<Animator>();
        sensitivity = 50f;
        rb = GetComponent<Rigidbody>();
        lastPunchTime = Time.time;
        currentPunchClipLength = 0;
        isAttacking = false;
        isFighting = false;
    }

    public Player()
    {
        damage = 10;
        hostile = true;
        _stamina = 100;
        movementSpeed = 4;
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

        _anim.SetFloat("forward", directionVector.z);
        _anim.SetFloat("right", directionVector.x);



    }


    void Rotate()
    {
        rotationX -= Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity * -1;
        //rotationY += Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
        //rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        transform.localEulerAngles = new Vector3(0, rotationX, 0);
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
        if (punchComboStage == 2)
        {
            punchComboStage = 0;
        }
        punchComboStage++;
        isFighting = true;

        lastPunchTime = Time.time;

        punchComboStage = Mathf.Clamp(punchComboStage, 0, 2);

        _anim.ResetTrigger("isPunching1");
        _anim.ResetTrigger("isPunching2");

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
    





    #endregion

}
