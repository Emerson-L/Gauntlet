using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTrigger : MonoBehaviour
{
    public GameObject Player;
    public GameObject UI_Timer;
    public GameObject UI_Prompts;

    Timer timerScript;
    Prompts promptScript;
    Checkpoints checkpointScript;
    TriggerValues triggerScript;

    int numLines;

    private void Awake()
    {
        timerScript = UI_Timer.GetComponent<Timer>();
        promptScript = UI_Prompts.GetComponent<Prompts>();
        checkpointScript = Player.GetComponent<Checkpoints>();
        triggerScript = gameObject.GetComponent<TriggerValues>();


        if (gameObject.tag == "Start" || gameObject.tag == "Prompt")
        {
            numLines = triggerScript.numberLines;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.tag == "Reset")
        {
            //timerScript.ResetTimer();
            checkpointScript.ResetPosition();
        }
        else if(triggerScript.isCheckpoint)
        {
            checkpointScript.pos = triggerScript.checkpointPos;
        }

        if (gameObject.tag == "Start")
        {
            timerScript.StartTimer();
            promptScript.UpdatePrompt(gameObject, numLines);
        }
        if (gameObject.tag == "Stop")
        {
            timerScript.StopTimer();
        }
        if (gameObject.tag == "Prompt")
        {
            promptScript.UpdatePrompt(gameObject, numLines);
        }
    }
}
