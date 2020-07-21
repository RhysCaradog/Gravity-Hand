using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : MonoBehaviour
{
    OutlineController OC;

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
        OC = knife.GetComponent<OutlineController>();

        rb = knife.GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        hasKnife = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasKnife)
        {
            OC.enabled = true;

            anim.enabled = true;
            rb.isKinematic = true;

            if (Input.GetMouseButtonDown(0))
            {
                buttonHeldTime = Time.timeSinceLevelLoad;
                buttonHeld = false;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (!buttonHeld)
                {
                    anim.SetTrigger("Stab");
                }
                else if(buttonHeld)
                {
                    anim.SetTrigger("Throw");
                }
                buttonHeld = false;
            }

            if (Input.GetMouseButton(0))
            {
                if (Time.timeSinceLevelLoad - buttonHeldTime > minButtonHold)
                {
                    buttonHeld = true;
                }                        
            }

            /*if(Input.GetMouseButtonDown(1))
            {
                anim.SetTrigger("Throw");
            }*/
        }

        if(!hasKnife)
        {
            OC.enabled = false;

            anim.enabled = false;

            if (Input.GetMouseButtonDown(1))
            {
                    RecallKnife();
             }
        }
    }

    public void ThrowKnife()
    {
        knife.transform.parent = null;
        rb.isKinematic = false;
        rb.AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);

        hasKnife = false;
    }

    public void RecallKnife()
    {
        knife.transform.parent = knifePar;
        knife.transform.position = knifePar.position;
        knife.transform.localRotation = Quaternion.identity;

        knife.transform.position = Vector3.Lerp(knife.transform.position, knifePar.position, throwForce * Time.deltaTime);
        hasKnife = true;
    }
}
