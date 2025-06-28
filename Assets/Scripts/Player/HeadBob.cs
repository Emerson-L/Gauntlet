using UnityEngine;

public class HeadBob : MonoBehaviour
{
    Vector2 BobSpeed;
    Vector2 BobAmount;

    Vector2 timer;

    public GameObject CameraPos;
    public GameObject player;
    PlayerMove playerScript;

    float posY;

    void Start()
    {
        playerScript = player.GetComponent<PlayerMove>();
        posY = CameraPos.transform.position.y - 2.22f;
    }

    void Update()
    {
        if (playerScript.crouching)
        {
            BobAmount = new Vector2(0.12f, 0.03f);
            BobSpeed = new Vector2(5f, 10f);
        }
        if (playerScript.wallSide != 0)
        {
            BobAmount = new Vector2(0.03f, 0.03f);
            BobSpeed = new Vector2(12f, 24f);
        }
        if (playerScript.sliding || playerScript.grappling)
        {
            BobAmount = new Vector2(0f, 0f);
        }
        if (!playerScript.crouching && playerScript.wallSide == 0 && !playerScript.sliding && !playerScript.grappling) 
        {
            BobAmount = new Vector2(0.2f, 0.05f);
            BobSpeed = new Vector2(7f, 14f);
        }

        if (Mathf.Abs(playerScript.horizontalVelocity) > 3f)
        {
            timer.y += Time.deltaTime * BobSpeed.y;

            if (Mathf.Abs(playerScript.relativeVelocity.y) > 3f)
            {
                timer.x += Time.deltaTime * BobSpeed.x;
            }
            else
            {
                timer.x = 0;
            }
          
            transform.localPosition = new Vector3(transform.localPosition.x, FindY() + Mathf.Sin(timer.y) * BobAmount.y, transform.localPosition.z);
            transform.localPosition = new Vector3(Mathf.Sin(timer.x) * BobAmount.x, transform.localPosition.y, transform.localPosition.z);
        }
        else
        {
            timer = new Vector2(0, 0);
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, FindY(), Time.deltaTime * BobSpeed.y), transform.localPosition.z);
            transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, 0, Time.deltaTime * BobSpeed.x), transform.localPosition.y, transform.localPosition.z);
        }

    }

    public float FindY()
    {
        if (playerScript.sliding)
        {
            return (0.55f * 0.4f);
        }
        else if (playerScript.crouching)
        {
            return (0.55f * 0.6f);
        }
        else
        {
            return 0.55f;
        }
    }
}