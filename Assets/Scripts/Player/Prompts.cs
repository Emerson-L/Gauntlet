using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Prompts : MonoBehaviour
{
    List<string> lines;
    List<GameObject> triggersHit;
    List<GameObject> queue;

    int lineCounter;
    bool canWrite;

    int charNum;
    char character;

    int linesToWrite;

    TextMeshProUGUI UI_text;

    void Start()
    {
        UI_text = gameObject.GetComponent<TextMeshProUGUI>();
        lineCounter = -1;
        charNum = 0;
        canWrite = true;

        lines = new List<string>();
        triggersHit = new List<GameObject>();
        queue = new List<GameObject>();

        //start - 3
        lines.Add("welcome");
        lines.Add("you must deliver this package to our customer");
        lines.Add("jump from wall to wall");

        //prompt 1 - 1
        lines.Add("good");

        //prompt 2 - 2
        lines.Add("good");
        lines.Add("now use your grappling hook");

        //prompt 3 - 3
        lines.Add("now, deliver the package");

    }

    public void UpdatePrompt(GameObject trigger, int numLines)
    {
        //Debug.Log("Update");
        if (!canWrite && queue.Count == 0)
        {
            //Debug.Log("cant write and queue empty, adding to queue");
            queue.Add(trigger);
            return;
        }

        if (triggersHit.Contains(trigger)) 
        {
            //Debug.Log("trigger already hit");
            return; 
        }

        if (lineCounter >= lines.Count)
        {
            //Debug.LogError("No more lines!");
            return;
        }

        linesToWrite = numLines;

        canWrite = false;
        triggersHit.Add(trigger);
        lineCounter += 1;

        //Debug.Log("Writing" + " " + linesToWrite);
        WritePrompt();
    }

    public void WritePrompt()
    {
        string text = lines[lineCounter];

        if (charNum > text.Length)
        {
            charNum = text.Length;
            //Debug.Log("Deleting");
            Invoke(nameof(DeletePrompt), 1f);
            return;
        }

        UI_text.text = text.Substring(0, charNum);
        charNum += 1;
        Invoke(nameof(WritePrompt), 0.07f);
    }

    public void DeletePrompt()
    {
        string text = lines[lineCounter];

        if (charNum < 1)
        {
            ResetPrompt();
            return;
        }

        UI_text.text = text.Substring(0, charNum);
        charNum -= 1;
        Invoke(nameof(DeletePrompt), 0.03f);
    }

    public void ResetPrompt()
    {
        //Debug.Log("Reseting prompt");
        UI_text.text = " ";
        charNum = 0;

        if (linesToWrite > 1)
        {
            //Debug.Log("continuing");
            lineCounter += 1;
            linesToWrite -= 1;
            WritePrompt();
            return;
        }
        if (queue.Count > 0)
        {
            //Debug.Log("Playing queue" + " " + linesToWrite);
            UpdatePrompt(queue[0], linesToWrite);
            queue.RemoveAt(queue.Count - 1);
        }
        else
        {
            canWrite = true;
        }
    }
}
