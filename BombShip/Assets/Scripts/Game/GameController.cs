using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController gc;

    [Header("Spawn points")]
    public Transform[] spawnPoints;

    [Header("Camera targets")]
    public List<GameObject> targets;

    private void OnEnable() {
        if (GameController.gc == null) {
            GameController.gc = this;
        }
    }

    public void DisconnectPlayer() {
        RoomController.room.DisconnectPlayer();
    }
}
