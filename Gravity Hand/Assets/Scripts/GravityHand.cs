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

        if (Input.GetMouseButton(1)) //If button is held down increase throw force until it reaches maxThrowForce
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
            anim.SetTrigger("Push");
        }

        if (Input.GetKeyDown(KeyCode.G) && hasObject)
        {
            DropObject();
        }

        if (hasObject)
        {
            anim.SetBool("Hold", true);
            anim.ResetTrigger("Pull");

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
            anim.SetBool("Grapple", true);
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

    public float CheckDist() //Checks the distance between player & object
    {
        float dist = Vector3.Distance(currentObject.transform.position, holdPos.transform.position);
        return dist;
    }

    private void PullToPlayer() //lerps object to holdPos
    {
        currentObject.transform.position = Vector3.Lerp(currentObject.transform.position, holdPos.position, attractSpeed * Time.deltaTime);

        anim.SetTrigger("Pull");
    }

    private void DropObject() //removes object from parent & it's rigidbody constraints 
    {
        objectRb.constraints = RigidbodyConstraints.None;
        currentObject.transform.parent = null;
        currentObject = null;
        hasObject = false;
    }

    private void ThrowObject() //Throws object in forward vector & drops object
    {
        throwForce = Mathf.Clamp(throwForce, minThrowForce, maxThrowForce);
        objectRb.AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);
        throwForce = minThrowForce;

        anim.SetTrigger("Throw");

        DropObject();
    }

    private void PushObject() //Applies force in a forward vector to any correctly tagged object in a radius
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

    private void Grapple() //Lerps player from their current transform.position to designated grapple location. Once close to grapple location lerp is disabled & player control is reactivated
    {
        transform.position = Vector3.Lerp(transform.position, grappleLocation, grappleSpeed * Time.deltaTime);

        float travelDist = Vector3.Distance(transform.position, grappleLocation);

        if (travelDist <= 0.5f)
        {
            canGrapple = false;
            playerControl.enabled = true;

            anim.SetBool("Grapple", false);
        }       
    }

    private void RaycastInfo() //Sends out raycast towards mouseposition
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, grabDist)) //Check to see targeted object is within distant to be interacted with
        {
            if (hit.collider.CompareTag("GravInteract")) //Parents the currentObject to holdPos 
            {
                currentObject = hit.collider.gameObject;
                currentObject.transform.SetParent(holdPos);

                objectRb = currentObject.GetComponent<Rigidbody>();
                objectRb.constraints = RigidbodyConstraints.FreezeAll;

                hasObject = true;

                CalculateRotVector();
            }

            if (hit.collider.CompareTag("GrapplePoint")) //Designates point to which player can grapple to & disables player control
            {
                grappleLocation = hit.point;
                canGrapple = true;
                playerControl.enabled = false;
            }
        }
    }
}
