using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TankBehaviour : MonoBehaviour
{
    private Rigidbody rb;
    public GameObject missile;
    private bool reloaded = true;
    public int reloadTime = 5;
    private PhotonView view;
    public float maxSpeed = 5;
    public float speedMod = 10;
    public AudioSource firing;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        view = GetComponent<PhotonView>();
    }
    void Update()
    {
        if(view.IsMine)
        {
            if (Input.GetButton("Horizontal"))
            {
                transform.Rotate(0f, Input.GetAxis("Horizontal") * 90f * Time.deltaTime, 0f, Space.World);
            }

            if (Input.GetButton("Vertical"))
            {
                if (rb.velocity.magnitude < maxSpeed)
                    rb.AddForce(transform.forward * Input.GetAxis("Vertical") * 10);
            }

            if (Input.GetButtonDown("Fire1") && reloaded)
            {
                reloaded = false;
                view.RPC("Fire", RpcTarget.All, new Vector3(0, 0.75f, 0) + transform.position + (transform.forward * 2), transform.rotation);
                firing.Play();
                StartCoroutine(reload());
            }
        }
    }
    
    IEnumerator reload()
    {
        yield return new WaitForSeconds(reloadTime);
        reloaded = true;
    }
    
    
    [PunRPC]
    public void Fire(Vector3 pos, Quaternion rot)
    {
        GameObject clone = Instantiate(missile, pos, rot);
    }

    public void SpeedUp()
    {
        maxSpeed = 10f;
        speedMod = 15f;
    }
}
