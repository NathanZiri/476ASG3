using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WallManager : MonoBehaviour
{
    private PhotonView view;

    private void Start()
    {
        view = GetComponent<PhotonView>();
    }

    public void deleteSelf()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }
}
