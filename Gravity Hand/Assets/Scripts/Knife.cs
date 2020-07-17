using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    public Camera cam;

    Rigidbody rb;

    public float throwForce;

    Animator anim;
    

    bool thrown;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Stab");
        }

        if (Input.GetMouseButtonDown(1))
        {
            anim.SetTrigger("Throw");
        }
    }

    void Stab()
    {

    }

    void ThrowKnife()
    {
        rb.AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);
        rb.isKinematic = false;

        thrown = true;
    }
}
