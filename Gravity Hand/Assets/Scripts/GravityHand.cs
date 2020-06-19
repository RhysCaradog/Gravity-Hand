using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityHand : MonoBehaviour
{
    public PlayerController playerControl;

    public Camera cam;
    public Animator anim;

    public float grabDist;
    public float pushRadius;

    public Transform holdPos;
    public float attractSpeed;
    public float grappleSpeed;

    public float minThrowForce;
    public float maxThrowForce;
    float throwForce;

    public float pushForce;

    private GameObject currentObject;
    private Rigidbody objectRb;

    private Vector3 rotateVector = Vector3.one;
    private Vector3 grappleLocation;

    private bool hasObject = false;
    private bool canGrapple = false;


    void Start()
    {
        throwForce = minThrowForce;
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !hasObject)
        {
            RaycastInfo();
        }

        if (Input.GetMouseButton(1))
        {
            throwForce += 0.1f;
        }

        if (Input.GetMouseButtonUp(1) && hasObject)
        {
            ThrowObject();
        }
        else if(Input.GetMouseButtonUp(1) && !hasObject)
        {
            PushObject();
        }

        if (Input.GetKeyDown(KeyCode.G) && hasObject)
        {
            DropObject();
        }

        if (hasObject)
        {
            anim.SetBool("Hold", true);

            RotateObject();
            if (CheckDist() >= 1f)
            {
                PullToPlayer();
            }
        }
        else
        {
            anim.SetBool("Hold", false);
        }

        if(canGrapple)
        {
            Grapple();
        }
    }


    //-----------------Polish Stuff

    private void CalculateRotVector()
    {
        float x = Random.Range(-0.5f, 0.5f);
        float y = Random.Range(-0.5f, 0.5f);
        float z = Random.Range(-0.5f, 0.5f);
    }

    private void RotateObject()
    {
        currentObject.transform.Rotate(rotateVector);
    }




    //-----------------Functional Stuff

    public float CheckDist()
    {
        float dist = Vector3.Distance(currentObject.transform.position, holdPos.transform.position);
        return dist;
    }

    private void PullToPlayer()
    {
        currentObject.transform.position = Vector3.Lerp(currentObject.transform.position, holdPos.position, attractSpeed * Time.deltaTime);
    }

    private void DropObject()
    {
        objectRb.constraints = RigidbodyConstraints.None;
        currentObject.transform.parent = null;
        currentObject = null;
        hasObject = false;
    }

    private void ThrowObject()
    {
        throwForce = Mathf.Clamp(throwForce, minThrowForce, maxThrowForce);
        objectRb.AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);
        throwForce = minThrowForce;

        anim.SetTrigger("Throw");

        DropObject();
    }

    private void PushObject()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, pushRadius);

        foreach (Collider gravObject in colliders)
        {
            if(gravObject.CompareTag("GravInteract"))
            {
                Rigidbody gravBody = gravObject.GetComponent<Rigidbody>();

                //gravBody.AddExplosionForce(pushForce, Vector3.up, pushRadius);
                gravBody.AddForce(transform.forward * pushForce);
            }
        }

    }

    private void Grapple()
    {
        transform.position = Vector3.Lerp(transform.position, grappleLocation, grappleSpeed * Time.deltaTime);

        float travelDist = Vector3.Distance(transform.position, grappleLocation);

        anim.SetTrigger("Pull");

        if (travelDist <= 0.5f)
        {
            canGrapple = false;
            playerControl.enabled = true;
        }
    }

    private void RaycastInfo()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, grabDist))
        {
            

            if (hit.collider.CompareTag("GravInteract"))
            {
                currentObject = hit.collider.gameObject;
                currentObject.transform.SetParent(holdPos);

                objectRb = currentObject.GetComponent<Rigidbody>();
                objectRb.constraints = RigidbodyConstraints.FreezeAll;

                hasObject = true;

                CalculateRotVector();
            }

            if (hit.collider.CompareTag("GrapplePoint"))
            {
                grappleLocation = hit.point;
                canGrapple = true;
                playerControl.enabled = false;
            }
        }


        /*if(Physics.Raycast(ray, out hit, grappleDist))
        {
            if (hit.collider.CompareTag("GrapplePoint"))
            {
                grappleLocation = hit.point;
                canGrapple = true;
                playerControl.enabled = false;
            }
        }*/
    }
}
