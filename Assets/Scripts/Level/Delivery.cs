using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delivery : MonoBehaviour
{
    public GameObject package;
    Collider collide;
    public GameObject timer;

    Timer timerScript;
    ItemPickup packageScript;


    private void Start()
    {
        packageScript = package.GetComponent<ItemPickup>();
        collide = gameObject.GetComponent<BoxCollider>();
        timerScript = timer.GetComponent<Timer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && packageScript.hasPackage1)
        {
            gameObject.transform.localScale = new Vector3(0, 0, 0);
            collide.enabled = false;
            packageScript.toggleIndicator(0);
            timerScript.StopTimer();
        }
    }
}
