using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : MonoBehaviour
{
    public GameObject knife;

    Knife k;

    public Camera cam;

    Rigidbody rb;

    public float throwForce;

    Animator anim;

    public Transform knifePar;
    //Vector3 knifePos;
    //Quaternion knifeRot;

    bool hasKnife;

    // Start is called before the first frame update
    void Start()
    {
        rb = knife.GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        //knifePar = gameObject.transform;
        //knifePos = gameObject.transform.position;
        //knifeRot = gameObject.transform.localRotation;

        k = knife.GetComponent<Knife>();

        hasKnife = true;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Knife Pos: " + knifePar);

        if (hasKnife)
        {
            anim.enabled = true;
            rb.isKinematic = true;

            if (Input.GetMouseButtonDown(0))
            {
                anim.SetTrigger("Stab");
            }

            if (Input.GetMouseButtonDown(1))
            {
                anim.SetTrigger("Throw");
            }
        }

        if(!hasKnife)
        {
            anim.enabled = false;
            k.thrown = true;

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

    void RecallKnife()
    {
        knife.transform.parent = knifePar;
        knife.transform.position = knifePar.position;
        knife.transform.localRotation = Quaternion.identity;

        knife.transform.position = Vector3.Lerp(knife.transform.position, knifePar.position, throwForce * Time.deltaTime);
        hasKnife = true;
    }
}
