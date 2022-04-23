using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MissleBehaviour : MonoBehaviour
{
    private bool shooting = false;
    private PhotonView view;
    public float speed = 15;
    private void Start()
    {
        view = GetComponent<PhotonView>();
    }
    
    void Update()
    {
        transform.position = transform.position + transform.forward * Time.deltaTime * speed;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Destructible"))
        {
            Debug.Log("destroy");
            Destroy(other.gameObject);
            Destroy(gameObject);
            Debug.Log("destroy out");
        }
        else if (other.transform.CompareTag("Obstacle"))
        {
            Debug.Log("destroy 2");
            Destroy(gameObject);
        }
        else if (other.transform.CompareTag("Player"))
        {
            Debug.Log("destroy 3");
            PhotonNetwork.Destroy(other.gameObject);
            //Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    [PunRPC]
    public void destroySelf(GameObject self)
    {
        Destroy(self);
    }
    
    [PunRPC]
    public void destroyOther(GameObject other)
    {
        Destroy(other);
    }
}
