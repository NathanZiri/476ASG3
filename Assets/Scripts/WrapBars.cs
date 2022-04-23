using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class WrapBars : MonoBehaviour
{
    public int pos;
    private void OnTriggerEnter(Collider other)
    {
        switch (pos)
        {
            case 1:
                other.transform.position = new Vector3(other.transform.position.x, other.transform.position.y,
                -other.transform.position.z + .1f);
                break;
            case 2:
                other.transform.position = new Vector3(-other.transform.position.x + .1f, other.transform.position.y,
                other.transform.position.z );
                break;
            case 3:
                other.transform.position = new Vector3(other.transform.position.x, other.transform.position.y,
                -other.transform.position.z - .1f);
                break;  
            case 4:
                other.transform.position = new Vector3(-other.transform.position.x - .1f, other.transform.position.y,
                other.transform.position.z);
                break;
        }
    }
}
