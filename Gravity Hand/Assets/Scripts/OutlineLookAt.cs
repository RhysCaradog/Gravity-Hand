using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineLookAt : MonoBehaviour
{
    private string[] tags = new string[] { "GravInteract", "GrapplePoint", "Armour", "Knife" };

    public Camera cam;
    public float dist;

    private OutlineController currentOC;
    private OutlineController prevOC;

    private Vector3 cursorPos;


    private void Update()
    {
        LookAtRay();
    }

    private void LookAtRay()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, dist))
        {
            cursorPos = hit.point;

            if(hit.collider.CompareTag("GravInteract") || hit.collider.CompareTag("GrapplePoint") || hit.collider.CompareTag("Armour") || hit.collider.CompareTag("Knife"))          
            {
                currentOC = hit.collider.GetComponent<OutlineController>();

                if (prevOC != currentOC)
                {
                    HideOutline();
                    ShowOutline();
                }

                prevOC = currentOC;
            }
            else
            {
                HideOutline();
            }
        }
        else
        {
            HideOutline();
        }
    }

    private void ShowOutline()
    {
        if (currentOC != null)
        {
            currentOC.ShowOutline();
        }
    }

    private void HideOutline()
    {
        if(prevOC != null)
        {
            prevOC.HideOutline();
            prevOC = null;
        }
    }
}
