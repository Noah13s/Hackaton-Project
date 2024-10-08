using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARDelete : MonoBehaviour
{
    public Master_Counter2 counter;
    public ARPlayer player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Delete()
    {
        RaycastHit hit;
        if (Physics.Raycast(player.camera.transform.position, player.camera.transform.forward, out hit))
        {
            if (hit.collider.gameObject.GetComponent<trashObject>())
            {
                if (hit.collider.gameObject.GetComponent<trashObject>().co2<= counter.GetCounterValue()) { 
                    Destroy(hit.collider.gameObject);
                } else
                {
                    Debug.Log("Not enough score");
                }
            }
        }
    }
}
