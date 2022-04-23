using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToBehaviour : MonoBehaviour
{
    public Transform MoveTo;
    public float speed=10;
    
    
    
    // Update is called once per frame
    void Update()
    {
        float dx = (MoveTo.position.x - transform.position.x);
        float dz = (MoveTo.position.z - transform.position.z);
        if (Mathf.Abs(dx) > 15)
            dx = Mathf.Sign(transform.position.x) * (30 - Mathf.Abs(MoveTo.position.x));
        else
            dx = MoveTo.transform.position.x;
        if(Mathf.Abs(dz) > 15)
            dz = Mathf.Sign(transform.position.z) * (30 - Mathf.Abs(MoveTo.position.z));
        else
            dz = MoveTo.transform.position.z;
        //Debug.Log(dx + "   "  + dz);
        transform.position = Vector3.Lerp(transform.position, new Vector3(dx, 1f, dz), Time.deltaTime * speed); 
    }
}
