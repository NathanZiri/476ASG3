using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject missile;
    public GameObject[] players;
    private bool reloaded = true;
    private int reloadTime = 3;
    public AudioSource firing;
    private PhotonView view;
    public LineRenderer lm;
    private void Start()
    {
        view = GetComponent<PhotonView>();
    }
    private void Update()
    {
        if (Input.GetKeyDown("k"))
        {
            PhotonNetwork.Destroy(gameObject);
        }
        
        players = GameObject.FindGameObjectsWithTag("Player");
        float distance = 100f;
        GameObject currentPlayer = players[0];
        foreach (var p in players)
        {
            float tempDis = Vector3.Distance(transform.position, p.transform.position);
            if (tempDis < distance)
            {
                currentPlayer = p;
                distance = tempDis;
                agent.SetDestination(p.transform.position);
            }
        }

        if (agent.hasPath)
        {
            lm.positionCount = agent.path.corners.Length;
            lm.SetPositions(agent.path.corners);
            lm.enabled = true;
        }
        
        float dx = (currentPlayer.transform.position.x - transform.position.x);
        float dz = (currentPlayer.transform.position.z - transform.position.z);
        
        if (Vector3.Distance(transform.position, currentPlayer.transform.position) < 10f)
        {
            if(reloaded)
            {
                float ang = Vector3.Angle(transform.forward, new Vector3(dx, 0, dz));
                if (ang > 0.1f)
                {
                    agent.Stop();
                    transform.rotation = Quaternion.Lerp(transform.rotation,
                        Quaternion.LookRotation(new Vector3(dx, 0, dz), Vector3.up), 40 * Time.deltaTime);
                }
                else
                {
                    Debug.Log("shoot");
                    reloaded = false;
                    firing.Play();
                    Instantiate(missile, new Vector3(0, 0.75f, 0) + transform.position + (transform.forward),
                        transform.rotation);
                    //GameObject clone = PhotonNetwork.Instantiate(missile.name, new Vector3(0, 0.75f, 0) + transform.position + (transform.forward * 2), transform.rotation);
                    StartCoroutine(reload());
                }
            }
            
        }
        else
        {
            agent.Resume();
        }
    }
    
    IEnumerator reload()
    {
        yield return new WaitForSeconds(1);
        firing.Play();
        Instantiate(missile, new Vector3(0, 0.75f, 0) + transform.position + (transform.forward * 2),
            transform.rotation);
        yield return new WaitForSeconds(reloadTime);
        reloaded = true;
    }

}
