using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : MonoBehaviour
{
    public GameObject knife;
    Knife k;

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
        k = knife.GetComponent<Knife>();

        rb = knife.GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        hasKnife = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasKnife)
        {
            anim.enabled = true;
            rb.isKinematic = true;
            k.thrown = false;

            if (Input.GetMouseButtonDown(0))//Button is pressed down. Need to check o see if it is "held".
            {
                buttonHeldTime = Time.timeSinceLevelLoad;
                buttonHeld = false;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (!buttonHeld)//If button is released without being held.
                {
                    anim.SetTrigger("Stab");
                }
                else if(buttonHeld)//If button released after being held.
                {
                    anim.SetTrigger("Throw");
                }
                buttonHeld = false;
            }

            if (Input.GetMouseButton(0))
            {
                if (Time.timeSinceLevelLoad - buttonHeldTime > minButtonHold)//Button is considered "held" if it is actually held down.
                {
                    buttonHeld = true;
                }                        
            }
        }

        if(!hasKnife)//Disable animator to ensure ThrowKnife() can function.
        {
            anim.enabled = false;
            k.thrown = true;
        }
    }

    public void ThrowKnife()//Throws knife forward
    {
        knife.transform.parent = null;
        rb.isKinematic = false;
        rb.AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);

        hasKnife = false;
    }

    public void RecallKnife()//Lerps knife from current position to KnifePos & resets rotation.
    {
        knife.transform.parent = knifePar;
        knife.transform.position = knifePar.position;
        knife.transform.localRotation = Quaternion.identity;

        knife.transform.position = Vector3.Lerp(knife.transform.position, knifePar.position, throwForce * Time.deltaTime);
        hasKnife = true;
    }
}
