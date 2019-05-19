using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerNetworkController : MonoBehaviour
{
    private PhotonView pv;
    public GameObject myAvatar;

    void Start() {
        pv = GetComponent<PhotonView>();

        //int spawnPicker = Random.Range(0, GameSetup.gs.spawnPoints.Length);
        int spawnPicker = pv.Owner.ActorNumber - 1;

        if (pv.IsMine) {
            myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", MultiplayerSettings.ms.selectedCharacter), GameController.gc.spawnPoints[spawnPicker].position, GameController.gc.spawnPoints[spawnPicker].rotation, 0);
            GameController.gc.targets.Add(myAvatar);
        }
    }
}
