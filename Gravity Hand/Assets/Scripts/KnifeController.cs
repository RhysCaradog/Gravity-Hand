using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : MonoBehaviour
{
    public GameObject knife;

    public Camera cam;

    Rigidbody rb;

    Animator anim;

    public Transform knifePar;

    public float throwForce;
    const float minButtonHold = 0.25f;
    float buttonHeldTime = 0f;

    bool buttonHeld = false;
    bool hasKnife;

    // Start is called before the first frame update
    void Start()
    {
        rb = knife.GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasKnife)
        {
            anim.enabled = true;
            rb.isKinematic = true;

            if (Input.GetMouseButtonDown(0))//Button is pressed. We then check to see if it is being held.
            {
                buttonHeldTime = Time.timeSinceLevelLoad;
                buttonHeld = false;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (!buttonHeld)//If button is released without being held, Stab.
                {
                    anim.SetTrigger("Stab");
                }
                else if(buttonHeld)//If button is released after being held, Throw.
                {
                    anim.SetTrigger("Throw");
                }
                buttonHeld = false;
            }

            if (Input.GetMouseButton(0))//Determines that button is being held.
            {
                if (Time.timeSinceLevelLoad - buttonHeldTime > minButtonHold)
                {
                    buttonHeld = true;
                }                        
            }
        }

        if(!hasKnife)//Disable animator so that ThrowKnife() can operate. 
        {
            anim.enabled = false;
        }
    }

    public void ThrowKnife()//Removes knife from player possession & throws it in a forward direction
    {
        knife.transform.parent = null;
        rb.isKinematic = false;
        rb.AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);

        hasKnife = false;
    }

    public void RecallKnife()//Lerps knife from it's current position back to KnifePos & re childs it.
    {
        knife.transform.parent = knifePar;
        knife.transform.position = knifePar.position;
        knife.transform.localRotation = Quaternion.identity;

        knife.transform.position = Vector3.Lerp(knife.transform.position, knifePar.position, throwForce * Time.deltaTime);
        hasKnife = true;
    }
}
