using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    public Vector3 pos;
    public Vector3 rot;

    private void Start()
    {
        rot = new Vector3(0, 90, 0);
        pos = new Vector3(0, 2, 0);
    }

    public void ResetPosition()
    {
        gameObject.transform.eulerAngles = rot;
        gameObject.transform.position = pos;
    }    
}
