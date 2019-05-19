using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHeathController : MonoBehaviour
{

    public Image[] hearts;

    private void Start() {
        //InitHearts();
    }

    private void InitHearts() {
        //TODO Futuramente criar uma classe de Game Management que ira informar o valor inicial da vida.
    }

    public void OnHealthUpdate(int health) {
        for (int i = 0; i < hearts.Length; i++) {
            if (i < health) {
                hearts[i].gameObject.SetActive(true);
            } else {
                hearts[i].gameObject.SetActive(false);
            }
        }
    }
}
