using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplosion : MonoBehaviour {
    [SerializeField] private float radius = 1.0f;

    // Start is called before the first frame update
    void Start() {
        StartCoroutine(ExplosionCoroutine());
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            PlayerController pc = other.GetComponent<PlayerController>();

            if (pc != null) {
                pc.OnDamage(1);
            }
        }
    }

    private IEnumerator ExplosionCoroutine() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), radius);

        foreach (Collider2D hit in colliders) {
            Rigidbody2D theRigidbody2D = hit.GetComponent<Rigidbody2D>();

            if (theRigidbody2D != null) {
                theRigidbody2D.AddForceAtPosition(new Vector2(2.5f, 2.5f), new Vector2(transform.position.x, transform.position.y), ForceMode2D.Impulse);
            }
        }

        yield return new WaitForSeconds(.4f);

        Destroy(this.gameObject);
    }
}
