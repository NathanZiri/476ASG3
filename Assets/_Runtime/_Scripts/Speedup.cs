using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedup : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<TankBehaviour>().SpeedUp();
            Destroy(gameObject);
        }
    }
}
