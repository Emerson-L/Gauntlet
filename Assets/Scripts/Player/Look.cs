using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Look : MonoBehaviour
{
    public Transform player;
    public GameObject CameraPos;
    public GameObject MyCamera;
    public GameObject UIMenuCanvas;
    public GameObject UIGameCanvas;

    PlayerMove moveScript;
    UI_Menu menuScript;

    public bool menu;

    public int playerMouseSensitivity = 450;
    public int playerFOV = 75;

    float mouseX;
    float mouseY;
    float xRotation = 0;
    public float camFOV = 75;

    //wallrun rotation and fov increase
    float camRotate;
    float rotateWallSpeed;
    float stopRotate = 80;
    float maxRotate = 15;
    float FOVWallSpeed = 25f;

    void Start()
    {
        moveScript = player.GetComponent<PlayerMove>();
        menuScript = UIMenuCanvas.transform.Find("Panel_Menu").GetComponent<UI_Menu>();

        transform.position = CameraPos.transform.position;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        menu = false;

        UIMenuCanvas.SetActive(false);
        UIGameCanvas.SetActive(true);

        playerFOV = 75;
        camFOV = 75;
        playerMouseSensitivity = 450;
    }

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            menu = !menu;
            menuScript.OnMenu();
            if (menu)
            {
                Time.timeScale = 0f;
                UIMenuCanvas.SetActive(true);
                UIGameCanvas.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Time.timeScale = 1f;
                UIMenuCanvas.SetActive(false);
                UIGameCanvas.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if (menu)
        {
            mouseX = 0;
            mouseY = 0;
        }
        else
        {
            mouseX = Input.GetAxis("Mouse X") * playerMouseSensitivity * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * playerMouseSensitivity * Time.deltaTime;
        }

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        player.Rotate(Vector3.up * mouseX);

        MyCamera.GetComponent<Camera>().fieldOfView = camFOV;

        //fov change for wallrun
        if (moveScript.wallSide != 0)
        {
            camFOV = playerFOV;
            if (camFOV < (playerFOV + 6))
            {
                camFOV += FOVWallSpeed * Time.deltaTime;
            }
        }
        else
        {
            if (camFOV > playerFOV)
            {
                camFOV -= FOVWallSpeed * Time.deltaTime;
            }
            if (camFOV < playerFOV)
            {
                camFOV += FOVWallSpeed * Time.deltaTime;
            }
            if (camFOV < (playerFOV + 0.2f) || camFOV > (playerFOV - 0.2f))
            {
                camFOV = playerFOV;
            }

        }

        //rotate camera gradually during wallrun
        Vector3 rotateValue;

        if (moveScript.wallSide == 1 && moveScript.wallTimer < stopRotate)
        {
            if (Mathf.Abs(camRotate) < maxRotate)
            {
                camRotate -= rotateWallSpeed * Time.deltaTime;
            }
        }
        if (moveScript.wallSide == -1 && moveScript.wallTimer < stopRotate)
        {
            if (Mathf.Abs(camRotate) < maxRotate)
            {
                camRotate += rotateWallSpeed * Time.deltaTime;
            }
        }

        if (moveScript.wallTimer > stopRotate)
        {
            rotateWallSpeed = 30;
        }
        else
        {
            rotateWallSpeed = 120;
        }

        if ((moveScript.wallSide == 0) || (moveScript.wallTimer > stopRotate))
        {
            if (camRotate > 0)
            {
                camRotate -= rotateWallSpeed * Time.deltaTime;
            }
            if (camRotate < 0)
            {
                camRotate += rotateWallSpeed * Time.deltaTime;
            }
        }
        rotateValue = new Vector3(mouseX, mouseY, camRotate);
        transform.eulerAngles -= rotateValue;
    }
}
