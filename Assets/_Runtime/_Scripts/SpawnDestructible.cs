using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SpawnDestructible : MonoBehaviour
{
    public Transform[] spawnPos;
    public GameObject[] walls;

    private void Start()
    {
        for(int i = 0; i < spawnPos.Length; i++)
        {
            PhotonNetwork.Instantiate(walls[i].name, spawnPos[i].position, spawnPos[i].rotation);
            walls[i].SetActive(false);
        }
    }
}
