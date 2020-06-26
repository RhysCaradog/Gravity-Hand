using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityHand : MonoBehaviour
{
    GameObject hand;

    public PlayerController playerControl;

    Transform player;

    public Camera cam;
    private Animator anim;

    public float grabDist;
    public float pushDist;

    public Transform holdPos;
    public float attractSpeed;
    public float grappleSpeed;

    public float minThrowForce;
    public float maxThrowForce;
    float throwForce;

    public float pushForce;
    public float pushDamage;

    private GameObject currentObject;
    private Rigidbody objectRb;

    private Vector3 rotateVector = Vector3.one;
    private Vector3 grappleLocation;
    Vector3 cursorPos;

    private bool hasObject = false;
    private bool canGrapple = false;

    void Start()
    {
        throwForce = minThrowForce;

        anim = GetComponent<Animator>();

        hand = gameObject;

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, grabDist))
        {
            Vector3 handDir = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, grabDist));
            Debug.DrawRay(player.transform.position, handDir, Color.green);
            Debug.Log(hit.collider.name);
            Debug.Log(canGrapple);
        }

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
            if (CheckDist() >= 1f) //If object parented to holdPos is not at holdPos lerp it towards the player
            {
                PullToPlayer();
            }
        }
        else
        {
            anim.SetBool("Hold", false);
        }

        if(canGrapple) // If object being interacted can be grappled to call Grapple function
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
        anim.SetTrigger("Pull");
        currentObject.transform.position = Vector3.Lerp(currentObject.transform.position, holdPos.position, attractSpeed * Time.deltaTime);
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

    private void PushObject() //Applies force in direction of the mousePosition to any correctly tagged object in the given direction
    {
        Vector3 pushDir = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, pushDist));

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        if(Physics.Raycast(ray, out hit, pushDist))
        {
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(pushDir * pushForce, ForceMode.Impulse);
            }

            EnemyHealth enemyHealth = hit.transform.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(pushDamage);
            }
        }
    }

    private void Grapple() //Lerps player from their current transform.position to designated grapple location
    {
        player.transform.position = Vector3.Lerp(player.transform.position, grappleLocation, grappleSpeed * Time.deltaTime);

        float travelDist = Vector3.Distance(player.transform.position, grappleLocation);

        if (travelDist <= 0.5f) //Once close to grapple location Grapple function is cancelled disabled & player control is reactivated
        {
            canGrapple = false;
            playerControl.enabled = true;

            anim.SetBool("Grapple", false);
        }       
    }

    private void RaycastInfo() //Sends out raycast towards cursor at centre of the screen
    { 
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, grabDist)) //Check to see targeted object is within distant to be interacted with
        {
            cursorPos = hit.point;

            if (hit.collider.CompareTag("GravInteract")) //Parents the currentObject to holdPos 
            {
                currentObject = hit.collider.gameObject;
                currentObject.transform.SetParent(holdPos);

                objectRb = currentObject.GetComponent<Rigidbody>();
                objectRb.constraints = RigidbodyConstraints.FreezeAll;

                hasObject = true;

                CalculateRotVector();
            }

            if (hit.collider.CompareTag("Armour"))
            {
                currentObject = hit.collider.gameObject;
                currentObject.transform.SetParent(holdPos);

                objectRb = currentObject.GetComponent<Rigidbody>();
                objectRb.isKinematic = false;
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
        hand.transform.LookAt(hit.point);
    }
}
