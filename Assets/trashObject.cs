using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trashObject : MonoBehaviour
{
    public int co2 = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.transform.position.y <= 0)
        {
            this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
        if (this.gameObject.transform.position.y <= -20)
        {
            Destroy(this.gameObject);
        }
    }
}

