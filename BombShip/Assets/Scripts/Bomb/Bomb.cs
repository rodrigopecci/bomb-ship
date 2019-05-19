using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Bomb : MonoBehaviour
{

    public GameObject bombExplosion;
    private PhotonView pv;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();

        IgnoreCollision();
        StartCoroutine(ExplosionCoroutine());
    }

    private void IgnoreCollision() {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Object");

        foreach (GameObject obj in objects) {
            if (obj != null) {
                Physics2D.IgnoreCollision(obj.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            }
        }
    }

    private IEnumerator ExplosionCoroutine() {
        yield return new WaitForSeconds(2f);

        if (pv.IsMine) {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "BombExplosion"), transform.position, Quaternion.identity, 0);
        }

        Destroy(this.gameObject);
    }
}
