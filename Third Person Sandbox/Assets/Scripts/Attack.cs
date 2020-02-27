using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public bool actionable = true;
    public bool canStomp = false;
    public bool canEvade = true;
    public Animator anim;
    public int state;
    public bool guarding = false;
    public bool evading = false;
    public MovementInput movement;

    // Start is called before the first frame update
    void Start()
    {
        movement = this.GetComponent<MovementInput>();
    }

    // Update is called once per frame
    void Update()
    {
        state = anim.GetInteger("AttackState");
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
    }

    IEnumerator Hit()
    {
        if (anim.GetInteger("AttackState") == 0 || anim.GetInteger("AttackState") == 10)
        {
            movement.blockRotationPlayer = true;
            anim.SetInteger("AttackState", 1);
            DisableActions();
            yield return new WaitForSeconds(0.3f);
            actionable = true;
            canStomp = true;
            yield return new WaitForSeconds(0.5f);
            RestartCombo();       
        }
        else if (anim.GetInteger("AttackState") == 1)
        {
            anim.SetInteger("AttackState", 2);
            DisableActions();
            yield return new WaitForSeconds(0.35f);
            actionable = true;
            canStomp = true;
            yield return new WaitForSeconds(0.7f);
            RestartCombo();
        }
        else if (anim.GetInteger("AttackState") == 2)
        {
            anim.SetInteger("AttackState", 3);
            DisableActions();
            yield return new WaitForSeconds(0.35f);
            movement.verticalVel = 0.0325f;
            yield return new WaitForSeconds(0.65f);
            actionable = true;
            yield return new WaitForSeconds(0.1f);
            canStomp = true;
            yield return new WaitForSeconds(0.4f);
            RestartCombo();
        }
        else if (anim.GetInteger("AttackState") == 3)
        {
            anim.SetInteger("AttackState", 4);
            DisableActions();
            yield return new WaitForSeconds(0.75f);
            anim.SetInteger("AttackState", 5);
            yield return new WaitForSeconds(0.35f);
            anim.SetInteger("AttackState", 6);
            yield return new WaitForSeconds(0.45f);
            RestartCombo();
        }
    }

    IEnumerator Stomp()
    {
        anim.SetInteger("AttackState", 10);
        actionable = false;
        yield return new WaitForSeconds(0.75f);
        actionable = true;
        yield return new WaitForSeconds(0.8f);
        RestartCombo();
    }

    IEnumerator Evade()
    {
        RestartCombo();
        if (Mathf.Abs(movement.InputX) < 0.8 && Mathf.Abs(movement.InputZ) < 0.8)
        {
            anim.SetInteger("EvadeState", 1);
            movement.blockRotationPlayer = true;
            actionable = false;
            yield return new WaitForSeconds(0.15f);
            guarding = true;
            yield return new WaitForSeconds(0.85f);
            guarding = false;
            yield return new WaitForSeconds(0.15f);
            anim.SetInteger("EvadeState", 0);
            actionable = true;
            movement.blockRotationPlayer = false;
        } else
        {
            anim.SetInteger("EvadeState", 2);
            movement.blockRotationPlayer = true;
            actionable = false;
            yield return new WaitForSeconds(0.15f);
            evading = true;
            yield return new WaitForSeconds(0.45f);
            evading = false;
            yield return new WaitForSeconds(0.15f);
            movement.blockRotationPlayer = false;
            anim.SetInteger("EvadeState", 0);
            yield return new WaitForSeconds(0.05f);           
            actionable = true;
            movement.blockRotationPlayer = false;
        }
    }

    void RestartCombo()
    {
        anim.SetInteger("AttackState", 0);
        actionable = true;
        movement.blockRotationPlayer = false;
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
    }
}
