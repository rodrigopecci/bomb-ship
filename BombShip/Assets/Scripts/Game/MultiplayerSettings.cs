using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSettings : MonoBehaviour
{
    public static MultiplayerSettings ms;
    public bool waitPlayerToStart;
    public int maxPlayers;

    public int menuScene;
    public int multiplayerScene;

    public string selectedCharacter;

    private void Awake() {
        if (MultiplayerSettings.ms == null) {
            MultiplayerSettings.ms = this;
        } else {
            if (MultiplayerSettings.ms != this) {
                Destroy(this.gameObject);
            }
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start() {
        if (PlayerPrefs.HasKey("MyCharacter")) {
            selectedCharacter = PlayerPrefs.GetString("MyCharacter");
        } else {
            selectedCharacter = "PlayerAvatar1";
            PlayerPrefs.SetString("MyCharacter", "PlayerAvatar1");
        }
    }
}
