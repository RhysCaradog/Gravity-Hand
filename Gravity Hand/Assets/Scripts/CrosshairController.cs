using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    public Camera cam;
    public float dist;

    public GameObject crosshair;
    public GameObject interactIcon;
   

    // Start is called before the first frame update
    void Start()
    {
        SetCrosshair();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastInfo();
    }

    private void RaycastInfo()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, dist))
        {
            if (hit.collider.CompareTag("GravInteract") || hit.collider.CompareTag("GrapplePoint"))
            {
                //Debug.Log(hit.collider.name);
                SetInteractIcon();
            }
            else
            {
                SetCrosshair();
            }
        }          
    }

    void SetCrosshair()
    {
        crosshair.SetActive(true);
        interactIcon.SetActive(false);
    }

    void SetInteractIcon()
    {
        crosshair.SetActive(false);
        interactIcon.SetActive(true);
    }
}
