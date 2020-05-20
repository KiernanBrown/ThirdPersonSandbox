
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CharacterController))]
public class PlayerHandler : MonoBehaviour {

	public float InputX;
	public float InputZ;
	public Vector3 desiredMoveDirection;
	public bool blockRotationPlayer;
	public float desiredRotationSpeed = 0.1f;
	public Animator anim;
	public float Speed;
	public float allowPlayerRotation = 0.1f;
	public Camera cam;
	public CharacterController controller;
	public bool isGrounded;
    public float maxJumpSpeed = 0.3f;
    public float maxFallSpeed = -0.3f;
    public float verticalVel = 0.0f;
    public Vector3 moveVector;
    public bool evadeOffset = false;
    private int prevAttackState = 0;

    [Header("Animation Smoothing")]
    [Range(0, 1f)]
    public float HorizontalAnimSmoothTime = 0.2f;
    [Range(0, 1f)]
    public float VerticalAnimTime = 0.2f;
    [Range(0,1f)]
    public float StartAnimTime = 0.3f;
    [Range(0, 1f)]
    public float StopAnimTime = 0.15f;

    [Header("Player States")]
    public bool actionable = true;
    public bool canStomp = false;
    public bool canEvade = true;
    public int attackState;
    public bool guarding = false;
    public bool evading = false;
    public bool jumping = false;
    public bool hasJumped = false;
    public Vector3 jumpDirection;

    [Header("Effects")]
    public CinemachineFreeLook cmCamera;
    public ParticleSystem stompParticles;

    // Use this for initialization
    void Start () {
		anim = this.GetComponent<Animator>();
		cam = Camera.main;
		controller = this.GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
        isGrounded = controller.isGrounded;
        if (jumping || hasJumped)
        {
            if (isGrounded && hasJumped)
            {
                anim.SetBool("Jumping", false);
                jumping = false;
                hasJumped = false;
                actionable = true;
                StopCoroutine("Jump");
            }
            else
            {
                controller.Move(jumpDirection);
            }
        }

        InputMagnitude();

        // Decrease vertical velocity if the player is not grounded
        if (isGrounded && !jumping)
        {
            verticalVel = -0.000155f;
        }
        else
        {
          if (jumping) {
            verticalVel -= 0.0001f;
            verticalVel = Mathf.Clamp(verticalVel, 0.0f, maxJumpSpeed);
            
          }
          else {
            verticalVel -= 0.0012f;
          }

        }

        if (attackState > 0 && !actionable)
        {
            verticalVel = Mathf.Clamp(verticalVel, 0.0f, maxJumpSpeed);
        }

        // Handle Player Actions
        attackState = anim.GetInteger("AttackState");
        if (Input.GetButtonDown("Fire1") && actionable == true)
        {
            CancelActions();
            StartCoroutine("Hit");
        }
        if (Input.GetButtonDown("Fire2") && actionable == true && canStomp)
        {
            CancelActions();
            StartCoroutine("Stomp");
        }
        if (Input.GetButtonDown("Fire3") && actionable == true && canEvade)
        {
            CancelActions();
            StartCoroutine("Evade");
        }
        if (Input.GetButtonDown("Jump") && actionable == true && isGrounded)
        {
            CancelActions();
            StartCoroutine("Jump");
        }

        moveVector = new Vector3(0, verticalVel, 0);
		controller.Move(moveVector);
	}

