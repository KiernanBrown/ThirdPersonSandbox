using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public bool actionable = true;
    public bool canStomp = false;
    public Animator anim;
    public int state;
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
        if (Input.GetButton("Fire1") && actionable == true)
        {
            StopCoroutine("Hit");
            StopCoroutine("Stomp");
            StartCoroutine("Hit");
        }
        if (Input.GetButton("Fire2") && actionable == true && canStomp)
        {
            StopCoroutine("Hit");
            StartCoroutine("Stomp");
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
            yield return new WaitForSeconds(0.7f);
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
}
