using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    private GameObject Racer;
    public float walkTimer;
    private Animator Anim;
    public float velocity;
    public float MoveSpeed;
    public bool ButtonPressed;


    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponent<Animator>();
        Racer = this.gameObject;
        ButtonPressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (ButtonPressed == true)
        {
            Anim.SetFloat("Velocity", velocity);
            Racer.transform.position += transform.forward * MoveSpeed * Time.deltaTime;
        }
        else if(MoveSpeed == 0)
        {

            ButtonPressed = false;
            Anim.SetFloat("Velocity", 0f);
        }

    }

    public void MoveRacerButton ()
    {
        StartCoroutine(RacerWalkTimer());
    }

    public IEnumerator RacerWalkTimer()
    {
        ButtonPressed = true;
        yield return new WaitForSeconds(walkTimer);
        MoveSpeed = 0;
        ButtonPressed = false;

    }
}
