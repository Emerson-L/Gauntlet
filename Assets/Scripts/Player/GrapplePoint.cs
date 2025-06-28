using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    public GameObject player;
    MeshRenderer render;
    PlayerMove moveScript;


    private void Start()
    {
        moveScript = player.GetComponent<PlayerMove>();
        render = gameObject.GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (moveScript.grappling)
        {
            render.enabled = true;
            gameObject.transform.position = moveScript.grapplePoint;
            render.material.SetColor("_Color", Color.red);
        }
        else
        {
            Ray grappleRay = new Ray(moveScript.myCamera.transform.position, moveScript.myCamera.transform.forward);
            if (Physics.Raycast(grappleRay, out RaycastHit grappleHit, moveScript.grappleDistance, 1 << LayerMask.NameToLayer("Default")))
            {
                gameObject.transform.position = grappleRay.GetPoint(grappleHit.distance);
                render.material.SetColor("_Color", Color.gray);
                
                //for grapple indictator before grapple, set true here
                render.enabled = false;
            }
            else
            {
                render.enabled = false;
            }
        }
    }
}
