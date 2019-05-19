using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomController : MonoBehaviourPunCallbacks, IInRoomCallbacks {
    public static RoomController room;

    public int playersInRoom;
    public int currentScene;

    private PhotonView pv;
    private Player[] players;

    private void Awake() {
        if (RoomController.room == null) {
            RoomController.room = this;
        } else {
            if (RoomController.room != this) {
                Destroy(RoomController.room.gameObject);
                RoomController.room = this;
            }
        }

        DontDestroyOnLoad(this.gameObject);
    }

    void Start() {
        pv = GetComponent<PhotonView>();
    }

    public override void OnEnable() {
        base.OnEnable();

        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable() {
        base.OnDisable();

        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    private void StartGame() {
        if (!PhotonNetwork.IsMasterClient) {
            return;
        }

        PhotonNetwork.LoadLevel(MultiplayerSettings.ms.multiplayerScene);
    }

    private void CreatePlayer() {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerNetwork"), transform.position, Quaternion.identity, 0);
    }

    public override void OnJoinedRoom() {
        Debug.Log("Joined a room.");

        base.OnJoinedRoom();

        players = PhotonNetwork.PlayerList;
        playersInRoom = players.Length;

        if (MultiplayerSettings.ms.waitPlayerToStart) {
            Debug.Log("Players in room: " + playersInRoom + " (players needed: " + MultiplayerSettings.ms.maxPlayers + ").");
            if (playersInRoom == MultiplayerSettings.ms.maxPlayers) {
                StartGame();
            }
        } else {
            StartGame();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        base.OnPlayerEnteredRoom(newPlayer);

        players = PhotonNetwork.PlayerList;
        playersInRoom++;

        if (MultiplayerSettings.ms.waitPlayerToStart) {
            Debug.Log("A new player entered the room. Players in room: " + playersInRoom + " (players needed: " + MultiplayerSettings.ms.maxPlayers + ").");
            if (playersInRoom == MultiplayerSettings.ms.maxPlayers) {
                StartGame();
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        Debug.Log("A player disconnected from the room.");

        base.OnPlayerLeftRoom(otherPlayer);

        playersInRoom--;
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode) {
        currentScene = scene.buildIndex;

        if (currentScene == MultiplayerSettings.ms.multiplayerScene) {
            CreatePlayer();
        }
    }

    public void DisconnectPlayer() {
        pv.RPC("EndGame", RpcTarget.All);
    }

    public IEnumerator DisconnectAndLoad() {
        Debug.Log("Disconnecting a player");

        Destroy(room.gameObject);

        //PhotonNetwork.Disconnect();
        PhotonNetwork.LeaveRoom();

        //while (PhotonNetwork.IsConnected) {
        while (PhotonNetwork.InRoom) {
            yield return null;
        }

        SceneManager.LoadScene(MultiplayerSettings.ms.menuScene);
    }

    [PunRPC]
    void EndGame() {
        StartCoroutine(DisconnectAndLoad());
    }
}
