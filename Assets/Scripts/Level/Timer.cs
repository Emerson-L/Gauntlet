using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    TextMeshProUGUI timerText;

    public bool timing;
    float time;
    int minutes;
    int seconds;
    int milliseconds;

    void Start()
    {
        timerText = gameObject.GetComponent<TextMeshProUGUI>();
        time = 0f;
    }

    void Update()
    {
        if (timing)
        {
            time += Time.deltaTime;
        }

        minutes = (int)Mathf.Floor(time / 60);
        seconds = (int)time % 60;
        milliseconds = (int)((time % 1) * 100);

        string strMinutes = minutes.ToString().PadLeft(2, '0');
        string strSeconds = seconds.ToString().PadLeft(2, '0');
        string strMilliseconds = milliseconds.ToString().PadLeft(2, '0');

        timerText.text = strMinutes + ":" + strSeconds + ":" + strMilliseconds;
    }

    public void StartTimer()
    {
        if (timing)
        {
            ResetTimer();
            return;
        }

        time = 0;
        timerText.text = "00:00:00";
        timing = true;
    }

    public void StopTimer()
    {
        timing = false;
    }
    public void ResetTimer()
    {
        time = 0;
        timerText.text = "00:00:00";
        timing = false;
    }
}
