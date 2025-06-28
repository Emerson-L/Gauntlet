using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPlate : MonoBehaviour
{
    bool playerReady;
    public GameObject Door;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.tag == "Player")
        {
            playerReady = true;
            StartTimer();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.transform.tag == "Player")
        {
            playerReady = false;
        }
    }

    public void StartTimer()
    {
        if (!playerReady) { return; }

        if (Door.transform.position.y < 15f)
        for (int i = 0; i < 100; i++)
        {
            Door.transform.position = (Vector3.up * Time.fixedDeltaTime);
        }
    }
}
