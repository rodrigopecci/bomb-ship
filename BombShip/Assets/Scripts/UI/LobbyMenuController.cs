using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMenuController : MonoBehaviour
{
    public void onClickCharacter(string character) {
        if (MultiplayerSettings.ms != null) {
            MultiplayerSettings.ms.selectedCharacter = character;
            PlayerPrefs.SetString("MyCharacter", character);
        }
    }
}
