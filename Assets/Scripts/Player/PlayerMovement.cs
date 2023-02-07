using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    //----------SETTINGS----------//
    //????????:

    //????????????????????????????
    /*public bool CURSOR_CONTROL = true;
    public bool CURSOR_INDICATOR = true;
    public enum eRotationMode { Instant, Fix, Ratio } //"INSTANT":???????????? "FIX":???????????? "RATIO":????????????????
    public eRotationMode ROTATION_MODE = eRotationMode.Instant;
    GameObject rotateComp;
    GameObject cursorComp;
    Vector2 cursorDir;
    public float rotationSpeed = 150;*/
    //----------SETTINGS ENDS----------//

    //bool canMove = true;
    public float movingSpeed = 5;
    float momentum = 0; //between 0 and 1
    public float momentumIndex = 5; //how fast accelerates to max speed
    Vector2  inputNow = new Vector2(0, 0);
    Vector3 movingDir = new Vector3(0, 0, 0);

    Rigidbody rb;
    //SpriteRenderer sr;
    Animator am;

    // Start is called before the first frame update

    float refreshCounter = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        am = GetComponentInChildren(typeof(Animator)) as Animator;
    }


    void Update()
    {
        refreshCounter += Time.deltaTime;
        if(refreshCounter >= 0.3)
        {
            PlayerInfo.i.refreshPlayerTransform(this.transform);
        }

        //Update Input
        inputNow = new Vector2(0, 0);
        if (MasterManager.i.inExploration())
        {
            if (Input.GetKey(KeyCode.W)) inputNow.y += 1;
            if (Input.GetKey(KeyCode.S)) inputNow.y -= 1;
            if (Input.GetKey(KeyCode.A)) inputNow.x -= 1;
            if (Input.GetKey(KeyCode.D)) inputNow.x += 1;
        }

        //Calculate current momentum
        if (inputNow.magnitude == 0) //no movement key pressed 
        {
            momentum -= momentumIndex * Time.deltaTime;
        }
        else //movement key pressed
        {
            if (!(inputNow.x == movingDir.x && inputNow.y == movingDir.z)) //there is a direction change
            {
                ChangeFacing(inputNow);
                movingDir = new Vector3(inputNow.x, 0, inputNow.y);
            }
            momentum += momentumIndex * Time.deltaTime;
        }
        momentum = Mathf.Clamp(momentum, 0, 1);

        if(momentum == 0)
        {
            am.Play("Boy_Idle");
        }
        else
        {
            //if (movingSpeed < 4) am.Play("Crood_Walk");
            //else am.Play("Crood_Run");
        }

        //Move
        if (rb.velocity != movingDir.normalized * movingSpeed * momentum)
        {
            rb.velocity = movingDir.normalized * movingSpeed * momentum;
        }
    }

    void ChangeFacing(Vector2 changeTo)
    {
        if(changeTo.x == 1)
        {
            am.Play("Boy_WalkRight");
        }
        else if(changeTo.x == -1)
        {
            am.Play("Boy_WalkLeft");
        }
        /*
        if (changeTo == new Vector2(0, 1)) { } // print("change facing to ????");
        else if (changeTo == new Vector2(1, 1)) { } // print("change facing to ????");
        else if (changeTo == new Vector2(1, 0)) { } // print("change facing to ????");
        else if (changeTo == new Vector2(1, -1)) { } // print("change facing to ????");
        else if (changeTo == new Vector2(0, -1)) { } // print("change facing to ????");
        else if (changeTo == new Vector2(-1, -1)) { } // print("change facing to ????");
        else if (changeTo == new Vector2(-1, 0)) { } // print("change facing to ????");
        else if (changeTo == new Vector2(-1, 1)) { } // print("change facing to ????");*/
    }

    Vector3 VectorChangeSingle(Vector3 origional, int which, float toWhat)
    {
        Vector3 output = origional;
        if (which == 1)
        {
            output = new Vector3(toWhat, output.y, output.z);
        }
        else if (which == 2)
        {
            output = new Vector3(output.x, toWhat, output.z);
        }
        else if (which == 3)
        {
            output = new Vector3(output.x, output.y, toWhat);
        }
        return output;
    }
    Vector2 VectorChangeSingle(Vector2 origional, int which, float toWhat)
    {
        Vector2 output = origional;
        if (which == 1)
        {
            output = new Vector2(toWhat, output.y);
        }
        else if (which == 2)
        {
            output = new Vector2(output.x, toWhat);
        }
        return output;
    }

    float ChangeToRotateFromRight(float z)
    {
        float output = z % 360;
        if (Mathf.Abs(z) > 180)
        {
            if (z > 0) z = z - 360;
            else z = z + 360;
        }
        return z;
    }
}