	void PlayerMoveAndRotation() {
		InputX = Input.GetAxis("Horizontal");
		InputZ = Input.GetAxis("Vertical");

		var camera = Camera.main;
		var forward = cam.transform.forward;
		var right = cam.transform.right;

		forward.y = 0f;
		right.y = 0f;

		forward.Normalize ();
		right.Normalize ();

		desiredMoveDirection = forward * InputZ + right * InputX;

		if (blockRotationPlayer == false) {
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (desiredMoveDirection), desiredRotationSpeed);
		}
	}

	void InputMagnitude() {
		// Calculate Input Vectors
		InputX = Input.GetAxis("Horizontal");
		InputZ = Input.GetAxis("Vertical");

		anim.SetFloat("InputZ", InputZ, VerticalAnimTime, Time.deltaTime * 2f);
		anim.SetFloat("InputX", InputX, HorizontalAnimSmoothTime, Time.deltaTime * 2f);

		// Calculate the Input Magnitude
		Speed = new Vector2(InputX, InputZ).sqrMagnitude;

		// Physically move player
		if (Speed > allowPlayerRotation) {
			anim.SetFloat("InputMagnitude", Speed, StartAnimTime, Time.deltaTime);
			PlayerMoveAndRotation ();
		} else if (Speed < allowPlayerRotation) {
			anim.SetFloat("InputMagnitude", Speed, StopAnimTime, Time.deltaTime);
		}
	}

    IEnumerator Hit()
    {
        int modifiedAttackState = anim.GetInteger("AttackState") + prevAttackState;
        if (modifiedAttackState == 0 || anim.GetInteger("AttackState") == 10)
        {
            StartHit(1);
            yield return new WaitForSeconds(0.3f);
            actionable = true;
            canStomp = true;
            yield return new WaitForSeconds(0.5f);
            RestartCombo();
        }
        else if (modifiedAttackState == 1)
        {
            StartHit(2);
            yield return new WaitForSeconds(0.35f);
            actionable = true;
            canStomp = true;
            yield return new WaitForSeconds(0.7f);
            RestartCombo();
        }
        else if (modifiedAttackState == 2)
        {
            StartHit(3);
            yield return new WaitForSeconds(0.95f);
            actionable = true;
            yield return new WaitForSeconds(0.05f);
            canStomp = true;
            yield return new WaitForSeconds(0.5f);
            RestartCombo();
        }
        else if (modifiedAttackState == 3)
        {
            StartHit(4);
            yield return new WaitForSeconds(0.75f);
            anim.SetInteger("AttackState", 5);
            yield return new WaitForSeconds(0.35f);
            anim.SetInteger("AttackState", 6);
            yield return new WaitForSeconds(0.3f);
            anim.SetInteger("AttackState", 0);
            yield return new WaitForSeconds(0.15f);
            RestartCombo();
        }
    }

    void StartHit(int hitNum)
    {
        prevAttackState = 0;
        blockRotationPlayer = true;
        anim.SetInteger("AttackState", hitNum);
        DisableActions();
    }

    IEnumerator Stomp()
    {
        anim.SetInteger("AttackState", 10);
        actionable = false;
        yield return new WaitForSeconds(0.48f);
        ShakeCamera();
        stompParticles.Play();
        yield return new WaitForSeconds(0.38f);
        actionable = true;
        yield return new WaitForSeconds(0.525f);
        RestartCombo();
    }

    IEnumerator Evade()
    {
        if (Mathf.Abs(InputX) < 0.5 && Mathf.Abs(InputZ) < 0.5)
        {
            RestartCombo();
            anim.SetInteger("EvadeState", 1);
            blockRotationPlayer = true;
            actionable = false;
            yield return new WaitForSeconds(0.15f);
            guarding = true;
            yield return new WaitForSeconds(0.85f);
            guarding = false;
            yield return new WaitForSeconds(0.15f);
            anim.SetInteger("EvadeState", 0);
            actionable = true;
            blockRotationPlayer = false;
        }
        else
        {
            if (evadeOffset)
            {
                prevAttackState = anim.GetInteger("AttackState");
                anim.SetInteger("AttackState", 0);
            }
            else RestartCombo();
            anim.SetInteger("EvadeState", 2);
            blockRotationPlayer = true;
            actionable = false;
            yield return new WaitForSeconds(0.10f);
            evading = true;
            yield return new WaitForSeconds(0.35f);
            evading = false;
            yield return new WaitForSeconds(0.10f);
            blockRotationPlayer = false;
            anim.SetInteger("EvadeState", 0);
            yield return new WaitForSeconds(0.05f);
            actionable = true;
            blockRotationPlayer = false;
            if (evadeOffset)
            {
                yield return new WaitForSeconds(0.5f);
                RestartCombo();
            }
        }
    }

    IEnumerator Jump()
    {
        jumpDirection = desiredMoveDirection / 23.5f;
        blockRotationPlayer = true;
        jumping = true;
        anim.SetBool("Jumping", true);
        actionable = false;
        yield return new WaitForSeconds(0.2f);
        verticalVel = 0.0255f;
        yield return new WaitForSeconds(0.15f);
        actionable = true;
        hasJumped = true;
        blockRotationPlayer = false;
        yield return new WaitForSeconds(0.15f);
        jumping = false;
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("Jumping", false);
        hasJumped = false;
    }
    
    void CancelJump() {
        StopCoroutine("Jump");
        jumping = false;
        anim.SetBool("Jumping", false);
        hasJumped = false;
    }

    void RestartCombo()
    {
        anim.SetInteger("AttackState", 0);
        prevAttackState = 0;
        actionable = true;
        blockRotationPlayer = false;
        canStomp = false;
    }

    void DisableActions()
    {
        actionable = false;
        canStomp = false;
    }

    void CancelActions()
    {
        StopCoroutine("Hit");
        StopCoroutine("Stomp");
        StopCoroutine("Evade");
        CancelJump();
    }

    void ShakeCamera()
    {
        cmCamera.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
    }
}
