using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightColor : MonoBehaviour
{
    public GameObject timer;
    Timer timerScript;
    Light pointLight;

    Color playingColor;
    Color stoppedColor;

    void Start()
    {
        timerScript = timer.GetComponent<Timer>();
        pointLight = GetComponent<Light>();

        playingColor = new Color(174f / 255f, 75f / 255f, 70f / 255f);
        stoppedColor = new Color(82f / 255f, 124f / 255f, 166f / 255f);
    }

    void Update()
    {
        if (timerScript.timing)
        {
            //pointLight.color = playingColor;
        }
        else
        {
            //pointLight.color = stoppedColor;
        }
    }
}
