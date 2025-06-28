using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMove : MonoBehaviour
{
    Rigidbody rb;
    public Transform groundTransform;
    public GameObject myCamera;
    public GameObject Body;

    Look lookScript;

    //input
    float xInput;
    float zInput;
    bool jumpInput;
    bool crouchInput;
    bool grappleInput;

    float lastxInput;
    float lastzInput;

    //movement
    float gravity = 3750f;
    float moveSpeed = 90f;
    float minSpeed = 0.01f;
    float stopThreshold = 3f;
    float slowdownSpeed = 10f;
    float jumpForce = 1250f;
    float maxSpeed;
    float airMovement;

    float xInputStop;
    float zInputStop;

    public float horizontalVelocity;
    public Vector2 relativeVelocity;
    Vector2 lastVelocity;

    float floorAngle;
    bool grounded;
    bool canJump;

    //sliding and crouching
    public bool crouching;
    float crouchDepth = 0.6f;

    public bool sliding;
    float slideDepth = 0.4f;
    float slideStartSpeed = 14f;
    float slideStopSpeed = 2f;
    float slideSlowdown = 11f;
    float xSlide;
    float zSlide;

    //wallrunning
    float offWallForce = 1700f;
    float wallRayDistance = 1f;
    public int wallTimer;
    public int wallSide;
    bool wallRight;
    bool wallLeft;
    float wallAngle;
    int wallID;
    int lastWallID;

    //grappling
    public GameObject GrappleRopeStart;
    GrappleRope ropeScript;
    public Vector3 grapplePoint;
    public bool grappling;
    public bool lastframeGrapple;
    private SpringJoint grappleJoint;
    private float ropeDistance;
    public float grappleDistance = 50f;

    //audio
    public GameObject audioManager;
    AudioManager audioScript;

    float footstepTimer;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lookScript = myCamera.GetComponent<Look>();
        audioScript = audioManager.GetComponent<AudioManager>();
        ropeScript = GrappleRopeStart.GetComponent<GrappleRope>();
    }


    void Update()
    {
        //manage inputs in and out of menu
        if (lookScript.menu)
        {
            xInput = 0f;
            zInput = 0f;
            jumpInput = false;
            crouchInput = false;
            grappleInput = false;

        }
        else
        {
            xInput = Input.GetAxisRaw("Horizontal");
            zInput = Input.GetAxisRaw("Vertical");
            jumpInput = Input.GetKey(KeyCode.Space);
            crouchInput = Input.GetKey(KeyCode.LeftShift);
            grappleInput = Input.GetMouseButton(1);
        }

        //audio
        if (grounded && !sliding && (xInput != 0 || zInput != 0))
        {
            if (footstepTimer == 0f && (lastxInput != xInput || lastzInput != zInput))
            {
                audioScript.PlayFootstep(0.2f);
                footstepTimer = 0;
            }

            footstepTimer += Time.deltaTime;
            if (crouching)
            {
                if (footstepTimer > 0.5f && horizontalVelocity > 2f)
                {
                    audioScript.PlayFootstep(0.2f);
                    footstepTimer = 0;
                }
            }
            else
            {
                if (footstepTimer > 0.34f && horizontalVelocity > 2f)
                {
                    audioScript.PlayFootstep(0.3f);
                    footstepTimer = 0;
                }
            }
        }
        else
        {
            footstepTimer = 0;
        }

        lastxInput = xInput;
        lastzInput = zInput;
    }

    void FixedUpdate()
    {
        //getting velocities
        relativeVelocity = FindVelocity();
        horizontalVelocity = Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2));

        //getting angle of floor
        Ray normalRay = new Ray(groundTransform.position, -transform.up);
        RaycastHit normalHit;
        if (Physics.Raycast(normalRay, out normalHit, 5, 1 << LayerMask.NameToLayer("Default")))
        {
            floorAngle = Mathf.Round(Mathf.Abs(180 - (Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(normalRay.direction, normalHit.normal)))));
            if (float.IsNaN(floorAngle))
            {
                floorAngle = 0f;
            }
        }
        else
        {
            floorAngle = 0f;
        }


        //grounded check
        Vector3 groundCheckBox = new Vector3(0.5f, 0.1f, 0.5f);
        if (Physics.CheckBox(groundTransform.position, groundCheckBox, transform.rotation, 1 << LayerMask.NameToLayer("Default")) && (floorAngle < 45))
        {
            grounded = true;
            canJump = true;
            airMovement = 1f;
            lastWallID = 0;
        }
        else
        {
            grounded = false;
            airMovement = 0.25f;
        }

        //gravity
        if (!grounded)
        {
            rb.AddForce(gravity * -transform.up * Time.deltaTime);
        }

        //change maxSpeed based on activity
        if (grounded)
        {
            if (crouching)
            {
                maxSpeed = 5f;
            }
            if (!crouching && !sliding)
            {
                maxSpeed = 16f;
            }
        }
        if (wallSide != 0)
        {
            maxSpeed = 21f;
        }


        //horizontal and diagonal input stop on maxspeed
        if (xInput != 0 && Mathf.Abs(relativeVelocity.x) > maxSpeed) { xInputStop = 0; }
        else { xInputStop = 1; }
        if (zInput != 0 && Mathf.Abs(relativeVelocity.y) > maxSpeed) { zInputStop = 0; }
        else { zInputStop = 1; }

        //diagonal slowdown
        if (xInput != 0 && zInput != 0 && (horizontalVelocity > maxSpeed) && !sliding && grounded)
        {
            Vector3 v = rb.velocity.normalized * maxSpeed;
            rb.velocity = v;
        }

        //min speed
        if ((Mathf.Abs(rb.velocity.x) < stopThreshold) && (xInput == 0) && (lastVelocity.x == rb.velocity.x))
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, rb.velocity.z);
        }
        if ((Mathf.Abs(rb.velocity.z) < stopThreshold) && (zInput == 0) && (lastVelocity.y == rb.velocity.z))
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0f);
        }
        lastVelocity = new Vector2(rb.velocity.x, rb.velocity.z);


        //slow down if not moving
        if (sliding)
        {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideSlowdown);
        }
        else
        {
            //these two if statements taken and modified from https://github.com/DaniDevy/FPS_Movement_Rigidbody/blob/master/PlayerMovement.cs
            if (Mathf.Abs(relativeVelocity.x) > minSpeed && Mathf.Abs(xInput) < 0.05f || (relativeVelocity.x < -minSpeed && xInput > 0) || (relativeVelocity.x > minSpeed && xInput < 0))
            {
                rb.AddForce(moveSpeed * transform.right * Time.deltaTime * -relativeVelocity.x * slowdownSpeed);
            }
            if (Mathf.Abs(relativeVelocity.y) > minSpeed && Mathf.Abs(zInput) < 0.05f || (relativeVelocity.y < -minSpeed && zInput > 0) || (relativeVelocity.y > minSpeed && zInput < 0))
            {
                rb.AddForce(moveSpeed * transform.forward * Time.deltaTime * -relativeVelocity.y * slowdownSpeed);
            }
        }

        //jumping
        if (grounded && jumpInput && canJump)
        {
            canJump = false;

            rb.AddForce(Vector2.up * jumpForce * 1.5f);

            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            else if (rb.velocity.y > 0)
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y / 2, rb.velocity.z);
        }

        //crouching and sliding
        if (crouchInput)
        {
            if (Mathf.Abs(horizontalVelocity) > slideStartSpeed)
            {
                if (!sliding)
                {
                    OnStartSlide();
                }
                if (grounded && !jumpInput)
                {
                    rb.AddForce(-rb.transform.up * 2, ForceMode.VelocityChange);
                }
                if (!grounded) { xSlide = 0.1f; }
                else { xSlide = 0f; }
            }
            if (!sliding && (Mathf.Abs(horizontalVelocity) < slideStartSpeed) || (sliding && (Mathf.Abs(horizontalVelocity) < slideStopSpeed)))
            {
                if (!crouching)
                {
                    OnStartCrouch();
                }
            }
        }
        if (!crouchInput)
        {
            Body.transform.localScale = new Vector3(1f, 1f, 1f);
            sliding = false;
            crouching = false;
            xSlide = 1f;
            zSlide = 1f;
        }

        //wallrun detection - wallSide: 1 = on wall to the right, -1 = on wall to the left, 0 = not on wall
        if (!grounded && zInput != 0)
        {
            Ray rightRay = new Ray(transform.position, transform.right);
            Ray leftRay = new Ray(transform.position, -transform.right);
            wallRight = Physics.Raycast(rightRay, out RaycastHit rightHit, wallRayDistance, 1 << LayerMask.NameToLayer("Default"));
            wallLeft = Physics.Raycast(leftRay, out RaycastHit leftHit, wallRayDistance, 1 << LayerMask.NameToLayer("Default"));
            if (wallRight)
            {
                wallAngle = Mathf.Abs(90 - (Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(rightRay.direction, rightHit.normal))));
            }
            if (wallLeft)
            {
                wallAngle = Mathf.Abs(90 - (Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(leftRay.direction, leftHit.normal))));
            }

            if (wallAngle > 75 && wallAngle < 105)
            {
                if (wallRight)
                {
                    wallID = rightHit.transform.gameObject.GetInstanceID();
                    if (wallID != lastWallID)
                    {
                        wallSide = 1;
                    }
                    lastWallID = rightHit.transform.gameObject.GetInstanceID();
                }
                if (wallLeft)
                {
                    wallID = leftHit.transform.gameObject.GetInstanceID();
                    if (wallID != lastWallID)
                    {
                        wallSide = -1;
                    }
                    lastWallID = leftHit.transform.gameObject.GetInstanceID();
                }
            }
        }
        if (grounded || zInput == 0 || (!wallLeft && !wallRight) || (wallTimer > 90))
        {
            wallSide = 0;
            wallTimer = 0;
        }


        //wallrun
        if (wallSide != 0)
        {
            wallTimer += 1;

            rb.AddForce(transform.up * 2100 * Time.deltaTime);
            if (rb.velocity.y < 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            }

            if (jumpInput)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(Vector2.up * jumpForce * 1.2f);
                if (wallSide == 1)
                {
                    rb.AddForce(-transform.right * offWallForce);
                }
                if (wallSide == -1)
                {
                    rb.AddForce(transform.right * offWallForce);
                }
            }
        }

        //grappling
        if (grappleInput && (grappling == false))
        {
            Ray grappleRay = new Ray(myCamera.transform.position, myCamera.transform.forward);
            if (Physics.Raycast(grappleRay, out RaycastHit grappleHit, grappleDistance, 1 << LayerMask.NameToLayer("Default")))
            {
                grapplePoint = grappleRay.GetPoint(grappleHit.distance);
                grappling = true;
            }
        }
        if (!grappleInput)
        {
            grappling = false;
        }
        if (grappling)
        {
            rb.AddForce(transform.forward * 30);
        }

        //grapple detection
        if (grappling && !lastframeGrapple)
        {
            StartGrapple();
            ropeScript.StartRope();
        }
        else if (!grappling && lastframeGrapple)
        {
            StopGrapple();
        }
        lastframeGrapple = grappling;

        //normal movement forces
        rb.AddForce(rb.transform.forward * zInput * moveSpeed * airMovement * zSlide * zInputStop * Time.deltaTime, ForceMode.VelocityChange);
        rb.AddForce(rb.transform.right * xInput * moveSpeed * airMovement * xSlide * xInputStop * Time.deltaTime, ForceMode.VelocityChange);
    }

    //Find velocity relative to where player is looking - from https://github.com/DaniDevy/FPS_Movement_Rigidbody/blob/master/PlayerMovement.cs
    public Vector2 FindVelocity()
    {
        float lookAngle = transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitude = rb.velocity.magnitude;
        float z = magnitude * Mathf.Cos(u * Mathf.Deg2Rad);
        float x = magnitude * Mathf.Cos(v * Mathf.Deg2Rad);
        return new Vector2(x, z);
    }

    public void OnStartCrouch()
    {
        crouching = true;
        sliding = false;
        xSlide = 1f;
        zSlide = 1f;
        Vector3 c = Body.transform.localScale;
        Body.transform.localScale = new Vector3(c.x, crouchDepth, c.z);
    }
    public void OnStartSlide()
    {
        sliding = true;
        zSlide = 0f;
        crouching = false;
        Vector3 c = Body.transform.localScale;
        Body.transform.localScale = new Vector3(c.x, slideDepth, c.z);
    }

    public void StartGrapple()
    {
        grappleJoint = gameObject.AddComponent<SpringJoint>();
        grappleJoint.autoConfigureConnectedAnchor = false;
        grappleJoint.connectedAnchor = grapplePoint;

        grappleJoint.minDistance = ropeDistance * 0.02f;

        grappleJoint.spring = 7f;
        grappleJoint.damper = 5f;
        grappleJoint.massScale = 1f;
    }

    public void StopGrapple()
    {
        Destroy(grappleJoint);
    }
}
