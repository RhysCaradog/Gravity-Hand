using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityHand : MonoBehaviour
{
    GameObject hand;

    public PlayerController playerControl;
    public KnifeController knifeControl;

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

    public ParticleSystem pushVFX;

    LineRenderer lr;

    public Light lt;
    Color pushColour = Color.red;
    Color pullColour = Color.blue;
    Color holdColour = Color.yellow;

    private bool hasObject = false;
    private bool canGrapple = false;

    void Start()
    {
        throwForce = minThrowForce;

        anim = GetComponent<Animator>();

        hand = gameObject;

        player = GameObject.FindGameObjectWithTag("Player").transform;

        lr = GetComponent<LineRenderer>();

        lt = GetComponentInChildren<Light>();
    }

    void Update()
    {
        Debug.Log(lt.color);
        var d = Input.GetAxis("Mouse ScrollWheel");
        
        if (d < 0f  && !hasObject)
        {
            RaycastInfo();
        }

        if (Input.GetMouseButton(2)) //If button is held down increase throw force until it reaches maxThrowForce
        {
            throwForce += 0.1f;
        }

        if (d > 0f && hasObject)
        {
                ThrowObject();         
        }
        else if(d > 0f && !hasObject)
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

            lt.color = holdColour;

            lr.enabled = false;

            RotateObject();

            if (CheckDist() >= 1f) //If object parented to holdPos is not at holdPos lerp it towards the holdPos.
            {
                ShowPullEffect();
                PullToPlayer();
            }
        }
        else
        {
            anim.SetBool("Hold", false);

            lr.enabled = false;

            lt.color = Color.clear;
        }

        if(canGrapple) // If object being interacted can be grappled to call Grapple function
        {
            anim.SetBool("Grapple", true);
            ShowPullEffect();
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
        //throwForce = Mathf.Clamp(throwForce, minThrowForce, maxThrowForce);
        objectRb.AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);
        throwForce = minThrowForce;

        anim.SetTrigger("Throw");

        Debug.Log("GLOW RED!!!");

        DropObject();
    }

    private void PushObject() //Applies force in direction of the mousePosition to any correctly tagged object in the given direction
    {
        //pushVFX.Play();

        Debug.Log("GLOW RED!!!");

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

            if (hit.collider.CompareTag("GravInteract")) //Parents the currentObject to holdPos.
            {
                currentObject = hit.collider.gameObject;
                currentObject.transform.SetParent(holdPos);

                objectRb = currentObject.GetComponent<Rigidbody>();
                objectRb.constraints = RigidbodyConstraints.FreezeAll;

                hasObject = true;

                CalculateRotVector();
            }

            if (hit.collider.CompareTag("Armour")) //Parents "Armour" component to holdPos;
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

            if (hit.collider.CompareTag("Knife"))
            {
                Debug.Log("It's a Knife!");
                knifeControl.RecallKnife();
            }
        }
        hand.transform.LookAt(hit.point);
    }

    void ShowPullEffect()//Enable Line Renderer between target & Gravity Hand.
    {
        lt.color = pullColour;

        if (hasObject == true)
        {
            lr.enabled = true;
            lr.SetPosition(0, hand.transform.position);
            lr.SetPosition(1, currentObject.transform.position);
        }
        else if (canGrapple)
        {
            lr.enabled = true;
            lr.SetPosition(0, hand.transform.position);
            lr.SetPosition(1, grappleLocation);
        }
    }
}
