using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform [] spawnpoints;

    private void Start()
    {
        int SpawnP = Random.RandomRange(0, 2);
        PhotonNetwork.Instantiate(playerPrefab.name, spawnpoints[SpawnP].position, Quaternion.identity);
    }
}
