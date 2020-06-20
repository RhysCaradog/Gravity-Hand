using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
    private MeshRenderer rend;

    public float maxOutlineWidth;

    public Color outlineColor;


    private void Start()
    {
        rend = GetComponent<MeshRenderer>();
    }

    public void ShowOutline()
    {
        foreach(var mat in rend.materials)
        {
            mat.SetFloat("_Outline", maxOutlineWidth);
            mat.SetColor("_OutlineColor", outlineColor);
        }
    }

    public void HideOutline()
    {
        foreach (var mat in rend.materials)
        {
            mat.SetFloat("_Outline", 0f);
        }
    }
}
