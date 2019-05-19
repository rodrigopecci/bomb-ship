using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyController : MonoBehaviourPunCallbacks {
    public static LobbyController lobby;

    public GameObject fightButton;
    public GameObject cancelButton;
    public GameObject connectingButton;

    private void Awake() {
        lobby = this; // Creates the singleton, lives withing the Main menu scene.
    }

    void Start() {
        if (!PhotonNetwork.IsConnected) {
            fightButton.SetActive(false);
            cancelButton.SetActive(false);
            connectingButton.SetActive(true);

            PhotonNetwork.ConnectUsingSettings(); // Connects to the master photon server.
        }
    }

    public void OnFightButtonClicked() {
        Debug.Log("The battle button was clicked.");

        fightButton.SetActive(false);
        cancelButton.SetActive(true);

        PhotonNetwork.JoinRandomRoom();
    }

    public void OnCancelButtonClicked() {
        Debug.Log("The cancel button was clicked.");

        cancelButton.SetActive(false);
        fightButton.SetActive(true);

        PhotonNetwork.LeaveRoom();
    }

    private void CreateRoom() {
        Debug.Log("Creating a new room.");

        int randomRoomName = Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions() {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = (byte) MultiplayerSettings.ms.maxPlayers
        };

        PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOptions);
    }

    // Callback to PhotonNetwork.ConnectUsingSettings
    public override void OnConnectedToMaster() {
        Debug.Log("The player has connected to the Photon master server.");
        PhotonNetwork.AutomaticallySyncScene = true;

        fightButton.SetActive(true);
        cancelButton.SetActive(false);
        connectingButton.SetActive(false);
    }

    // Callback to PhotonNetwork.JoinRandomRoom
    public override void OnJoinRandomFailed(short returnCode, string message) {
        Debug.Log("Tried to join a random room but failed. There must be no open games available.");
        CreateRoom();
    }

    // Callback to PhotonNetwork.CreateRoom
    public override void OnCreateRoomFailed(short returnCode, string message) {
        Debug.Log("Tried to create a new room but failed, there must already be a room with the same name");
        CreateRoom();
    }
}
