using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Menu: MonoBehaviour
{
    [SerializeField] private TMP_InputField sensitivityInputField;
    [SerializeField] private TMP_InputField FOVInputField;

    [SerializeField] private GameObject MyCamera;

    Look lookScript;

    public void OnMenu()
    {
        lookScript = MyCamera.GetComponent<Look>();

        sensitivityInputField.text = lookScript.playerMouseSensitivity.ToString();
        FOVInputField.text = lookScript.playerFOV.ToString();
    }

    public void SavePlayerMouseSensitivity()
    {
        if (string.IsNullOrEmpty(sensitivityInputField.text))
        {
            sensitivityInputField.text = lookScript.playerMouseSensitivity.ToString();
            return;
        }

        int intInput = int.Parse(sensitivityInputField.text);

        if ((intInput >= 1) && (intInput <= 10000))
        {
            lookScript.playerMouseSensitivity = intInput;
        }
        else
        {
            lookScript.playerMouseSensitivity = 400;
        }
    }

    public void SavePlayerFOV()
    {
        if (string.IsNullOrEmpty(FOVInputField.text))
        {
            FOVInputField.text = lookScript.playerFOV.ToString();
            return;
        }

        int intInput = int.Parse(FOVInputField.text);

        if ((intInput >= 30) && (intInput <= 110))
        {
            lookScript.playerFOV = intInput;
            lookScript.camFOV = intInput;
        }
        else
        {
            lookScript.playerFOV = 75;
        }
    }   

    public void ResumeGame()
    {
        lookScript.menu = !lookScript.menu;
        lookScript.UIMenuCanvas.SetActive(false);
        lookScript.UIGameCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
