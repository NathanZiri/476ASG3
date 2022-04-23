using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRateUp : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<TankBehaviour>().reloadTime = 1;
            Destroy(gameObject);
        }
    }
}
