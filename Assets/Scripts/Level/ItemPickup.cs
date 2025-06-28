using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickup : MonoBehaviour
{
    public bool hasPackage1;
    public Image Indicator1;
    Collider collide;


    private void Start()
    {
        collide = gameObject.GetComponent<BoxCollider>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            hasPackage1 = true;
            gameObject.transform.localScale = new Vector3(0, 0, 0);
            collide.enabled = false;
            toggleIndicator(1);
        }
    }

    public void toggleIndicator(int num)
    {
        if (num == 1)
        {
            Indicator1.enabled = true;  
        }
        if (num == 0)
        {
            Indicator1.enabled = false;
        }

    }

}
