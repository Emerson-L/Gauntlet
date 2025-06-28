using UnityEngine;

public class GrappleRope : MonoBehaviour
{

    private LineRenderer lr;
    public GameObject player;

    PlayerMove moveScript;

    private Vector3 currentGrapplePoint;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        moveScript = player.GetComponent<PlayerMove>();
    }

    void LateUpdate()
    {
        if (moveScript.grappling)
        {
            lr.positionCount = 2;
            currentGrapplePoint = Vector3.Lerp(currentGrapplePoint, moveScript.grapplePoint, Time.deltaTime * 10f);

            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, currentGrapplePoint);
        }
        else
        {
            lr.positionCount = 0;
        }
    }

    public void StartRope()
    {
        currentGrapplePoint = transform.position;
    }
}